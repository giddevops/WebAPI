using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models
{
    public class EmailAddress
    {
        public int Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public string Address { get; set; }
        // public int EmailAddressTypeId { get; set; }

        // public List<CompanyEmailAddress> CompanyEmailAddresses { get; set; }
        // public List<ContactEmailAddress> ContactEmailAddresses { get; set; }

    }
    class EmailAddressDBConfiguration : IEntityTypeConfiguration<EmailAddress> {
        public void Configure(EntityTypeBuilder<EmailAddress> modelBuilder) {
            modelBuilder.HasIndex(item => item.Address);
        }
    }
}
