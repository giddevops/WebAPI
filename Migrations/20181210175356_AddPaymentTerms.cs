using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GidIndustrial.Gideon.WebApi.Migrations
{
    public partial class AddPaymentTerms : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PaymentTerm",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true),
                    DueDays = table.Column<int>(nullable: true),
                    DiscountDays = table.Column<int>(nullable: true),
                    DiscountPercent = table.Column<decimal>(nullable: true),
                    DueNextMonthDays = table.Column<int>(nullable: true),
                    DiscountDayOfMonth = table.Column<int>(nullable: true),
                    DayOfMonthDue = table.Column<int>(nullable: true),
                    QuickBooksSyncToken = table.Column<string>(nullable: true),
                    QuickBooksId = table.Column<string>(nullable: true),
                    Locked = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentTerm", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PaymentTerm");
        }
    }
}
