using Microsoft.EntityFrameworkCore.Migrations;

namespace GidIndustrial.Gideon.WebApi.Migrations
{
    public partial class AddContactToSource : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ContactId",
                table: "Source",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Source_ContactId",
                table: "Source",
                column: "ContactId");

            migrationBuilder.AddForeignKey(
                name: "FK_Source_Contact_ContactId",
                table: "Source",
                column: "ContactId",
                principalTable: "Contact",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Source_Contact_ContactId",
                table: "Source");

            migrationBuilder.DropIndex(
                name: "IX_Source_ContactId",
                table: "Source");

            migrationBuilder.DropColumn(
                name: "ContactId",
                table: "Source");
        }
    }
}
