using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models {
    public class Repair {
        public int? Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int? CreatedById { get; set; }

        public int? SalesOrderId { get; set; }
        public SalesOrder SalesOrder { get; set; }

        public DateTime? DateIssued { get; set; }
        public List<RepairIncomingShipment> IncomingShipments { get; set; }

        public int? RepairAuthorizationAttachmentId { get; set; }
        public Attachment Attachment { get; set; }
    }

    class RepairDBConfiguration : IEntityTypeConfiguration<Repair> {
        public void Configure(EntityTypeBuilder<Repair> modelBuilder) {
            modelBuilder.HasOne(item => item.SalesOrder).WithMany(item => item.Repairs).HasForeignKey(item => item.SalesOrderId);
            modelBuilder.HasOne(item => item.Attachment).WithMany(item => item.Repairs).HasForeignKey(item => item.RepairAuthorizationAttachmentId).OnDelete(DeleteBehavior.SetNull);
        }
    }
}
