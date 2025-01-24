using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models {
    public class GidLocationOption {
        public int Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public string Value { get; set; }
        public bool Locked { get; set; }
        public string DefaultCurrency { get; set; }

        public string BankingInfo { get; set; }

        public string PhoneNumber { get; set; }


        public Address DefaultShippingAddress { get; set; }
        public Address MainAddress { get; set; }
        public Address BillingAddress { get; set; }

        public static void ConvertSubLocationOptions(AppDBContext context) {

            using (var transaction = context.Database.BeginTransaction()) {
                // var subLocations = context.InventoryItemLocationOptions.Where(item => item.Value.Contains("-")).ToList();
                // foreach (var subLocation in subLocations) {
                //     Console.WriteLine("doing sublocation " + subLocation.Value);
                //     var newSubLocation = new GidSubLocationOption {
                //         CreatedAt = DateTime.UtcNow,
                //         GidLocationOptionId = 1,
                //         Name = subLocation.Value,
                //     };
                //     context.GidSubLocationOptions.Add(newSubLocation);
                //     context.SaveChanges();
                //     var inventoryItemsToUpdate = context.InventoryItems.Where(item => item.GidSubLocationOptionId == subLocation.Id).ToList();
                //     foreach(var inventoryItem in inventoryItemsToUpdate){
                //         inventoryItem.GidSubLocationOptionId = null;
                //         inventoryItem.GidSubLocationOptionId = newSubLocation.Id;
                //     }
                //     context.Entry(subLocation).State = EntityState.Deleted;
                //     context.SaveChanges();
                // }
                var inventoryItems = context.InventoryItems
                    .Include(item => item.GidSubLocationOption)
                    .Where(item => item.GidSubLocationOptionId != null).ToList();

                foreach(var inventoryItem in inventoryItems){
                    inventoryItem.GidLocationOptionId = inventoryItem.GidSubLocationOption.GidLocationOptionId;
                }
                context.SaveChanges();
                transaction.Commit();
            }
        }
    }

    class GidLocationOptionDBConfiguration : IEntityTypeConfiguration<GidLocationOption> {
        public void Configure(EntityTypeBuilder<GidLocationOption> modelBuilder) {
            modelBuilder.HasOne(item => item.DefaultShippingAddress).WithMany().OnDelete(DeleteBehavior.Cascade);
            modelBuilder.HasOne(item => item.MainAddress).WithMany().OnDelete(DeleteBehavior.Cascade);
            modelBuilder.HasOne(item => item.BillingAddress).WithMany().OnDelete(DeleteBehavior.Cascade);
        }
    }
}
