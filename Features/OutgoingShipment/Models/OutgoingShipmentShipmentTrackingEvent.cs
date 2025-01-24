using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models {
    public class OutgoingShipmentShipmentTrackingEvent {
        public int? ShipmentTrackingEventId { get; set; }
        public ShipmentTrackingEvent ShipmentTrackingEvent { get; set; }
        public int? OutgoingShipmentId { get; set; }
        public OutgoingShipment OutgoingShipment { get; set; }
    }

    class OutgoingShipmentShipmentTrackingEventDBConfiguration : IEntityTypeConfiguration<OutgoingShipmentShipmentTrackingEvent> {
        public void Configure(EntityTypeBuilder<OutgoingShipmentShipmentTrackingEvent> modelBuilder) {
            modelBuilder.HasKey(item => new { item.OutgoingShipmentId, item.ShipmentTrackingEventId });
            modelBuilder.HasOne(item => item.ShipmentTrackingEvent).WithMany(item => item.OutgoingShipmentShipmentTrackingEvents).HasForeignKey(item => item.OutgoingShipmentId);
            modelBuilder.HasOne(item => item.OutgoingShipment).WithMany(item => item.TrackingEvents).HasForeignKey(item => item.OutgoingShipmentId);
        }
    }
}