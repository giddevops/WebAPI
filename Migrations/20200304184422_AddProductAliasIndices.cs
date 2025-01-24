using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GidIndustrial.Gideon.WebApi.Migrations
{
    public partial class AddProductAliasIndices : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PartNumber",
                table: "ProductAlias",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ManufacturerName",
                table: "ProductAlias",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "ProductAlias",
                nullable: true,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductAlias_CreatedAt",
                table: "ProductAlias",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ProductAlias_ManufacturerName",
                table: "ProductAlias",
                column: "ManufacturerName");

            migrationBuilder.CreateIndex(
                name: "IX_ProductAlias_PartNumber",
                table: "ProductAlias",
                column: "PartNumber");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProductAlias_CreatedAt",
                table: "ProductAlias");

            migrationBuilder.DropIndex(
                name: "IX_ProductAlias_ManufacturerName",
                table: "ProductAlias");

            migrationBuilder.DropIndex(
                name: "IX_ProductAlias_PartNumber",
                table: "ProductAlias");

            migrationBuilder.AlterColumn<string>(
                name: "PartNumber",
                table: "ProductAlias",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ManufacturerName",
                table: "ProductAlias",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "ProductAlias",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldNullable: true,
                oldDefaultValueSql: "GETUTCDATE()");
        }
    }
}
