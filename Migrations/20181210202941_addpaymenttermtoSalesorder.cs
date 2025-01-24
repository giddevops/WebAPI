using Microsoft.EntityFrameworkCore.Migrations;

namespace GidIndustrial.Gideon.WebApi.Migrations
{
    public partial class addpaymenttermtoSalesorder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PaymentTermId",
                table: "SalesOrder",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrder_PaymentTermId",
                table: "SalesOrder",
                column: "PaymentTermId");

            migrationBuilder.AddForeignKey(
                name: "FK_SalesOrder_PaymentTerm_PaymentTermId",
                table: "SalesOrder",
                column: "PaymentTermId",
                principalTable: "PaymentTerm",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SalesOrder_PaymentTerm_PaymentTermId",
                table: "SalesOrder");

            migrationBuilder.DropIndex(
                name: "IX_SalesOrder_PaymentTermId",
                table: "SalesOrder");

            migrationBuilder.DropColumn(
                name: "PaymentTermId",
                table: "SalesOrder");
        }
    }
}
