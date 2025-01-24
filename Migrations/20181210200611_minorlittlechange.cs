using Microsoft.EntityFrameworkCore.Migrations;

namespace GidIndustrial.Gideon.WebApi.Migrations
{
    public partial class minorlittlechange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PaymentTermId",
                table: "Quote",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DueDays",
                table: "PaymentTerm",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Quote_PaymentTermId",
                table: "Quote",
                column: "PaymentTermId");

            migrationBuilder.AddForeignKey(
                name: "FK_Quote_PaymentTerm_PaymentTermId",
                table: "Quote",
                column: "PaymentTermId",
                principalTable: "PaymentTerm",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Quote_PaymentTerm_PaymentTermId",
                table: "Quote");

            migrationBuilder.DropIndex(
                name: "IX_Quote_PaymentTermId",
                table: "Quote");

            migrationBuilder.DropColumn(
                name: "PaymentTermId",
                table: "Quote");

            migrationBuilder.AlterColumn<int>(
                name: "DueDays",
                table: "PaymentTerm",
                nullable: true,
                oldClrType: typeof(int));
        }
    }
}
