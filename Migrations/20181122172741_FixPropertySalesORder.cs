using Microsoft.EntityFrameworkCore.Migrations;

namespace GidIndustrial.Gideon.WebApi.Migrations
{
    public partial class FixPropertySalesORder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SalesOrder_Company_CompanyId1",
                table: "SalesOrder");

            migrationBuilder.DropIndex(
                name: "IX_SalesOrder_CompanyId1",
                table: "SalesOrder");

            migrationBuilder.DropColumn(
                name: "CompanyId1",
                table: "SalesOrder");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CompanyId1",
                table: "SalesOrder",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrder_CompanyId1",
                table: "SalesOrder",
                column: "CompanyId1");

            migrationBuilder.AddForeignKey(
                name: "FK_SalesOrder_Company_CompanyId1",
                table: "SalesOrder",
                column: "CompanyId1",
                principalTable: "Company",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
