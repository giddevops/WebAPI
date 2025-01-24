//not used anymore... hardcoding billing and shipping only into quote
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Metadata.Builders;

//namespace GidIndustrial.Gideon.WebApi.Models
//{
//    /// <summary>
//    /// This class creates the joining table
//    /// This is necessary because EF Core won't build it automatically otherwise
//    /// </summary>
//    public class QuoteAddress
//    {

//        public int? AddressId { get; set; }
//        public Address Address { get; set; }

//        public int QuoteId { get; set; }
//        public Quote Quote { get; set; }

//        public int QuoteAddressTypeId { get; set; }
//    }

//    /// <summary>
//    /// This sets up foreign keys in the database foreign keys
//    /// </summary>
//    class QuoteAddressDBConfiguration : IEntityTypeConfiguration<QuoteAddress>
//    {
//        public void Configure(EntityTypeBuilder<QuoteAddress> modelBuilder)
//        {
//            modelBuilder.HasKey(t => new { t.QuoteId, t.AddressId });

//            modelBuilder
//                .HasOne(QuoteAddress => QuoteAddress.Address);
//                // .WithMany(address => address.QuoteAddresses)
//                // .HasForeignKey(QuoteAddress => QuoteAddress.AddressId);

//            modelBuilder
//                .HasOne(QuoteAddress => QuoteAddress.Quote)
//                .WithMany(Quote => Quote.Addresses)
//                .HasForeignKey(QuoteAddress => QuoteAddress.QuoteId);
//        }
//    }
//}
