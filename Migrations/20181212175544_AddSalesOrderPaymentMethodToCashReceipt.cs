using Microsoft.EntityFrameworkCore.Migrations;

namespace GidIndustrial.Gideon.WebApi.Migrations
{
    public partial class AddSalesOrderPaymentMethodToCashReceipt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SalesOrderPaymentMethodId",
                table: "CashReceipt",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CashReceipt_SalesOrderPaymentMethodId",
                table: "CashReceipt",
                column: "SalesOrderPaymentMethodId");

            migrationBuilder.AddForeignKey(
                name: "FK_CashReceipt_SalesOrderPaymentMethod_SalesOrderPaymentMethodId",
                table: "CashReceipt",
                column: "SalesOrderPaymentMethodId",
                principalTable: "SalesOrderPaymentMethod",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CashReceipt_SalesOrderPaymentMethod_SalesOrderPaymentMethodId",
                table: "CashReceipt");

            migrationBuilder.DropIndex(
                name: "IX_CashReceipt_SalesOrderPaymentMethodId",
                table: "CashReceipt");

            migrationBuilder.DropColumn(
                name: "SalesOrderPaymentMethodId",
                table: "CashReceipt");
        }
    }
}
