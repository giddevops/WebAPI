// using System;
// using System.Collections.Generic;
// using System.ComponentModel.DataAnnotations.Schema;
// using System.Linq;
// using System.Threading.Tasks;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.EntityFrameworkCore.Metadata.Builders;

// namespace GidIndustrial.Gideon.WebApi.Models
// {
//     /// <summary>
//     /// This class creates the joining table for SalesOrders and purchaseOrders
//     /// This is necessary because EF Core won't build it automatically otherwise
//     /// </summary>
//     public class SalesOrderQuote
//     {
//         public int? SalesOrderId { get; set; }
//         public SalesOrder SalesOrder { get; set; }

//         public int? QuoteId { get; set; }
//         public Quote Quote { get; set; }
//     }
    
//     /// <summary>
//     /// This sets up foreign keys in the database connecting SalesOrders and purchaseOrders many to many relationship
//     /// </summary>
//     class SalesOrderQuoteDBConfiguration : IEntityTypeConfiguration<SalesOrderQuote>
//     {
//         public void Configure(EntityTypeBuilder<SalesOrderQuote> modelBuilder)
//         {
//             modelBuilder
//             .HasKey(t => new { t.QuoteId, t.SalesOrderId });

//             modelBuilder
//                 .HasOne(salesOrderQuote => salesOrderQuote.SalesOrder)
//                 .WithMany(SalesOrder => SalesOrder.Quotes)
//                 .HasForeignKey(salesOrderQuote => salesOrderQuote.SalesOrderId)
//                 .OnDelete(DeleteBehavior.Restrict);

//             modelBuilder
//                 .HasOne(salesOrderQuote => salesOrderQuote.Quote)
//                 .WithMany(purchaseOrder => purchaseOrder.SalesOrders)
//                 .HasForeignKey(salesOrderQuote => salesOrderQuote.QuoteId)
//                 .OnDelete(DeleteBehavior.Restrict);
//         }
//     }
// }
