using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.IdentityModel.Protocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GidIndustrial.Gideon.WebApi.Models;
using QuickBooks.Models;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Reflection;

namespace GidIndustrial.Gideon.WebApi.Models {

    public class ApplicationContextDbFactory : IDesignTimeDbContextFactory<AppDBContext>
    {
        AppDBContext IDesignTimeDbContextFactory<AppDBContext>.CreateDbContext(string[] args)
        {

            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
                .AddJsonFile("appsettings.Development.json")
                .Build();

            Console.WriteLine("The location is " + Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));

            var optionsBuilder = new DbContextOptionsBuilder<AppDBContext>();
            optionsBuilder.UseSqlServer<AppDBContext>(config.GetConnectionString("GideonLocalDB"));

            return new AppDBContext(optionsBuilder.Options);
        }
    }
    public partial class AppDBContext : DbContext {
        public DbSet<AccessLogItem> AccessLogItems { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Attachment> Attachments { get; set; }
        public DbSet<AttachmentType> AttachmentTypes { get; set; }
        public DbSet<AuthenticationToken> AuthenticationTokens { get; set; }
        public DbSet<BankAccount> BankAccounts { get; set; }
        public DbSet<Bill> Bills { get; set; }
        public DbSet<BillAttachment> BillAttachments { get; set; }
        public DbSet<BillCashDisbursement> BillCashDisbursements { get; set; }
        public DbSet<BillLineItem> BillLineItems { get; set; }
        public DbSet<CashAccount> CashAccounts { get; set; }
        public DbSet<CashDisbursement> CashDisbursements { get; set; }
        public DbSet<CashDisbursementReasonOption> CashDisbursementReasonOptions { get; set; }
        public DbSet<CashReceipt> CashReceipts { get; set; }
        public DbSet<CashReceiptType> CashReceiptTypes { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<ChatMessageAttachment> ChatMessageAttachments { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<CompanyAddress> CompanyAddresses { get; set; }
        public DbSet<CompanyAddressType> CompanyAddressTypes { get; set; }
        public DbSet<CompanyAlias> CompanyAliases { get; set; }
        public DbSet<CompanyAttachment> CompanyAttachments { get; set; }
        public DbSet<CompanyContact> CompanyContacts { get; set; }
        public DbSet<CompanyContactRelationshipType> CompanyContactRelationshipTypes { get; set; }
        public DbSet<CompanyCompany> CompanyCompanies { get; set; }
        public DbSet<CompanyCompanyRelationshipType> CompanyCompanyRelationshipTypes { get; set; }
        public DbSet<CompanyEmailAddress> CompanyEmailAddresses { get; set; }
        public DbSet<CompanyEmailAddressType> CompanyEmailAddressTypes { get; set; }
        public DbSet<CompanyEventLogEntry> CompanyEventLogEntries { get; set; }
        public DbSet<CompanyNote> CompanyNotes { get; set; }
        public DbSet<CompanyPhoneNumber> CompanyPhoneNumbers { get; set; }
        public DbSet<CompanyPhoneNumberType> CompanyPhoneNumberTypes { get; set; }
        public DbSet<CompanyPortal> CompanyPortals { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<ContactAddress> ContactAddresses { get; set; }
        public DbSet<ContactAddressType> ContactAddressTypes { get; set; }
        public DbSet<ContactAttachment> ContactAttachments { get; set; }
        public DbSet<ContactEmailAddress> ContactEmailAddresses { get; set; }
        public DbSet<ContactEmailAddressType> ContactEmailAddressTypes { get; set; }
        public DbSet<ContactEventLogEntry> ContactEventLogEntries { get; set; }
        public DbSet<ContactNote> ContactNotes { get; set; }
        public DbSet<ContactPhoneNumber> ContactPhoneNumbers { get; set; }
        public DbSet<ContactPhoneNumberType> ContactPhoneNumberTypes { get; set; }
        public DbSet<Config> Configs { get; set; }
        public DbSet<ContactMethodOption> ContactMethodOptions { get; set; }
        public DbSet<ContactReasonOption> ContactReasonOptions { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<CpuStockVerifiedOption> CpuStockVerifiedOptions { get; set; }
        public DbSet<Credit> Credits { get; set; }
        public DbSet<CreditAccount> CreditAccounts { get; set; }
        public DbSet<CreditCardTransaction> CreditCardTransactions { get; set; }
        public DbSet<CreditCard> CreditCards { get; set; }
        public DbSet<ContactLogItem> ContactLogItems { get; set; }
        public DbSet<ChatMessageUserMention> ChatMessageUserMentions { get; set; }
        // public DbSet<CreditCardCharge> CreditCardCharges { get; set; }
        public DbSet<CurrencyOption> CurrencyOptions { get; set; }
        public DbSet<CustomerType> CustomerTypes { get; set; }
        public DbSet<EmailAddress> EmailAddresses { get; set; }
        public DbSet<EmailAddressType> EmailAddressTypes { get; set; }
        public DbSet<EmailTemplate> EmailTemplates { get; set; }
        public DbSet<EmailTemplateType> EmailTemplateTypes { get; set; }
        public DbSet<EndScannerLabel> EndScannerLabels { get; set; }
        public DbSet<EventLogEntry> EventLogEntries { get; set; }
        public DbSet<GLAccount> GLAccounts { get; set; }
        public DbSet<GidLocationOption> GidLocationOptions { get; set; }
        public DbSet<GidSubLocationOption> GidSubLocationOptions { get; set; }
        public DbSet<IncomingShipment> IncomingShipments { get; set; }
        public DbSet<IncomingShipmentAttachment> IncomingShipmentAttachments { get; set; }
        public DbSet<IncomingShipmentInventoryItem> IncomingShipmentInventoryItems { get; set; }
        public DbSet<IncomingShipmentShipmentTrackingEvent> IncomingShipmentShipmentTrackingEvents { get; set; }
        public DbSet<InventoryItem> InventoryItems { get; set; }
        public DbSet<InventoryItemAttachment> InventoryItemAttachments { get; set; }
        public DbSet<InventoryItemConditionOption> InventoryItemConditionOptions { get; set; }
        public DbSet<InventoryItemEventLogEntry> InventoryItemEventLogEntries { get; set; }
        public DbSet<InventoryItemLocationOption> InventoryItemLocationOptions { get; set; }
        public DbSet<InventoryItemNote> InventoryItemNotes { get; set; }
        public DbSet<InventoryItemRelatedInventoryItem> InventoryItemRelatedInventoryItems { get; set; }
        public DbSet<InventoryItemStatusOption> InventoryItemStatusOptions { get; set; }
        // public DbSet<InventoryItemWorkLogItem> InventoryItemWorkLogItems { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceAttachment> InvoiceAttachments { get; set; }
        public DbSet<InvoiceEventLogEntry> InvoiceEventLogEntries { get; set; }
        public DbSet<InvoiceCashReceipt> InvoiceCashReceipts { get; set; }
        public DbSet<InvoiceChatMessage> InvoiceChatMessages { get; set; }
        public DbSet<InvoiceCredit> InvoiceCredits { get; set; }
        public DbSet<InvoiceLineItem> InvoiceLineItems { get; set; }
        public DbSet<InvoiceSyncLogItem> InvoiceSyncLogItems { get; set; }
        public DbSet<Lead> Leads { get; set; }
        public DbSet<LeadAttachment> LeadAttachments { get; set; }
        public DbSet<LeadChatMessage> LeadChatMessages { get; set; }
        public DbSet<LeadContactLogItem> LeadContactLogItems { get; set; }
        public DbSet<LeadEventLogEntry> LeadEventLogEntries { get; set; }
        public DbSet<LeadFilter> LeadFilters { get; set; }
        public DbSet<LeadLineItem> LeadLineItems { get; set; }
        public DbSet<LeadLineItemSource> LeadLineItemSources { get; set; }
        public DbSet<LeadNote> LeadNotes { get; set; }
        public DbSet<LeadOriginOption> LeadOriginOptions { get; set; }
        public DbSet<LeadRoutingAction> LeadRoutingActions { get; set; }
        public DbSet<LeadRoutingRule> LeadRoutingRules { get; set; }
        public DbSet<LeadRoutingRuleCountry> LeadRoutingRuleCountries { get; set; }
        public DbSet<LeadRoutingRuleCompanyName> LeadRoutingRuleCompanyNames { get; set; }
        public DbSet<LeadRoutingRuleLeadWebsite> LeadRoutingRuleLeadWebsites { get; set; }
        public DbSet<LeadRoutingRuleLineItemServiceType> LeadRoutingRuleLineItemServiceTypes { get; set; }
        public DbSet<LeadRoutingRuleProductType> LeadRoutingRuleProductTypes { get; set; }
        public DbSet<LeadStatusOption> LeadStatusOptions { get; set; }
        public DbSet<LeadWebsite> LeadWebsites { get; set; }
        public DbSet<LineItemConditionType> LineItemConditionTypes { get; set; }
        public DbSet<LineItemServiceType> LineItemServiceTypes { get; set; }
        public DbSet<ListingsContainer> ListingsContainers { get; set; }
        public DbSet<Note> Notes { get; set; }
        public DbSet<OutgoingLineItemLeadTimeOption> OutgoingLineItemLeadTimeOptions { get; set; }
        public DbSet<OutgoingLineItemWarrantyOption> OutgoingLineItemWarrantyOptions { get; set; }
        public DbSet<OutgoingShipment> OutgoingShipments { get; set; }
        public DbSet<OutgoingShipmentBox> OutgoingShipmentBoxes { get; set; }
        public DbSet<OutgoingShipmentBoxDimensionOption> OutgoingShipmentBoxDimensionOptions { get; set; }
        public DbSet<OutgoingShipmentBoxInventoryItem> OutgoingShipmentBoxInventoryItems { get; set; }
        public DbSet<OutgoingShipmentShippingTermOption> OutgoingShipmentShippingTermOptions { get; set; }
        public DbSet<OutgoingShipmentShipmentTrackingEvent> OutgoingShipmentShipmentTrackingEvents { get; set; }
        public DbSet<PaymentAccount> PaymentAccounts { get; set; }
        public DbSet<PaymentProfileCode> PaymentProfileCodes { get; set; }
        public DbSet<PaymentTerm> PaymentTerms { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<PhoneNumber> PhoneNumbers { get; set; }
        public DbSet<PhoneNumberType> PhoneNumberTypes { get; set; }
        public DbSet<Portal> Portals { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductAlias> ProductAliases { get; set; }
        public DbSet<ProductAliasType> ProductAliasTypes { get; set; }
        public DbSet<ProductAttachment> ProductAttachments { get; set; }
        public DbSet<ProductCompositeItemOption> ProductCompositeItemOptions { get; set; }
        public DbSet<ProductConditionOption> ProductConditionOptions { get; set; }
        public DbSet<ProductEndOfLifeOption> ProductEndOfLifeOptions { get; set; }
        public DbSet<ProductNote> ProductNotes { get; set; }
        public DbSet<ProductType> ProductTypes { get; set; }
        public DbSet<ProductTypeAlias> ProductTypeAliases { get; set; }
        public DbSet<ProductAttribute> ProductAttributes { get; set; }
        public DbSet<ProductAttributeValue> ProductAttributeValues { get; set; }
        public DbSet<ProductAttributeValueOption> ProductAttributeValueOptions { get; set; }
        public DbSet<ProductKitItem> ProductKitItems { get; set; }
        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public DbSet<PurchaseOrderAttachment> PurchaseOrderAttachments { get; set; }
        public DbSet<PurchaseOrderChatMessage> PurchaseOrderChatMessages { get; set; }
        public DbSet<PurchaseOrderEventLogEntry> PurchaseOrderEventLogEntries { get; set; }
        public DbSet<PurchaseOrderFilter> PurchaseOrderFilters { get; set; }
        public DbSet<PurchaseOrderIncomingShipment> PurchaseOrderIncomingShipments { get; set; }
        public DbSet<PurchaseOrderLineItem> PurchaseOrderLineItems { get; set; }
        public DbSet<PurchaseOrderNote> PurchaseOrderNotes { get; set; }
        public DbSet<PurchaseOrderPaymentMethod> PurchaseOrderPaymentMethods { get; set; }
        public DbSet<PurchaseOrderReasonOption> PurchaseOrderReasonOptions { get; set; }
        public DbSet<PurchaseOrderStatusOption> PurchaseOrderStatusOptions { get; set; }
        public DbSet<PurchaseOrderExpectedShipDateChangeReason> PurchaseOrderExpectedShipDateChangeReasons { get; set; }
        public DbSet<QuickBooksInvoice> QuickBooksInvoices { get; set; }
        public DbSet<QuickBooksPayment> QuickBooksPayments { get; set; }
        public DbSet<Quote> Quotes { get; set; }
        public DbSet<QuoteAddressType> QuoteAddressTypes { get; set; }
        public DbSet<QuoteAttachment> QuoteAttachments { get; set; }
        public DbSet<QuoteChatMessage> QuoteChatMessages { get; set; }
        public DbSet<QuoteEventLogEntry> QuoteEventLogEntries { get; set; }
        public DbSet<QuoteFilter> QuoteFilters { get; set; }
        public DbSet<QuoteLineItem> QuoteLineItems { get; set; }
        public DbSet<QuoteLostReasonOption> QuoteLostReasonOptions { get; set; }
        public DbSet<QuoteLineItemSource> QuoteLineItemSources { get; set; }
        public DbSet<QuoteNote> QuoteNotes { get; set; }
        public DbSet<QuoteStatusOption> QuoteStatusOptions { get; set; }
        public DbSet<QuoteContactLogItem> QuoteContactLogItems { get; set; }
        public DbSet<RequiredDeliveryTimeOption> RequiredDeliveryTimeOptions { get; set; }
        public DbSet<Repair> Repairs { get; set; }
        public DbSet<Rma> Rmas { get; set; }
        public DbSet<RmaActionOption> RmaActionOptions { get; set; }
        public DbSet<RmaAttachment> RmaAttachments { get; set; }
        public DbSet<RmaChatMessage> RmaChatMessages { get; set; }
        public DbSet<RmaEventLogEntry> RmaEventLogEntries { get; set; }
        public DbSet<RmaFilter> RmaFilters { get; set; }
        public DbSet<RmaIncomingShipment> RmaIncomingShipments { get; set; }
        public DbSet<RmaLineItem> RmaLineItems { get; set; }
        public DbSet<RmaOutgoingShipment> RmaOutgoingShipments { get; set; }
        public DbSet<RmaReasonOption> RmaReasonOptions { get; set; }
        public DbSet<RmaStatusOption> RmaStatusOptions { get; set; }
        public DbSet<RmaNote> RmaNotes { get; set; }
        public DbSet<SalesOrder> SalesOrders { get; set; }
        public DbSet<SalesOrderAttachment> SalesOrderAttachments { get; set; }
        public DbSet<SalesOrderChatMessage> SalesOrderChatMessages { get; set; }
        public DbSet<SalesOrderEventLogEntry> SalesOrderEventLogEntries { get; set; }
        public DbSet<SalesOrderFilter> SalesOrderFilters { get; set; }
        public DbSet<SalesOrderInventoryItem> SalesOrderInventoryItems { get; set; }
        public DbSet<RepairIncomingShipment> RepairIncomingShipments { get; set; }
        public DbSet<SalesOrderLineItem> SalesOrderLineItems { get; set; }
        public DbSet<SalesOrderLineItemInventoryItem> SalesOrderLineItemInventoryItems { get; set; }
        public DbSet<SalesOrderLineItemSource> SalesOrderLineItemSources { get; set; }
        public DbSet<SalesOrderNote> SalesOrderNotes { get; set; }
        public DbSet<SalesOrderOutgoingShipment> SalesOrderOutgoingShipments { get; set; }
        public DbSet<SalesOrderPaymentMethod> SalesOrderPaymentMethods { get; set; }
        public DbSet<SalesOrderPurchaseOrder> SalesOrderPurchaseOrders { get; set; }
        public DbSet<SalesOrderStatusOption> SalesOrderStatusOptions { get; set; }
        public DbSet<ShippingCarrier> ShippingCarriers { get; set; }
        public DbSet<ShippingCarrierShippingMethod> ShippingCarrierShippingMethods { get; set; }
        public DbSet<ShippingMethod> ShippingMethods { get; set; }
        public DbSet<ShippingType> ShippingTypes { get; set; }
        public DbSet<ShipmentTrackingEvent> ShipmentTrackingEvents { get; set; }
        public DbSet<Source> Sources { get; set; }
        public DbSet<SourceAttachment> SourceAttachments { get; set; }
        public DbSet<SourceLeadTimeOption> SourceLeadTimeOptions { get; set; }
        public DbSet<SourceNote> SourceNotes { get; set; }
        public DbSet<SourceWarrantyOption> SourceWarrantyOptions { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserGroup> UserGroups { get; set; }
        public DbSet<UserUserGroup> UserUserGroups { get; set; }
        public DbSet<View> Views { get; set; }
        public DbSet<ViewConditionalFormatter> ViewConditionalFormatters { get; set; }
        public DbSet<ViewConditionalFormatterCondition> ViewConditionalFormatterConditions { get; set; }
        public DbSet<ViewDisplayField> ViewDisplayFields { get; set; }
        public DbSet<ViewFilter> ViewFilters { get; set; }
        public DbSet<ViewObjectNameOption> ViewObjectNameOptions { get; set; }
        public DbSet<ViewCacheItem> ViewCacheItems { get; set; }
        public DbSet<WorkLogItem> WorkLogItems { get; set; }
        public DbSet<WorkLogItemActivityOption> WorkLogItemActivityOptions { get; set; }
        public DbSet<QuickBooksLogItem> QuickBooksLogItems { get; set; }
        // public DbSet<QuickBooksMetaData> QuickBooksMetaDatas { get; set; }

        public DbSet<ToDoItem> ToDoItems { get; set; }
        public DbSet<ToDoTypeOption> ToDoTypeOptions { get; set; }

        public DbSet<QuoteToDoItem> QuoteToDoItems { get; set; }

        public DbSet<LeadToDoItem> LeadToDoItems { get; set; }

        public DbSet<SalesOrderToDoItem> SalesOrderToDoItems { get; set; }

        public DbSet<PurchaseOrderToDoItem> PurchaseOrderToDoItems { get; set; }

        public DbSet<RMAToDoItem> RMAToDoItems { get; set; }

        public DbSet<Scan> Scans { get; set; }
        public DbSet<ScanGroup> ScanGroups { get; set; }
        public DbSet<Scanner> Scanners { get; set; }
        public DbSet<ScannerActionRelatePieceParts> ScannerActionsRelatePieceParts { get; set; }
        public DbSet<ScannerActionUpdateLocation> ScannerActionsUpdateLocation { get; set; }
        public DbSet<ScannerActionUpdateSystemData> ScannerActionsUpdateSystemData { get; set; }
        public DbSet<ScannerActionUpdateWorkLog> ScannerActionsUpdateWorkLog { get; set; }
        public DbSet<ScannerActionUpdateSystemDataCommand> ScannerActionUpdateSystemDataCommands { get; set; }
        // public DbSet<ScannerEvent> ScannerEvents { get; set; }
        // public DbSet<ScannerEventScannerLabelType> ScannerEventScannerLabelTypes { get; set; }
        public DbSet<ScannerLabel> ScannerLabels { get; set; }
        // public DbSet<ScannerLabelLogEntry> ScannerLabelLogEntries { get; set; }
        public DbSet<ScannerLabelType> ScannerLabelTypes { get; set; }
        public DbSet<ScannerLabelTypeVariable> ScannerLabelTypeVariables { get; set; }
        public DbSet<ScannerStation> ScannerStations { get; set; }
        public DbSet<ScannerStationLogEntry> ScannerStationLogEntries { get; set; }



        /// <summary>
        /// This constructor uses Dependency Injection
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options) {
        }

        // public AppDBContext() { }

        /// <summary>
        /// This is not used right now. The db connection is set up in Startup.cs
        /// </summary>
        /// <param name="optionsBuilder"></param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.EnableSensitiveDataLogging();
            // string conString;
            // if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development") {
            //     Environment.Get
            //     conString = System.Configuration.ConfigurationManager.ConnectionStrings["GideonLocalDB"].ConnectionString;
            // } else {
            //     conString = System.Configuration.ConfigurationManager.ConnectionStrings["ProductionDB"].ConnectionString;
            // }
            // optionsBuilder.UseSqlServer(conString);
            // optionsBuilder.UseSqlite("Data Source=gidcrm.db");
            // optionsBuilder.UseSqlServer(ConfigurationManager.ConnectionStrings["GideonLocalDB"].ConnectionString);
        }

        public bool Exists<T>(T entity) where T : class {
            return this.Set<T>().Local.Any(e => e == entity);
        }

        /// <summary>
        /// This function runs configuration that sets up join tables for various classes
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            //make table names singular
            foreach (var entityType in modelBuilder.Model.GetEntityTypes()) {
                // Skip shadow types
                if (entityType.ClrType == null)
                    continue;
                entityType.Relational().TableName = entityType.ClrType.Name;
            }
            foreach (var property in modelBuilder.Model.GetEntityTypes().SelectMany(t => t.GetProperties()).Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?))) {
                property.Relational().ColumnType = "decimal(18, 2)";
            }

            // modelBuilder.ApplyConfiguration(new CreditCardChargeDBConfiguration());
            modelBuilder.ApplyConfiguration(new AccessLogItemDBConfiguration());
            modelBuilder.ApplyConfiguration(new AddressDBConfiguration());
            modelBuilder.ApplyConfiguration(new AttachmentDBConfiguration());
            modelBuilder.ApplyConfiguration(new AuthenticationTokenDBConfiguration());
            modelBuilder.ApplyConfiguration(new EndScannerLabelDBConfiguration());
            modelBuilder.ApplyConfiguration(new BillAttachmentDBConfiguration());
            modelBuilder.ApplyConfiguration(new BillCashDisbursementDBConfiguration());
            modelBuilder.ApplyConfiguration(new BillChatMessageDBConfiguration());
            modelBuilder.ApplyConfiguration(new BillDBConfiguration());
            modelBuilder.ApplyConfiguration(new BillLineItemDBConfiguration());
            modelBuilder.ApplyConfiguration(new CashDisbursementDBConfiguration());
            modelBuilder.ApplyConfiguration(new CashReceiptDBConfiguration());
            modelBuilder.ApplyConfiguration(new ChatMessageAttachmentDBConfiguration());
            modelBuilder.ApplyConfiguration(new ChatMessageDBConfiguration());
            modelBuilder.ApplyConfiguration(new CompanyAddressDBConfiguration());
            modelBuilder.ApplyConfiguration(new CompanyAliasDBConfiguration());
            modelBuilder.ApplyConfiguration(new CompanyAttachmentDBConfiguration());
            modelBuilder.ApplyConfiguration(new CompanyContactDBConfiguration());
            modelBuilder.ApplyConfiguration(new CompanyCompanyDBConfiguration());
            modelBuilder.ApplyConfiguration(new CompanyDBConfiguration());
            modelBuilder.ApplyConfiguration(new CompanyEmailAddressDBConfiguration());
            modelBuilder.ApplyConfiguration(new CompanyEventLogEntryDBConfiguration());
            modelBuilder.ApplyConfiguration(new CompanyNoteDBConfiguration());
            modelBuilder.ApplyConfiguration(new CompanyPhoneNumberDBConfiguration());
            modelBuilder.ApplyConfiguration(new CompanyPortalDBConfiguration());
            modelBuilder.ApplyConfiguration(new ContactAddressDBConfiguration());
            modelBuilder.ApplyConfiguration(new ContactAttachmentDBConfiguration());
            modelBuilder.ApplyConfiguration(new ContactDBConfiguration());
            modelBuilder.ApplyConfiguration(new ContactEmailAddressDBConfiguration());
            modelBuilder.ApplyConfiguration(new ContactEventLogEntryDBConfiguration());
            modelBuilder.ApplyConfiguration(new ContactNoteDBConfiguration());
            modelBuilder.ApplyConfiguration(new ContactPhoneNumberDBConfiguration());
            modelBuilder.ApplyConfiguration(new ContactLogItemDBConfiguration());
            modelBuilder.ApplyConfiguration(new CreditCardTransactionDBConfiguration());
            modelBuilder.ApplyConfiguration(new CreditDBConfiguration());
            modelBuilder.ApplyConfiguration(new EmailAddressDBConfiguration());
            modelBuilder.ApplyConfiguration(new EventLogEntryDBConfiguration());
            modelBuilder.ApplyConfiguration(new IncomingShipmentAttachmentDBConfiguration());
            modelBuilder.ApplyConfiguration(new IncomingShipmentDBConfiguration());
            modelBuilder.ApplyConfiguration(new IncomingShipmentInventoryItemDBConfiguration());
            modelBuilder.ApplyConfiguration(new IncomingShipmentShipmentTrackingEventDBConfiguration());
            modelBuilder.ApplyConfiguration(new InventoryItemAttachmentDBConfiguration());
            modelBuilder.ApplyConfiguration(new InventoryItemDBConfiguration());
            modelBuilder.ApplyConfiguration(new InventoryItemEventLogEntryDBConfiguration());
            modelBuilder.ApplyConfiguration(new InventoryItemNoteDBConfiguration());
            modelBuilder.ApplyConfiguration(new InventoryItemRelatedInventoryItemDBConfiguration());
            // modelBuilder.ApplyConfiguration(new InventoryItemWorkLogItemDBConfiguration());
            modelBuilder.ApplyConfiguration(new InvoiceAttachmentDBConfiguration());
            modelBuilder.ApplyConfiguration(new InvoiceCashReceiptDBConfiguration());
            modelBuilder.ApplyConfiguration(new InvoiceChatMessageDBConfiguration());
            modelBuilder.ApplyConfiguration(new InvoiceCreditDBConfiguration());
            modelBuilder.ApplyConfiguration(new InvoiceDBConfiguration());
            modelBuilder.ApplyConfiguration(new InvoiceEventLogEntryDBConfiguration());
            modelBuilder.ApplyConfiguration(new InvoiceLineItemDBConfiguration());
            modelBuilder.ApplyConfiguration(new LeadAttachmentDBConfiguration());
            modelBuilder.ApplyConfiguration(new LeadChatMessageDBConfiguration());
            modelBuilder.ApplyConfiguration(new LeadContactLogItemDBConfiguration());
            modelBuilder.ApplyConfiguration(new LeadDBConfiguration());
            modelBuilder.ApplyConfiguration(new LeadEventLogEntryDBConfiguration());
            modelBuilder.ApplyConfiguration(new LeadLineItemDBConfiguration());
            modelBuilder.ApplyConfiguration(new LeadLineItemSourceDBConfiguration());
            modelBuilder.ApplyConfiguration(new LeadNoteDBConfiguration());
            modelBuilder.ApplyConfiguration(new LeadRoutingRuleCountryDBConfiguration());
            modelBuilder.ApplyConfiguration(new LeadRoutingRuleDBConfiguration());
            modelBuilder.ApplyConfiguration(new LeadRoutingRuleLeadWebsiteDBConfiguration());
            modelBuilder.ApplyConfiguration(new LeadRoutingRuleLineItemServiceTypeDBConfiguration());
            modelBuilder.ApplyConfiguration(new LeadRoutingRuleProductTypeDBConfiguration());
            modelBuilder.ApplyConfiguration(new LeadToDoItemDBConfiguration());
            modelBuilder.ApplyConfiguration(new NoteDBConfiguration());
            modelBuilder.ApplyConfiguration(new OutgoingShipmentBoxDBConfiguration());
            modelBuilder.ApplyConfiguration(new OutgoingShipmentBoxInventoryItemDBConfiguration());
            modelBuilder.ApplyConfiguration(new OutgoingShipmentDBConfiguration());
            modelBuilder.ApplyConfiguration(new OutgoingShipmentShipmentTrackingEventDBConfiguration());
            modelBuilder.ApplyConfiguration(new PhoneNumberDBConfiguration());
            modelBuilder.ApplyConfiguration(new ProductAliasDbConfiguration());
            modelBuilder.ApplyConfiguration(new ProductAttachmentDBConfiguration());
            modelBuilder.ApplyConfiguration(new ProductDbConfiguration());
            modelBuilder.ApplyConfiguration(new ProductAttributeValueDBConfiguration());
            modelBuilder.ApplyConfiguration(new ProductNoteDBConfiguration());
            modelBuilder.ApplyConfiguration(new ProductAttributeDBConfiguration());
            modelBuilder.ApplyConfiguration(new ProductKitItemDBConfiguration());
            modelBuilder.ApplyConfiguration(new PurchaseOrderAttachmentDBConfiguration());
            modelBuilder.ApplyConfiguration(new PurchaseOrderChatMessageDBConfiguration());
            modelBuilder.ApplyConfiguration(new PurchaseOrderDBConfiguration());
            modelBuilder.ApplyConfiguration(new PurchaseOrderEventLogEntryDBConfiguration());
            modelBuilder.ApplyConfiguration(new PurchaseOrderIncomingShipmentDBConfiguration());
            modelBuilder.ApplyConfiguration(new PurchaseOrderLineItemDBConfiguration());
            modelBuilder.ApplyConfiguration(new PurchaseOrderNoteDBConfiguration());
            modelBuilder.ApplyConfiguration(new PurchaseOrderToDoItemDBConfiguration());
            modelBuilder.ApplyConfiguration(new QuoteAttachmentDBConfiguration());
            modelBuilder.ApplyConfiguration(new QuoteChatMessageDBConfiguration());
            modelBuilder.ApplyConfiguration(new QuoteDBConfiguration());
            modelBuilder.ApplyConfiguration(new QuoteEventLogEntryDBConfiguration());
            modelBuilder.ApplyConfiguration(new QuoteLineItemDBConfiguration());
            modelBuilder.ApplyConfiguration(new QuoteLineItemSourceDBConfiguration());
            modelBuilder.ApplyConfiguration(new QuoteNoteDBConfiguration());
            modelBuilder.ApplyConfiguration(new QuoteContactLogItemDBConfiguration());
            modelBuilder.ApplyConfiguration(new QuoteToDoItemDBConfiguration());
            modelBuilder.ApplyConfiguration(new QuickBooksInvoiceDBConfiguration());
            modelBuilder.ApplyConfiguration(new QuickBooksPaymentDBConfiguration());
            modelBuilder.ApplyConfiguration(new RepairDBConfiguration());
            modelBuilder.ApplyConfiguration(new RepairDBConfiguration());
            modelBuilder.ApplyConfiguration(new RepairIncomingShipmentDBConfiguration());
            modelBuilder.ApplyConfiguration(new RmaAttachmentDBConfiguration());
            modelBuilder.ApplyConfiguration(new RmaChatMessageDBConfiguration());
            modelBuilder.ApplyConfiguration(new RmaDBConfiguration());
            modelBuilder.ApplyConfiguration(new RmaEventLogEntryDBConfiguration());
            modelBuilder.ApplyConfiguration(new RmaIncomingShipmentDBConfiguration());
            modelBuilder.ApplyConfiguration(new RmaLineItemDBConfiguration());
            modelBuilder.ApplyConfiguration(new RmaOutgoingShipmentDBConfiguration());
            modelBuilder.ApplyConfiguration(new RmaNoteDBConfiguration());
            modelBuilder.ApplyConfiguration(new RMAToDoItemDBConfiguration());
            modelBuilder.ApplyConfiguration(new SalesOrderAttachmentDBConfiguration());
            modelBuilder.ApplyConfiguration(new SalesOrderChatMessageDBConfiguration());
            modelBuilder.ApplyConfiguration(new SalesOrderDBConfiguration());
            modelBuilder.ApplyConfiguration(new SalesOrderEventLogEntryDBConfiguration());
            modelBuilder.ApplyConfiguration(new SalesOrderInventoryItemDBConfiguration());
            modelBuilder.ApplyConfiguration(new SalesOrderInventoryItemDBConfiguration());
            modelBuilder.ApplyConfiguration(new SalesOrderLineItemDBConfiguration());
            modelBuilder.ApplyConfiguration(new SalesOrderLineItemInventoryItemDBConfiguration());
            modelBuilder.ApplyConfiguration(new SalesOrderLineItemSourceDBConfiguration());
            modelBuilder.ApplyConfiguration(new SalesOrderNoteDBConfiguration());
            modelBuilder.ApplyConfiguration(new SalesOrderOutgoingShipmentDBConfiguration());
            modelBuilder.ApplyConfiguration(new SalesOrderPurchaseOrderDBConfiguration());
            modelBuilder.ApplyConfiguration(new SalesOrderToDoItemDBConfiguration());
            modelBuilder.ApplyConfiguration(new SourceAttachmentDBConfiguration());
            modelBuilder.ApplyConfiguration(new SourceDBConfiguration());
            modelBuilder.ApplyConfiguration(new SourceNoteDBConfiguration());
            modelBuilder.ApplyConfiguration(new ToDoItemDBConfiguration());
            modelBuilder.ApplyConfiguration(new UserDBConfiguration());
            modelBuilder.ApplyConfiguration(new UserGroupDBConfiguration());
            modelBuilder.ApplyConfiguration(new UserUserGroupDBConfiguration());
            modelBuilder.ApplyConfiguration(new ViewDBConfiguration());
            modelBuilder.ApplyConfiguration(new ViewFilterDBConfiguration());
            modelBuilder.ApplyConfiguration(new ViewConditionalFormatDBConfiguration());
            modelBuilder.ApplyConfiguration(new ViewCacheItemDBConfiguration());
            modelBuilder.ApplyConfiguration(new WorkLogItemDBConfiguration());
            modelBuilder.ApplyConfiguration(new WorkLogItemActivityOptionDBConfiguration());

            modelBuilder.ApplyConfiguration(new ScanDBConfiguration());
            modelBuilder.ApplyConfiguration(new ScanGroupDBConfiguration());
            modelBuilder.ApplyConfiguration(new ScannerDBConfiguration());
            modelBuilder.ApplyConfiguration(new ScannerLabelDBConfiguration());
            modelBuilder.ApplyConfiguration(new ScannerActionRelatePiecePartsDBConfiguration());
            modelBuilder.ApplyConfiguration(new ScannerActionUpdateLocationDBConfiguration());
            modelBuilder.ApplyConfiguration(new ScannerActionUpdateSystemDataDBConfiguration());
            modelBuilder.ApplyConfiguration(new ScannerActionUpdateSystemDataCommandDBConfiguration());
            modelBuilder.ApplyConfiguration(new ScannerActionUpdateWorkLogDBConfiguration());
            // modelBuilder.ApplyConfiguration(new ScannerEventDBConfiguration());
            // modelBuilder.ApplyConfiguration(new ScannerEventScannerLabelTypeDBConfiguration());
            modelBuilder.ApplyConfiguration(new ScannerLabelTypeVariableDBConfiguration());
            modelBuilder.ApplyConfiguration(new ScannerLabelVariableValueDBConfiguration());    
            modelBuilder.ApplyConfiguration(new ScannerStationLogEntryDBConfiguration());
            modelBuilder.ApplyConfiguration(new ScannerStationDBConfiguration());
            modelBuilder.ApplyConfiguration(new ScannerLabelTypeDBConfiguration());
            modelBuilder.ApplyConfiguration(new QuickBooksLogItemDBConfiguration());
            modelBuilder.ApplyConfiguration(new ChatMessageUserMentionDBConfiguration());
        }
    }
}
