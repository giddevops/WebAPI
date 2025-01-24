using Microsoft.EntityFrameworkCore.Migrations;

namespace GidIndustrial.Gideon.WebApi.Migrations
{
    public partial class AddQuickBooksInvoiceToDbCnotext : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "QuickBooksInvoice",
                columns: table => new
                {
                    InvoiceId = table.Column<int>(nullable: true),
                    Deposit = table.Column<int>(nullable: false),
                    domain = table.Column<string>(nullable: true),
                    sparse = table.Column<bool>(nullable: false),
                    Id = table.Column<string>(nullable: false),
                    SyncToken = table.Column<string>(nullable: true),
                    DocNumber = table.Column<string>(nullable: true),
                    TxnDate = table.Column<string>(nullable: true),
                    DueDate = table.Column<string>(nullable: true),
                    ShipDate = table.Column<string>(nullable: true),
                    TotalAmt = table.Column<double>(nullable: false),
                    ApplyTaxAfterDiscount = table.Column<bool>(nullable: false),
                    PrintStatus = table.Column<string>(nullable: true),
                    EmailStatus = table.Column<string>(nullable: true),
                    Balance = table.Column<double>(nullable: false),
                    TrackingNum = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuickBooksInvoice", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QuickBooksInvoice");
        }
    }
}
