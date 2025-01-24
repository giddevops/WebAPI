using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using GidIndustrial.Gideon.WebApi.Libraries;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models
{
    public class Lead
    {
        public int Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? CreatedById { get; set; }

        public int? OldId { get; set; }
        public int? NewId { get; set; }
        public string PeanutSystemId { get; set; }

        public int? SalesPersonId { get; set; }
        public User SalesPerson { get; set; }

        public string FullName { get; set; }
        public string Position { get; set; }

        public string CompanyName { get; set; }

        public Company Company { get; set; }
        public int? CompanyId { get; set; }

        public Contact Contact { get; set; }
        public int? ContactId { get; set; }

        [ForeignKey("CustomerTypeId")]
        public int? CustomerTypeId { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        [Column(TypeName = "text")]
        public string Comments { get; set; }
        public int? RequiredDeliveryTimeId { get; set; }
        public int? LeadStatusOptionId { get; set; }

        //address
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string City { get; set; }
        public string ZipPostalCode { get; set; }
        public string State { get; set; }
        public int? CountryId { get; set; }

        public string Website { get; set; }
        public string DunsNumber { get; set; }
        public int? RequiredDeliveryWeeks { get; set; }

        // public int? CurrencyId { get; set; }

        public long? LALeadNumber { get; set; }
        // public int SourceId { get; set; }
        public DateTime? Timestamp { get; set; }
        public string SourceUrl { get; set; }
        public string ReferrerUrl { get; set; }
        public string WebServerName { get; set; }

        public string IPAddress { get; set; }
        public string GeolocationCity { get; set; }
        public string GeolocationCountryCode { get; set; }
        public string GeolocationCountryName { get; set; }
        public decimal? GeolocationLatitude { get; set; }
        public decimal? GeolocationLongitude { get; set; }
        public string GeolocationPostalCode { get; set; }
        public string GeolocationRegionCode { get; set; }
        public string GeolocationRegionName { get; set; }
        public string GeolocationTimeZone { get; set; }

        // public string IPCountry { get; set; }
        // public string IPAddressRegion { get; set; }
        // public string IPAddressCity { get; set; }

        public List<LeadLineItem> LineItems { get; set; }
        public List<Quote> Quotes { get; set; }

        public List<LeadAttachment> Attachments { get; set; }
        public List<LeadNote> Notes { get; set; }
        public List<LeadEventLogEntry> EventLogEntries { get; set; }

        public string NotesText { get; set; }
        public int? Quality { get; set; }
        public Boolean? AutoResponseSent { get; set; }
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
        public string CustomerType { get; set; }

        public string Category { get; set; }
        public string Manufacturer { get; set; }
        public string PartNumber { get; set; }
        public int? Quantity { get; set; }
        public string Service { get; set; }
        public string AdditionalData { get; set; }
        public string BrowserName { get; set; }
        public string BrowserVersion { get; set; }
        public int? LeadOriginOptionId { get; set; }
        public string Key { get; set; }
        public bool? Freeform { get; set; }
        public string UserAgent { get; set; }
        public int? Score { get; set; }
        public int? Rating { get; set; }
        // public int? SourceSystemCode { get; set; }

        public string Emoji { get; set; }
        public List<LeadContactLogItem> ContactLogItems { get; set; }

        public List<LeadToDoItem> ToDoItems { get; set; }

        public string OriginText { get; set; }
        public List<LeadChatMessage> ChatMessages { get; set; }
        // public List<LeadNote> Notes { get; set; }

        /// <summary>
        /// When the lead is saved, the status may need to be updated to reflect the fact that it is now assigned to a user or something like that
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task ComputeStatus(AppDBContext context)
        {
            //             - Open (no sales person assigned)
            // - Assigned (sales person assigned)
            // - Converted (quote created)
            // - Duplicate (manual or system set)
            // - No Bid (see no bid below)

            var statuses = await context.LeadStatusOptions.ToListAsync();
            LeadStatusOption currentLeadStatusOption = statuses.FirstOrDefault(status => status.Id == this.LeadStatusOptionId); ;
            if (currentLeadStatusOption == null)
            {
                currentLeadStatusOption = statuses.First(status => status.Value.ToLower() == "open");
                this.LeadStatusOptionId = currentLeadStatusOption.Id;
            }
            //if (new List<string> { "open", "assigned", "converted" }.Contains(currentLeadStatusOption.Value.ToLower()))
            {
                if (this.SalesPersonId != null && currentLeadStatusOption.Value.ToLower() == "open")
                {
                    currentLeadStatusOption = statuses.First(status => status.Value.ToLower() == "assigned");
                    this.LeadStatusOptionId = currentLeadStatusOption.Id;
                }
                //check if there is a quote for the lead
                if (currentLeadStatusOption.Value.ToLower() != "converted")
                {
                    var quoteForLead = await context.Quotes.FirstOrDefaultAsync(quote => quote.LeadId == this.Id);
                    if (quoteForLead != null)
                    {
                        currentLeadStatusOption = statuses.First(status => status.Value.ToLower() == "converted");
                        this.LeadStatusOptionId = currentLeadStatusOption.Id;
                    }
                }
            }

        }
        

        public static async Task<Product> MatchProduct(AppDBContext context, string partNumber, string manufacturer, string category, bool freeform){
            Product matchingProduct = null;

            if (manufacturer != null && manufacturer != "")
            {
                matchingProduct = await context.Products
                .Where(
                    item =>
                    (item.PartNumber == partNumber && item.Manufacturer.Name == manufacturer)
                    || (item.Aliases.Any(
                        alias => (alias.PartNumber == partNumber && alias.ManufacturerName == manufacturer)
                    ))
                )
                .FirstOrDefaultAsync();
            }

            //If the product/manufacturer combo was not found, need to add them
            if (matchingProduct == null && !String.IsNullOrWhiteSpace(manufacturer) && !freeform)
            {
                matchingProduct = new Product
                {
                    PartNumber = partNumber
                };

                //check if there is a matching category
                if (!String.IsNullOrWhiteSpace(category))
                {
                    var productTypeString = category.Split("»").Last().Trim();
                    var productType = await context.ProductTypes.FirstOrDefaultAsync(item => item.Value == productTypeString || item.Aliases.Any(item2 => item2.Alias == productTypeString));
                    if (productType == null)
                    {
                        matchingProduct.ProductType = new ProductType
                        {
                            Value = productTypeString,
                            IsPiecePart = false
                        };
                    }
                    else
                    {
                        matchingProduct.ProductTypeId = productType.Id;
                    }
                }
                //Check if there is a company with the same name already in the databsae.  If so, set the products manufacturer to that one
                Company relatedManufacturer = await context.Companies.FirstOrDefaultAsync(c => c.Name == manufacturer);
                if (relatedManufacturer != null)
                {
                    matchingProduct.ManufacturerId = relatedManufacturer.Id;
                }
                //If there is is no company, go ahead and create it
                matchingProduct.Manufacturer = new Company
                {
                    Name = manufacturer
                };
                context.Products.Add(matchingProduct);
                await context.SaveChangesAsync();
            }
            return matchingProduct;
        }

        /// <summary>
        /// Creates line items based on the information that comes from the api
        /// This attempts to automatically match the product and company to ones that are already in the databsae.  If it cannot find matching products and companies, it will add them automatically if if the Freeform flag is not set to true. If it contains that string it means it was entered freehand, and should not be trusted.
        /// </summary>
        /// <param name="context"></param>
        public async Task CreateLineItemsFromLALead(AppDBContext context, LeadAutomation.Pigeon.Exchange.Entities.LALead laLead)
        {
            var laLeadLineItems = new List<LeadAutomation.Pigeon.Exchange.Entities.LALead.LeadProductInfo> { };
            if (laLead.Products != null && laLead.Products.Count > 0)
            {
                laLeadLineItems = laLead.Products;
            }
            else if (laLead.ProductInfo != null)
            {
                laLeadLineItems.Add(laLead.ProductInfo);
            }

            foreach (var product in laLeadLineItems)
            {
                if (product.PartNumber == null || product.PartNumber == "")
                {
                    return;
                }

                Product matchingProduct = null;

                if (product.Manufacturer != null && product.Manufacturer != "") {
                    matchingProduct = await context.Products
                    .Where(
                        item =>
                        (item.PartNumber == product.PartNumber && item.Manufacturer.Name == product.Manufacturer)
                        || (item.Aliases.Any(
                            alias => (alias.PartNumber == product.PartNumber && alias.ManufacturerName == product.Manufacturer)
                        ))
                    )
                    .FirstOrDefaultAsync();
                }
                // Product matchingProduct = await (from product in context.Products
                //                                  join company in context.Companies on product.ManufacturerId equals company.Id
                //                                  where product.PartNumber == product.PartNumber && company.Name == product.Manufacturer
                //                                  select product).FirstOrDefaultAsync();

                //If the product/manufacturer combo was not found, need to add them
                if (matchingProduct == null && !String.IsNullOrWhiteSpace(product.Manufacturer) && !product.Freeform)
                {
                    matchingProduct = new Product
                    {
                        PartNumber = product.PartNumber
                    };

                    //check if there is a matching category
                    if (!String.IsNullOrWhiteSpace(product.Category))
                    {
                        var productTypeString = product.Category.Split("»").Last().Trim();
                        var productType = await context.ProductTypes.FirstOrDefaultAsync(item => item.Value == productTypeString || item.Aliases.Any(item2 => item2.Alias == productTypeString));
                        if (productType == null)
                        {
                            matchingProduct.ProductType = new ProductType
                            {
                                Value = productTypeString,
                                IsPiecePart = false
                            };
                        }
                        else
                        {
                            matchingProduct.ProductTypeId = productType.Id;
                        }
                    }
                    //Check if there is a company with the same name already in the databsae.  If so, set the products manufacturer to that one
                    Company relatedManufacturer = await context.Companies.FirstOrDefaultAsync(c => c.Name == product.Manufacturer);
                    if (relatedManufacturer != null)
                    {
                        matchingProduct.ManufacturerId = relatedManufacturer.Id;
                    }
                    //If there is is no company, go ahead and create it
                    matchingProduct.Manufacturer = new Company
                    {
                        Name = product.Manufacturer
                    };
                    context.Products.Add(matchingProduct);
                    await context.SaveChangesAsync();
                }


                //if product was found, relate it and we are done
                LeadLineItem newLeadLineItem = new LeadLineItem
                {
                    Quantity = (int?)product.Quantity
                };
                if(matchingProduct != null)
                {
                    newLeadLineItem.ProductId = matchingProduct.Id;
                }
                else
                {
                    newLeadLineItem.ProductName = product.PartNumber;
                    newLeadLineItem.ManufacturerName = product.Manufacturer;
                }

                //Convert service 
                if (!String.IsNullOrWhiteSpace(product.Service))
                {
                    var serviceTypeOptions = await context.LineItemServiceTypes.ToListAsync();
                    var matchingServiceType = serviceTypeOptions.FirstOrDefault(item => item.Value.Trim().ToLower() == product.Service.Trim().ToLower());
                    // if(matchingServiceType == null && product.Service.Trim().ToLower() == "repair"){
                    //     matchingServiceType = serviceTypeOptions.FirstOrDefault(item => item.Value.Trim().ToLower() == "service");
                    // }
                    if (matchingServiceType != null)
                    {
                        newLeadLineItem.LineItemServiceTypeId = matchingServiceType.Id;
                    } // else {
                      //     var newServiceType = new ServiceType {
                      //         Value = product.Service.Trim()
                      //     };
                      //     this.ServiceTypes.Add(newServiceType);
                      //     await this.SaveChangesAsync();
                      // }
                }

                //If the lead hasn't yet been inserted, the new lead line item can simply be added to the lead LineItems array. Otherwise it needs to be inserted separately
                if (this.Id != 0)
                {
                    newLeadLineItem.Id = this.Id;
                }
                else
                {
                    if (this.LineItems == null)
                    {
                        this.LineItems = new List<LeadLineItem> { };
                    }
                    this.LineItems.Add(newLeadLineItem);
                }
            }
            return;
        }

        public async Task<bool> SortLineItems()
        {
            if (this.LineItems != null)
            {
                bool hasOrderedItems = false;

                foreach (var lineItem in this.LineItems)
                {
                    if (lineItem.Order > 0)
                    {
                        hasOrderedItems = true;
                        break;
                    }
                }

                if (hasOrderedItems)
                {
                    List<LeadLineItem> sortedItems = this.LineItems.OrderBy(i => i.Order).ToList();
                    this.LineItems = sortedItems;
                }
            }

            return true;
        }

        /// <summary>
        /// THis function calculates which sales person should be assigned to this lead
        /// Make sure to only call this function AFTER the lead has already been saved (so it has an id)
        /// Also, you need to call saveChanges() after it runs
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task CalculateLeadRouting(AppDBContext context)
        {

            /*
            var leadWebsiteText = "";
            try
            {
                leadWebsiteText = new Uri(this.SourceUrl).Host.Trim("www.".ToCharArray());
            }
            catch (Exception ex)
            {
                leadWebsiteText = this.SourceUrl;
            }
            string productTypeText = null;
            if (!String.IsNullOrWhiteSpace(this.Category))
                productTypeText = this.Category.Split("»").Last().Trim();
            var countryCodeText = String.IsNullOrWhiteSpace(this.CountryCode) ? this.GeolocationCountryCode : this.CountryCode;
            var lineItemServiceTypeText = this.Service;

            //First, check if there are any other leads within a certain period of time with the same companyname and fullname.  Route them to that salesperson
            Lead matchingLead = null;
            if (!String.IsNullOrWhiteSpace(FullName) && !String.IsNullOrWhiteSpace(CompanyName))
            {
                var leadQuery = from lead in context.Leads select lead;
                leadQuery = leadQuery.Where(
                    item => item.CreatedAt > DateTime.UtcNow.AddDays(-1) &&
                    String.Equals(item.FullName, FullName, StringComparison.OrdinalIgnoreCase) &&
                    String.Equals(item.CompanyName, CompanyName, StringComparison.OrdinalIgnoreCase) &&
                    item.SalesPersonId != null
                );
                matchingLead = await leadQuery.FirstOrDefaultAsync();
                if (matchingLead != null)
                {
                    this.SalesPersonId = matchingLead.SalesPersonId;
                    return;
                }
            }

            // Next, check if this contact and company were in a recent sales order
            var names = this.FullName.Split(' ');
            if (names.Length == 2 && !string.IsNullOrWhiteSpace(names[0]) && !string.IsNullOrWhiteSpace(names[1]) && !string.IsNullOrWhiteSpace(this.CompanyName))
            {
                var matchingSalesOrder = await context.SalesOrders
                    .Where(item => item.Contact.FirstName == names[0])
                    .Where(item => item.Contact.LastName == names[1])
                    .Where(item => item.Company.Name == this.CompanyName)
                    .FirstOrDefaultAsync();
                if (matchingSalesOrder != null)
                {
                    if (matchingSalesOrder.SalesPersonId != null)
                    {
                        this.SalesPersonId = matchingSalesOrder.SalesPersonId;
                        return;
                    }
                }
            }

            var query = from leadRoutingRule in context.LeadRoutingRules select leadRoutingRule;

            if (!String.IsNullOrWhiteSpace(leadWebsiteText))
            {
                query = query
                    .Where(item =>
                        item.LeadWebsiteIncludeOptionId == (int?)LeadRoutingRuleIncludeOptions.All ||
                        (item.LeadWebsiteIncludeOptionId == (int?)LeadRoutingRuleIncludeOptions.OnlySpecified && item.LeadWebsites.Any(website => website.LeadWebsite.Value == leadWebsiteText)) ||
                        (item.LeadWebsiteIncludeOptionId == (int?)LeadRoutingRuleIncludeOptions.AllExceptSpecified && !item.LeadWebsites.Any(website => website.LeadWebsite.Value == leadWebsiteText))
                    );
            }
            if (!String.IsNullOrWhiteSpace(productTypeText))
            {
                query = query
                    .Where(item =>
                        item.ProductTypeIncludeOptionId == (int?)LeadRoutingRuleIncludeOptions.All ||
                        (item.ProductTypeIncludeOptionId == (int?)LeadRoutingRuleIncludeOptions.OnlySpecified && item.ProductTypes.Any(leadRoutingRuleProductType => leadRoutingRuleProductType.ProductType.Value == productTypeText)) ||
                        (item.ProductTypeIncludeOptionId == (int?)LeadRoutingRuleIncludeOptions.AllExceptSpecified && !item.ProductTypes.Any(leadRoutingRuleProductType => leadRoutingRuleProductType.ProductType.Value == productTypeText))
                    );
            }
            if (!String.IsNullOrWhiteSpace(countryCodeText))
            {
                query = query
                    .Where(item =>
                        item.CountryIncludeOptionId == (int?)LeadRoutingRuleIncludeOptions.All ||
                        (item.CountryIncludeOptionId == (int?)LeadRoutingRuleIncludeOptions.OnlySpecified && item.Countries.Any(leadRoutingRuleCountryCode => leadRoutingRuleCountryCode.Country.CountryCode == countryCodeText)) ||
                        (item.CountryIncludeOptionId == (int?)LeadRoutingRuleIncludeOptions.AllExceptSpecified && !item.Countries.Any(leadRoutingRuleCountryCode => leadRoutingRuleCountryCode.Country.CountryCode == countryCodeText))
                    );
            }
            if (!String.IsNullOrWhiteSpace(lineItemServiceTypeText))
            {
                query = query
                    .Where(item => item.LineItemServiceTypes.Any(website => website.LineItemServiceType.Value == lineItemServiceTypeText));
            }
            query = query.Include(item => item.CompanyNames);

            var matchingRules = await query.ToListAsync();
            //If this lead won't be assigned, let somebody know
            if (matchingRules.Count == 0)
            {
                var client = EmailGenerator.GetNewSendGridClient();
                var msg = await EmailGenerator.GenerateEmail(context, new EmailGeneratorParameters
                {
                    To = "joe@gidindustrial.com",
                    From = "errors@gidindustrial.com",
                    Subject = "Unassigned lead ID # " + this.Id.ToString(),
                    HtmlContent = $"<p>A new lead came in but no matching rule was found to assign it. <a href='https://gideon.gidindustrial.com/leads/{this.Id}'>Link</a></p>"
                });
                var response = await client.SendEmailAsync(msg);
                int responseStatusCodeNumber = (int)response.StatusCode;
                if (responseStatusCodeNumber >= 200 && responseStatusCodeNumber < 300)
                {
                    Console.WriteLine("ERROR SENDING EMAIL THAT IT FAILED TO MATCH A LEAD TO A SALESPERSON");
                }
                return;
            }


            //If it has a matching company name - automatically send to this person
            var matchingUserIds = matchingRules
                .Where(item => item.CompanyNames.Any(item2 => item2.CompanyName == this.CompanyName))
                .Select(item => item.UserId).ToList();

            if (matchingUserIds.Count == 0)
                matchingUserIds = matchingRules.Select(item => item.UserId).OrderBy(item => item).ToList();

            if (matchingUserIds.Count > 0)
            {
                var matchingUserIdsString = string.Join(",", matchingUserIds);
                var lastLeadAction = await context.LeadRoutingActions
                    .OrderByDescending(item => item.CreatedAt)
                    .FirstOrDefaultAsync(item => item.MatchedUserIds == matchingUserIdsString);

                int? assignedToUserId;
                if (lastLeadAction != null)
                {
                    var lastAssignedToIndex = matchingUserIds.IndexOf(lastLeadAction.SelectedUserId);
                    var newIndex = lastAssignedToIndex + 1;
                    if (newIndex > matchingUserIds.Count - 1)
                    {
                        newIndex = 0;
                    }
                    assignedToUserId = matchingUserIds[newIndex];
                }
                else
                {
                    assignedToUserId = matchingUserIds[0];
                }

                var leadStatusAssigned = await context.LeadStatusOptions.FirstOrDefaultAsync(item => item.Value == "Assigned");
                if (leadStatusAssigned != null)
                {
                    this.LeadStatusOptionId = leadStatusAssigned.Id;
                }

                context.LeadRoutingActions.Add(new LeadRoutingAction
                {
                    CreatedAt = DateTime.UtcNow,
                    LeadId = this.Id,
                    MatchedUserIds = matchingUserIdsString,
                    SelectedUserId = assignedToUserId
                });
                }
                */

            SqlParameter leadParam = new SqlParameter("@leadId", System.Data.SqlDbType.Int);
                leadParam.Value = this.Id;
                SqlParameter salesPersonIdParam = new SqlParameter("@salesPersonId", System.Data.SqlDbType.Int);
                salesPersonIdParam.Direction = System.Data.ParameterDirection.Output;
                context.Database.ExecuteSqlCommand("exec LeadRoute @leadId, @salesPersonId out", leadParam, salesPersonIdParam);

                this.SalesPersonId = Convert.ToInt32(salesPersonIdParam.Value);
                var salesPerson = await context.Users.FirstOrDefaultAsync(item => item.Id == this.SalesPersonId);
                



                //Send email to the sales person that they got a new lead
                var client = EmailGenerator.GetNewSendGridClient();
                var msg = await EmailGenerator.GenerateEmail(context, new EmailGeneratorParameters
                {
                    To = salesPerson.Email,
                    From = "leads@gidindustrial.com",
                    Subject = "New Lead for you - ID # " + this.Id.ToString(),
                    HtmlContent = $@"
<p>A new lead came in for you: <a href='https://gideon.gidindustrial.com/leads/{this.Id}'>Link</a></p>
<p>Company Name: {this.CompanyName}</p>
<p>Contact Name: {this.FullName}</p>
<p>Part Number: {this.PartNumber}</p>
<p>Manufacturer: {this.Manufacturer}</p>
<p>Qty Requested: {this.Quantity}</p>"
                });
                var response = await client.SendEmailAsync(msg);
                int responseStatusCodeNumber = (int)response.StatusCode;
                if (responseStatusCodeNumber >= 200 && responseStatusCodeNumber < 300)
                {
                    Console.WriteLine("ERROR SENDING EMAIL THAT LEAD was assigned");
                }

            
            context.Entry(this).State = EntityState.Modified;
        }

        public async Task SendAutoResponse(AppDBContext context)
        {
            //Send email to the sales person that they got a new lead
            try
            {
                var template = await context.EmailTemplates.FirstOrDefaultAsync(item => item.EmailTemplateTypeId == 13);
                if (template == null)
                {
                    Console.WriteLine("No default email template for auto response");
                    return;
                }
                if (this.SalesPersonId == null)
                {
                    return;
                }
                this.SalesPerson = await context.Users.FirstOrDefaultAsync(item => item.Id == this.SalesPersonId);

                if (this.SalesPerson == null || this.SalesPerson.Email == null)
                {
                    Console.WriteLine("Sales person user not found");
                    return;
                }

                
                TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
                TimeSpan offset = tzi.GetUtcOffset(DateTime.UtcNow);
                string utcOffset = string.Concat(offset.TotalMinutes > 0 ? "+" : string.Empty, offset.TotalHours);

                var client = EmailGenerator.GetNewSendGridClient();
                var subject = template.Subject
                    .Replace("[[FullName]]", this.FullName)
                    .Replace("[[LeadOrigin]]", this.OriginText)
                    .Replace("[[EntityId]]", this.Id.ToString());

                var signatureTemplate = await context.EmailTemplates.FirstOrDefaultAsync(item => item.EmailTemplateTypeId == 9);
                
                template.ReplaceVariables(context, this.SalesPersonId);
                signatureTemplate.ReplaceVariables(context, this.SalesPersonId);

                var body = template.HtmlContent + signatureTemplate.HtmlContent;

                var msg = await EmailGenerator.GenerateEmail(context, new EmailGeneratorParameters
                {
                    To = this.Email,
                    From = this.SalesPerson.Email,
                    Subject = subject,
                    HtmlContent = body
                });
                
                var response = await client.SendEmailAsync(msg);
                
                int responseStatusCodeNumber = (int)response.StatusCode;
                if (responseStatusCodeNumber >= 200 && responseStatusCodeNumber < 300)
                {
                    Console.WriteLine("ERROR SENDING Auto response email");
                }
                
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError("Error sending auto response email " + ex.Message);
            }
        }

        /// <summary>
        /// This parses a lead from any of the LA websites and converts it into a lead for gideon
        /// </summary>
        /// <param name="laLead"></param>
        /// <param name="_context"></param>
        /// <returns></returns>
        public static async Task<Lead> ParseWebsiteLead(LeadAutomation.Pigeon.Exchange.Entities.LALead laLead, AppDBContext _context)
        {

            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

            var newLead = new Lead { };
            newLead.LALeadNumber = laLead.Id;
            newLead.Address1 = laLead.CustomerInfo.Address1;
            newLead.Address2 = laLead.CustomerInfo.Address2;
            newLead.Address3 = laLead.CustomerInfo.Address3;
            newLead.City = laLead.CustomerInfo.City;
            newLead.State = laLead.CustomerInfo.RegionName;
            newLead.ZipPostalCode = laLead.CustomerInfo.PostalCode;
            newLead.PeanutSystemId = laLead._Id;
            newLead.Comments = laLead.CustomerInfo.Comments;
            newLead.CompanyName = textInfo.ToTitleCase(laLead.CustomerInfo.Company ?? "");
            newLead.CountryCode = laLead.CustomerInfo.CountryCode;
            newLead.CountryName = laLead.CustomerInfo.CountryName;
            newLead.CustomerType = laLead.CustomerInfo.CustomerType;
            newLead.DunsNumber = laLead.CustomerInfo.DunsNumber;
            newLead.Email = laLead.CustomerInfo.Email;
            newLead.FullName = textInfo.ToTitleCase(laLead.CustomerInfo.FullName ?? "");
            newLead.Phone = laLead.CustomerInfo.Phone;
            newLead.Position = laLead.CustomerInfo.Position;
            newLead.Website = laLead.CustomerInfo.Website;
            if (laLead.ProductInfo != null)
            {
                newLead.Category = laLead.ProductInfo.Category;
                newLead.Manufacturer = laLead.ProductInfo.Manufacturer;
                newLead.PartNumber = laLead.ProductInfo.PartNumber;
                int? quantity = null;
                if (laLead.ProductInfo.Quantity.HasValue)
                    quantity = (int)laLead.ProductInfo.Quantity;
                newLead.Quantity = quantity;
                newLead.Freeform = laLead.ProductInfo.Freeform;
                newLead.RequiredDeliveryWeeks = laLead.ProductInfo.RequiredDeliveryWeeks;
                newLead.Service = laLead.ProductInfo.Service;
            }
            if(laLead.ProductInfo != null && laLead.ProductInfo.Service == null && laLead.Products != null && laLead.Products.Count > 0)
            {
                newLead.Category = laLead.Products[0].Category;
                newLead.Manufacturer = laLead.Products[0].Manufacturer;
                newLead.PartNumber = laLead.Products[0].PartNumber;
                int? quantity = null;
                if (laLead.Products[0].Quantity.HasValue)
                    quantity = (int)laLead.Products[0].Quantity;
                newLead.Quantity = quantity;
                newLead.Freeform = laLead.Products[0].Freeform;
                newLead.RequiredDeliveryWeeks = laLead.Products[0].RequiredDeliveryWeeks;
                newLead.Service = laLead.Products[0].Service;
            }
            newLead.AdditionalData = laLead.TechnicalInfo.AdditionalData;
            newLead.BrowserName = laLead.TechnicalInfo.BrowserName;
            newLead.BrowserVersion = laLead.TechnicalInfo.BrowserVersion;
            newLead.IPAddress = laLead.TechnicalInfo.IPAddress;
            newLead.Key = laLead.TechnicalInfo.Key;
            newLead.OriginText = laLead.TechnicalInfo.Origin;
            newLead.ReferrerUrl = laLead.TechnicalInfo.ReferrerUrl;
            newLead.SourceUrl = laLead.TechnicalInfo.SourceUrl;
            newLead.UserAgent = laLead.TechnicalInfo.UserAgent;
            newLead.WebServerName = laLead.TechnicalInfo.WebServerName;
            newLead.GeolocationCity = laLead.TechnicalInfo.Geolocation.City;
            newLead.GeolocationCountryCode = laLead.TechnicalInfo.Geolocation.CountryCode;
            newLead.GeolocationCountryName = laLead.TechnicalInfo.Geolocation.CountryName;
            newLead.GeolocationLatitude = laLead.TechnicalInfo.Geolocation.Latitude;
            newLead.GeolocationLongitude = laLead.TechnicalInfo.Geolocation.Longitude;
            newLead.GeolocationPostalCode = laLead.TechnicalInfo.Geolocation.PostalCode;
            newLead.GeolocationRegionCode = laLead.TechnicalInfo.Geolocation.RegionCode;
            newLead.GeolocationRegionName = laLead.TechnicalInfo.Geolocation.RegionName;
            newLead.GeolocationTimeZone = laLead.TechnicalInfo.Geolocation.TimeZone;
            newLead.Emoji = laLead.Emoji;

            //get required delivery time option
            if (laLead.ProductInfo.RequiredDeliveryWeeks != null)
            {
                var matchingRequiredDeliveryOption = await _context.RequiredDeliveryTimeOptions
                    .FirstOrDefaultAsync(item =>
                        item.Value == laLead.ProductInfo.RequiredDeliveryWeeks + " Weeks"
                        || item.Value == laLead.ProductInfo.RequiredDeliveryWeeks + " Week");
                if (matchingRequiredDeliveryOption != null)
                {
                    newLead.RequiredDeliveryTimeId = matchingRequiredDeliveryOption.Id;
                }
                else
                {
                    var newRequiredDeliveryOption = new RequiredDeliveryTimeOption
                    {
                        Value = laLead.ProductInfo.RequiredDeliveryWeeks + " Weeks"
                    };
                    _context.RequiredDeliveryTimeOptions.Add(newRequiredDeliveryOption);
                    await _context.SaveChangesAsync();
                    newLead.RequiredDeliveryTimeId = newRequiredDeliveryOption.Id;
                }
            }

            if (!String.IsNullOrWhiteSpace(newLead.CountryCode))
            {
                var country = await _context.Countries.FirstOrDefaultAsync(item => String.Equals(item.CountryCode, newLead.CountryCode, StringComparison.OrdinalIgnoreCase));
                if (country != null)
                {
                    newLead.CountryId = country.Id;
                }
            }
            //Customer Type needs to be converted to CustomerTypeOptionId
            if (!String.IsNullOrWhiteSpace(laLead.CustomerInfo.CustomerType))
            {
                var customerTypeOptions = await _context.CustomerTypes.ToListAsync();
                var matchingCustomerType = customerTypeOptions.FirstOrDefault(item => item.Value.Trim().ToLower() == laLead.CustomerInfo.CustomerType.Trim().ToLower());
                if (matchingCustomerType != null)
                {
                    newLead.CustomerTypeId = matchingCustomerType.Id;
                }
                else
                {
                    var newCustomerType = new CustomerType
                    {
                        Value = laLead.CustomerInfo.CustomerType.Trim()
                    };
                    _context.CustomerTypes.Add(newCustomerType);
                    await _context.SaveChangesAsync();
                }
            }
            return newLead;
        }
    }


    /// <summary>
    /// set up createdAt, updatedAt and createdBy auto properties
    /// </summary>
    class LeadDBConfiguration : IEntityTypeConfiguration<Lead>
    {
        public void Configure(EntityTypeBuilder<Lead> modelBuilder)
        {
            modelBuilder
                .Property(b => b.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            modelBuilder.HasOne(l => l.SalesPerson)
            .WithMany()
            .HasForeignKey(l => l.SalesPersonId)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.HasOne(l => l.Company)
            .WithMany()
            .HasForeignKey(l => l.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.HasOne(l => l.Contact)
            .WithMany()
            .HasForeignKey(l => l.ContactId)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.HasIndex(item => item.CreatedAt);
            modelBuilder.HasIndex(item => item.FullName);
            modelBuilder.HasIndex(item => item.OriginText);
            modelBuilder.HasIndex(item => item.LeadStatusOptionId);
            modelBuilder.HasIndex(item => item.Phone);
            modelBuilder.HasIndex(item => item.Email);
            modelBuilder.HasIndex(item => item.CompanyName);
            modelBuilder.HasIndex(item => item.LALeadNumber);
            modelBuilder.HasIndex(item => item.Emoji);

            modelBuilder.HasMany(l => l.Quotes).WithOne(q => q.Lead).OnDelete(DeleteBehavior.Restrict);
        }
    }
}


/** questions day 2 
For history, what fields should we have?  should the Event field be a textbox, or an enum? Or just use constants like UPDATED

event is TEXT
whoever is responsible for the event system user
datetime
file (view-quote) nullable



EventLog


can we change the name of the history table to LogItems or Events or something like that? The reason is that it works better for naming conventions. Otherwise we'll have a HistoryId column which doesn't make sense
*************** */


/**.


-add option to copy notes over when converting lead to quote
-duplicate attachment record, but don't duplicate attachment blob. If attachment still has another attachment record when deleting, do't delete blob


need list of countries
do full state names in dropdown

 */


