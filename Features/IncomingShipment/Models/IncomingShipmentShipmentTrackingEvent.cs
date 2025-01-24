using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models {
    public class IncomingShipmentShipmentTrackingEvent {

        public int? ShipmentTrackingEventId { get; set; }
        public ShipmentTrackingEvent ShipmentTrackingEvent { get; set; }
        
        
        public int? IncomingShipmentId { get; set; }
        public IncomingShipment IncomingShipment { get; set; }
    }

    class IncomingShipmentShipmentTrackingEventDBConfiguration : IEntityTypeConfiguration<IncomingShipmentShipmentTrackingEvent> {
        public void Configure(EntityTypeBuilder<IncomingShipmentShipmentTrackingEvent> modelBuilder) {
            modelBuilder.HasKey(item => new { item.IncomingShipmentId, item.ShipmentTrackingEventId });

            modelBuilder
                .HasOne(item => item.ShipmentTrackingEvent)
                .WithMany(item => item.IncomingShipmentShipmentTrackingEvents)
                .HasForeignKey(item => item.ShipmentTrackingEventId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder
                .HasOne(item => item.IncomingShipment)
                .WithMany(item => item.TrackingEvents)
                .HasForeignKey(item => item.IncomingShipmentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}