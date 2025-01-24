using Microsoft.EntityFrameworkCore.Migrations;

namespace GidIndustrial.Gideon.WebApi.Migrations
{
    public partial class DoMoreStuffN : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CustomerPurchaseOrderNumber",
                table: "SalesOrder",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrder_CustomerPurchaseOrderNumber",
                table: "SalesOrder",
                column: "CustomerPurchaseOrderNumber");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SalesOrder_CustomerPurchaseOrderNumber",
                table: "SalesOrder");

            migrationBuilder.AlterColumn<string>(
                name: "CustomerPurchaseOrderNumber",
                table: "SalesOrder",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
