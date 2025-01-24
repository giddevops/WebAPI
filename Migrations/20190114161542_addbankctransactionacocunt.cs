using Microsoft.EntityFrameworkCore.Migrations;

namespace GidIndustrial.Gideon.WebApi.Migrations
{
    public partial class addbankctransactionacocunt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TransactionFeePaymentBankAccountId",
                table: "CashReceipt",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CashReceipt_TransactionFeePaymentBankAccountId",
                table: "CashReceipt",
                column: "TransactionFeePaymentBankAccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_CashReceipt_BankAccount_TransactionFeePaymentBankAccountId",
                table: "CashReceipt",
                column: "TransactionFeePaymentBankAccountId",
                principalTable: "BankAccount",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CashReceipt_BankAccount_TransactionFeePaymentBankAccountId",
                table: "CashReceipt");

            migrationBuilder.DropIndex(
                name: "IX_CashReceipt_TransactionFeePaymentBankAccountId",
                table: "CashReceipt");

            migrationBuilder.DropColumn(
                name: "TransactionFeePaymentBankAccountId",
                table: "CashReceipt");
        }
    }
}
