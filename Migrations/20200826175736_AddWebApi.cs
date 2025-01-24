using Microsoft.EntityFrameworkCore.Migrations;

namespace GidIndustrial.Gideon.WebApi.Migrations
{
    public partial class AddWebApi : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CreditCardFee",
                table: "SalesOrder",
                type: "decimal(18, 2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CreditCardFee",
                table: "Quote",
                type: "decimal(18, 2)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrder_ProjectedProfit",
                table: "SalesOrder",
                column: "ProjectedProfit");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SalesOrder_ProjectedProfit",
                table: "SalesOrder");

            migrationBuilder.DropColumn(
                name: "CreditCardFee",
                table: "SalesOrder");

            migrationBuilder.DropColumn(
                name: "CreditCardFee",
                table: "Quote");
        }
    }
}
