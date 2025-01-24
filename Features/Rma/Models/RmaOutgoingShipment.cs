using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models
{
    /// <summary>
    /// This class creates the joining table for Rmas and inventoryItems
    /// This is necessary because EF Core won't build it automatically otherwise
    /// </summary>
    public class RmaOutgoingShipment
    {
        public int? RmaId { get; set; }
        public Rma Rma { get; set; }

        public int? OutgoingShipmentId { get; set; }
        public OutgoingShipment OutgoingShipment { get; set; }
    }
    
    /// <summary>
    /// This sets up foreign keys in the database connecting Rmas and inventoryItems many to many relationship
    /// </summary>
    class RmaOutgoingShipmentDBConfiguration : IEntityTypeConfiguration<RmaOutgoingShipment>
    {
        public void Configure(EntityTypeBuilder<RmaOutgoingShipment> modelBuilder)
        {
            modelBuilder
            .HasKey(t => new { t.OutgoingShipmentId, t.RmaId });

            modelBuilder
                .HasOne(rmaOutgoingShipment => rmaOutgoingShipment.Rma)
                .WithMany(Rma => Rma.OutgoingShipments)
                .HasForeignKey(rmaOutgoingShipment => rmaOutgoingShipment.RmaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder
                .HasOne(rmaOutgoingShipment => rmaOutgoingShipment.OutgoingShipment)
                .WithOne(item => item.RmaOutgoingShipment);
                // .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
