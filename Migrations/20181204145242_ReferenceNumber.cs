using Microsoft.EntityFrameworkCore.Migrations;

namespace GidIndustrial.Gideon.WebApi.Migrations
{
    public partial class ReferenceNumber : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerPurchaseOrderNumber",
                table: "Invoice");

            migrationBuilder.AddColumn<string>(
                name: "ReferenceNumber",
                table: "CashReceipt",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReferenceNumber",
                table: "CashReceipt");

            migrationBuilder.AddColumn<string>(
                name: "CustomerPurchaseOrderNumber",
                table: "Invoice",
                nullable: true);
        }
    }
}
