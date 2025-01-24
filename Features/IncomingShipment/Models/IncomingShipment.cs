using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using QuickBooks.Models;
using WebApi.Features.Controllers;

namespace GidIndustrial.Gideon.WebApi.Models {
    public class IncomingShipment {
        public int? Id { get; set; }
        public DateTime? CreatedAt { get; set; }

        public string TrackingNumber { get; set; }
        public int? ShippingCarrierId { get; set; }
        public ShippingCarrier ShippingCarrier { get; set; }
        public int? ShippingCarrierShippingMethodId { get; set; }
        public DateTime? ExpectedArrivalDate { get; set; }
        public DateTime? ActualArrivalDate { get; set; }
        public DateTime? DateShipped { get; set; }

        public string CurrentLocation { get; set; }
        public string CurrentShippingStatus { get; set; }
        public int? ReceiptSignerId { get; set; }
        public User ReceiptSigner { get; set; }

        public List<IncomingShipmentShipmentTrackingEvent> TrackingEvents { get; set; }
        public List<IncomingShipmentInventoryItem> InventoryItems { get; set; }
        public List<PurchaseOrderIncomingShipment> PurchaseOrders { get; set; }
        public List<RepairIncomingShipment> Repairs { get; set; }
        public List<RmaIncomingShipment> Rmas { get; set; }
        public List<IncomingShipmentAttachment> Attachments { get; set; }

        public async Task TryUpdateStatus(HttpClient httpClient, DbContextOptions<AppDBContext> contextOptions, string azureStorageConnectionString, ILogger logger) {
            using (var context = new GidIndustrial.Gideon.WebApi.Models.AppDBContext(contextOptions)) {
                if (String.IsNullOrWhiteSpace(this.TrackingNumber)) {
                    return;
                }

                try {
                    var shippingCarrierName = this.ShippingCarrier != null && !string.IsNullOrWhiteSpace(this.ShippingCarrier.TrackingNumberLink) ? this.ShippingCarrier.Name.ToLower() : "";
                    var url = $"https://fr8r-api.gidindustrial.com/v1/packages/trackingnumber?query={this.TrackingNumber}&carriers={shippingCarrierName}";
                    var request = new HttpRequestMessage(HttpMethod.Get, url);

                    Console.WriteLine("The request url is " + url);

                    request.Headers.Add("Api-Key", "q7dUUm7hTuhhLn9gOKuvLqnhkRJFc1jjduslivRh1fpfk1dt6u45FRaHDVFwaIY");
                    request.Headers.Add("Accept", "application/json");

                    var response = await httpClient.SendAsync(request);

                    if (!response.IsSuccessStatusCode) {
                        Console.WriteLine("Error doing fr8r request", response);
                        logger.LogError("Error doing http request to fr8r", response);
                        return;
                    }
                    context.Entry(this).State = EntityState.Modified;

                    var data = await response.Content.ReadAsAsync<GidIndustrial.Fr8r.Entities.Result<GidIndustrial.Fr8r.Entities.Package>>();

                    logger.LogInformation("data is " + JsonConvert.SerializeObject(data));

                    if (data.Items == null || data.Items.Length < 1)
                        return;
                    var trackingData = data.Items[0];

                    logger.LogInformation("Tracking data is " + JsonConvert.SerializeObject(trackingData));
                    var timeZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");


                    //update estimated delivery
                    if (trackingData.Tracking.EstimatedDeliveryDate != null) {
                        this.ExpectedArrivalDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(trackingData.Tracking.EstimatedDeliveryDate.Value, DateTimeKind.Unspecified), timeZone);
                        logger.LogInformation("expected date = " + this.ExpectedArrivalDate.ToString());
                        await context.SaveChangesAsync();
                    }
                    if (trackingData.Tracking.ActualDeliveryDate != null) {
                        var deliveryDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(trackingData.Tracking.ActualDeliveryDate.Value, DateTimeKind.Unspecified), timeZone);
                        this.ActualArrivalDate = deliveryDate;
                        this.ExpectedArrivalDate = deliveryDate;
                    }

                    //need to check if all status options are in the database
                    var trackingEventsForThisPackage = await context.IncomingShipmentShipmentTrackingEvents
                        .Include(item => item.ShipmentTrackingEvent)
                        .Where(item => item.IncomingShipmentId == this.Id)
                        .Select(item => item.ShipmentTrackingEvent)
                        .ToListAsync();

                    var missingTrackingEvents = trackingData.Tracking.Events.Where(item => !trackingEventsForThisPackage.Any(item2 => item.Order == item2.Order));

                    //insert missing tracking events
                    var newEvents = new List<ShipmentTrackingEvent> { };
                    foreach (var missingTrackingEvent in missingTrackingEvents) {
                        var newShipmentTrackingEvent = new ShipmentTrackingEvent {
                            Code = missingTrackingEvent.Code,
                            Description = missingTrackingEvent.Description,
                            ExceptionCode = missingTrackingEvent.ExceptionCode,
                            Location = missingTrackingEvent.Location,
                            Order = missingTrackingEvent.Order,
                            Date = missingTrackingEvent.Timestamp
                        };
                        if (missingTrackingEvent.Address != null) {
                            newShipmentTrackingEvent.City = missingTrackingEvent.Address.City;
                            newShipmentTrackingEvent.CountryCode = missingTrackingEvent.Address.CountryCode;
                            newShipmentTrackingEvent.OrganizationName = missingTrackingEvent.Address.OrganizationName;
                            newShipmentTrackingEvent.PostalCode = missingTrackingEvent.Address.PostalCode;
                            newShipmentTrackingEvent.StateProvinceCode = missingTrackingEvent.Address.StateProvinceCode;
                            newShipmentTrackingEvent.Street = missingTrackingEvent.Address.Street != null ? string.Join(", ", missingTrackingEvent.Address.Street) : null;
                        }
                        newShipmentTrackingEvent.IncomingShipmentShipmentTrackingEvents = new List<IncomingShipmentShipmentTrackingEvent>{
                            new IncomingShipmentShipmentTrackingEvent{
                                IncomingShipmentId = this.Id
                            }
                        };
                        context.ShipmentTrackingEvents.Add(newShipmentTrackingEvent);
                        // newEvents.Add(newShipmentTrackingEvent);
                    }
                    // await context.ShipmentTrackingEvents.AddRangeAsync(newEvents);
                    await context.SaveChangesAsync();

                    var existingAttachments = await context.IncomingShipmentAttachments
                        .Include(item => item.Attachment)
                        .Where(item => item.IncomingShipmentId == this.Id).ToListAsync();

                    //now try to save any attatchments
                    if (trackingData.Attachments != null && trackingData.Attachments.Length > 0) {
                        foreach (var attachment in trackingData.Attachments) {
                            attachment.Name = GidIndustrial.Gideon.WebApi.Libraries.MimeTypesConverter.AddExtensionIfNotPresentBasedOnMimeType(attachment.Name, attachment.MimeType);
                            var existingAttachment = existingAttachments.FirstOrDefault(item => item.Attachment.Name == attachment.Name);
                            if (existingAttachment == null) {
                                var newAttachment = await Attachment.CreateNewAuthorizedAttachment(azureStorageConnectionString, attachment.Content.Length, attachment.Name, attachment.MimeType, null);
                                await newAttachment.UploadAttachment(attachment.Content);

                                context.IncomingShipmentAttachments.Add(new IncomingShipmentAttachment {
                                    IncomingShipmentId = (int)this.Id,
                                    Attachment = newAttachment
                                });
                                await context.SaveChangesAsync();
                            }
                        }
                    }

                    //update to find latest event
                    var events = await context.IncomingShipmentShipmentTrackingEvents
                        .Where(item => item.IncomingShipmentId == this.Id)
                        .OrderByDescending(item => item.ShipmentTrackingEvent.Order)
                        .Include(item => item.ShipmentTrackingEvent)
                        .Select(item => item.ShipmentTrackingEvent).ToListAsync();

                    var latestEvent = events.FirstOrDefault();
                    if (latestEvent != null) {
                        this.CurrentLocation = "";
                        if (!String.IsNullOrWhiteSpace(latestEvent.Description)) {
                            this.CurrentLocation = latestEvent.Description;
                        } else {
                            if (!String.IsNullOrWhiteSpace(latestEvent.City))
                                this.CurrentLocation += " " + latestEvent.City;
                            if (latestEvent.CountryName != null) {
                                if (!String.IsNullOrWhiteSpace(latestEvent.City)) {
                                    this.CurrentLocation += ", ";
                                }
                                this.CurrentLocation += latestEvent.CountryName;
                            }
                            if (!String.IsNullOrWhiteSpace(latestEvent.PostalCode)) {
                                if (!String.IsNullOrWhiteSpace(this.CurrentLocation)) {
                                    this.CurrentLocation += ", ";
                                }
                                this.CurrentLocation += latestEvent.PostalCode;
                            }
                        }

                        this.CurrentShippingStatus = latestEvent.ExceptionDescription;
                        await context.SaveChangesAsync();
                    }
                }
                catch (Exception ex) {
                    logger.LogError(ex.Message + "\n" + ex.StackTrace);
                }
            }
        }
    }
    class IncomingShipmentDBConfiguration : IEntityTypeConfiguration<IncomingShipment> {
        public void Configure(EntityTypeBuilder<IncomingShipment> modelBuilder) {
            modelBuilder.HasOne(item => item.ReceiptSigner).WithMany().HasForeignKey(item => item.ReceiptSignerId);
            modelBuilder.HasOne(item => item.ShippingCarrier).WithMany().HasForeignKey(item => item.ShippingCarrierId);

            modelBuilder.HasIndex(item => item.TrackingNumber);
        }
    }
}