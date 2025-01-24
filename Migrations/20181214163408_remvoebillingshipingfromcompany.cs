using Microsoft.EntityFrameworkCore.Migrations;

namespace GidIndustrial.Gideon.WebApi.Migrations
{
    public partial class remvoebillingshipingfromcompany : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Company_CompanyAddress_BillingAddressCompanyId_BillingAddressAddressId",
                table: "Company");

            migrationBuilder.DropForeignKey(
                name: "FK_Company_CompanyAddress_ShippingAddressCompanyId_ShippingAddressAddressId",
                table: "Company");

            migrationBuilder.DropIndex(
                name: "IX_Company_BillingAddressCompanyId_BillingAddressAddressId",
                table: "Company");

            migrationBuilder.DropIndex(
                name: "IX_Company_ShippingAddressCompanyId_ShippingAddressAddressId",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "BillingAddressAddressId",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "BillingAddressCompanyId",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "ShippingAddressAddressId",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "ShippingAddressCompanyId",
                table: "Company");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BillingAddressAddressId",
                table: "Company",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BillingAddressCompanyId",
                table: "Company",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ShippingAddressAddressId",
                table: "Company",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ShippingAddressCompanyId",
                table: "Company",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Company_BillingAddressCompanyId_BillingAddressAddressId",
                table: "Company",
                columns: new[] { "BillingAddressCompanyId", "BillingAddressAddressId" });

            migrationBuilder.CreateIndex(
                name: "IX_Company_ShippingAddressCompanyId_ShippingAddressAddressId",
                table: "Company",
                columns: new[] { "ShippingAddressCompanyId", "ShippingAddressAddressId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Company_CompanyAddress_BillingAddressCompanyId_BillingAddressAddressId",
                table: "Company",
                columns: new[] { "BillingAddressCompanyId", "BillingAddressAddressId" },
                principalTable: "CompanyAddress",
                principalColumns: new[] { "CompanyId", "AddressId" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Company_CompanyAddress_ShippingAddressCompanyId_ShippingAddressAddressId",
                table: "Company",
                columns: new[] { "ShippingAddressCompanyId", "ShippingAddressAddressId" },
                principalTable: "CompanyAddress",
                principalColumns: new[] { "CompanyId", "AddressId" },
                onDelete: ReferentialAction.Restrict);
        }
    }
}
