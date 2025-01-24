using Microsoft.EntityFrameworkCore.Migrations;

namespace GidIndustrial.Gideon.WebApi.Migrations
{
    public partial class RenameTransactionErrorCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ErrorCode",
                table: "CreditCardTransaction",
                newName: "TransactionErrorCode");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TransactionErrorCode",
                table: "CreditCardTransaction",
                newName: "ErrorCode");
        }
    }
}
