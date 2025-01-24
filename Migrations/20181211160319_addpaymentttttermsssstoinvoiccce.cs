using Microsoft.EntityFrameworkCore.Migrations;

namespace GidIndustrial.Gideon.WebApi.Migrations
{
    public partial class addpaymentttttermsssstoinvoiccce : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PaymentTermId",
                table: "Invoice",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Invoice_PaymentTermId",
                table: "Invoice",
                column: "PaymentTermId");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoice_PaymentTerm_PaymentTermId",
                table: "Invoice",
                column: "PaymentTermId",
                principalTable: "PaymentTerm",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoice_PaymentTerm_PaymentTermId",
                table: "Invoice");

            migrationBuilder.DropIndex(
                name: "IX_Invoice_PaymentTermId",
                table: "Invoice");

            migrationBuilder.DropColumn(
                name: "PaymentTermId",
                table: "Invoice");
        }
    }
}
