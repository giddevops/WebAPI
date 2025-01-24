using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models {
    public class CreditCard {
        public int? Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? CreatedById { get; set; }

        [NotMapped]
        public string NameOnCard { get; set; }
        public string NameOnCardEncrypted { get; set; }

        [NotMapped]
        public string CardNumber { get; set; }
        public string CardNumberEncrypted { get; set; }

        [NotMapped]
        public string SecurityCode { get; set; }
        public string SecurityCodeEncrypted { get; set; }

        [NotMapped]
        public string ExpirationMonth { get; set; }
        public string ExpirationMonthEncrypted { get; set; }

        [NotMapped]
        public string ExpirationYear { get; set; }
        public string ExpirationYearEncrypted { get; set; }

        public Boolean? IsPrimary { get; set; }

        public int? CompanyId { get; set; }
        
        // public List<CreditCardCharge> Charges { get; set; }
    }
    // class CompanyDBConfiguration : IEntityTypeConfiguration<Company> {
    //     public void Configure(EntityTypeBuilder<Company> modelBuilder) {
    //     }
    // }
}