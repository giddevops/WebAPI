using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models {
    public class ShipmentTrackingEvent {
        public int? Id { get; set; }

        public int? Order { get; set; }

        public string Location { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string ExceptionCode { get; set; }
        public string ExceptionDescription { get; set; }

        public string City { get; set; }
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
        // public string IsResidential { get; set; }
        public string OrganizationName { get; set; }
        public string PostalCode { get; set; }
        public string StateProvinceCode { get; set; }
        public string Street { get; set; }
        // public string UrbanizationCode { get; set; }
        public DateTime? Date { get; set; }

        public List<IncomingShipmentShipmentTrackingEvent> IncomingShipmentShipmentTrackingEvents { get; set; }
        public List<OutgoingShipmentShipmentTrackingEvent> OutgoingShipmentShipmentTrackingEvents { get; set; }

    }

    class ShipmentTrackingEventDBConfiguration : IEntityTypeConfiguration<ShipmentTrackingEvent> {
        public void Configure(EntityTypeBuilder<ShipmentTrackingEvent> modelBuilder) {

        }
    }
}