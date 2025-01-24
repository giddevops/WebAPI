using Microsoft.EntityFrameworkCore.Migrations;

namespace GidIndustrial.Gideon.WebApi.Migrations
{
    public partial class AddTransactionId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "TransactionId",
                table: "CreditCardTransaction",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Rma_RmaStatusOptionId",
                table: "Rma",
                column: "RmaStatusOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditCardTransaction_TransactionId",
                table: "CreditCardTransaction",
                column: "TransactionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Rma_RmaStatusOption_RmaStatusOptionId",
                table: "Rma",
                column: "RmaStatusOptionId",
                principalTable: "RmaStatusOption",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rma_RmaStatusOption_RmaStatusOptionId",
                table: "Rma");

            migrationBuilder.DropIndex(
                name: "IX_Rma_RmaStatusOptionId",
                table: "Rma");

            migrationBuilder.DropIndex(
                name: "IX_CreditCardTransaction_TransactionId",
                table: "CreditCardTransaction");

            migrationBuilder.AlterColumn<string>(
                name: "TransactionId",
                table: "CreditCardTransaction",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
