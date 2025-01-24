using Microsoft.EntityFrameworkCore.Migrations;

namespace GidIndustrial.Gideon.WebApi.Migrations
{
    public partial class AddBillingAddress : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BillingAddressId",
                table: "GidLocationOption",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_GidLocationOption_BillingAddressId",
                table: "GidLocationOption",
                column: "BillingAddressId");

            migrationBuilder.AddForeignKey(
                name: "FK_GidLocationOption_Address_BillingAddressId",
                table: "GidLocationOption",
                column: "BillingAddressId",
                principalTable: "Address",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GidLocationOption_Address_BillingAddressId",
                table: "GidLocationOption");

            migrationBuilder.DropIndex(
                name: "IX_GidLocationOption_BillingAddressId",
                table: "GidLocationOption");

            migrationBuilder.DropColumn(
                name: "BillingAddressId",
                table: "GidLocationOption");
        }
    }
}
