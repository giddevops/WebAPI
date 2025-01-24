using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GidIndustrial.Gideon.WebApi.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AttachmentType",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    Value = table.Column<string>(nullable: true),
                    Locked = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttachmentType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BankAccount",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Locked = table.Column<bool>(nullable: false),
                    IsDefaultCCAccount = table.Column<bool>(nullable: true),
                    QuickBooksId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankAccount", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CashAccount",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Locked = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CashAccount", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CashDisbursementReasonOption",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    Value = table.Column<string>(nullable: true),
                    Locked = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CashDisbursementReasonOption", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CashReceiptType",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    Value = table.Column<string>(nullable: true),
                    Locked = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CashReceiptType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChatMessage",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    CreatedById = table.Column<int>(nullable: true),
                    InReplyToChatMessageId = table.Column<int>(nullable: true),
                    Message = table.Column<string>(nullable: true),
                    ChatMessageId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatMessage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatMessage_ChatMessage_ChatMessageId",
                        column: x => x.ChatMessageId,
                        principalTable: "ChatMessage",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CompanyAddressType",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    Value = table.Column<string>(nullable: true),
                    Locked = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyAddressType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CompanyContactRelationshipType",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyContactRelationshipType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CompanyEmailAddressType",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyEmailAddressType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CompanyPhoneNumberType",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyPhoneNumberType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Config",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Data = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Config", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Contact",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    FirstName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contact", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ContactAddressType",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactAddressType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ContactEmailAddressType",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactEmailAddressType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ContactPhoneNumberType",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactPhoneNumberType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Country",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    Value = table.Column<string>(nullable: true),
                    CountryCode = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Country", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CpuStockVerifiedOption",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CpuStockVerifiedOption", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CreditAccount",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Locked = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreditAccount", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CreditCard",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    CreatedById = table.Column<int>(nullable: true),
                    NameOnCardEncrypted = table.Column<string>(nullable: true),
                    CardNumberEncrypted = table.Column<string>(nullable: true),
                    SecurityCodeEncrypted = table.Column<string>(nullable: true),
                    ExpirationMonthEncrypted = table.Column<string>(nullable: true),
                    ExpirationYearEncrypted = table.Column<string>(nullable: true),
                    IsPrimary = table.Column<bool>(nullable: true),
                    CompanyId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreditCard", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CurrencyOption",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    Value = table.Column<string>(nullable: true),
                    Symbol = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    WireTransferFee = table.Column<decimal>(nullable: true),
                    Locked = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurrencyOption", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CustomerType",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmailAddress",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    Address = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailAddress", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmailAddressType",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailAddressType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmailTemplate",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    Subject = table.Column<string>(nullable: true),
                    HtmlContent = table.Column<string>(nullable: true),
                    EmailTemplateTypeId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailTemplate", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmailTemplateType",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    Value = table.Column<string>(nullable: true),
                    Locked = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailTemplateType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EventLogEntry",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    CreatedById = table.Column<int>(nullable: true),
                    OccurredAt = table.Column<DateTime>(nullable: false),
                    Event = table.Column<string>(nullable: true),
                    UserId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventLogEntry", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GLAccount",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Locked = table.Column<bool>(nullable: false),
                    IsDefaultCCAccount = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GLAccount", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IncomingShipment",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    TrackingNumber = table.Column<string>(nullable: true),
                    ShippingCarrierId = table.Column<int>(nullable: true),
                    ShippingCarrierShippingMethodId = table.Column<int>(nullable: true),
                    ExpectedArrivalDate = table.Column<DateTime>(nullable: true),
                    ActualArrivalDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncomingShipment", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InventoryItemConditionOption",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryItemConditionOption", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InventoryItemLocationOption",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    Value = table.Column<string>(nullable: true),
                    Locked = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryItemLocationOption", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InventoryItemStatusOption",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    Value = table.Column<string>(nullable: true),
                    Locked = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryItemStatusOption", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InventoryItemWorkLogItem",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    CreatedById = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryItemWorkLogItem", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LeadFilter",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedById = table.Column<int>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Public = table.Column<bool>(nullable: false),
                    SearchCriteria = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeadFilter", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LeadOriginOption",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    Value = table.Column<string>(nullable: true),
                    Locked = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeadOriginOption", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LeadRoutingAction",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    MatchedUserIds = table.Column<string>(nullable: true),
                    SelectedUserId = table.Column<int>(nullable: true),
                    LeadId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeadRoutingAction", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LeadStatusOption",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    Value = table.Column<string>(nullable: true),
                    Locked = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeadStatusOption", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LeadWebsite",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    Value = table.Column<string>(nullable: true),
                    Locked = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeadWebsite", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LineItemConditionType",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LineItemConditionType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LineItemServiceType",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    Value = table.Column<string>(nullable: true),
                    Locked = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LineItemServiceType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ListingsContainer",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    CreatedById = table.Column<int>(nullable: true),
                    ProductId = table.Column<int>(nullable: true),
                    ListingsJSON = table.Column<string>(nullable: true),
                    ListingsToHideJSON = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ListingsContainer", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OutgoingLineItemLeadTimeOption",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    Value = table.Column<string>(nullable: true),
                    NumberOfBusinessDays = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutgoingLineItemLeadTimeOption", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OutgoingLineItemWarrantyOption",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutgoingLineItemWarrantyOption", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OutgoingShipmentBoxDimensionOption",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutgoingShipmentBoxDimensionOption", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OutgoingShipmentShippingTermOption",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutgoingShipmentShippingTermOption", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PaymentAccount",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Locked = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentAccount", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Permission",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    AllowedGroups = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permission", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PhoneNumber",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    Number = table.Column<string>(nullable: true),
                    Extension = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhoneNumber", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PhoneNumberType",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhoneNumberType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Portal",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Portal", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductCompositeItemOption",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductCompositeItemOption", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductConditionOption",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductConditionOption", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductEndOfLifeOption",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductEndOfLifeOption", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductType",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseOrderFilter",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedById = table.Column<int>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Public = table.Column<bool>(nullable: false),
                    SearchCriteria = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseOrderFilter", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseOrderPaymentMethod",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    Value = table.Column<string>(nullable: true),
                    Locked = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseOrderPaymentMethod", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseOrderReasonOption",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    Value = table.Column<string>(nullable: true),
                    Locked = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseOrderReasonOption", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseOrderStatusOption",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseOrderStatusOption", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "QuoteAddressType",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuoteAddressType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "QuoteFilter",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedById = table.Column<int>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Public = table.Column<bool>(nullable: false),
                    SearchCriteria = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuoteFilter", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "QuoteStatusOption",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    Value = table.Column<string>(nullable: true),
                    Locked = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuoteStatusOption", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RequiredDeliveryTimeOption",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequiredDeliveryTimeOption", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RmaActionOption",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RmaActionOption", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RmaFilter",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedById = table.Column<int>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Public = table.Column<bool>(nullable: false),
                    SearchCriteria = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RmaFilter", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RmaReasonOption",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RmaReasonOption", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RmaStatusOption",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RmaStatusOption", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SalesOrderFilter",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedById = table.Column<int>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Public = table.Column<bool>(nullable: false),
                    SearchCriteria = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesOrderFilter", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SalesOrderPaymentMethod",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    Value = table.Column<string>(nullable: true),
                    Locked = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesOrderPaymentMethod", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SalesOrderStatusOption",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    Value = table.Column<string>(nullable: true),
                    Locked = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesOrderStatusOption", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShippingCarrier",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    TrackingNumberLink = table.Column<string>(nullable: true),
                    AccountNumberLength = table.Column<int>(nullable: true),
                    HideFromCustomer = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingCarrier", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShippingMethod",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingMethod", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShippingType",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SourceLeadTimeOption",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SourceLeadTimeOption", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SourceWarrantyOption",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SourceWarrantyOption", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "State",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_State", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserGroup",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    AzureId = table.Column<string>(nullable: true),
                    Deactivated = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserGroup", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ViewObjectNameOption",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ViewObjectNameOption", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Address",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Attention = table.Column<string>(nullable: true),
                    Address1 = table.Column<string>(nullable: true),
                    Address2 = table.Column<string>(nullable: true),
                    Address3 = table.Column<string>(nullable: true),
                    City = table.Column<string>(nullable: true),
                    ZipPostalCode = table.Column<string>(nullable: true),
                    State = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    CountryId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Address", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Address_Country_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Country",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ContactEmailAddress",
                columns: table => new
                {
                    EmailAddressId = table.Column<int>(nullable: false),
                    ContactId = table.Column<int>(nullable: false),
                    ContactEmailAddressTypeId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactEmailAddress", x => new { x.ContactId, x.EmailAddressId });
                    table.ForeignKey(
                        name: "FK_ContactEmailAddress_Contact_ContactId",
                        column: x => x.ContactId,
                        principalTable: "Contact",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContactEmailAddress_EmailAddress_EmailAddressId",
                        column: x => x.EmailAddressId,
                        principalTable: "EmailAddress",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContactEventLogEntry",
                columns: table => new
                {
                    EventLogEntryId = table.Column<int>(nullable: false),
                    ContactId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactEventLogEntry", x => new { x.ContactId, x.EventLogEntryId });
                    table.ForeignKey(
                        name: "FK_ContactEventLogEntry_Contact_ContactId",
                        column: x => x.ContactId,
                        principalTable: "Contact",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContactEventLogEntry_EventLogEntry_EventLogEntryId",
                        column: x => x.EventLogEntryId,
                        principalTable: "EventLogEntry",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContactPhoneNumber",
                columns: table => new
                {
                    PhoneNumberId = table.Column<int>(nullable: false),
                    ContactId = table.Column<int>(nullable: false),
                    ContactPhoneNumberTypeId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactPhoneNumber", x => new { x.ContactId, x.PhoneNumberId });
                    table.ForeignKey(
                        name: "FK_ContactPhoneNumber_Contact_ContactId",
                        column: x => x.ContactId,
                        principalTable: "Contact",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContactPhoneNumber_PhoneNumber_PhoneNumberId",
                        column: x => x.PhoneNumberId,
                        principalTable: "PhoneNumber",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShippingCarrierShippingMethod",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ShippingCarrierId = table.Column<int>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    SortPosition = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingCarrierShippingMethod", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShippingCarrierShippingMethod_ShippingCarrier_ShippingCarrierId",
                        column: x => x.ShippingCarrierId,
                        principalTable: "ShippingCarrier",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "View",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    CreatedById = table.Column<int>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    ObjectName = table.Column<string>(nullable: true),
                    UserGroupId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_View", x => x.Id);
                    table.ForeignKey(
                        name: "FK_View_UserGroup_UserGroupId",
                        column: x => x.UserGroupId,
                        principalTable: "UserGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ContactAddress",
                columns: table => new
                {
                    AddressId = table.Column<int>(nullable: false),
                    ContactId = table.Column<int>(nullable: false),
                    ContactAddressTypeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactAddress", x => new { x.ContactId, x.AddressId });
                    table.ForeignKey(
                        name: "FK_ContactAddress_Address_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Address",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContactAddress_Contact_ContactId",
                        column: x => x.ContactId,
                        principalTable: "Contact",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GidLocationOption",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    Value = table.Column<string>(nullable: true),
                    DefaultShippingAddressId = table.Column<int>(nullable: true),
                    Locked = table.Column<bool>(nullable: false),
                    DefaultCurrency = table.Column<string>(nullable: true),
                    MainAddressId = table.Column<int>(nullable: true),
                    BankingInfo = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GidLocationOption", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GidLocationOption_Address_DefaultShippingAddressId",
                        column: x => x.DefaultShippingAddressId,
                        principalTable: "Address",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GidLocationOption_Address_MainAddressId",
                        column: x => x.MainAddressId,
                        principalTable: "Address",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OutgoingShipment",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    CreatedById = table.Column<int>(nullable: true),
                    ShippedAt = table.Column<DateTime>(nullable: true),
                    ShippingAddressId = table.Column<int>(nullable: true),
                    ShippingCarrierId = table.Column<int>(nullable: true),
                    ShippingCarrierShippingMethodId = table.Column<int>(nullable: true),
                    OutgoingShipmentShippingTermOptionId = table.Column<int>(nullable: true),
                    ExpectedArrival = table.Column<DateTime>(nullable: true),
                    ActualArrival = table.Column<DateTime>(nullable: true),
                    PackingSlipSpecialNotes = table.Column<string>(nullable: true),
                    CommercialInvoiceSpecialInstructions = table.Column<string>(nullable: true),
                    LicenseRequired = table.Column<string>(nullable: true),
                    ECCN = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutgoingShipment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OutgoingShipment_OutgoingShipmentShippingTermOption_OutgoingShipmentShippingTermOptionId",
                        column: x => x.OutgoingShipmentShippingTermOptionId,
                        principalTable: "OutgoingShipmentShippingTermOption",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OutgoingShipment_Address_ShippingAddressId",
                        column: x => x.ShippingAddressId,
                        principalTable: "Address",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OutgoingShipment_ShippingCarrier_ShippingCarrierId",
                        column: x => x.ShippingCarrierId,
                        principalTable: "ShippingCarrier",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OutgoingShipment_ShippingCarrierShippingMethod_ShippingCarrierShippingMethodId",
                        column: x => x.ShippingCarrierShippingMethodId,
                        principalTable: "ShippingCarrierShippingMethod",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ViewDisplayField",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ViewId = table.Column<int>(nullable: false),
                    Header = table.Column<string>(nullable: true),
                    FieldName = table.Column<string>(nullable: true),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    Sortable = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ViewDisplayField", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ViewDisplayField_View_ViewId",
                        column: x => x.ViewId,
                        principalTable: "View",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ViewFilter",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ViewId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    FieldName = table.Column<string>(nullable: true),
                    ViewFilterCondition = table.Column<int>(type: "int", nullable: false),
                    UserViewable = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ViewFilter", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ViewFilter_View_ViewId",
                        column: x => x.ViewId,
                        principalTable: "View",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    FirstName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    DisplayName = table.Column<string>(nullable: true),
                    WorkPhone = table.Column<string>(nullable: true),
                    MobilePhone = table.Column<string>(nullable: true),
                    JobTitle = table.Column<string>(nullable: true),
                    Department = table.Column<string>(nullable: true),
                    AzureObjectId = table.Column<Guid>(nullable: false),
                    DefaultGidLocationOptionId = table.Column<int>(nullable: true),
                    MostRecentLeadFilterId = table.Column<int>(nullable: true),
                    MostRecentQuoteFilterId = table.Column<int>(nullable: true),
                    MostRecentSalesOrderFilterId = table.Column<int>(nullable: true),
                    MostRecentInvoiceFilterId = table.Column<int>(nullable: true),
                    MostRecentPurchaseOrderFilterId = table.Column<int>(nullable: true),
                    MostRecentRmaFilterId = table.Column<int>(nullable: true),
                    MostRecentInventoryItemFilterId = table.Column<int>(nullable: true),
                    MostRecentProductFilterId = table.Column<int>(nullable: true),
                    MostRecentSourceFilterId = table.Column<int>(nullable: true),
                    MostRecentCompanyFilterId = table.Column<int>(nullable: true),
                    MostRecentContactFilterId = table.Column<int>(nullable: true),
                    MostRecentBillFilterId = table.Column<int>(nullable: true),
                    AutoCCSelf = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                    table.UniqueConstraint("AK_User_AzureObjectId", x => x.AzureObjectId);
                    table.ForeignKey(
                        name: "FK_User_GidLocationOption_DefaultGidLocationOptionId",
                        column: x => x.DefaultGidLocationOptionId,
                        principalTable: "GidLocationOption",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OutgoingShipmentBox",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    CreatedById = table.Column<int>(nullable: true),
                    OutgoingShipmentId = table.Column<int>(nullable: true),
                    Length = table.Column<float>(nullable: true),
                    Width = table.Column<float>(nullable: true),
                    Height = table.Column<float>(nullable: true),
                    Weight = table.Column<decimal>(nullable: true),
                    TrackingNumber = table.Column<string>(nullable: true),
                    ExpectedArrival = table.Column<DateTime>(nullable: true),
                    ActualArrival = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutgoingShipmentBox", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OutgoingShipmentBox_OutgoingShipment_OutgoingShipmentId",
                        column: x => x.OutgoingShipmentId,
                        principalTable: "OutgoingShipment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Attachment",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    Size = table.Column<long>(nullable: false),
                    AttachmentTypeId = table.Column<int>(nullable: true),
                    CreatedById = table.Column<int>(nullable: true),
                    Uri = table.Column<string>(nullable: true),
                    Confirmed = table.Column<bool>(nullable: false),
                    ContentType = table.Column<string>(nullable: true),
                    OfficialFilename = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attachment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Attachment_User_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LeadRoutingRule",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(nullable: true),
                    ProductTypeIncludeOptionId = table.Column<int>(nullable: true),
                    LeadWebsiteIncludeOptionId = table.Column<int>(nullable: true),
                    CountryIncludeOptionId = table.Column<int>(nullable: true),
                    SBCDailyLimit = table.Column<int>(nullable: true),
                    DailyLeadLimit = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeadRoutingRule", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LeadRoutingRule_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Note",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    CreatedById = table.Column<int>(nullable: true),
                    Text = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Note", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Note_User_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserUserGroup",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false),
                    UserGroupId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserUserGroup", x => new { x.UserId, x.UserGroupId });
                    table.ForeignKey(
                        name: "FK_UserUserGroup_UserGroup_UserGroupId",
                        column: x => x.UserGroupId,
                        principalTable: "UserGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserUserGroup_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChatMessageAttachment",
                columns: table => new
                {
                    AttachmentId = table.Column<int>(nullable: false),
                    ChatMessageId = table.Column<int>(nullable: false),
                    ChatMessageAttachmentTypeId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatMessageAttachment", x => new { x.ChatMessageId, x.AttachmentId });
                    table.ForeignKey(
                        name: "FK_ChatMessageAttachment_Attachment_AttachmentId",
                        column: x => x.AttachmentId,
                        principalTable: "Attachment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChatMessageAttachment_ChatMessage_ChatMessageId",
                        column: x => x.ChatMessageId,
                        principalTable: "ChatMessage",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContactAttachment",
                columns: table => new
                {
                    AttachmentId = table.Column<int>(nullable: false),
                    ContactId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactAttachment", x => new { x.ContactId, x.AttachmentId });
                    table.ForeignKey(
                        name: "FK_ContactAttachment_Attachment_AttachmentId",
                        column: x => x.AttachmentId,
                        principalTable: "Attachment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContactAttachment_Contact_ContactId",
                        column: x => x.ContactId,
                        principalTable: "Contact",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LeadRoutingRuleCountry",
                columns: table => new
                {
                    LeadRoutingRuleId = table.Column<int>(nullable: false),
                    CountryId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeadRoutingRuleCountry", x => new { x.LeadRoutingRuleId, x.CountryId });
                    table.ForeignKey(
                        name: "FK_LeadRoutingRuleCountry_Country_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Country",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LeadRoutingRuleCountry_LeadRoutingRule_LeadRoutingRuleId",
                        column: x => x.LeadRoutingRuleId,
                        principalTable: "LeadRoutingRule",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LeadRoutingRuleLeadWebsite",
                columns: table => new
                {
                    LeadRoutingRuleId = table.Column<int>(nullable: false),
                    LeadWebsiteId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeadRoutingRuleLeadWebsite", x => new { x.LeadRoutingRuleId, x.LeadWebsiteId });
                    table.ForeignKey(
                        name: "FK_LeadRoutingRuleLeadWebsite_LeadRoutingRule_LeadRoutingRuleId",
                        column: x => x.LeadRoutingRuleId,
                        principalTable: "LeadRoutingRule",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LeadRoutingRuleLeadWebsite_LeadWebsite_LeadWebsiteId",
                        column: x => x.LeadWebsiteId,
                        principalTable: "LeadWebsite",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LeadRoutingRuleLineItemServiceType",
                columns: table => new
                {
                    LeadRoutingRuleId = table.Column<int>(nullable: false),
                    LineItemServiceTypeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeadRoutingRuleLineItemServiceType", x => new { x.LeadRoutingRuleId, x.LineItemServiceTypeId });
                    table.ForeignKey(
                        name: "FK_LeadRoutingRuleLineItemServiceType_LeadRoutingRule_LeadRoutingRuleId",
                        column: x => x.LeadRoutingRuleId,
                        principalTable: "LeadRoutingRule",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LeadRoutingRuleLineItemServiceType_LineItemServiceType_LineItemServiceTypeId",
                        column: x => x.LineItemServiceTypeId,
                        principalTable: "LineItemServiceType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LeadRoutingRuleProductType",
                columns: table => new
                {
                    LeadRoutingRuleId = table.Column<int>(nullable: false),
                    ProductTypeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeadRoutingRuleProductType", x => new { x.LeadRoutingRuleId, x.ProductTypeId });
                    table.ForeignKey(
                        name: "FK_LeadRoutingRuleProductType_LeadRoutingRule_LeadRoutingRuleId",
                        column: x => x.LeadRoutingRuleId,
                        principalTable: "LeadRoutingRule",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LeadRoutingRuleProductType_ProductType_ProductTypeId",
                        column: x => x.ProductTypeId,
                        principalTable: "ProductType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContactNote",
                columns: table => new
                {
                    NoteId = table.Column<int>(nullable: false),
                    ContactId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactNote", x => new { x.ContactId, x.NoteId });
                    table.ForeignKey(
                        name: "FK_ContactNote_Contact_ContactId",
                        column: x => x.ContactId,
                        principalTable: "Contact",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContactNote_Note_NoteId",
                        column: x => x.NoteId,
                        principalTable: "Note",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CompanyAddress",
                columns: table => new
                {
                    AddressId = table.Column<int>(nullable: false),
                    CompanyId = table.Column<int>(nullable: false),
                    CompanyAddressTypeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyAddress", x => new { x.CompanyId, x.AddressId });
                    table.ForeignKey(
                        name: "FK_CompanyAddress_Address_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Address",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Company",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    CustomerTypeId = table.Column<string>(nullable: true),
                    AnnualRevenue = table.Column<int>(nullable: true),
                    NumberOfEmployees = table.Column<int>(nullable: true),
                    DunsNumber = table.Column<string>(nullable: true),
                    ApprovedForTerms = table.Column<bool>(nullable: true),
                    BillingAddressCompanyId = table.Column<int>(nullable: true),
                    BillingAddressAddressId = table.Column<int>(nullable: true),
                    ShippingAddressCompanyId = table.Column<int>(nullable: true),
                    ShippingAddressAddressId = table.Column<int>(nullable: true),
                    ParentCompanyId = table.Column<int>(nullable: true),
                    QuickBooksCustomerId = table.Column<string>(nullable: true),
                    QuickBooksCustomerSyncToken = table.Column<string>(nullable: true),
                    QuickBooksVendorId = table.Column<string>(nullable: true),
                    QuickBooksVendorSyncToken = table.Column<string>(nullable: true),
                    AuthorizeNetProfileId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Company", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Company_Company_ParentCompanyId",
                        column: x => x.ParentCompanyId,
                        principalTable: "Company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Company_CompanyAddress_BillingAddressCompanyId_BillingAddressAddressId",
                        columns: x => new { x.BillingAddressCompanyId, x.BillingAddressAddressId },
                        principalTable: "CompanyAddress",
                        principalColumns: new[] { "CompanyId", "AddressId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Company_CompanyAddress_ShippingAddressCompanyId_ShippingAddressAddressId",
                        columns: x => new { x.ShippingAddressCompanyId, x.ShippingAddressAddressId },
                        principalTable: "CompanyAddress",
                        principalColumns: new[] { "CompanyId", "AddressId" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CashReceipt",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    CreatedById = table.Column<int>(nullable: true),
                    CompanyId = table.Column<int>(nullable: true),
                    Amount = table.Column<decimal>(nullable: false),
                    Balance = table.Column<decimal>(nullable: false),
                    CurrencyOptionId = table.Column<int>(nullable: true),
                    CashReceiptTypeId = table.Column<int>(nullable: true),
                    BankAccountId = table.Column<int>(nullable: true),
                    CreditCardTransactionId = table.Column<int>(nullable: true),
                    QuickBooksId = table.Column<string>(nullable: true),
                    QuickBooksSyncToken = table.Column<string>(nullable: true),
                    DateReceived = table.Column<DateTime>(nullable: true),
                    SalesOrderId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CashReceipt", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CashReceipt_BankAccount_BankAccountId",
                        column: x => x.BankAccountId,
                        principalTable: "BankAccount",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CashReceipt_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CompanyAlias",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CompanyId = table.Column<int>(nullable: true),
                    Alias = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyAlias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyAlias_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CompanyAttachment",
                columns: table => new
                {
                    AttachmentId = table.Column<int>(nullable: false),
                    CompanyId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyAttachment", x => new { x.CompanyId, x.AttachmentId });
                    table.ForeignKey(
                        name: "FK_CompanyAttachment_Attachment_AttachmentId",
                        column: x => x.AttachmentId,
                        principalTable: "Attachment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompanyAttachment_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CompanyContact",
                columns: table => new
                {
                    CompanyId = table.Column<int>(nullable: false),
                    ContactId = table.Column<int>(nullable: false),
                    CompanyContactRelationshipTypeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyContact", x => new { x.ContactId, x.CompanyId });
                    table.ForeignKey(
                        name: "FK_CompanyContact_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CompanyContact_Contact_ContactId",
                        column: x => x.ContactId,
                        principalTable: "Contact",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CompanyEmailAddress",
                columns: table => new
                {
                    EmailAddressId = table.Column<int>(nullable: false),
                    CompanyId = table.Column<int>(nullable: false),
                    CompanyEmailAddressTypeId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyEmailAddress", x => new { x.CompanyId, x.EmailAddressId });
                    table.ForeignKey(
                        name: "FK_CompanyEmailAddress_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompanyEmailAddress_EmailAddress_EmailAddressId",
                        column: x => x.EmailAddressId,
                        principalTable: "EmailAddress",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CompanyEventLogEntry",
                columns: table => new
                {
                    EventLogEntryId = table.Column<int>(nullable: false),
                    CompanyId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyEventLogEntry", x => new { x.CompanyId, x.EventLogEntryId });
                    table.ForeignKey(
                        name: "FK_CompanyEventLogEntry_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompanyEventLogEntry_EventLogEntry_EventLogEntryId",
                        column: x => x.EventLogEntryId,
                        principalTable: "EventLogEntry",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CompanyNote",
                columns: table => new
                {
                    NoteId = table.Column<int>(nullable: false),
                    CompanyId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyNote", x => new { x.CompanyId, x.NoteId });
                    table.ForeignKey(
                        name: "FK_CompanyNote_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompanyNote_Note_NoteId",
                        column: x => x.NoteId,
                        principalTable: "Note",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CompanyPhoneNumber",
                columns: table => new
                {
                    PhoneNumberId = table.Column<int>(nullable: false),
                    CompanyId = table.Column<int>(nullable: false),
                    CompanyPhoneNumberTypeId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyPhoneNumber", x => new { x.CompanyId, x.PhoneNumberId });
                    table.ForeignKey(
                        name: "FK_CompanyPhoneNumber_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompanyPhoneNumber_PhoneNumber_PhoneNumberId",
                        column: x => x.PhoneNumberId,
                        principalTable: "PhoneNumber",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CompanyPortal",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    CreatedById = table.Column<int>(nullable: true),
                    CompanyId = table.Column<int>(nullable: true),
                    PortalId = table.Column<int>(nullable: true),
                    Username = table.Column<string>(nullable: true),
                    Rating = table.Column<string>(nullable: true),
                    PositiveFeedbackPercent = table.Column<decimal>(nullable: true),
                    MemberSince = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyPortal", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyPortal_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CompanyPortal_Portal_PortalId",
                        column: x => x.PortalId,
                        principalTable: "Portal",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Lead",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    CreatedById = table.Column<int>(nullable: true),
                    OldId = table.Column<int>(nullable: true),
                    NewId = table.Column<int>(nullable: true),
                    SalesPersonId = table.Column<int>(nullable: true),
                    FullName = table.Column<string>(nullable: true),
                    Position = table.Column<string>(nullable: true),
                    CompanyName = table.Column<string>(nullable: true),
                    CompanyId = table.Column<int>(nullable: true),
                    ContactId = table.Column<int>(nullable: true),
                    CustomerTypeId = table.Column<int>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    Phone = table.Column<string>(nullable: true),
                    Comments = table.Column<string>(type: "text", nullable: true),
                    RequiredDeliveryTimeId = table.Column<int>(nullable: true),
                    LeadStatusOptionId = table.Column<int>(nullable: true),
                    Address1 = table.Column<string>(nullable: true),
                    Address2 = table.Column<string>(nullable: true),
                    Address3 = table.Column<string>(nullable: true),
                    City = table.Column<string>(nullable: true),
                    ZipPostalCode = table.Column<string>(nullable: true),
                    State = table.Column<string>(nullable: true),
                    CountryId = table.Column<int>(nullable: true),
                    Website = table.Column<string>(nullable: true),
                    DunsNumber = table.Column<string>(nullable: true),
                    RequiredDeliveryWeeks = table.Column<int>(nullable: true),
                    LALeadNumber = table.Column<long>(nullable: true),
                    Timestamp = table.Column<DateTime>(nullable: true),
                    SourceUrl = table.Column<string>(nullable: true),
                    ReferrerUrl = table.Column<string>(nullable: true),
                    WebServerName = table.Column<string>(nullable: true),
                    IPAddress = table.Column<string>(nullable: true),
                    GeolocationCity = table.Column<string>(nullable: true),
                    GeolocationCountryCode = table.Column<string>(nullable: true),
                    GeolocationCountryName = table.Column<string>(nullable: true),
                    GeolocationLatitude = table.Column<decimal>(nullable: true),
                    GeolocationLongitude = table.Column<decimal>(nullable: true),
                    GeolocationPostalCode = table.Column<string>(nullable: true),
                    GeolocationRegionCode = table.Column<string>(nullable: true),
                    GeolocationRegionName = table.Column<string>(nullable: true),
                    GeolocationTimeZone = table.Column<string>(nullable: true),
                    NotesText = table.Column<string>(nullable: true),
                    Quality = table.Column<int>(nullable: true),
                    AutoResponseSent = table.Column<bool>(nullable: true),
                    CountryCode = table.Column<string>(nullable: true),
                    CountryName = table.Column<string>(nullable: true),
                    CustomerType = table.Column<string>(nullable: true),
                    Category = table.Column<string>(nullable: true),
                    Manufacturer = table.Column<string>(nullable: true),
                    PartNumber = table.Column<string>(nullable: true),
                    Quantity = table.Column<int>(nullable: true),
                    Service = table.Column<string>(nullable: true),
                    AdditionalData = table.Column<string>(nullable: true),
                    BrowserName = table.Column<string>(nullable: true),
                    BrowserVersion = table.Column<string>(nullable: true),
                    LeadOriginOptionId = table.Column<int>(nullable: true),
                    Key = table.Column<string>(nullable: true),
                    Freeform = table.Column<bool>(nullable: true),
                    UserAgent = table.Column<string>(nullable: true),
                    Emoji = table.Column<string>(nullable: true),
                    OriginText = table.Column<string>(nullable: true),
                    UserId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lead", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Lead_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Lead_Contact_ContactId",
                        column: x => x.ContactId,
                        principalTable: "Contact",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Lead_User_SalesPersonId",
                        column: x => x.SalesPersonId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Lead_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Product",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    PartNumber = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    ShortDescription = table.Column<string>(nullable: true),
                    GidPartNumber = table.Column<string>(nullable: true),
                    ManufacturerId = table.Column<int>(nullable: true),
                    Serialized = table.Column<bool>(nullable: false),
                    ProductTypeId = table.Column<int>(nullable: true),
                    QuickBooksId = table.Column<string>(nullable: true),
                    QuickBooksSyncToken = table.Column<string>(nullable: true),
                    ProductEndOfLifeOptionId = table.Column<int>(nullable: true),
                    ProductCompositeItemOptionId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Product", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Product_Company_ManufacturerId",
                        column: x => x.ManufacturerId,
                        principalTable: "Company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Product_ProductType_ProductTypeId",
                        column: x => x.ProductTypeId,
                        principalTable: "ProductType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseOrder",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    CreatedById = table.Column<int>(nullable: true),
                    BuyerId = table.Column<int>(nullable: true),
                    Ncnr = table.Column<bool>(nullable: true),
                    NeedsFunding = table.Column<bool>(nullable: true),
                    SupplierId = table.Column<int>(nullable: true),
                    ContactId = table.Column<int>(nullable: true),
                    Phone = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    PurchaseOrderStatusOptionId = table.Column<int>(nullable: true),
                    PurchaseOrderReasonOptionId = table.Column<int>(nullable: true),
                    CurrencyOptionId = table.Column<int>(nullable: false),
                    GidLocationOptionId = table.Column<int>(nullable: true),
                    ConfirmationNumber = table.Column<string>(nullable: true),
                    SalesTax = table.Column<decimal>(nullable: true),
                    WireTransferFee = table.Column<decimal>(nullable: true),
                    ShippingAndHandlingFee = table.Column<decimal>(nullable: true),
                    ExpediteFee = table.Column<decimal>(nullable: true),
                    ShippingMethodId = table.Column<int>(nullable: true),
                    PurchaseOrderPaymentMethodId = table.Column<int>(nullable: true),
                    ExpectedShipDate = table.Column<DateTime>(nullable: true),
                    ExpectedArrivalDate = table.Column<DateTime>(nullable: true),
                    ShippingAddressId = table.Column<int>(nullable: true),
                    Sent = table.Column<bool>(nullable: true),
                    SentAt = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseOrder", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchaseOrder_Contact_ContactId",
                        column: x => x.ContactId,
                        principalTable: "Contact",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseOrder_CurrencyOption_CurrencyOptionId",
                        column: x => x.CurrencyOptionId,
                        principalTable: "CurrencyOption",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseOrder_GidLocationOption_GidLocationOptionId",
                        column: x => x.GidLocationOptionId,
                        principalTable: "GidLocationOption",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseOrder_PurchaseOrderPaymentMethod_PurchaseOrderPaymentMethodId",
                        column: x => x.PurchaseOrderPaymentMethodId,
                        principalTable: "PurchaseOrderPaymentMethod",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseOrder_Address_ShippingAddressId",
                        column: x => x.ShippingAddressId,
                        principalTable: "Address",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseOrder_Company_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LeadAttachment",
                columns: table => new
                {
                    AttachmentId = table.Column<int>(nullable: false),
                    LeadId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeadAttachment", x => new { x.LeadId, x.AttachmentId });
                    table.ForeignKey(
                        name: "FK_LeadAttachment_Attachment_AttachmentId",
                        column: x => x.AttachmentId,
                        principalTable: "Attachment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LeadAttachment_Lead_LeadId",
                        column: x => x.LeadId,
                        principalTable: "Lead",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LeadChatMessage",
                columns: table => new
                {
                    ChatMessageId = table.Column<int>(nullable: false),
                    LeadId = table.Column<int>(nullable: false),
                    LeadChatMessageTypeId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeadChatMessage", x => new { x.LeadId, x.ChatMessageId });
                    table.ForeignKey(
                        name: "FK_LeadChatMessage_ChatMessage_ChatMessageId",
                        column: x => x.ChatMessageId,
                        principalTable: "ChatMessage",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LeadChatMessage_Lead_LeadId",
                        column: x => x.LeadId,
                        principalTable: "Lead",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LeadEventLogEntry",
                columns: table => new
                {
                    EventLogEntryId = table.Column<int>(nullable: false),
                    LeadId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeadEventLogEntry", x => new { x.LeadId, x.EventLogEntryId });
                    table.ForeignKey(
                        name: "FK_LeadEventLogEntry_EventLogEntry_EventLogEntryId",
                        column: x => x.EventLogEntryId,
                        principalTable: "EventLogEntry",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LeadEventLogEntry_Lead_LeadId",
                        column: x => x.LeadId,
                        principalTable: "Lead",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LeadNote",
                columns: table => new
                {
                    NoteId = table.Column<int>(nullable: false),
                    LeadId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeadNote", x => new { x.LeadId, x.NoteId });
                    table.ForeignKey(
                        name: "FK_LeadNote_Lead_LeadId",
                        column: x => x.LeadId,
                        principalTable: "Lead",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LeadNote_Note_NoteId",
                        column: x => x.NoteId,
                        principalTable: "Note",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Quote",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    CreatedById = table.Column<int>(nullable: true),
                    SalesPersonId = table.Column<int>(nullable: true),
                    CompanyId = table.Column<int>(nullable: true),
                    ContactId = table.Column<int>(nullable: true),
                    Phone = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    Expiration = table.Column<DateTime>(nullable: true),
                    LeadId = table.Column<int>(nullable: true),
                    CustomerTypeId = table.Column<int>(nullable: true),
                    QuoteFormLink = table.Column<string>(nullable: true),
                    GidLocationOptionId = table.Column<int>(nullable: true),
                    ShippingMethodId = table.Column<int>(nullable: true),
                    CurrencyOptionId = table.Column<int>(nullable: false),
                    QuoteStatusOptionId = table.Column<int>(nullable: true),
                    CustomerNotes = table.Column<string>(nullable: true),
                    ShippingAddressId = table.Column<int>(nullable: true),
                    BillingAddressId = table.Column<int>(nullable: true),
                    CopyBillingToShipping = table.Column<bool>(nullable: true),
                    SalesTax = table.Column<decimal>(nullable: true),
                    WireTransferFee = table.Column<decimal>(nullable: true),
                    ShippingAndHandlingFee = table.Column<decimal>(nullable: true),
                    ExpediteFee = table.Column<decimal>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quote", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Quote_Address_BillingAddressId",
                        column: x => x.BillingAddressId,
                        principalTable: "Address",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Quote_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Quote_Contact_ContactId",
                        column: x => x.ContactId,
                        principalTable: "Contact",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Quote_CurrencyOption_CurrencyOptionId",
                        column: x => x.CurrencyOptionId,
                        principalTable: "CurrencyOption",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Quote_GidLocationOption_GidLocationOptionId",
                        column: x => x.GidLocationOptionId,
                        principalTable: "GidLocationOption",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Quote_Lead_LeadId",
                        column: x => x.LeadId,
                        principalTable: "Lead",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Quote_QuoteStatusOption_QuoteStatusOptionId",
                        column: x => x.QuoteStatusOptionId,
                        principalTable: "QuoteStatusOption",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Quote_User_SalesPersonId",
                        column: x => x.SalesPersonId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Quote_Address_ShippingAddressId",
                        column: x => x.ShippingAddressId,
                        principalTable: "Address",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Quote_ShippingMethod_ShippingMethodId",
                        column: x => x.ShippingMethodId,
                        principalTable: "ShippingMethod",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LeadLineItem",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    CreatedById = table.Column<int>(nullable: true),
                    ProductName = table.Column<string>(nullable: true),
                    ProductId = table.Column<int>(nullable: true),
                    ManufacturerName = table.Column<string>(nullable: true),
                    CompanyId = table.Column<int>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    LineItemServiceTypeId = table.Column<int>(nullable: true),
                    Quantity = table.Column<int>(nullable: true),
                    LeadId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeadLineItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LeadLineItem_Lead_LeadId",
                        column: x => x.LeadId,
                        principalTable: "Lead",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LeadLineItem_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductAttachment",
                columns: table => new
                {
                    AttachmentId = table.Column<int>(nullable: false),
                    ProductId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductAttachment", x => new { x.ProductId, x.AttachmentId });
                    table.ForeignKey(
                        name: "FK_ProductAttachment_Attachment_AttachmentId",
                        column: x => x.AttachmentId,
                        principalTable: "Attachment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductAttachment_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductNote",
                columns: table => new
                {
                    NoteId = table.Column<int>(nullable: false),
                    ProductId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductNote", x => new { x.ProductId, x.NoteId });
                    table.ForeignKey(
                        name: "FK_ProductNote_Note_NoteId",
                        column: x => x.NoteId,
                        principalTable: "Note",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductNote_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Source",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    CreatedById = table.Column<int>(nullable: true),
                    ProductId = table.Column<int>(nullable: true),
                    SupplierId = table.Column<int>(nullable: true),
                    Ncnr = table.Column<bool>(nullable: true),
                    ProductSourceWebPage = table.Column<string>(nullable: true),
                    ProductConditionOptionId = table.Column<int>(nullable: true),
                    Cost = table.Column<decimal>(nullable: true),
                    Quantity = table.Column<int>(nullable: true),
                    VerifiedAt = table.Column<DateTime>(nullable: true),
                    SourceLeadTimeOptionId = table.Column<int>(nullable: true),
                    ShippingCost = table.Column<decimal>(nullable: true),
                    PortalId = table.Column<int>(nullable: true),
                    ListingId = table.Column<string>(nullable: true),
                    WarrantyDuration = table.Column<int>(nullable: true),
                    WarrantyDurationUnit = table.Column<string>(nullable: true),
                    LeadTimeRangeStart = table.Column<int>(nullable: true),
                    LeadTimeRangeEnd = table.Column<int>(nullable: true),
                    LeadTimeRangeUnit = table.Column<string>(nullable: true),
                    CurrencyOptionId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Source", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Source_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Source_Company_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Bill",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    CreatedById = table.Column<int>(nullable: true),
                    CompanyId = table.Column<int>(nullable: true),
                    PurchaseOrderId = table.Column<int>(nullable: true),
                    Balance = table.Column<decimal>(nullable: false),
                    DateDue = table.Column<DateTime>(nullable: true),
                    EnteredAt = table.Column<DateTime>(nullable: true),
                    QuickBooksId = table.Column<string>(nullable: true),
                    QuickBooksSyncToken = table.Column<string>(nullable: true),
                    CurrencyOptionId = table.Column<int>(nullable: true),
                    GidLocationOptionId = table.Column<int>(nullable: true),
                    ShippingAndHandlingFee = table.Column<decimal>(nullable: false),
                    SalesTax = table.Column<decimal>(nullable: false),
                    WireTransferFee = table.Column<decimal>(nullable: false),
                    ExpediteFee = table.Column<decimal>(nullable: false),
                    DiscountPercent = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bill", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bill_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Bill_PurchaseOrder_PurchaseOrderId",
                        column: x => x.PurchaseOrderId,
                        principalTable: "PurchaseOrder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseOrderAttachment",
                columns: table => new
                {
                    AttachmentId = table.Column<int>(nullable: false),
                    PurchaseOrderId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseOrderAttachment", x => new { x.PurchaseOrderId, x.AttachmentId });
                    table.ForeignKey(
                        name: "FK_PurchaseOrderAttachment_Attachment_AttachmentId",
                        column: x => x.AttachmentId,
                        principalTable: "Attachment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PurchaseOrderAttachment_PurchaseOrder_PurchaseOrderId",
                        column: x => x.PurchaseOrderId,
                        principalTable: "PurchaseOrder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseOrderChatMessage",
                columns: table => new
                {
                    ChatMessageId = table.Column<int>(nullable: false),
                    PurchaseOrderId = table.Column<int>(nullable: false),
                    PurchaseOrderChatMessageTypeId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseOrderChatMessage", x => new { x.PurchaseOrderId, x.ChatMessageId });
                    table.ForeignKey(
                        name: "FK_PurchaseOrderChatMessage_ChatMessage_ChatMessageId",
                        column: x => x.ChatMessageId,
                        principalTable: "ChatMessage",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PurchaseOrderChatMessage_PurchaseOrder_PurchaseOrderId",
                        column: x => x.PurchaseOrderId,
                        principalTable: "PurchaseOrder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseOrderEventLogEntry",
                columns: table => new
                {
                    EventLogEntryId = table.Column<int>(nullable: false),
                    PurchaseOrderId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseOrderEventLogEntry", x => new { x.PurchaseOrderId, x.EventLogEntryId });
                    table.ForeignKey(
                        name: "FK_PurchaseOrderEventLogEntry_EventLogEntry_EventLogEntryId",
                        column: x => x.EventLogEntryId,
                        principalTable: "EventLogEntry",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PurchaseOrderEventLogEntry_PurchaseOrder_PurchaseOrderId",
                        column: x => x.PurchaseOrderId,
                        principalTable: "PurchaseOrder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseOrderIncomingShipment",
                columns: table => new
                {
                    PurchaseOrderId = table.Column<int>(nullable: false),
                    IncomingShipmentId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseOrderIncomingShipment", x => new { x.PurchaseOrderId, x.IncomingShipmentId });
                    table.ForeignKey(
                        name: "FK_PurchaseOrderIncomingShipment_IncomingShipment_IncomingShipmentId",
                        column: x => x.IncomingShipmentId,
                        principalTable: "IncomingShipment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PurchaseOrderIncomingShipment_PurchaseOrder_PurchaseOrderId",
                        column: x => x.PurchaseOrderId,
                        principalTable: "PurchaseOrder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseOrderLineItem",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    CreatedById = table.Column<int>(nullable: true),
                    ProductName = table.Column<string>(nullable: true),
                    ProductId = table.Column<int>(nullable: true),
                    ManufacturerName = table.Column<string>(nullable: true),
                    ManufacturerId = table.Column<int>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    LineItemServiceTypeId = table.Column<int>(nullable: true),
                    LineItemConditionTypeId = table.Column<int>(nullable: true),
                    Quantity = table.Column<int>(nullable: true),
                    Cost = table.Column<decimal>(nullable: true),
                    DiscountPercent = table.Column<decimal>(nullable: true),
                    WarrantyDuration = table.Column<int>(nullable: true),
                    WarrantyDurationUnit = table.Column<string>(nullable: true),
                    PurchaseOrderId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseOrderLineItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchaseOrderLineItem_LineItemConditionType_LineItemConditionTypeId",
                        column: x => x.LineItemConditionTypeId,
                        principalTable: "LineItemConditionType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseOrderLineItem_Company_ManufacturerId",
                        column: x => x.ManufacturerId,
                        principalTable: "Company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseOrderLineItem_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseOrderLineItem_PurchaseOrder_PurchaseOrderId",
                        column: x => x.PurchaseOrderId,
                        principalTable: "PurchaseOrder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseOrderNote",
                columns: table => new
                {
                    NoteId = table.Column<int>(nullable: false),
                    PurchaseOrderId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseOrderNote", x => new { x.PurchaseOrderId, x.NoteId });
                    table.ForeignKey(
                        name: "FK_PurchaseOrderNote_Note_NoteId",
                        column: x => x.NoteId,
                        principalTable: "Note",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PurchaseOrderNote_PurchaseOrder_PurchaseOrderId",
                        column: x => x.PurchaseOrderId,
                        principalTable: "PurchaseOrder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuoteAttachment",
                columns: table => new
                {
                    AttachmentId = table.Column<int>(nullable: false),
                    QuoteId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuoteAttachment", x => new { x.QuoteId, x.AttachmentId });
                    table.ForeignKey(
                        name: "FK_QuoteAttachment_Attachment_AttachmentId",
                        column: x => x.AttachmentId,
                        principalTable: "Attachment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuoteAttachment_Quote_QuoteId",
                        column: x => x.QuoteId,
                        principalTable: "Quote",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuoteChatMessage",
                columns: table => new
                {
                    ChatMessageId = table.Column<int>(nullable: false),
                    QuoteId = table.Column<int>(nullable: false),
                    QuoteChatMessageTypeId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuoteChatMessage", x => new { x.QuoteId, x.ChatMessageId });
                    table.ForeignKey(
                        name: "FK_QuoteChatMessage_ChatMessage_ChatMessageId",
                        column: x => x.ChatMessageId,
                        principalTable: "ChatMessage",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuoteChatMessage_Quote_QuoteId",
                        column: x => x.QuoteId,
                        principalTable: "Quote",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuoteEventLogEntry",
                columns: table => new
                {
                    EventLogEntryId = table.Column<int>(nullable: false),
                    QuoteId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuoteEventLogEntry", x => new { x.QuoteId, x.EventLogEntryId });
                    table.ForeignKey(
                        name: "FK_QuoteEventLogEntry_EventLogEntry_EventLogEntryId",
                        column: x => x.EventLogEntryId,
                        principalTable: "EventLogEntry",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuoteEventLogEntry_Quote_QuoteId",
                        column: x => x.QuoteId,
                        principalTable: "Quote",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuoteLineItem",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    CreatedById = table.Column<int>(nullable: true),
                    ProductName = table.Column<string>(nullable: true),
                    ProductId = table.Column<int>(nullable: true),
                    DisplayPartNumber = table.Column<string>(nullable: true),
                    ManufacturerName = table.Column<string>(nullable: true),
                    ManufacturerId = table.Column<int>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    LineItemServiceTypeId = table.Column<int>(nullable: true),
                    LineItemConditionTypeId = table.Column<int>(nullable: true),
                    OutgoingLineItemWarrantyOptionId = table.Column<int>(nullable: true),
                    OutgoingLineItemLeadTimeOptionId = table.Column<int>(nullable: true),
                    Quantity = table.Column<int>(nullable: true),
                    Price = table.Column<decimal>(nullable: true),
                    DiscountPercent = table.Column<decimal>(nullable: true),
                    QuoteId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuoteLineItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuoteLineItem_LineItemConditionType_LineItemConditionTypeId",
                        column: x => x.LineItemConditionTypeId,
                        principalTable: "LineItemConditionType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuoteLineItem_LineItemServiceType_LineItemServiceTypeId",
                        column: x => x.LineItemServiceTypeId,
                        principalTable: "LineItemServiceType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuoteLineItem_Company_ManufacturerId",
                        column: x => x.ManufacturerId,
                        principalTable: "Company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuoteLineItem_OutgoingLineItemLeadTimeOption_OutgoingLineItemLeadTimeOptionId",
                        column: x => x.OutgoingLineItemLeadTimeOptionId,
                        principalTable: "OutgoingLineItemLeadTimeOption",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuoteLineItem_OutgoingLineItemWarrantyOption_OutgoingLineItemWarrantyOptionId",
                        column: x => x.OutgoingLineItemWarrantyOptionId,
                        principalTable: "OutgoingLineItemWarrantyOption",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuoteLineItem_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuoteLineItem_Quote_QuoteId",
                        column: x => x.QuoteId,
                        principalTable: "Quote",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuoteNote",
                columns: table => new
                {
                    NoteId = table.Column<int>(nullable: false),
                    QuoteId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuoteNote", x => new { x.QuoteId, x.NoteId });
                    table.ForeignKey(
                        name: "FK_QuoteNote_Note_NoteId",
                        column: x => x.NoteId,
                        principalTable: "Note",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuoteNote_Quote_QuoteId",
                        column: x => x.QuoteId,
                        principalTable: "Quote",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SalesOrder",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    CreatedById = table.Column<int>(nullable: true),
                    SalesPersonId = table.Column<int>(nullable: true),
                    CompanyId = table.Column<int>(nullable: true),
                    ContactId = table.Column<int>(nullable: true),
                    Phone = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    SalesOrderPaymentMethodId = table.Column<int>(nullable: true),
                    ShippingMethodId = table.Column<int>(nullable: true),
                    SalesOrderStatusOptionId = table.Column<int>(nullable: true),
                    QuoteId = table.Column<int>(nullable: true),
                    LeadId = table.Column<int>(nullable: true),
                    LeadOriginOptionId = table.Column<int>(nullable: true),
                    GidLocationOptionId = table.Column<int>(nullable: true),
                    CustomerTypeId = table.Column<int>(nullable: true),
                    CurrencyOptionId = table.Column<int>(nullable: false),
                    CustomerNotes = table.Column<string>(nullable: true),
                    ShippingAddressId = table.Column<int>(nullable: true),
                    BillingAddressId = table.Column<int>(nullable: true),
                    SalesTax = table.Column<decimal>(nullable: true),
                    WireTransferFee = table.Column<decimal>(nullable: true),
                    ShippingAndHandlingFee = table.Column<decimal>(nullable: true),
                    ExpediteFee = table.Column<decimal>(nullable: true),
                    CustomerPurchaseOrderNumber = table.Column<string>(nullable: true),
                    SentAt = table.Column<DateTime>(nullable: true),
                    CancelledAt = table.Column<DateTime>(nullable: true),
                    CreditCardId = table.Column<int>(nullable: true),
                    PaypalEmailAddress = table.Column<string>(nullable: true),
                    ShippingCarrierId = table.Column<int>(nullable: true),
                    ShippingCarrierShippingMethodId = table.Column<int>(nullable: true),
                    Total = table.Column<decimal>(nullable: false),
                    ShippingTypeId = table.Column<int>(nullable: true),
                    ShippingAccountNumber = table.Column<string>(nullable: true),
                    PartialShipAccepted = table.Column<bool>(nullable: false),
                    SaturdayDeliveryAccepted = table.Column<bool>(nullable: false),
                    InternalReferenceNumber = table.Column<string>(nullable: true),
                    FreightAccountNumber = table.Column<string>(nullable: true),
                    OutgoingShipmentShippingTermOptionId = table.Column<int>(nullable: true),
                    BiosRequirements = table.Column<string>(nullable: true),
                    CompanyId1 = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesOrder", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SalesOrder_Address_BillingAddressId",
                        column: x => x.BillingAddressId,
                        principalTable: "Address",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesOrder_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesOrder_Company_CompanyId1",
                        column: x => x.CompanyId1,
                        principalTable: "Company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesOrder_Contact_ContactId",
                        column: x => x.ContactId,
                        principalTable: "Contact",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesOrder_CreditCard_CreditCardId",
                        column: x => x.CreditCardId,
                        principalTable: "CreditCard",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesOrder_CurrencyOption_CurrencyOptionId",
                        column: x => x.CurrencyOptionId,
                        principalTable: "CurrencyOption",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesOrder_GidLocationOption_GidLocationOptionId",
                        column: x => x.GidLocationOptionId,
                        principalTable: "GidLocationOption",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesOrder_Quote_QuoteId",
                        column: x => x.QuoteId,
                        principalTable: "Quote",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesOrder_SalesOrderPaymentMethod_SalesOrderPaymentMethodId",
                        column: x => x.SalesOrderPaymentMethodId,
                        principalTable: "SalesOrderPaymentMethod",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesOrder_User_SalesPersonId",
                        column: x => x.SalesPersonId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesOrder_Address_ShippingAddressId",
                        column: x => x.ShippingAddressId,
                        principalTable: "Address",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesOrder_ShippingCarrier_ShippingCarrierId",
                        column: x => x.ShippingCarrierId,
                        principalTable: "ShippingCarrier",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesOrder_ShippingCarrierShippingMethod_ShippingCarrierShippingMethodId",
                        column: x => x.ShippingCarrierShippingMethodId,
                        principalTable: "ShippingCarrierShippingMethod",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LeadLineItemSource",
                columns: table => new
                {
                    LeadLineItemId = table.Column<int>(nullable: false),
                    SourceId = table.Column<int>(nullable: false),
                    Quantity = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeadLineItemSource", x => new { x.LeadLineItemId, x.SourceId });
                    table.ForeignKey(
                        name: "FK_LeadLineItemSource_LeadLineItem_LeadLineItemId",
                        column: x => x.LeadLineItemId,
                        principalTable: "LeadLineItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LeadLineItemSource_Source_SourceId",
                        column: x => x.SourceId,
                        principalTable: "Source",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SourceAttachment",
                columns: table => new
                {
                    AttachmentId = table.Column<int>(nullable: false),
                    SourceId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SourceAttachment", x => new { x.SourceId, x.AttachmentId });
                    table.ForeignKey(
                        name: "FK_SourceAttachment_Attachment_AttachmentId",
                        column: x => x.AttachmentId,
                        principalTable: "Attachment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SourceAttachment_Source_SourceId",
                        column: x => x.SourceId,
                        principalTable: "Source",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SourceNote",
                columns: table => new
                {
                    NoteId = table.Column<int>(nullable: false),
                    SourceId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SourceNote", x => new { x.SourceId, x.NoteId });
                    table.ForeignKey(
                        name: "FK_SourceNote_Note_NoteId",
                        column: x => x.NoteId,
                        principalTable: "Note",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SourceNote_Source_SourceId",
                        column: x => x.SourceId,
                        principalTable: "Source",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BillAttachment",
                columns: table => new
                {
                    AttachmentId = table.Column<int>(nullable: false),
                    BillId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BillAttachment", x => new { x.BillId, x.AttachmentId });
                    table.ForeignKey(
                        name: "FK_BillAttachment_Attachment_AttachmentId",
                        column: x => x.AttachmentId,
                        principalTable: "Attachment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BillAttachment_Bill_BillId",
                        column: x => x.BillId,
                        principalTable: "Bill",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BillChatMessage",
                columns: table => new
                {
                    ChatMessageId = table.Column<int>(nullable: false),
                    BillId = table.Column<int>(nullable: false),
                    BillChatMessageTypeId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BillChatMessage", x => new { x.BillId, x.ChatMessageId });
                    table.ForeignKey(
                        name: "FK_BillChatMessage_Bill_BillId",
                        column: x => x.BillId,
                        principalTable: "Bill",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BillChatMessage_ChatMessage_ChatMessageId",
                        column: x => x.ChatMessageId,
                        principalTable: "ChatMessage",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BillLineItem",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    CreatedById = table.Column<int>(nullable: true),
                    BillId = table.Column<int>(nullable: true),
                    ProductId = table.Column<int>(nullable: true),
                    ManufacturerName = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Quantity = table.Column<int>(nullable: false),
                    Price = table.Column<decimal>(nullable: false),
                    DiscountPercent = table.Column<decimal>(nullable: false),
                    GLAccountId = table.Column<int>(nullable: true),
                    PurchaseOrderLineItemId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BillLineItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BillLineItem_Bill_BillId",
                        column: x => x.BillId,
                        principalTable: "Bill",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BillLineItem_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BillLineItem_PurchaseOrderLineItem_PurchaseOrderLineItemId",
                        column: x => x.PurchaseOrderLineItemId,
                        principalTable: "PurchaseOrderLineItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InventoryItem",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    CreatedById = table.Column<int>(nullable: true),
                    ProductId = table.Column<int>(nullable: true),
                    ManufacturerId = table.Column<int>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    SerialNumber = table.Column<string>(nullable: true),
                    InventoryItemLocationOptionId = table.Column<int>(nullable: true),
                    ProductConditionOptionId = table.Column<int>(nullable: true),
                    InventoryItemStatusOptionId = table.Column<int>(nullable: false),
                    PurchaseOrderLineItemId = table.Column<int>(nullable: true),
                    CurrencyOptionId = table.Column<int>(nullable: true),
                    UnitCost = table.Column<decimal>(nullable: true),
                    TotalCost = table.Column<decimal>(nullable: true),
                    InventoryItemId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryItem_InventoryItem_InventoryItemId",
                        column: x => x.InventoryItemId,
                        principalTable: "InventoryItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventoryItem_Company_ManufacturerId",
                        column: x => x.ManufacturerId,
                        principalTable: "Company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventoryItem_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventoryItem_PurchaseOrderLineItem_PurchaseOrderLineItemId",
                        column: x => x.PurchaseOrderLineItemId,
                        principalTable: "PurchaseOrderLineItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "QuoteLineItemSource",
                columns: table => new
                {
                    QuoteLineItemId = table.Column<int>(nullable: false),
                    SourceId = table.Column<int>(nullable: false),
                    Quantity = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuoteLineItemSource", x => new { x.QuoteLineItemId, x.SourceId });
                    table.ForeignKey(
                        name: "FK_QuoteLineItemSource_QuoteLineItem_QuoteLineItemId",
                        column: x => x.QuoteLineItemId,
                        principalTable: "QuoteLineItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuoteLineItemSource_Source_SourceId",
                        column: x => x.SourceId,
                        principalTable: "Source",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Invoice",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    CreatedById = table.Column<int>(nullable: true),
                    CompanyId = table.Column<int>(nullable: true),
                    ContactId = table.Column<int>(nullable: true),
                    SalesPersonId = table.Column<int>(nullable: true),
                    SalesOrderId = table.Column<int>(nullable: true),
                    Balance = table.Column<decimal>(nullable: true),
                    DateSent = table.Column<DateTime>(nullable: true),
                    DateDue = table.Column<DateTime>(nullable: true),
                    ShippedOnly = table.Column<bool>(nullable: true),
                    CurrencyOptionId = table.Column<int>(nullable: true),
                    GidLocationOptionId = table.Column<int>(nullable: true),
                    ShippingAndHandlingFee = table.Column<decimal>(nullable: false),
                    SalesTax = table.Column<decimal>(nullable: false),
                    WireTransferFee = table.Column<decimal>(nullable: false),
                    ExpediteFee = table.Column<decimal>(nullable: false),
                    DiscountPercent = table.Column<decimal>(nullable: false),
                    SentAt = table.Column<DateTime>(nullable: true),
                    CancelledAt = table.Column<DateTime>(nullable: true),
                    BillingAddressId = table.Column<int>(nullable: true),
                    ShippingAddressId = table.Column<int>(nullable: true),
                    CustomerPurchaseOrderNumber = table.Column<string>(nullable: true),
                    ShippingCarrierId = table.Column<int>(nullable: true),
                    ShippingCarrierShippingMethodId = table.Column<int>(nullable: true),
                    Phone = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    QuickBooksId = table.Column<string>(nullable: true),
                    QuickBooksSyncToken = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invoice", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Invoice_Address_BillingAddressId",
                        column: x => x.BillingAddressId,
                        principalTable: "Address",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Invoice_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Invoice_Contact_ContactId",
                        column: x => x.ContactId,
                        principalTable: "Contact",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Invoice_CurrencyOption_CurrencyOptionId",
                        column: x => x.CurrencyOptionId,
                        principalTable: "CurrencyOption",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Invoice_GidLocationOption_GidLocationOptionId",
                        column: x => x.GidLocationOptionId,
                        principalTable: "GidLocationOption",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Invoice_SalesOrder_SalesOrderId",
                        column: x => x.SalesOrderId,
                        principalTable: "SalesOrder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Invoice_User_SalesPersonId",
                        column: x => x.SalesPersonId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Invoice_Address_ShippingAddressId",
                        column: x => x.ShippingAddressId,
                        principalTable: "Address",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Invoice_ShippingCarrier_ShippingCarrierId",
                        column: x => x.ShippingCarrierId,
                        principalTable: "ShippingCarrier",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Invoice_ShippingCarrierShippingMethod_ShippingCarrierShippingMethodId",
                        column: x => x.ShippingCarrierShippingMethodId,
                        principalTable: "ShippingCarrierShippingMethod",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Repair",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    CreatedById = table.Column<int>(nullable: true),
                    SalesOrderId = table.Column<int>(nullable: true),
                    DateIssued = table.Column<DateTime>(nullable: true),
                    RepairAuthorizationAttachmentId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Repair", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Repair_Attachment_RepairAuthorizationAttachmentId",
                        column: x => x.RepairAuthorizationAttachmentId,
                        principalTable: "Attachment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Repair_SalesOrder_SalesOrderId",
                        column: x => x.SalesOrderId,
                        principalTable: "SalesOrder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Rma",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    CreatedById = table.Column<int>(nullable: true),
                    CompanyId = table.Column<int>(nullable: true),
                    RmaStatusOptionId = table.Column<int>(nullable: true),
                    SalesOrderId = table.Column<int>(nullable: true),
                    RmaReasonOptionId = table.Column<int>(nullable: true),
                    RmaActionOptionId = table.Column<int>(nullable: true),
                    SentAt = table.Column<DateTime>(nullable: true),
                    CreditAmount = table.Column<decimal>(nullable: true),
                    CurrencyOptionId = table.Column<int>(nullable: true),
                    GidLocationOptionId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rma", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rma_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Rma_CurrencyOption_CurrencyOptionId",
                        column: x => x.CurrencyOptionId,
                        principalTable: "CurrencyOption",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Rma_GidLocationOption_GidLocationOptionId",
                        column: x => x.GidLocationOptionId,
                        principalTable: "GidLocationOption",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Rma_RmaActionOption_RmaActionOptionId",
                        column: x => x.RmaActionOptionId,
                        principalTable: "RmaActionOption",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Rma_RmaReasonOption_RmaReasonOptionId",
                        column: x => x.RmaReasonOptionId,
                        principalTable: "RmaReasonOption",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Rma_SalesOrder_SalesOrderId",
                        column: x => x.SalesOrderId,
                        principalTable: "SalesOrder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SalesOrderAttachment",
                columns: table => new
                {
                    AttachmentId = table.Column<int>(nullable: false),
                    SalesOrderId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesOrderAttachment", x => new { x.SalesOrderId, x.AttachmentId });
                    table.ForeignKey(
                        name: "FK_SalesOrderAttachment_Attachment_AttachmentId",
                        column: x => x.AttachmentId,
                        principalTable: "Attachment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SalesOrderAttachment_SalesOrder_SalesOrderId",
                        column: x => x.SalesOrderId,
                        principalTable: "SalesOrder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SalesOrderChatMessage",
                columns: table => new
                {
                    ChatMessageId = table.Column<int>(nullable: false),
                    SalesOrderId = table.Column<int>(nullable: false),
                    SalesOrderChatMessageTypeId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesOrderChatMessage", x => new { x.SalesOrderId, x.ChatMessageId });
                    table.ForeignKey(
                        name: "FK_SalesOrderChatMessage_ChatMessage_ChatMessageId",
                        column: x => x.ChatMessageId,
                        principalTable: "ChatMessage",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SalesOrderChatMessage_SalesOrder_SalesOrderId",
                        column: x => x.SalesOrderId,
                        principalTable: "SalesOrder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SalesOrderEventLogEntry",
                columns: table => new
                {
                    EventLogEntryId = table.Column<int>(nullable: false),
                    SalesOrderId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesOrderEventLogEntry", x => new { x.SalesOrderId, x.EventLogEntryId });
                    table.ForeignKey(
                        name: "FK_SalesOrderEventLogEntry_EventLogEntry_EventLogEntryId",
                        column: x => x.EventLogEntryId,
                        principalTable: "EventLogEntry",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SalesOrderEventLogEntry_SalesOrder_SalesOrderId",
                        column: x => x.SalesOrderId,
                        principalTable: "SalesOrder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SalesOrderLineItem",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    CreatedById = table.Column<int>(nullable: true),
                    ProductName = table.Column<string>(nullable: true),
                    ProductId = table.Column<int>(nullable: true),
                    DisplayPartNumber = table.Column<string>(nullable: true),
                    ManufacturerName = table.Column<string>(nullable: true),
                    ManufacturerId = table.Column<int>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    LineItemServiceTypeId = table.Column<int>(nullable: true),
                    LineItemConditionTypeId = table.Column<int>(nullable: true),
                    Quantity = table.Column<int>(nullable: true),
                    Price = table.Column<decimal>(nullable: true),
                    DiscountPercent = table.Column<decimal>(nullable: true),
                    SalesOrderId = table.Column<int>(nullable: false),
                    CpuRequirements = table.Column<string>(nullable: true),
                    CpuQuantity = table.Column<int>(nullable: true),
                    CpuStockVerifiedOptionId = table.Column<int>(nullable: true),
                    MemoryQuantity = table.Column<int>(nullable: true),
                    MemoryStockVerifiedOptionId = table.Column<int>(nullable: true),
                    DeclaredValue = table.Column<decimal>(nullable: true),
                    CountryOfOriginId = table.Column<int>(nullable: true),
                    ScheduleB = table.Column<string>(nullable: true),
                    LeadTimeRangeStart = table.Column<int>(nullable: true),
                    LeadTimeRangeEnd = table.Column<int>(nullable: true),
                    LeadTimeRangeUnit = table.Column<string>(nullable: true),
                    OutgoingLineItemWarrantyOptionId = table.Column<int>(nullable: true),
                    OutgoingLineItemLeadTimeOptionId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesOrderLineItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SalesOrderLineItem_Country_CountryOfOriginId",
                        column: x => x.CountryOfOriginId,
                        principalTable: "Country",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesOrderLineItem_LineItemConditionType_LineItemConditionTypeId",
                        column: x => x.LineItemConditionTypeId,
                        principalTable: "LineItemConditionType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesOrderLineItem_Company_ManufacturerId",
                        column: x => x.ManufacturerId,
                        principalTable: "Company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesOrderLineItem_OutgoingLineItemLeadTimeOption_OutgoingLineItemLeadTimeOptionId",
                        column: x => x.OutgoingLineItemLeadTimeOptionId,
                        principalTable: "OutgoingLineItemLeadTimeOption",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesOrderLineItem_OutgoingLineItemWarrantyOption_OutgoingLineItemWarrantyOptionId",
                        column: x => x.OutgoingLineItemWarrantyOptionId,
                        principalTable: "OutgoingLineItemWarrantyOption",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesOrderLineItem_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesOrderLineItem_SalesOrder_SalesOrderId",
                        column: x => x.SalesOrderId,
                        principalTable: "SalesOrder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SalesOrderNote",
                columns: table => new
                {
                    NoteId = table.Column<int>(nullable: false),
                    SalesOrderId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesOrderNote", x => new { x.SalesOrderId, x.NoteId });
                    table.ForeignKey(
                        name: "FK_SalesOrderNote_Note_NoteId",
                        column: x => x.NoteId,
                        principalTable: "Note",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SalesOrderNote_SalesOrder_SalesOrderId",
                        column: x => x.SalesOrderId,
                        principalTable: "SalesOrder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SalesOrderOutgoingShipment",
                columns: table => new
                {
                    SalesOrderId = table.Column<int>(nullable: false),
                    OutgoingShipmentId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesOrderOutgoingShipment", x => new { x.OutgoingShipmentId, x.SalesOrderId });
                    table.ForeignKey(
                        name: "FK_SalesOrderOutgoingShipment_OutgoingShipment_OutgoingShipmentId",
                        column: x => x.OutgoingShipmentId,
                        principalTable: "OutgoingShipment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SalesOrderOutgoingShipment_SalesOrder_SalesOrderId",
                        column: x => x.SalesOrderId,
                        principalTable: "SalesOrder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SalesOrderPurchaseOrder",
                columns: table => new
                {
                    SalesOrderId = table.Column<int>(nullable: false),
                    PurchaseOrderId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesOrderPurchaseOrder", x => new { x.PurchaseOrderId, x.SalesOrderId });
                    table.ForeignKey(
                        name: "FK_SalesOrderPurchaseOrder_PurchaseOrder_PurchaseOrderId",
                        column: x => x.PurchaseOrderId,
                        principalTable: "PurchaseOrder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesOrderPurchaseOrder_SalesOrder_SalesOrderId",
                        column: x => x.SalesOrderId,
                        principalTable: "SalesOrder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IncomingShipmentInventoryItem",
                columns: table => new
                {
                    IncomingShipmentId = table.Column<int>(nullable: false),
                    InventoryItemId = table.Column<int>(nullable: false),
                    ReceivedAt = table.Column<DateTime>(nullable: true),
                    ReceivedById = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncomingShipmentInventoryItem", x => new { x.IncomingShipmentId, x.InventoryItemId });
                    table.ForeignKey(
                        name: "FK_IncomingShipmentInventoryItem_IncomingShipment_IncomingShipmentId",
                        column: x => x.IncomingShipmentId,
                        principalTable: "IncomingShipment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IncomingShipmentInventoryItem_InventoryItem_InventoryItemId",
                        column: x => x.InventoryItemId,
                        principalTable: "InventoryItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InventoryItemAttachment",
                columns: table => new
                {
                    AttachmentId = table.Column<int>(nullable: false),
                    InventoryItemId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryItemAttachment", x => new { x.InventoryItemId, x.AttachmentId });
                    table.ForeignKey(
                        name: "FK_InventoryItemAttachment_Attachment_AttachmentId",
                        column: x => x.AttachmentId,
                        principalTable: "Attachment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InventoryItemAttachment_InventoryItem_InventoryItemId",
                        column: x => x.InventoryItemId,
                        principalTable: "InventoryItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InventoryItemEventLogEntry",
                columns: table => new
                {
                    EventLogEntryId = table.Column<int>(nullable: false),
                    InventoryItemId = table.Column<int>(nullable: false),
                    EventLogEntryId1 = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryItemEventLogEntry", x => new { x.InventoryItemId, x.EventLogEntryId });
                    table.ForeignKey(
                        name: "FK_InventoryItemEventLogEntry_EventLogEntry_EventLogEntryId",
                        column: x => x.EventLogEntryId,
                        principalTable: "EventLogEntry",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InventoryItemEventLogEntry_EventLogEntry_EventLogEntryId1",
                        column: x => x.EventLogEntryId1,
                        principalTable: "EventLogEntry",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventoryItemEventLogEntry_InventoryItem_InventoryItemId",
                        column: x => x.InventoryItemId,
                        principalTable: "InventoryItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InventoryItemNote",
                columns: table => new
                {
                    NoteId = table.Column<int>(nullable: false),
                    InventoryItemId = table.Column<int>(nullable: false),
                    NoteId1 = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryItemNote", x => new { x.InventoryItemId, x.NoteId });
                    table.ForeignKey(
                        name: "FK_InventoryItemNote_InventoryItem_InventoryItemId",
                        column: x => x.InventoryItemId,
                        principalTable: "InventoryItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InventoryItemNote_Note_NoteId",
                        column: x => x.NoteId,
                        principalTable: "Note",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InventoryItemNote_Note_NoteId1",
                        column: x => x.NoteId1,
                        principalTable: "Note",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InventoryItemRelatedInventoryItem",
                columns: table => new
                {
                    CreatedAt = table.Column<DateTime>(nullable: true, defaultValueSql: "GETUTCDATE()"),
                    ParentInventoryItemId = table.Column<int>(nullable: false),
                    ChildInventoryItemId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryItemRelatedInventoryItem", x => new { x.ParentInventoryItemId, x.ChildInventoryItemId });
                    table.ForeignKey(
                        name: "FK_InventoryItemRelatedInventoryItem_InventoryItem_ChildInventoryItemId",
                        column: x => x.ChildInventoryItemId,
                        principalTable: "InventoryItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventoryItemRelatedInventoryItem_InventoryItem_ParentInventoryItemId",
                        column: x => x.ParentInventoryItemId,
                        principalTable: "InventoryItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OutgoingShipmentBoxInventoryItem",
                columns: table => new
                {
                    CreatedAt = table.Column<DateTime>(nullable: true, defaultValueSql: "GETUTCDATE()"),
                    CreatedById = table.Column<int>(nullable: true),
                    OutgoingShipmentBoxId = table.Column<int>(nullable: false),
                    InventoryItemId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutgoingShipmentBoxInventoryItem", x => new { x.OutgoingShipmentBoxId, x.InventoryItemId });
                    table.ForeignKey(
                        name: "FK_OutgoingShipmentBoxInventoryItem_InventoryItem_InventoryItemId",
                        column: x => x.InventoryItemId,
                        principalTable: "InventoryItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OutgoingShipmentBoxInventoryItem_OutgoingShipmentBox_OutgoingShipmentBoxId",
                        column: x => x.OutgoingShipmentBoxId,
                        principalTable: "OutgoingShipmentBox",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SalesOrderInventoryItem",
                columns: table => new
                {
                    SalesOrderId = table.Column<int>(nullable: false),
                    InventoryItemId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesOrderInventoryItem", x => new { x.InventoryItemId, x.SalesOrderId });
                    table.ForeignKey(
                        name: "FK_SalesOrderInventoryItem_InventoryItem_InventoryItemId",
                        column: x => x.InventoryItemId,
                        principalTable: "InventoryItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesOrderInventoryItem_SalesOrder_SalesOrderId",
                        column: x => x.SalesOrderId,
                        principalTable: "SalesOrder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CreditCardTransaction",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true, defaultValueSql: "GETUTCDATE()"),
                    CreatedById = table.Column<int>(nullable: true),
                    InvoiceId = table.Column<int>(nullable: true),
                    SalesOrderId = table.Column<int>(nullable: true),
                    CurrencyOptionId = table.Column<int>(nullable: true),
                    Amount = table.Column<decimal>(nullable: false),
                    AmountCaptured = table.Column<decimal>(nullable: true),
                    NotCaptured = table.Column<bool>(nullable: true),
                    AmountRefunded = table.Column<decimal>(nullable: true),
                    FullyRefunded = table.Column<bool>(nullable: true),
                    VoidedAt = table.Column<DateTime>(nullable: true),
                    ResultCode = table.Column<string>(nullable: true),
                    ResultMessageCode = table.Column<string>(nullable: true),
                    ResultMessageText = table.Column<string>(nullable: true),
                    ErrorCode = table.Column<string>(nullable: true),
                    ErrorMessage = table.Column<string>(nullable: true),
                    OverallStatus = table.Column<string>(nullable: true),
                    Succeeded = table.Column<bool>(nullable: false),
                    Last4 = table.Column<string>(nullable: true),
                    CardBrand = table.Column<string>(nullable: true),
                    CreditCardTransactionType = table.Column<int>(nullable: false),
                    BankAccountId = table.Column<int>(nullable: true),
                    PaymentProfileId = table.Column<string>(nullable: true),
                    CompanyId = table.Column<int>(nullable: true),
                    TransactionId = table.Column<string>(nullable: true),
                    RelatedTransactionId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreditCardTransaction", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CreditCardTransaction_Invoice_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "Invoice",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CreditCardTransaction_SalesOrder_SalesOrderId",
                        column: x => x.SalesOrderId,
                        principalTable: "SalesOrder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InvoiceAttachment",
                columns: table => new
                {
                    AttachmentId = table.Column<int>(nullable: false),
                    InvoiceId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceAttachment", x => new { x.InvoiceId, x.AttachmentId });
                    table.ForeignKey(
                        name: "FK_InvoiceAttachment_Attachment_AttachmentId",
                        column: x => x.AttachmentId,
                        principalTable: "Attachment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InvoiceAttachment_Invoice_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "Invoice",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InvoiceCashReceipt",
                columns: table => new
                {
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    CreatedById = table.Column<int>(nullable: true),
                    InvoiceId = table.Column<int>(nullable: false),
                    CashReceiptId = table.Column<int>(nullable: false),
                    Amount = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceCashReceipt", x => new { x.InvoiceId, x.CashReceiptId });
                    table.ForeignKey(
                        name: "FK_InvoiceCashReceipt_CashReceipt_CashReceiptId",
                        column: x => x.CashReceiptId,
                        principalTable: "CashReceipt",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InvoiceCashReceipt_Invoice_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "Invoice",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InvoiceChatMessage",
                columns: table => new
                {
                    ChatMessageId = table.Column<int>(nullable: false),
                    InvoiceId = table.Column<int>(nullable: false),
                    InvoiceChatMessageTypeId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceChatMessage", x => new { x.InvoiceId, x.ChatMessageId });
                    table.ForeignKey(
                        name: "FK_InvoiceChatMessage_ChatMessage_ChatMessageId",
                        column: x => x.ChatMessageId,
                        principalTable: "ChatMessage",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InvoiceChatMessage_Invoice_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "Invoice",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InvoiceEventLogEntry",
                columns: table => new
                {
                    EventLogEntryId = table.Column<int>(nullable: false),
                    InvoiceId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceEventLogEntry", x => new { x.InvoiceId, x.EventLogEntryId });
                    table.ForeignKey(
                        name: "FK_InvoiceEventLogEntry_EventLogEntry_EventLogEntryId",
                        column: x => x.EventLogEntryId,
                        principalTable: "EventLogEntry",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InvoiceEventLogEntry_Invoice_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "Invoice",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RepairIncomingShipment",
                columns: table => new
                {
                    RepairId = table.Column<int>(nullable: false),
                    IncomingShipmentId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RepairIncomingShipment", x => new { x.RepairId, x.IncomingShipmentId });
                    table.ForeignKey(
                        name: "FK_RepairIncomingShipment_IncomingShipment_IncomingShipmentId",
                        column: x => x.IncomingShipmentId,
                        principalTable: "IncomingShipment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RepairIncomingShipment_Repair_RepairId",
                        column: x => x.RepairId,
                        principalTable: "Repair",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CashDisbursement",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    CreatedById = table.Column<int>(nullable: true),
                    CompanyId = table.Column<int>(nullable: true),
                    CashDisbursementReasonOptionId = table.Column<int>(nullable: true),
                    CurrencyOptionId = table.Column<int>(nullable: true),
                    PurchaseOrderPaymentMethodId = table.Column<int>(nullable: true),
                    Amount = table.Column<decimal>(nullable: false),
                    Balance = table.Column<decimal>(nullable: false),
                    PaymentAccountId = table.Column<int>(nullable: true),
                    ReferenceNumber = table.Column<string>(nullable: true),
                    Note = table.Column<string>(nullable: true),
                    QuickBooksBillPaymentId = table.Column<string>(nullable: true),
                    QuickBooksBillPaymentSyncToken = table.Column<string>(nullable: true),
                    QuickBooksRefundReceiptId = table.Column<string>(nullable: true),
                    QuickBooksRefundReceiptSyncToken = table.Column<string>(nullable: true),
                    DateDisbursed = table.Column<DateTime>(nullable: true),
                    SalesOrderId = table.Column<int>(nullable: true),
                    RmaId = table.Column<int>(nullable: true),
                    CashDisbursementTypeId = table.Column<int>(nullable: true),
                    BankAccountId = table.Column<int>(nullable: true),
                    CreditCardTransactionId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CashDisbursement", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CashDisbursement_BankAccount_BankAccountId",
                        column: x => x.BankAccountId,
                        principalTable: "BankAccount",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CashDisbursement_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CashDisbursement_PaymentAccount_PaymentAccountId",
                        column: x => x.PaymentAccountId,
                        principalTable: "PaymentAccount",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CashDisbursement_Rma_RmaId",
                        column: x => x.RmaId,
                        principalTable: "Rma",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Credit",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    CreatedById = table.Column<int>(nullable: true),
                    CompanyId = table.Column<int>(nullable: true),
                    RmaId = table.Column<int>(nullable: true),
                    Amount = table.Column<decimal>(nullable: false),
                    Balance = table.Column<decimal>(nullable: false),
                    CurrencyOptionId = table.Column<decimal>(nullable: false),
                    Note = table.Column<string>(nullable: true),
                    CreditedAt = table.Column<DateTime>(nullable: true),
                    CreditAccountId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Credit", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Credit_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Credit_CreditAccount_CreditAccountId",
                        column: x => x.CreditAccountId,
                        principalTable: "CreditAccount",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Credit_Rma_RmaId",
                        column: x => x.RmaId,
                        principalTable: "Rma",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RmaAttachment",
                columns: table => new
                {
                    AttachmentId = table.Column<int>(nullable: false),
                    RmaId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RmaAttachment", x => new { x.RmaId, x.AttachmentId });
                    table.ForeignKey(
                        name: "FK_RmaAttachment_Attachment_AttachmentId",
                        column: x => x.AttachmentId,
                        principalTable: "Attachment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RmaAttachment_Rma_RmaId",
                        column: x => x.RmaId,
                        principalTable: "Rma",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RmaChatMessage",
                columns: table => new
                {
                    ChatMessageId = table.Column<int>(nullable: false),
                    RmaId = table.Column<int>(nullable: false),
                    RmaChatMessageTypeId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RmaChatMessage", x => new { x.RmaId, x.ChatMessageId });
                    table.ForeignKey(
                        name: "FK_RmaChatMessage_ChatMessage_ChatMessageId",
                        column: x => x.ChatMessageId,
                        principalTable: "ChatMessage",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RmaChatMessage_Rma_RmaId",
                        column: x => x.RmaId,
                        principalTable: "Rma",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RmaEventLogEntry",
                columns: table => new
                {
                    EventLogEntryId = table.Column<int>(nullable: false),
                    RmaId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RmaEventLogEntry", x => new { x.RmaId, x.EventLogEntryId });
                    table.ForeignKey(
                        name: "FK_RmaEventLogEntry_EventLogEntry_EventLogEntryId",
                        column: x => x.EventLogEntryId,
                        principalTable: "EventLogEntry",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RmaEventLogEntry_Rma_RmaId",
                        column: x => x.RmaId,
                        principalTable: "Rma",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RmaIncomingShipment",
                columns: table => new
                {
                    RmaId = table.Column<int>(nullable: false),
                    IncomingShipmentId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RmaIncomingShipment", x => new { x.RmaId, x.IncomingShipmentId });
                    table.ForeignKey(
                        name: "FK_RmaIncomingShipment_IncomingShipment_IncomingShipmentId",
                        column: x => x.IncomingShipmentId,
                        principalTable: "IncomingShipment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RmaIncomingShipment_Rma_RmaId",
                        column: x => x.RmaId,
                        principalTable: "Rma",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RmaOutgoingShipment",
                columns: table => new
                {
                    RmaId = table.Column<int>(nullable: false),
                    OutgoingShipmentId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RmaOutgoingShipment", x => new { x.OutgoingShipmentId, x.RmaId });
                    table.ForeignKey(
                        name: "FK_RmaOutgoingShipment_OutgoingShipment_OutgoingShipmentId",
                        column: x => x.OutgoingShipmentId,
                        principalTable: "OutgoingShipment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RmaOutgoingShipment_Rma_RmaId",
                        column: x => x.RmaId,
                        principalTable: "Rma",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InvoiceLineItem",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    CreatedById = table.Column<int>(nullable: true),
                    InvoiceId = table.Column<int>(nullable: true),
                    ProductId = table.Column<int>(nullable: true),
                    ManufacturerName = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Quantity = table.Column<int>(nullable: true),
                    Price = table.Column<decimal>(nullable: true),
                    DiscountPercent = table.Column<decimal>(nullable: true),
                    SalesOrderLineItemId = table.Column<int>(nullable: true),
                    QuickBooksId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceLineItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvoiceLineItem_Invoice_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "Invoice",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InvoiceLineItem_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InvoiceLineItem_SalesOrderLineItem_SalesOrderLineItemId",
                        column: x => x.SalesOrderLineItemId,
                        principalTable: "SalesOrderLineItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RmaLineItem",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    RmaId = table.Column<int>(nullable: false),
                    InventoryItemId = table.Column<int>(nullable: false),
                    Price = table.Column<decimal>(nullable: true),
                    SalesOrderLineItemId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RmaLineItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RmaLineItem_InventoryItem_InventoryItemId",
                        column: x => x.InventoryItemId,
                        principalTable: "InventoryItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RmaLineItem_Rma_RmaId",
                        column: x => x.RmaId,
                        principalTable: "Rma",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RmaLineItem_SalesOrderLineItem_SalesOrderLineItemId",
                        column: x => x.SalesOrderLineItemId,
                        principalTable: "SalesOrderLineItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SalesOrderLineItemInventoryItem",
                columns: table => new
                {
                    SalesOrderLineItemId = table.Column<int>(nullable: false),
                    InventoryItemId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesOrderLineItemInventoryItem", x => new { x.InventoryItemId, x.SalesOrderLineItemId });
                    table.ForeignKey(
                        name: "FK_SalesOrderLineItemInventoryItem_InventoryItem_InventoryItemId",
                        column: x => x.InventoryItemId,
                        principalTable: "InventoryItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SalesOrderLineItemInventoryItem_SalesOrderLineItem_SalesOrderLineItemId",
                        column: x => x.SalesOrderLineItemId,
                        principalTable: "SalesOrderLineItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SalesOrderLineItemSource",
                columns: table => new
                {
                    SalesOrderLineItemId = table.Column<int>(nullable: false),
                    SourceId = table.Column<int>(nullable: false),
                    PurchaseOrderId = table.Column<int>(nullable: true),
                    Quantity = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesOrderLineItemSource", x => new { x.SalesOrderLineItemId, x.SourceId });
                    table.ForeignKey(
                        name: "FK_SalesOrderLineItemSource_PurchaseOrder_PurchaseOrderId",
                        column: x => x.PurchaseOrderId,
                        principalTable: "PurchaseOrder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_SalesOrderLineItemSource_SalesOrderLineItem_SalesOrderLineItemId",
                        column: x => x.SalesOrderLineItemId,
                        principalTable: "SalesOrderLineItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SalesOrderLineItemSource_Source_SourceId",
                        column: x => x.SourceId,
                        principalTable: "Source",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BillCashDisbursement",
                columns: table => new
                {
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    CreatedById = table.Column<int>(nullable: true),
                    BillId = table.Column<int>(nullable: false),
                    CashDisbursementId = table.Column<int>(nullable: false),
                    Amount = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BillCashDisbursement", x => new { x.BillId, x.CashDisbursementId });
                    table.ForeignKey(
                        name: "FK_BillCashDisbursement_Bill_BillId",
                        column: x => x.BillId,
                        principalTable: "Bill",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BillCashDisbursement_CashDisbursement_CashDisbursementId",
                        column: x => x.CashDisbursementId,
                        principalTable: "CashDisbursement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InvoiceCredit",
                columns: table => new
                {
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    CreatedById = table.Column<int>(nullable: true),
                    InvoiceId = table.Column<int>(nullable: false),
                    CreditId = table.Column<int>(nullable: false),
                    Amount = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceCredit", x => new { x.InvoiceId, x.CreditId });
                    table.ForeignKey(
                        name: "FK_InvoiceCredit_Credit_CreditId",
                        column: x => x.CreditId,
                        principalTable: "Credit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InvoiceCredit_Invoice_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "Invoice",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Address_CountryId",
                table: "Address",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Attachment_CreatedById",
                table: "Attachment",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Bill_CompanyId",
                table: "Bill",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Bill_PurchaseOrderId",
                table: "Bill",
                column: "PurchaseOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_BillAttachment_AttachmentId",
                table: "BillAttachment",
                column: "AttachmentId");

            migrationBuilder.CreateIndex(
                name: "IX_BillCashDisbursement_CashDisbursementId",
                table: "BillCashDisbursement",
                column: "CashDisbursementId");

            migrationBuilder.CreateIndex(
                name: "IX_BillChatMessage_ChatMessageId",
                table: "BillChatMessage",
                column: "ChatMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_BillLineItem_BillId",
                table: "BillLineItem",
                column: "BillId");

            migrationBuilder.CreateIndex(
                name: "IX_BillLineItem_ProductId",
                table: "BillLineItem",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_BillLineItem_PurchaseOrderLineItemId",
                table: "BillLineItem",
                column: "PurchaseOrderLineItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CashDisbursement_BankAccountId",
                table: "CashDisbursement",
                column: "BankAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_CashDisbursement_CompanyId",
                table: "CashDisbursement",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_CashDisbursement_CurrencyOptionId",
                table: "CashDisbursement",
                column: "CurrencyOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_CashDisbursement_PaymentAccountId",
                table: "CashDisbursement",
                column: "PaymentAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_CashDisbursement_RmaId",
                table: "CashDisbursement",
                column: "RmaId");

            migrationBuilder.CreateIndex(
                name: "IX_CashReceipt_BankAccountId",
                table: "CashReceipt",
                column: "BankAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_CashReceipt_CompanyId",
                table: "CashReceipt",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_CashReceipt_CurrencyOptionId",
                table: "CashReceipt",
                column: "CurrencyOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessage_ChatMessageId",
                table: "ChatMessage",
                column: "ChatMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessageAttachment_AttachmentId",
                table: "ChatMessageAttachment",
                column: "AttachmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Company_CreatedAt",
                table: "Company",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Company_Name",
                table: "Company",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Company_ParentCompanyId",
                table: "Company",
                column: "ParentCompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Company_BillingAddressCompanyId_BillingAddressAddressId",
                table: "Company",
                columns: new[] { "BillingAddressCompanyId", "BillingAddressAddressId" });

            migrationBuilder.CreateIndex(
                name: "IX_Company_ShippingAddressCompanyId_ShippingAddressAddressId",
                table: "Company",
                columns: new[] { "ShippingAddressCompanyId", "ShippingAddressAddressId" });

            migrationBuilder.CreateIndex(
                name: "IX_CompanyAddress_AddressId",
                table: "CompanyAddress",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyAlias_Alias",
                table: "CompanyAlias",
                column: "Alias",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CompanyAlias_CompanyId",
                table: "CompanyAlias",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyAttachment_AttachmentId",
                table: "CompanyAttachment",
                column: "AttachmentId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyContact_CompanyId",
                table: "CompanyContact",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyEmailAddress_CompanyId",
                table: "CompanyEmailAddress",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyEmailAddress_EmailAddressId",
                table: "CompanyEmailAddress",
                column: "EmailAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyEventLogEntry_EventLogEntryId",
                table: "CompanyEventLogEntry",
                column: "EventLogEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyNote_NoteId",
                table: "CompanyNote",
                column: "NoteId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyPhoneNumber_CompanyId",
                table: "CompanyPhoneNumber",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyPhoneNumber_PhoneNumberId",
                table: "CompanyPhoneNumber",
                column: "PhoneNumberId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyPortal_CompanyId",
                table: "CompanyPortal",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyPortal_PortalId",
                table: "CompanyPortal",
                column: "PortalId");

            migrationBuilder.CreateIndex(
                name: "IX_Contact_CreatedAt",
                table: "Contact",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Contact_FirstName",
                table: "Contact",
                column: "FirstName");

            migrationBuilder.CreateIndex(
                name: "IX_Contact_LastName",
                table: "Contact",
                column: "LastName");

            migrationBuilder.CreateIndex(
                name: "IX_ContactAddress_AddressId",
                table: "ContactAddress",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_ContactAttachment_AttachmentId",
                table: "ContactAttachment",
                column: "AttachmentId");

            migrationBuilder.CreateIndex(
                name: "IX_ContactEmailAddress_ContactId",
                table: "ContactEmailAddress",
                column: "ContactId");

            migrationBuilder.CreateIndex(
                name: "IX_ContactEmailAddress_EmailAddressId",
                table: "ContactEmailAddress",
                column: "EmailAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_ContactEventLogEntry_EventLogEntryId",
                table: "ContactEventLogEntry",
                column: "EventLogEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_ContactNote_NoteId",
                table: "ContactNote",
                column: "NoteId");

            migrationBuilder.CreateIndex(
                name: "IX_ContactPhoneNumber_ContactId",
                table: "ContactPhoneNumber",
                column: "ContactId");

            migrationBuilder.CreateIndex(
                name: "IX_ContactPhoneNumber_PhoneNumberId",
                table: "ContactPhoneNumber",
                column: "PhoneNumberId");

            migrationBuilder.CreateIndex(
                name: "IX_Credit_CompanyId",
                table: "Credit",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Credit_CreditAccountId",
                table: "Credit",
                column: "CreditAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Credit_CurrencyOptionId",
                table: "Credit",
                column: "CurrencyOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_Credit_RmaId",
                table: "Credit",
                column: "RmaId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditCardTransaction_CreditCardTransactionType",
                table: "CreditCardTransaction",
                column: "CreditCardTransactionType");

            migrationBuilder.CreateIndex(
                name: "IX_CreditCardTransaction_InvoiceId",
                table: "CreditCardTransaction",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditCardTransaction_SalesOrderId",
                table: "CreditCardTransaction",
                column: "SalesOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailAddress_Address",
                table: "EmailAddress",
                column: "Address");

            migrationBuilder.CreateIndex(
                name: "IX_GidLocationOption_DefaultShippingAddressId",
                table: "GidLocationOption",
                column: "DefaultShippingAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_GidLocationOption_MainAddressId",
                table: "GidLocationOption",
                column: "MainAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_IncomingShipmentInventoryItem_InventoryItemId",
                table: "IncomingShipmentInventoryItem",
                column: "InventoryItemId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItem_CreatedAt",
                table: "InventoryItem",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItem_InventoryItemId",
                table: "InventoryItem",
                column: "InventoryItemId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItem_ManufacturerId",
                table: "InventoryItem",
                column: "ManufacturerId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItem_ProductId",
                table: "InventoryItem",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItem_PurchaseOrderLineItemId",
                table: "InventoryItem",
                column: "PurchaseOrderLineItemId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItem_SerialNumber",
                table: "InventoryItem",
                column: "SerialNumber");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItemAttachment_AttachmentId",
                table: "InventoryItemAttachment",
                column: "AttachmentId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItemEventLogEntry_EventLogEntryId",
                table: "InventoryItemEventLogEntry",
                column: "EventLogEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItemEventLogEntry_EventLogEntryId1",
                table: "InventoryItemEventLogEntry",
                column: "EventLogEntryId1");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItemNote_NoteId",
                table: "InventoryItemNote",
                column: "NoteId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItemNote_NoteId1",
                table: "InventoryItemNote",
                column: "NoteId1");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItemRelatedInventoryItem_ChildInventoryItemId",
                table: "InventoryItemRelatedInventoryItem",
                column: "ChildInventoryItemId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Invoice_BillingAddressId",
                table: "Invoice",
                column: "BillingAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoice_CompanyId",
                table: "Invoice",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoice_ContactId",
                table: "Invoice",
                column: "ContactId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoice_CreatedAt",
                table: "Invoice",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Invoice_CurrencyOptionId",
                table: "Invoice",
                column: "CurrencyOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoice_GidLocationOptionId",
                table: "Invoice",
                column: "GidLocationOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoice_SalesOrderId",
                table: "Invoice",
                column: "SalesOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoice_SalesPersonId",
                table: "Invoice",
                column: "SalesPersonId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoice_ShippingAddressId",
                table: "Invoice",
                column: "ShippingAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoice_ShippingCarrierId",
                table: "Invoice",
                column: "ShippingCarrierId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoice_ShippingCarrierShippingMethodId",
                table: "Invoice",
                column: "ShippingCarrierShippingMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceAttachment_AttachmentId",
                table: "InvoiceAttachment",
                column: "AttachmentId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceCashReceipt_CashReceiptId",
                table: "InvoiceCashReceipt",
                column: "CashReceiptId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceChatMessage_ChatMessageId",
                table: "InvoiceChatMessage",
                column: "ChatMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceCredit_CreditId",
                table: "InvoiceCredit",
                column: "CreditId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceEventLogEntry_EventLogEntryId",
                table: "InvoiceEventLogEntry",
                column: "EventLogEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceLineItem_InvoiceId",
                table: "InvoiceLineItem",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceLineItem_ProductId",
                table: "InvoiceLineItem",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceLineItem_SalesOrderLineItemId",
                table: "InvoiceLineItem",
                column: "SalesOrderLineItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Lead_CompanyId",
                table: "Lead",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Lead_CompanyName",
                table: "Lead",
                column: "CompanyName");

            migrationBuilder.CreateIndex(
                name: "IX_Lead_ContactId",
                table: "Lead",
                column: "ContactId");

            migrationBuilder.CreateIndex(
                name: "IX_Lead_CreatedAt",
                table: "Lead",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Lead_Email",
                table: "Lead",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_Lead_LALeadNumber",
                table: "Lead",
                column: "LALeadNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Lead_Phone",
                table: "Lead",
                column: "Phone");

            migrationBuilder.CreateIndex(
                name: "IX_Lead_SalesPersonId",
                table: "Lead",
                column: "SalesPersonId");

            migrationBuilder.CreateIndex(
                name: "IX_Lead_UserId",
                table: "Lead",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_LeadAttachment_AttachmentId",
                table: "LeadAttachment",
                column: "AttachmentId");

            migrationBuilder.CreateIndex(
                name: "IX_LeadChatMessage_ChatMessageId",
                table: "LeadChatMessage",
                column: "ChatMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_LeadEventLogEntry_EventLogEntryId",
                table: "LeadEventLogEntry",
                column: "EventLogEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_LeadLineItem_LeadId",
                table: "LeadLineItem",
                column: "LeadId");

            migrationBuilder.CreateIndex(
                name: "IX_LeadLineItem_ProductId",
                table: "LeadLineItem",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_LeadLineItem_ProductName",
                table: "LeadLineItem",
                column: "ProductName");

            migrationBuilder.CreateIndex(
                name: "IX_LeadLineItemSource_SourceId",
                table: "LeadLineItemSource",
                column: "SourceId");

            migrationBuilder.CreateIndex(
                name: "IX_LeadNote_NoteId",
                table: "LeadNote",
                column: "NoteId");

            migrationBuilder.CreateIndex(
                name: "IX_LeadRoutingRule_UserId",
                table: "LeadRoutingRule",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_LeadRoutingRuleCountry_CountryId",
                table: "LeadRoutingRuleCountry",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_LeadRoutingRuleLeadWebsite_LeadWebsiteId",
                table: "LeadRoutingRuleLeadWebsite",
                column: "LeadWebsiteId");

            migrationBuilder.CreateIndex(
                name: "IX_LeadRoutingRuleLineItemServiceType_LineItemServiceTypeId",
                table: "LeadRoutingRuleLineItemServiceType",
                column: "LineItemServiceTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_LeadRoutingRuleProductType_ProductTypeId",
                table: "LeadRoutingRuleProductType",
                column: "ProductTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Note_CreatedById",
                table: "Note",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_OutgoingShipment_OutgoingShipmentShippingTermOptionId",
                table: "OutgoingShipment",
                column: "OutgoingShipmentShippingTermOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_OutgoingShipment_ShippingAddressId",
                table: "OutgoingShipment",
                column: "ShippingAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_OutgoingShipment_ShippingCarrierId",
                table: "OutgoingShipment",
                column: "ShippingCarrierId");

            migrationBuilder.CreateIndex(
                name: "IX_OutgoingShipment_ShippingCarrierShippingMethodId",
                table: "OutgoingShipment",
                column: "ShippingCarrierShippingMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_OutgoingShipmentBox_OutgoingShipmentId",
                table: "OutgoingShipmentBox",
                column: "OutgoingShipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_OutgoingShipmentBoxInventoryItem_InventoryItemId",
                table: "OutgoingShipmentBoxInventoryItem",
                column: "InventoryItemId");

            migrationBuilder.CreateIndex(
                name: "IX_PhoneNumber_Number",
                table: "PhoneNumber",
                column: "Number");

            migrationBuilder.CreateIndex(
                name: "IX_Product_CreatedAt",
                table: "Product",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Product_GidPartNumber",
                table: "Product",
                column: "GidPartNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Product_ManufacturerId",
                table: "Product",
                column: "ManufacturerId");

            migrationBuilder.CreateIndex(
                name: "IX_Product_PartNumber",
                table: "Product",
                column: "PartNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Product_ProductTypeId",
                table: "Product",
                column: "ProductTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductAttachment_AttachmentId",
                table: "ProductAttachment",
                column: "AttachmentId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductNote_NoteId",
                table: "ProductNote",
                column: "NoteId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrder_ContactId",
                table: "PurchaseOrder",
                column: "ContactId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrder_CreatedAt",
                table: "PurchaseOrder",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrder_CurrencyOptionId",
                table: "PurchaseOrder",
                column: "CurrencyOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrder_Email",
                table: "PurchaseOrder",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrder_GidLocationOptionId",
                table: "PurchaseOrder",
                column: "GidLocationOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrder_Phone",
                table: "PurchaseOrder",
                column: "Phone");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrder_PurchaseOrderPaymentMethodId",
                table: "PurchaseOrder",
                column: "PurchaseOrderPaymentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrder_ShippingAddressId",
                table: "PurchaseOrder",
                column: "ShippingAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrder_SupplierId",
                table: "PurchaseOrder",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderAttachment_AttachmentId",
                table: "PurchaseOrderAttachment",
                column: "AttachmentId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderChatMessage_ChatMessageId",
                table: "PurchaseOrderChatMessage",
                column: "ChatMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderEventLogEntry_EventLogEntryId",
                table: "PurchaseOrderEventLogEntry",
                column: "EventLogEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderIncomingShipment_IncomingShipmentId",
                table: "PurchaseOrderIncomingShipment",
                column: "IncomingShipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderLineItem_LineItemConditionTypeId",
                table: "PurchaseOrderLineItem",
                column: "LineItemConditionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderLineItem_ManufacturerId",
                table: "PurchaseOrderLineItem",
                column: "ManufacturerId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderLineItem_ProductId",
                table: "PurchaseOrderLineItem",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderLineItem_PurchaseOrderId",
                table: "PurchaseOrderLineItem",
                column: "PurchaseOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderNote_NoteId",
                table: "PurchaseOrderNote",
                column: "NoteId");

            migrationBuilder.CreateIndex(
                name: "IX_Quote_BillingAddressId",
                table: "Quote",
                column: "BillingAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Quote_CompanyId",
                table: "Quote",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Quote_ContactId",
                table: "Quote",
                column: "ContactId");

            migrationBuilder.CreateIndex(
                name: "IX_Quote_CreatedAt",
                table: "Quote",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Quote_CurrencyOptionId",
                table: "Quote",
                column: "CurrencyOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_Quote_Email",
                table: "Quote",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_Quote_GidLocationOptionId",
                table: "Quote",
                column: "GidLocationOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_Quote_LeadId",
                table: "Quote",
                column: "LeadId");

            migrationBuilder.CreateIndex(
                name: "IX_Quote_Phone",
                table: "Quote",
                column: "Phone");

            migrationBuilder.CreateIndex(
                name: "IX_Quote_QuoteStatusOptionId",
                table: "Quote",
                column: "QuoteStatusOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_Quote_SalesPersonId",
                table: "Quote",
                column: "SalesPersonId");

            migrationBuilder.CreateIndex(
                name: "IX_Quote_ShippingAddressId",
                table: "Quote",
                column: "ShippingAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Quote_ShippingMethodId",
                table: "Quote",
                column: "ShippingMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_QuoteAttachment_AttachmentId",
                table: "QuoteAttachment",
                column: "AttachmentId");

            migrationBuilder.CreateIndex(
                name: "IX_QuoteChatMessage_ChatMessageId",
                table: "QuoteChatMessage",
                column: "ChatMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_QuoteEventLogEntry_EventLogEntryId",
                table: "QuoteEventLogEntry",
                column: "EventLogEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_QuoteLineItem_LineItemConditionTypeId",
                table: "QuoteLineItem",
                column: "LineItemConditionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_QuoteLineItem_LineItemServiceTypeId",
                table: "QuoteLineItem",
                column: "LineItemServiceTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_QuoteLineItem_ManufacturerId",
                table: "QuoteLineItem",
                column: "ManufacturerId");

            migrationBuilder.CreateIndex(
                name: "IX_QuoteLineItem_OutgoingLineItemLeadTimeOptionId",
                table: "QuoteLineItem",
                column: "OutgoingLineItemLeadTimeOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_QuoteLineItem_OutgoingLineItemWarrantyOptionId",
                table: "QuoteLineItem",
                column: "OutgoingLineItemWarrantyOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_QuoteLineItem_ProductId",
                table: "QuoteLineItem",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_QuoteLineItem_QuoteId",
                table: "QuoteLineItem",
                column: "QuoteId");

            migrationBuilder.CreateIndex(
                name: "IX_QuoteLineItemSource_SourceId",
                table: "QuoteLineItemSource",
                column: "SourceId");

            migrationBuilder.CreateIndex(
                name: "IX_QuoteNote_NoteId",
                table: "QuoteNote",
                column: "NoteId");

            migrationBuilder.CreateIndex(
                name: "IX_Repair_RepairAuthorizationAttachmentId",
                table: "Repair",
                column: "RepairAuthorizationAttachmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Repair_SalesOrderId",
                table: "Repair",
                column: "SalesOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_RepairIncomingShipment_IncomingShipmentId",
                table: "RepairIncomingShipment",
                column: "IncomingShipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Rma_CompanyId",
                table: "Rma",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Rma_CreatedAt",
                table: "Rma",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Rma_CurrencyOptionId",
                table: "Rma",
                column: "CurrencyOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_Rma_GidLocationOptionId",
                table: "Rma",
                column: "GidLocationOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_Rma_RmaActionOptionId",
                table: "Rma",
                column: "RmaActionOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_Rma_RmaReasonOptionId",
                table: "Rma",
                column: "RmaReasonOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_Rma_SalesOrderId",
                table: "Rma",
                column: "SalesOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_RmaAttachment_AttachmentId",
                table: "RmaAttachment",
                column: "AttachmentId");

            migrationBuilder.CreateIndex(
                name: "IX_RmaChatMessage_ChatMessageId",
                table: "RmaChatMessage",
                column: "ChatMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_RmaEventLogEntry_EventLogEntryId",
                table: "RmaEventLogEntry",
                column: "EventLogEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_RmaIncomingShipment_IncomingShipmentId",
                table: "RmaIncomingShipment",
                column: "IncomingShipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_RmaLineItem_InventoryItemId",
                table: "RmaLineItem",
                column: "InventoryItemId");

            migrationBuilder.CreateIndex(
                name: "IX_RmaLineItem_RmaId",
                table: "RmaLineItem",
                column: "RmaId");

            migrationBuilder.CreateIndex(
                name: "IX_RmaLineItem_SalesOrderLineItemId",
                table: "RmaLineItem",
                column: "SalesOrderLineItemId");

            migrationBuilder.CreateIndex(
                name: "IX_RmaOutgoingShipment_OutgoingShipmentId",
                table: "RmaOutgoingShipment",
                column: "OutgoingShipmentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RmaOutgoingShipment_RmaId",
                table: "RmaOutgoingShipment",
                column: "RmaId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrder_BillingAddressId",
                table: "SalesOrder",
                column: "BillingAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrder_CompanyId",
                table: "SalesOrder",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrder_CompanyId1",
                table: "SalesOrder",
                column: "CompanyId1");

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrder_ContactId",
                table: "SalesOrder",
                column: "ContactId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrder_CreatedAt",
                table: "SalesOrder",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrder_CreditCardId",
                table: "SalesOrder",
                column: "CreditCardId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrder_CurrencyOptionId",
                table: "SalesOrder",
                column: "CurrencyOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrder_Email",
                table: "SalesOrder",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrder_GidLocationOptionId",
                table: "SalesOrder",
                column: "GidLocationOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrder_Phone",
                table: "SalesOrder",
                column: "Phone");

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrder_QuoteId",
                table: "SalesOrder",
                column: "QuoteId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrder_SalesOrderPaymentMethodId",
                table: "SalesOrder",
                column: "SalesOrderPaymentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrder_SalesPersonId",
                table: "SalesOrder",
                column: "SalesPersonId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrder_ShippingAddressId",
                table: "SalesOrder",
                column: "ShippingAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrder_ShippingCarrierId",
                table: "SalesOrder",
                column: "ShippingCarrierId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrder_ShippingCarrierShippingMethodId",
                table: "SalesOrder",
                column: "ShippingCarrierShippingMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrder_Total",
                table: "SalesOrder",
                column: "Total");

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrderAttachment_AttachmentId",
                table: "SalesOrderAttachment",
                column: "AttachmentId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrderChatMessage_ChatMessageId",
                table: "SalesOrderChatMessage",
                column: "ChatMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrderEventLogEntry_EventLogEntryId",
                table: "SalesOrderEventLogEntry",
                column: "EventLogEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrderInventoryItem_InventoryItemId",
                table: "SalesOrderInventoryItem",
                column: "InventoryItemId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrderInventoryItem_SalesOrderId",
                table: "SalesOrderInventoryItem",
                column: "SalesOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrderLineItem_CountryOfOriginId",
                table: "SalesOrderLineItem",
                column: "CountryOfOriginId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrderLineItem_LineItemConditionTypeId",
                table: "SalesOrderLineItem",
                column: "LineItemConditionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrderLineItem_ManufacturerId",
                table: "SalesOrderLineItem",
                column: "ManufacturerId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrderLineItem_OutgoingLineItemLeadTimeOptionId",
                table: "SalesOrderLineItem",
                column: "OutgoingLineItemLeadTimeOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrderLineItem_OutgoingLineItemWarrantyOptionId",
                table: "SalesOrderLineItem",
                column: "OutgoingLineItemWarrantyOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrderLineItem_ProductId",
                table: "SalesOrderLineItem",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrderLineItem_SalesOrderId",
                table: "SalesOrderLineItem",
                column: "SalesOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrderLineItemInventoryItem_SalesOrderLineItemId",
                table: "SalesOrderLineItemInventoryItem",
                column: "SalesOrderLineItemId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrderLineItemSource_PurchaseOrderId",
                table: "SalesOrderLineItemSource",
                column: "PurchaseOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrderLineItemSource_SourceId",
                table: "SalesOrderLineItemSource",
                column: "SourceId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrderNote_NoteId",
                table: "SalesOrderNote",
                column: "NoteId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrderOutgoingShipment_OutgoingShipmentId",
                table: "SalesOrderOutgoingShipment",
                column: "OutgoingShipmentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrderOutgoingShipment_SalesOrderId",
                table: "SalesOrderOutgoingShipment",
                column: "SalesOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrderPurchaseOrder_SalesOrderId",
                table: "SalesOrderPurchaseOrder",
                column: "SalesOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingCarrierShippingMethod_ShippingCarrierId",
                table: "ShippingCarrierShippingMethod",
                column: "ShippingCarrierId");

            migrationBuilder.CreateIndex(
                name: "IX_Source_ProductId",
                table: "Source",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Source_SupplierId",
                table: "Source",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_SourceAttachment_AttachmentId",
                table: "SourceAttachment",
                column: "AttachmentId");

            migrationBuilder.CreateIndex(
                name: "IX_SourceNote_NoteId",
                table: "SourceNote",
                column: "NoteId");

            migrationBuilder.CreateIndex(
                name: "IX_User_DefaultGidLocationOptionId",
                table: "User",
                column: "DefaultGidLocationOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_UserGroup_AzureId",
                table: "UserGroup",
                column: "AzureId");

            migrationBuilder.CreateIndex(
                name: "IX_UserUserGroup_UserGroupId",
                table: "UserUserGroup",
                column: "UserGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_View_UserGroupId",
                table: "View",
                column: "UserGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_ViewDisplayField_ViewId",
                table: "ViewDisplayField",
                column: "ViewId");

            migrationBuilder.CreateIndex(
                name: "IX_ViewFilter_ViewId",
                table: "ViewFilter",
                column: "ViewId");

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyAddress_Company_CompanyId",
                table: "CompanyAddress",
                column: "CompanyId",
                principalTable: "Company",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Address_Country_CountryId",
                table: "Address");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyAddress_Company_CompanyId",
                table: "CompanyAddress");

            migrationBuilder.DropTable(
                name: "AttachmentType");

            migrationBuilder.DropTable(
                name: "BillAttachment");

            migrationBuilder.DropTable(
                name: "BillCashDisbursement");

            migrationBuilder.DropTable(
                name: "BillChatMessage");

            migrationBuilder.DropTable(
                name: "BillLineItem");

            migrationBuilder.DropTable(
                name: "CashAccount");

            migrationBuilder.DropTable(
                name: "CashDisbursementReasonOption");

            migrationBuilder.DropTable(
                name: "CashReceiptType");

            migrationBuilder.DropTable(
                name: "ChatMessageAttachment");

            migrationBuilder.DropTable(
                name: "CompanyAddressType");

            migrationBuilder.DropTable(
                name: "CompanyAlias");

            migrationBuilder.DropTable(
                name: "CompanyAttachment");

            migrationBuilder.DropTable(
                name: "CompanyContact");

            migrationBuilder.DropTable(
                name: "CompanyContactRelationshipType");

            migrationBuilder.DropTable(
                name: "CompanyEmailAddress");

            migrationBuilder.DropTable(
                name: "CompanyEmailAddressType");

            migrationBuilder.DropTable(
                name: "CompanyEventLogEntry");

            migrationBuilder.DropTable(
                name: "CompanyNote");

            migrationBuilder.DropTable(
                name: "CompanyPhoneNumber");

            migrationBuilder.DropTable(
                name: "CompanyPhoneNumberType");

            migrationBuilder.DropTable(
                name: "CompanyPortal");

            migrationBuilder.DropTable(
                name: "Config");

            migrationBuilder.DropTable(
                name: "ContactAddress");

            migrationBuilder.DropTable(
                name: "ContactAddressType");

            migrationBuilder.DropTable(
                name: "ContactAttachment");

            migrationBuilder.DropTable(
                name: "ContactEmailAddress");

            migrationBuilder.DropTable(
                name: "ContactEmailAddressType");

            migrationBuilder.DropTable(
                name: "ContactEventLogEntry");

            migrationBuilder.DropTable(
                name: "ContactNote");

            migrationBuilder.DropTable(
                name: "ContactPhoneNumber");

            migrationBuilder.DropTable(
                name: "ContactPhoneNumberType");

            migrationBuilder.DropTable(
                name: "CpuStockVerifiedOption");

            migrationBuilder.DropTable(
                name: "CreditCardTransaction");

            migrationBuilder.DropTable(
                name: "CustomerType");

            migrationBuilder.DropTable(
                name: "EmailAddressType");

            migrationBuilder.DropTable(
                name: "EmailTemplate");

            migrationBuilder.DropTable(
                name: "EmailTemplateType");

            migrationBuilder.DropTable(
                name: "GLAccount");

            migrationBuilder.DropTable(
                name: "IncomingShipmentInventoryItem");

            migrationBuilder.DropTable(
                name: "InventoryItemAttachment");

            migrationBuilder.DropTable(
                name: "InventoryItemConditionOption");

            migrationBuilder.DropTable(
                name: "InventoryItemEventLogEntry");

            migrationBuilder.DropTable(
                name: "InventoryItemLocationOption");

            migrationBuilder.DropTable(
                name: "InventoryItemNote");

            migrationBuilder.DropTable(
                name: "InventoryItemRelatedInventoryItem");

            migrationBuilder.DropTable(
                name: "InventoryItemStatusOption");

            migrationBuilder.DropTable(
                name: "InventoryItemWorkLogItem");

            migrationBuilder.DropTable(
                name: "InvoiceAttachment");

            migrationBuilder.DropTable(
                name: "InvoiceCashReceipt");

            migrationBuilder.DropTable(
                name: "InvoiceChatMessage");

            migrationBuilder.DropTable(
                name: "InvoiceCredit");

            migrationBuilder.DropTable(
                name: "InvoiceEventLogEntry");

            migrationBuilder.DropTable(
                name: "InvoiceLineItem");

            migrationBuilder.DropTable(
                name: "LeadAttachment");

            migrationBuilder.DropTable(
                name: "LeadChatMessage");

            migrationBuilder.DropTable(
                name: "LeadEventLogEntry");

            migrationBuilder.DropTable(
                name: "LeadFilter");

            migrationBuilder.DropTable(
                name: "LeadLineItemSource");

            migrationBuilder.DropTable(
                name: "LeadNote");

            migrationBuilder.DropTable(
                name: "LeadOriginOption");

            migrationBuilder.DropTable(
                name: "LeadRoutingAction");

            migrationBuilder.DropTable(
                name: "LeadRoutingRuleCountry");

            migrationBuilder.DropTable(
                name: "LeadRoutingRuleLeadWebsite");

            migrationBuilder.DropTable(
                name: "LeadRoutingRuleLineItemServiceType");

            migrationBuilder.DropTable(
                name: "LeadRoutingRuleProductType");

            migrationBuilder.DropTable(
                name: "LeadStatusOption");

            migrationBuilder.DropTable(
                name: "ListingsContainer");

            migrationBuilder.DropTable(
                name: "OutgoingShipmentBoxDimensionOption");

            migrationBuilder.DropTable(
                name: "OutgoingShipmentBoxInventoryItem");

            migrationBuilder.DropTable(
                name: "Permission");

            migrationBuilder.DropTable(
                name: "PhoneNumberType");

            migrationBuilder.DropTable(
                name: "ProductAttachment");

            migrationBuilder.DropTable(
                name: "ProductCompositeItemOption");

            migrationBuilder.DropTable(
                name: "ProductConditionOption");

            migrationBuilder.DropTable(
                name: "ProductEndOfLifeOption");

            migrationBuilder.DropTable(
                name: "ProductNote");

            migrationBuilder.DropTable(
                name: "PurchaseOrderAttachment");

            migrationBuilder.DropTable(
                name: "PurchaseOrderChatMessage");

            migrationBuilder.DropTable(
                name: "PurchaseOrderEventLogEntry");

            migrationBuilder.DropTable(
                name: "PurchaseOrderFilter");

            migrationBuilder.DropTable(
                name: "PurchaseOrderIncomingShipment");

            migrationBuilder.DropTable(
                name: "PurchaseOrderNote");

            migrationBuilder.DropTable(
                name: "PurchaseOrderReasonOption");

            migrationBuilder.DropTable(
                name: "PurchaseOrderStatusOption");

            migrationBuilder.DropTable(
                name: "QuoteAddressType");

            migrationBuilder.DropTable(
                name: "QuoteAttachment");

            migrationBuilder.DropTable(
                name: "QuoteChatMessage");

            migrationBuilder.DropTable(
                name: "QuoteEventLogEntry");

            migrationBuilder.DropTable(
                name: "QuoteFilter");

            migrationBuilder.DropTable(
                name: "QuoteLineItemSource");

            migrationBuilder.DropTable(
                name: "QuoteNote");

            migrationBuilder.DropTable(
                name: "RepairIncomingShipment");

            migrationBuilder.DropTable(
                name: "RequiredDeliveryTimeOption");

            migrationBuilder.DropTable(
                name: "RmaAttachment");

            migrationBuilder.DropTable(
                name: "RmaChatMessage");

            migrationBuilder.DropTable(
                name: "RmaEventLogEntry");

            migrationBuilder.DropTable(
                name: "RmaFilter");

            migrationBuilder.DropTable(
                name: "RmaIncomingShipment");

            migrationBuilder.DropTable(
                name: "RmaLineItem");

            migrationBuilder.DropTable(
                name: "RmaOutgoingShipment");

            migrationBuilder.DropTable(
                name: "RmaStatusOption");

            migrationBuilder.DropTable(
                name: "SalesOrderAttachment");

            migrationBuilder.DropTable(
                name: "SalesOrderChatMessage");

            migrationBuilder.DropTable(
                name: "SalesOrderEventLogEntry");

            migrationBuilder.DropTable(
                name: "SalesOrderFilter");

            migrationBuilder.DropTable(
                name: "SalesOrderInventoryItem");

            migrationBuilder.DropTable(
                name: "SalesOrderLineItemInventoryItem");

            migrationBuilder.DropTable(
                name: "SalesOrderLineItemSource");

            migrationBuilder.DropTable(
                name: "SalesOrderNote");

            migrationBuilder.DropTable(
                name: "SalesOrderOutgoingShipment");

            migrationBuilder.DropTable(
                name: "SalesOrderPurchaseOrder");

            migrationBuilder.DropTable(
                name: "SalesOrderStatusOption");

            migrationBuilder.DropTable(
                name: "ShippingType");

            migrationBuilder.DropTable(
                name: "SourceAttachment");

            migrationBuilder.DropTable(
                name: "SourceLeadTimeOption");

            migrationBuilder.DropTable(
                name: "SourceNote");

            migrationBuilder.DropTable(
                name: "SourceWarrantyOption");

            migrationBuilder.DropTable(
                name: "State");

            migrationBuilder.DropTable(
                name: "UserUserGroup");

            migrationBuilder.DropTable(
                name: "ViewDisplayField");

            migrationBuilder.DropTable(
                name: "ViewFilter");

            migrationBuilder.DropTable(
                name: "ViewObjectNameOption");

            migrationBuilder.DropTable(
                name: "CashDisbursement");

            migrationBuilder.DropTable(
                name: "Bill");

            migrationBuilder.DropTable(
                name: "Portal");

            migrationBuilder.DropTable(
                name: "EmailAddress");

            migrationBuilder.DropTable(
                name: "PhoneNumber");

            migrationBuilder.DropTable(
                name: "CashReceipt");

            migrationBuilder.DropTable(
                name: "Credit");

            migrationBuilder.DropTable(
                name: "Invoice");

            migrationBuilder.DropTable(
                name: "LeadLineItem");

            migrationBuilder.DropTable(
                name: "LeadWebsite");

            migrationBuilder.DropTable(
                name: "LeadRoutingRule");

            migrationBuilder.DropTable(
                name: "OutgoingShipmentBox");

            migrationBuilder.DropTable(
                name: "QuoteLineItem");

            migrationBuilder.DropTable(
                name: "Repair");

            migrationBuilder.DropTable(
                name: "IncomingShipment");

            migrationBuilder.DropTable(
                name: "ChatMessage");

            migrationBuilder.DropTable(
                name: "EventLogEntry");

            migrationBuilder.DropTable(
                name: "InventoryItem");

            migrationBuilder.DropTable(
                name: "SalesOrderLineItem");

            migrationBuilder.DropTable(
                name: "Note");

            migrationBuilder.DropTable(
                name: "Source");

            migrationBuilder.DropTable(
                name: "View");

            migrationBuilder.DropTable(
                name: "PaymentAccount");

            migrationBuilder.DropTable(
                name: "BankAccount");

            migrationBuilder.DropTable(
                name: "CreditAccount");

            migrationBuilder.DropTable(
                name: "Rma");

            migrationBuilder.DropTable(
                name: "OutgoingShipment");

            migrationBuilder.DropTable(
                name: "LineItemServiceType");

            migrationBuilder.DropTable(
                name: "Attachment");

            migrationBuilder.DropTable(
                name: "PurchaseOrderLineItem");

            migrationBuilder.DropTable(
                name: "OutgoingLineItemLeadTimeOption");

            migrationBuilder.DropTable(
                name: "OutgoingLineItemWarrantyOption");

            migrationBuilder.DropTable(
                name: "UserGroup");

            migrationBuilder.DropTable(
                name: "RmaActionOption");

            migrationBuilder.DropTable(
                name: "RmaReasonOption");

            migrationBuilder.DropTable(
                name: "SalesOrder");

            migrationBuilder.DropTable(
                name: "OutgoingShipmentShippingTermOption");

            migrationBuilder.DropTable(
                name: "LineItemConditionType");

            migrationBuilder.DropTable(
                name: "Product");

            migrationBuilder.DropTable(
                name: "PurchaseOrder");

            migrationBuilder.DropTable(
                name: "CreditCard");

            migrationBuilder.DropTable(
                name: "Quote");

            migrationBuilder.DropTable(
                name: "SalesOrderPaymentMethod");

            migrationBuilder.DropTable(
                name: "ShippingCarrierShippingMethod");

            migrationBuilder.DropTable(
                name: "ProductType");

            migrationBuilder.DropTable(
                name: "PurchaseOrderPaymentMethod");

            migrationBuilder.DropTable(
                name: "CurrencyOption");

            migrationBuilder.DropTable(
                name: "Lead");

            migrationBuilder.DropTable(
                name: "QuoteStatusOption");

            migrationBuilder.DropTable(
                name: "ShippingMethod");

            migrationBuilder.DropTable(
                name: "ShippingCarrier");

            migrationBuilder.DropTable(
                name: "Contact");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "GidLocationOption");

            migrationBuilder.DropTable(
                name: "Country");

            migrationBuilder.DropTable(
                name: "Company");

            migrationBuilder.DropTable(
                name: "CompanyAddress");

            migrationBuilder.DropTable(
                name: "Address");
        }
    }
}
