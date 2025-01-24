using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GidIndustrial.Gideon.WebApi.Migrations
{
    public partial class AddContactLogItemsStuff : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ContactLogItem",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    CreatedById = table.Column<int>(nullable: true),
                    ContactDate = table.Column<DateTime>(nullable: true),
                    FollowUpDate = table.Column<DateTime>(nullable: true),
                    Notes = table.Column<string>(nullable: true),
                    UserId = table.Column<int>(nullable: true),
                    ContactName = table.Column<string>(nullable: true),
                    ContactMethodId = table.Column<int>(nullable: true),
                    ContactReasonId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactLogItem", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ContactMethodOption",
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
                    table.PrimaryKey("PK_ContactMethodOption", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ContactReasonOption",
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
                    table.PrimaryKey("PK_ContactReasonOption", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LeadContactLogItem",
                columns: table => new
                {
                    LeadId = table.Column<int>(nullable: false),
                    ContactLogItemId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeadContactLogItem", x => new { x.ContactLogItemId, x.LeadId });
                    table.ForeignKey(
                        name: "FK_LeadContactLogItem_ContactLogItem_ContactLogItemId",
                        column: x => x.ContactLogItemId,
                        principalTable: "ContactLogItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LeadContactLogItem_Lead_LeadId",
                        column: x => x.LeadId,
                        principalTable: "Lead",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuoteContactLogItem",
                columns: table => new
                {
                    QuoteId = table.Column<int>(nullable: false),
                    ContactLogItemId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuoteContactLogItem", x => new { x.ContactLogItemId, x.QuoteId });
                    table.ForeignKey(
                        name: "FK_QuoteContactLogItem_ContactLogItem_ContactLogItemId",
                        column: x => x.ContactLogItemId,
                        principalTable: "ContactLogItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuoteContactLogItem_Quote_QuoteId",
                        column: x => x.QuoteId,
                        principalTable: "Quote",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LeadContactLogItem_LeadId",
                table: "LeadContactLogItem",
                column: "LeadId");

            migrationBuilder.CreateIndex(
                name: "IX_QuoteContactLogItem_QuoteId",
                table: "QuoteContactLogItem",
                column: "QuoteId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContactMethodOption");

            migrationBuilder.DropTable(
                name: "ContactReasonOption");

            migrationBuilder.DropTable(
                name: "LeadContactLogItem");

            migrationBuilder.DropTable(
                name: "QuoteContactLogItem");

            migrationBuilder.DropTable(
                name: "ContactLogItem");
        }
    }
}
