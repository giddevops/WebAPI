using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AngleSharp.Css.Values;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models {
    public class Quote {
        public int Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? CreatedById { get; set; }

        public int? SalesPersonId { get; set; }
        public User SalesPerson { get; set; }

        public int? CompanyId { get; set; }
        public Company Company { get; set; }

        public int? ContactId { get; set; }
        public Contact Contact { get; set; }

        public string Phone { get; set; }
        public string Email { get; set; }

        public DateTime? Expiration { get; set; }

        public int? LeadId { get; set; }
        public Lead Lead { get; set; }

        [ForeignKey("CustomerTypeId")]
        public int? CustomerTypeId { get; set; }

        public string QuoteFormLink { get; set; }

        // public bool IsGIDEurope { get; set; }
        public GidLocationOption GidLocationOption { get; set; }
        public int? GidLocationOptionId { get; set; }

        // public int? ShippingMethodOptionId { get; set; }

        // public int? ShippingCarrierId { get; set; }
        // public int? ShippingCarrierShippingMethodId { get; set; }
        public int? ShippingMethodId { get; set; }
        public ShippingMethod ShippingMethod { get; set; }

        [Required]
        public int? CurrencyOptionId { get; set; }
        public CurrencyOption CurrencyOption { get; set; }

        public int? QuoteStatusOptionId { get; set; }
        public QuoteStatusOption QuoteStatusOption { get; set; }

        // public string Source { get; set; }

        public string CustomerNotes { get; set; }

        public Address ShippingAddress { get; set; }
        public Address BillingAddress { get; set; }

        public bool? CopyBillingToShipping { get; set; }

        public List<SalesOrder> SalesOrders { get; set; }

        public int? PaymentTermId { get; set; }
        public PaymentTerm PaymentTerm { get; set; }

        public bool Pipeline { get; set; }
        public bool OpportunityLost { get; set; }
        public int? QuoteLostReasonOptionId { get; set; }


        public int? SalesOrderPaymentMethodId { get; set; }
        public SalesOrderPaymentMethod SalesOrderPaymentMethod { get; set; }

        public decimal? SalesTax { get; set; }
        public decimal? WireTransferFee { get; set; }
        public decimal? ShippingAndHandlingFee { get; set; }
        public decimal? ExpediteFee { get; set; }
        public decimal? CreditCardFee { get; set; }

        public string ShippingAndHandlingDisplay
        {
            get
            { 
                return ShippingAndHandlingFee.HasValue && ShippingAndHandlingFee.Value > 0 ? (CurrencyOption != null ? CurrencyOption.Symbol  : String.Empty) + ShippingAndHandlingFee.Value.ToString("N2") : "TBD";
            }
        }

        public decimal Total { get; set; }
        public decimal? ProjectedProfit { get; set; }

        public List<QuoteNote> Notes { get; set; }
        public List<QuoteAttachment> Attachments { get; set; }
        public List<QuoteEventLogEntry> EventLogEntries { get; set; }
        public List<QuoteLineItem> LineItems { get; set; }
        public List<QuoteChatMessage> ChatMessages { get; set; }
        public List<QuoteContactLogItem> ContactLogItems { get; set; }

        public List<QuoteToDoItem> ToDoItems { get; set; }

        public string PhoneNumberFormatted
        {
            get
            {
                if (!String.IsNullOrWhiteSpace(Phone))
                {
                    var number = Phone;
                    number = Regex.Replace(number, "/[^\\d]/g", "");
                    number = number.Replace("-", "").Replace("+", "").Replace(" ", "");

                    if (number.Length < 3)
                        return number;
                    else if (number.Length < 7)
                        return "(" + number.Substring(0, 3) + ") " + number.Substring(3);
                    else if (number.Length < 11)
                        return "(" + number.Substring(0, 3) + ") " + number.Substring(3, 3) + "-" + number.Substring(6);
                    else
                    {
                        var mainNumber = number.Substring(number.Length - 10); //get last 10 digits
                        var countryCode = number.Substring(0, number.Length - 10);
                        return "+" + countryCode + " (" + mainNumber.Substring(0, 3) + ") " + mainNumber.Substring(3, 3) + "-" + mainNumber.Substring(6);
                    }
                }

                return String.Empty;
            }
        }


        public async Task<bool> CheckIfIsGidEurope(AppDBContext _context) {
            var GidEuropeOption = await _context.GidLocationOptions.FirstOrDefaultAsync(item => item.Value.Contains("Europe"));
            if (GidEuropeOption == null) {
                throw new Exception("Unable to find GID Europe option");
            }
            return this.GidLocationOptionId == GidEuropeOption.Id;
        }

        /// <summary>
        /// Updates the total of the quote but does NOT save to database. 
        /// This method will get the line items from the database if not already present.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<decimal> UpdateTotal(AppDBContext context){
            this.Total = await this.GetTotal(context);
            return this.Total;
        }

        public async Task<decimal> GetTotal(AppDBContext context){
            if(this.LineItems == null){
                this.LineItems = await context.QuoteLineItems.AsNoTracking().Where(item => item.QuoteId == this.Id).ToListAsync();
            }
            var total = (this.SalesTax ?? 0) + (this.WireTransferFee ?? 0) + (this.ShippingAndHandlingFee ?? 0) + (this.ExpediteFee ?? 0) + (this.CreditCardFee ?? 0);
            total += this.LineItems.Select(item => item.GetTotal()).Sum();
            return total;
        }

        public static async Task SetTotalForAllQuotes(AppDBContext dbContext){
            var dateTime = new DateTime(2018,10,1);

            var quotes = await dbContext.Quotes
                .Where(item => item.CreatedAt > dateTime)
                .Include(item => item.LineItems)
                .ToListAsync();
            foreach(var quote in quotes){
                await quote.UpdateTotal(dbContext);
                Console.WriteLine("Updating quote " + quote.Id);
            }
            await dbContext.SaveChangesAsync();
        }

        public async Task<bool> SortLineItems()
        {
            if(this.LineItems != null)
            {
                bool hasOrderedItems = false;

                foreach (var lineItem in this.LineItems)
                {
                    if (lineItem.Order > 0)
                    { 
                        hasOrderedItems = true;
                        break;
                    }
                }

                if(hasOrderedItems) {
                    List<QuoteLineItem> sortedItems = this.LineItems.OrderBy(i => i.Order).ToList();
                    this.LineItems = sortedItems;
                }
            }

            return true;
        }
    }


    /// <summary>
    /// set up createdAt, updatedAt and createdBy auto properties
    /// </summary>
    class QuoteDBConfiguration : IEntityTypeConfiguration<Quote> {
        public void Configure(EntityTypeBuilder<Quote> modelBuilder) {
            modelBuilder
                .Property(b => b.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            modelBuilder.HasOne(l => l.SalesPerson)
                .WithMany()
                .HasForeignKey(l => l.SalesPersonId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.HasOne(l => l.Company)
                .WithMany()
                .HasForeignKey(l => l.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.HasOne(l => l.Contact)
                .WithMany()
                .HasForeignKey(l => l.ContactId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.HasOne(item => item.CurrencyOption)
                .WithMany()
                .HasForeignKey(item => item.CurrencyOptionId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.HasOne(item => item.ShippingMethod)
                .WithMany()
                .HasForeignKey(item => item.ShippingMethodId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.HasOne(item => item.GidLocationOption)
                .WithMany()
                .HasForeignKey(item => item.GidLocationOptionId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.HasOne(item => item.QuoteStatusOption)
                .WithMany()
                .HasForeignKey(item => item.QuoteStatusOptionId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.HasOne(item => item.PaymentTerm).WithMany().HasForeignKey(item => item.PaymentTermId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.HasOne(item => item.SalesOrderPaymentMethod).WithMany().HasForeignKey(item => item.SalesOrderPaymentMethodId).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.HasIndex(item => item.Email);
            modelBuilder.HasIndex(item => item.Phone);
            modelBuilder.HasIndex(item => item.CreatedAt);
            modelBuilder.HasIndex(item => item.QuoteStatusOptionId);
            modelBuilder.HasIndex(item => item.Total);

            // modelBuilder
            //     .Property(b => b.UpdatedAt)
            //     .ValueGeneratedOnAddOrUpdate();

            // modelBuilder.Entity<Employee>()
            //     .Property(b => b.LastPayRaise)
            //     .Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;
        }
    }
}