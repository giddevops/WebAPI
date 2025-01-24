using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models
{
    public class RmaIncomingShipment
    {
        // public int? Id { get; set; }
        // public DateTime? CreatedAt { get; set; }
        // public DateTime? UpdatedAt { get; set; }
        // public int? CreatedById { get; set; }
        
        public int? RmaId { get; set; }
        public Rma Rma { get; set; }

        public int? IncomingShipmentId { get; set; }
        public IncomingShipment IncomingShipment { get; set; }
    }
    
    class RmaIncomingShipmentDBConfiguration : IEntityTypeConfiguration<RmaIncomingShipment>
    {
        public void Configure(EntityTypeBuilder<RmaIncomingShipment> modelBuilder)
        {
            modelBuilder.HasKey(t => new { t.RmaId, t.IncomingShipmentId });
            modelBuilder
                .HasOne(rmaIncomingShipment => rmaIncomingShipment.IncomingShipment)
                .WithMany(incomingShipment => incomingShipment.Rmas)
                .HasForeignKey(rmaIncomingShipment => rmaIncomingShipment.IncomingShipmentId);

            modelBuilder
                .HasOne(rmaIncomingShipment => rmaIncomingShipment.Rma)
                .WithMany(rma => rma.IncomingShipments)
                .HasForeignKey(rmaIncomingShipment => rmaIncomingShipment.RmaId);
        }
    }
}
