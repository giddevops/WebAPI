using Microsoft.EntityFrameworkCore.Migrations;

namespace GidIndustrial.Gideon.WebApi.Migrations
{
    public partial class Addedagoodbitofstuff : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "QuickBooksPurchaseId",
                table: "CashReceipt",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "QuickBooksPurchaseSyncToken",
                table: "CashReceipt",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QuickBooksPurchaseId",
                table: "CashReceipt");

            migrationBuilder.DropColumn(
                name: "QuickBooksPurchaseSyncToken",
                table: "CashReceipt");
        }
    }
}
