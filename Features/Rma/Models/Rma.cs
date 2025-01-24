using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models {
    public class Rma {
        public int? Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? CreatedById { get; set; }

        public Company Company { get; set; }
        public int? CompanyId { get; set; }

        [NotMapped]
        public User PreparedBy { get; set; }

        public int? RmaStatusOptionId { get; set; }
        public RmaStatusOption RmaStatusOption { get; set; }
        public int? SalesOrderId { get; set; }
        public SalesOrder SalesOrder { get; set; }
        public int? RmaReasonOptionId { get; set; }
        public RmaReasonOption RmaReasonOption { get; set; }
        public int? RmaActionOptionId { get; set; }
        public RmaActionOption RmaActionOption { get; set; }
        public DateTime? SentAt { get; set; }
        public decimal? CreditAmount { get; set; }

        public int? CurrencyOptionId { get; set; }
        public CurrencyOption CurrencyOption { get; set; }

        public int? GidLocationOptionId { get; set; }
        public GidLocationOption GidLocationOption { get; set; }

        public List<RmaEventLogEntry> EventLogEntries { get; set; }
        public List<RmaLineItem> LineItems { get; set; }
        public List<RmaIncomingShipment> IncomingShipments { get; set; }
        public List<RmaOutgoingShipment> OutgoingShipments { get; set; }
        public List<RmaAttachment> Attachments { get; set; }
        public List<RmaChatMessage> ChatMessages { get; set; }
        public List<Credit> Credits { get; set; }

        public List<RmaNote> Notes { get; set; }

        public List<RMAToDoItem> ToDoItems { get; set; }

        public List<RmaLineItemGrouped> GetGroupedLineItems() {
            var groupedLineItemsDictionary = new Dictionary<int, RmaLineItemGrouped>();
            foreach (var lineItem in this.LineItems) {
                var salesOrderLineItemId = lineItem.SalesOrderLineItemId ?? 0;
                if (groupedLineItemsDictionary.ContainsKey(salesOrderLineItemId)) {
                    var item = groupedLineItemsDictionary[salesOrderLineItemId];
                    item.Quantity += 1;
                } else {
                    groupedLineItemsDictionary.Add(salesOrderLineItemId, new RmaLineItemGrouped {
                        Quantity = 1,
                        Price = lineItem.Price ?? 0,
                        SalesOrderLineItemId = lineItem.SalesOrderLineItemId ?? 0,
                        ProductId = lineItem.InventoryItem.ProductId,
                        Product = lineItem.SalesOrderLineItem.Product
                    });
                }
            }
            return groupedLineItemsDictionary.Select(item => item.Value).ToList();
        }
    }

    /// <summary>
    /// set up createdAt, updatedAt and createdBy auto properties
    /// </summary>
    class RmaDBConfiguration : IEntityTypeConfiguration<Rma> {
        public void Configure(EntityTypeBuilder<Rma> modelBuilder) {
            modelBuilder.HasOne(item => item.Company)
                .WithMany()
                .HasForeignKey(item => item.CompanyId);
            modelBuilder.HasIndex(item => item.CreatedAt);

            modelBuilder.HasOne(item => item.RmaActionOption).WithMany().HasForeignKey(item => item.RmaActionOptionId);
            modelBuilder.HasOne(item => item.RmaReasonOption).WithMany().HasForeignKey(item => item.RmaReasonOptionId);
            modelBuilder.HasOne(item => item.GidLocationOption).WithMany().HasForeignKey(item => item.GidLocationOptionId);
            modelBuilder.HasOne(item => item.SalesOrder).WithMany().HasForeignKey(item => item.SalesOrderId);
            modelBuilder.HasOne(item => item.CurrencyOption).WithMany().HasForeignKey(item => item.CurrencyOptionId);
            modelBuilder.HasOne(item => item.RmaStatusOption).WithMany().HasForeignKey(item => item.RmaStatusOptionId);
        }
    }
}
