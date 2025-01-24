using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuickBooks.Models;

namespace GidIndustrial.Gideon.WebApi.Models {
    public class Product {
        public int Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public string PartNumber { get; set; }
        public string PartNumberNoSpecialChars { 
            get {
                return this.PartNumber != null ? this.PartNumber.Trim().Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "") : null;
            }
            private set { } 
        }
        public string Description { get; set; }
        public string ShortDescription { get; set; }
        public string GidPartNumber { get; set; }
        public string GidPartNumberNoSpecialChars {
            get
            {
                return this.GidPartNumber != null ? this.GidPartNumber.Trim().Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "") : null;
            }
            private set { }
        }

        public int? ManufacturerId { get; set; }
        public Company Manufacturer { get; set; }

        public bool Serialized { get; set; }

        public bool IsPoorQuality { get; set; }

        public ProductType ProductType { get; set; }
        public int? ProductTypeId { get; set; }

        [NotMapped]
        public int? LeadCount { get; set; }
        [NotMapped]
        public int? QuoteCount { get; set; }
        [NotMapped]
        public int? SalesOrderCount { get; set; }
        [NotMapped]
        public int? PurchaseOrderCount { get; set; }

        public string QuickBooksId { get; set; }
        public string QuickBooksSyncToken { get; set; }

        // public int? ProductSerializedOptionId { get; set; }
        public int? ProductEndOfLifeOptionId { get; set; }
        public int? ProductCompositeItemOptionId { get; set; }
        public List<ProductNote> Notes { get; set; }
        public List<ProductAttachment> Attachments { get; set; }

        public List<QuoteLineItem> QuoteLineItems { get; set; }
        public List<InvoiceLineItem> InvoiceLineItems { get; set; }
        public List<BillLineItem> BillLineItems { get; set; }
        public List<ProductAlias> Aliases { get; set; }
        public string AliasesCache { get; set; }
        public List<ProductKitItem> ProductKitItems { get; set; }

        public List<ProductAttributeValue> AttributeValues { get; set; }

        // public async dynamic ProductOrProductAliasExists(AppDBContext _context, string partNumber, string manufacturerName){
        //     //first check products
        //     // var query = from _context.Products select
        //     return null;
        // }

        public async Task GetCounts(AppDBContext _context) {
            this.LeadCount = await _context.Leads.Where(item => item.LineItems.Any(leadLineItem => leadLineItem.ProductId == this.Id)).CountAsync();
            this.QuoteCount = await _context.Quotes.Where(item => item.LineItems.Any(quoteLineItem => quoteLineItem.ProductId == this.Id)).CountAsync();
            this.SalesOrderCount = await _context.SalesOrders.Where(item => item.LineItems.Any(salesOrderLineItem => salesOrderLineItem.ProductId == this.Id)).CountAsync();
            this.PurchaseOrderCount = await _context.PurchaseOrders.Where(item => item.LineItems.Any(purchaseOrderLineItem => purchaseOrderLineItem.ProductId == this.Id)).CountAsync();
        }

        public async Task<QuickBooksSyncResult> EnsureInQuickBooks(QuickBooksConnector quickBooksConnector, AppDBContext _context) {
            if (!String.IsNullOrWhiteSpace(this.QuickBooksId)) {
                return new QuickBooksSyncResult {
                    Succeeded = true,
                    Message = "Product already has a quickbooks Id so program is assuming it must already in quickbooks"
                };
            }
            //first check by name for duplicates
            Newtonsoft.Json.Linq.JObject existingProductInfoResponseData = await quickBooksConnector.QueryResource("item", $"SELECT * FROM Item WHERE Name='{this.PartNumber}'");
            var existingProductInfo = existingProductInfoResponseData.ToObject<QuickBooks.Models.ItemQueryResponseContainer>();
            if (existingProductInfo.QueryResponse.Item != null && existingProductInfo.QueryResponse.Item.Count > 0) {
                this.QuickBooksId = existingProductInfo.QueryResponse.Item[0].Id;
                this.QuickBooksSyncToken = existingProductInfo.QueryResponse.Item[0].SyncToken;
                await _context.SaveChangesAsync();
                return new QuickBooksSyncResult {
                    Succeeded = true,
                    Message = "Product was found in quickbooks already, so the quickbooks Id was copied into the local product"
                };
            }


            try {
                // var parent = await _context.Products.Include(item => item..FirstOrDefaultAsync(item => item.Id == this.Id);
                Newtonsoft.Json.Linq.JObject responseData = await quickBooksConnector.PostResource("item", new QuickBooks.Models.Item {
                    Id = this.QuickBooksId,
                    SyncToken = this.QuickBooksSyncToken,
                    Name = this.PartNumber,
                    Type = "NonInventory",
                    Sku = this.GidPartNumber,
                    // Taxable = false,
                    TrackQtyOnHand = false,
                    AssetAccountRef = new QuickBooks.Models.AssetAccountRef {
                        value = QuickBooksConnector.DefaultProductAssetAccountId
                    },
                    ExpenseAccountRef = new QuickBooks.Models.ExpenseAccountRef {
                        value = QuickBooksConnector.DefaultProductExpenseAccountId
                    },
                    InvStartDate = "2000-01-01",
                    Description = this.Description,
                    IncomeAccountRef = new QuickBooks.Models.IncomeAccountRef {
                        value = QuickBooksConnector.DefaultProductIncomingAccountId
                    },
                    ItemCategoryType = "Product",

                });
                var newItem = responseData.ToObject<QuickBooks.Models.ItemResponse>().Item;
                this.QuickBooksId = newItem.Id;
                this.QuickBooksSyncToken = newItem.SyncToken;
                _context.Entry(this).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return new QuickBooksSyncResult {
                    Succeeded = true,
                    Message = "Product was added to quickbooks"
                };
            }
            catch (Exception ex) {
                return new QuickBooksSyncResult {
                    Succeeded = false,
                    Message =  "Error adding product to quickbooks.  Product Id is " + this.Id + ".  The message is: " + ex.Message,
                    AdditionalData = ex.StackTrace
                };
            }
        }

    }

    /// <summary>
    /// set up createdAt, fluent api config
    /// </summary>
    class ProductDbConfiguration : IEntityTypeConfiguration<Product> {
        public void Configure(EntityTypeBuilder<Product> modelBuilder) {
            modelBuilder
                .Property(b => b.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            modelBuilder.HasOne(l => l.Manufacturer)
                .WithMany()
                .HasForeignKey(l => l.ManufacturerId);

            modelBuilder.HasOne(item => item.ProductType)
                .WithMany()
                .HasForeignKey(item => item.ProductTypeId);

            modelBuilder.HasIndex(item => item.PartNumber);
            modelBuilder.HasIndex(item => item.CreatedAt);
            modelBuilder.HasIndex(item => item.GidPartNumber);
            modelBuilder.HasIndex(item => item.PartNumberNoSpecialChars);
            

            modelBuilder.HasMany(item => item.Aliases)
                .WithOne(item => item.Product)
                .HasForeignKey(item => item.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}