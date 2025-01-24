using Microsoft.EntityFrameworkCore.Migrations;

namespace GidIndustrial.Gideon.WebApi.Migrations
{
    public partial class addqubpaymentmethods : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Active",
                table: "SalesOrderPaymentMethod",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "QuickBooksId",
                table: "SalesOrderPaymentMethod",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "QuickBooksSyncToken",
                table: "SalesOrderPaymentMethod",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "SalesOrderPaymentMethod",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Active",
                table: "SalesOrderPaymentMethod");

            migrationBuilder.DropColumn(
                name: "QuickBooksId",
                table: "SalesOrderPaymentMethod");

            migrationBuilder.DropColumn(
                name: "QuickBooksSyncToken",
                table: "SalesOrderPaymentMethod");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "SalesOrderPaymentMethod");
        }
    }
}
