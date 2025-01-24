using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models
{
    public class RepairIncomingShipment
    {
        // public int? Id { get; set; }
        // public DateTime? CreatedAt { get; set; }
        // public DateTime? UpdatedAt { get; set; }
        // public int? CreatedById { get; set; }
        
        public int? RepairId { get; set; }
        public Repair Repair { get; set; }

        public int? IncomingShipmentId { get; set; }
        public IncomingShipment IncomingShipment { get; set; }
    }
    
    class RepairIncomingShipmentDBConfiguration : IEntityTypeConfiguration<RepairIncomingShipment>
    {
        public void Configure(EntityTypeBuilder<RepairIncomingShipment> modelBuilder)
        {
            modelBuilder.HasKey(t => new { t.RepairId, t.IncomingShipmentId });
            modelBuilder
                .HasOne(repairIncomingShipment => repairIncomingShipment.IncomingShipment)
                .WithMany(incomingShipment => incomingShipment.Repairs)
                .HasForeignKey(repairIncomingShipment => repairIncomingShipment.IncomingShipmentId);

            modelBuilder
                .HasOne(repairIncomingShipment => repairIncomingShipment.Repair)
                .WithMany(repair => repair.IncomingShipments)
                .HasForeignKey(repairIncomingShipment => repairIncomingShipment.RepairId);
        }
    }
}
