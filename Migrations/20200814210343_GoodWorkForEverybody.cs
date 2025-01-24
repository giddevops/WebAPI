using Microsoft.EntityFrameworkCore.Migrations;

namespace GidIndustrial.Gideon.WebApi.Migrations
{
    public partial class GoodWorkForEverybody : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "AverageCost",
                table: "SalesOrderLineItem",
                type: "decimal(18, 2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 2)");

            migrationBuilder.AddColumn<decimal>(
                name: "ProjectedProfit",
                table: "SalesOrderLineItem",
                type: "decimal(18, 2)",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "ShippedProfit",
                table: "SalesOrder",
                type: "decimal(18, 2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 2)");

            migrationBuilder.AddColumn<decimal>(
                name: "ProjectedProfits",
                table: "SalesOrder",
                type: "decimal(18, 2)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProjectedProfit",
                table: "SalesOrderLineItem");

            migrationBuilder.DropColumn(
                name: "ProjectedProfits",
                table: "SalesOrder");

            migrationBuilder.AlterColumn<decimal>(
                name: "AverageCost",
                table: "SalesOrderLineItem",
                type: "decimal(18, 2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "ShippedProfit",
                table: "SalesOrder",
                type: "decimal(18, 2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 2)",
                oldNullable: true);
        }
    }
}
