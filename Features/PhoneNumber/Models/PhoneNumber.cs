using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models {
    public class PhoneNumber {
        public int? Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public string Number { get; set; }
        public string Extension { get; set; }

        public List<CompanyPhoneNumber> CompanyPhoneNumbers { get; set; }
        // public List<ContactPhoneNumber> ContactPhoneNumbers { get; set; }
    }

    class PhoneNumberDBConfiguration : IEntityTypeConfiguration<PhoneNumber> {
        public void Configure(EntityTypeBuilder<PhoneNumber> modelBuilder) {
            modelBuilder.HasIndex(item => item.Number);
        }
    }
}
