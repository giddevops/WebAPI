using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models {
    public class    Address {
        public int? Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public string Name { get; set; }
        public string Attention { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string City { get; set; }
        public string ZipPostalCode { get; set; }
        public string State { get; set; }
        public string PhoneNumber { get; set; }

        public int? CountryId { get; set; }
        public Country Country { get; set; }

        // [NotMapped]
        // public string CountryCode { get; set; }
        // public async void MapCountryCode(AppDBContext context) {
        //     if (CountryCode != null) {
        //         var Country = await context.Countries.FirstOrDefaultAsync(item => item.CountryCode.ToLower() == CountryCode.Trim().ToLower());
        //         if (Country != null) {
        //             this.CountryId = Country.Id;
        //             this.Country = Country;
        //         }
        //     }
        // }

        // public List<CompanyAddress> CompanyAddresses { get; set; }
        // public List<ContactAddress> ContactAddresses { get; set; }
        public static async Task<int?> GetCountryIdFromCode(AppDBContext context, string countryCode) {
            if(String.IsNullOrWhiteSpace(countryCode))
                return null;
            var Country = await context.Countries.FirstOrDefaultAsync(item => item.CountryCode.ToLower() == countryCode.Trim().ToLower());
            if (Country != null) {
                return Country.Id;
            }
            return null;
        }
    }

    class AddressDBConfiguration : IEntityTypeConfiguration<Address> {
        public void Configure(EntityTypeBuilder<Address> modelBuilder) {
            modelBuilder.HasOne(item => item.Country)
            .WithMany()
            .HasForeignKey(item => item.CountryId);
        }
    }
}
