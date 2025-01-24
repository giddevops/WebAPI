using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models
{
    public class ListingsContainer
    {
        public int? Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int? CreatedById { get; set; }

        public int? ProductId { get; set; }
        public string ListingsJSON { get; set; }
        public string ListingsToHideJSON { get; set; } // array of listings to hide

    }
    class ListingsContainerDbConfiguration : IEntityTypeConfiguration<ListingsContainer> {
        public void Configure(EntityTypeBuilder<ListingsContainer> modelBuilder) {
                
            modelBuilder.Property(item => item.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

            modelBuilder.HasIndex(item => item.ProductId);
        }
    }
}
