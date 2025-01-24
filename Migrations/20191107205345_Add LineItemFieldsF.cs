using Microsoft.EntityFrameworkCore.Migrations;

namespace GidIndustrial.Gideon.WebApi.Migrations
{
    public partial class AddLineItemFieldsF : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CpuProductId",
                table: "SalesOrderLineItem",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FanHeatSinkProductId",
                table: "SalesOrderLineItem",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MemoryProductId",
                table: "SalesOrderLineItem",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CpuProductId",
                table: "SalesOrderLineItem");

            migrationBuilder.DropColumn(
                name: "FanHeatSinkProductId",
                table: "SalesOrderLineItem");

            migrationBuilder.DropColumn(
                name: "MemoryProductId",
                table: "SalesOrderLineItem");
        }
    }
}
