using Microsoft.EntityFrameworkCore.Migrations;

namespace GidIndustrial.Gideon.WebApi.Migrations
{
    public partial class AddPaymentTerm : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SalesOrderPaymentMethodId",
                table: "Quote",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Quote_SalesOrderPaymentMethodId",
                table: "Quote",
                column: "SalesOrderPaymentMethodId");

            migrationBuilder.AddForeignKey(
                name: "FK_Quote_SalesOrderPaymentMethod_SalesOrderPaymentMethodId",
                table: "Quote",
                column: "SalesOrderPaymentMethodId",
                principalTable: "SalesOrderPaymentMethod",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Quote_SalesOrderPaymentMethod_SalesOrderPaymentMethodId",
                table: "Quote");

            migrationBuilder.DropIndex(
                name: "IX_Quote_SalesOrderPaymentMethodId",
                table: "Quote");

            migrationBuilder.DropColumn(
                name: "SalesOrderPaymentMethodId",
                table: "Quote");
        }
    }
}
