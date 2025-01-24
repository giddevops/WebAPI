using Microsoft.EntityFrameworkCore.Migrations;

namespace GidIndustrial.Gideon.WebApi.Migrations
{
    public partial class AddProjectedProfit4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Source_CurrencyOptionId",
                table: "Source",
                column: "CurrencyOptionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Source_CurrencyOption_CurrencyOptionId",
                table: "Source",
                column: "CurrencyOptionId",
                principalTable: "CurrencyOption",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Source_CurrencyOption_CurrencyOptionId",
                table: "Source");

            migrationBuilder.DropIndex(
                name: "IX_Source_CurrencyOptionId",
                table: "Source");
        }
    }
}
