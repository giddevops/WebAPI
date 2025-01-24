using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models
{
    public class OutgoingShipmentBox
    {
        public int? Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? CreatedById { get; set; }

        public OutgoingShipment OutgoingShipment { get; set; }
        public int? OutgoingShipmentId { get; set; }
        public List<OutgoingShipmentBoxInventoryItem> InventoryItems { get; set; }

        public float? Length { get; set; }
        public float? Width { get; set; }
        public float? Height { get; set; }

        public decimal? Weight { get; set; }

        public string TrackingNumber { get; set; }
        public DateTime? ExpectedArrival { get; set; }
        public DateTime? ActualArrival { get; set; }

    }

    /// <summary>
    /// set up createdAt, updatedAt and createdBy auto properties
    /// </summary>
    class OutgoingShipmentBoxDBConfiguration : IEntityTypeConfiguration<OutgoingShipmentBox>
    {
        public void Configure(EntityTypeBuilder<OutgoingShipmentBox> modelBuilder)
        {
            modelBuilder
                .Property(b => b.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");
            
            modelBuilder.HasOne(item => item.OutgoingShipment)
                .WithMany(item => item.Boxes)
                .HasForeignKey(item => item.OutgoingShipmentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}