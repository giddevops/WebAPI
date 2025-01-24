// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.EntityFrameworkCore.Metadata.Builders;

// namespace GidIndustrial.Gideon.WebApi.Models
// {
//     /// <summary>
//     /// This class creates the joining table
//     /// This is necessary because EF Core won't build it automatically otherwise
//     /// </summary>
//     public class QuoteEmailAddress
//     {
        
//         public int? EmailAddressId { get; set; }
//         public EmailAddress EmailAddress { get; set; }

//         public int QuoteId { get; set; }
//         public Quote Quote { get; set; }

//         public int QuoteEmailAddressTypeId { get; set; }
//     }

//     /// <summary>
//     /// This sets up foreign keys in the database foreign keys
//     /// </summary>
//     class QuoteEmailAddressDBConfiguration : IEntityTypeConfiguration<QuoteEmailAddress>
//     {
//         public void Configure(EntityTypeBuilder<QuoteEmailAddress> modelBuilder)
//         {
//             modelBuilder.HasKey(t => new { t.QuoteId, t.EmailAddressId });

//             modelBuilder
//                 .HasOne(quoteEmailAddress => quoteEmailAddress.EmailAddress);
//                 // .WithMany(emailAddress => emailAddress.QuoteEmailAddresses)
//                 // .HasForeignKey(quoteAddress => quoteAddress.EmailAddressId);

//             modelBuilder
//                 .HasOne(quoteEmailAddress => quoteEmailAddress.Quote)
//                 .WithMany(quote => quote.EmailAddresses)
//                 .HasForeignKey(quoteEmailAddress => quoteEmailAddress.QuoteId);
//         }
//     }
// }
