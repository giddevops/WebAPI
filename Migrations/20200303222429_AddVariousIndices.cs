using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GidIndustrial.Gideon.WebApi.Migrations
{
    public partial class AddVariousIndices : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "ProductAttributeValue",
                nullable: true,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "OriginText",
                table: "Lead",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Emoji",
                table: "Lead",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Source_CreatedAt",
                table: "Source",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Source_CreatedById",
                table: "Source",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Quote_Total",
                table: "Quote",
                column: "Total");

            migrationBuilder.CreateIndex(
                name: "IX_Lead_Emoji",
                table: "Lead",
                column: "Emoji");

            migrationBuilder.CreateIndex(
                name: "IX_Lead_LeadStatusOptionId",
                table: "Lead",
                column: "LeadStatusOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_Lead_OriginText",
                table: "Lead",
                column: "OriginText");

            migrationBuilder.CreateIndex(
                name: "IX_Invoice_Balance",
                table: "Invoice",
                column: "Balance");

            migrationBuilder.CreateIndex(
                name: "IX_Invoice_CreatedById",
                table: "Invoice",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Bill_Balance",
                table: "Bill",
                column: "Balance");

            migrationBuilder.CreateIndex(
                name: "IX_Bill_CreatedAt",
                table: "Bill",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Bill_CreatedById",
                table: "Bill",
                column: "CreatedById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Source_CreatedAt",
                table: "Source");

            migrationBuilder.DropIndex(
                name: "IX_Source_CreatedById",
                table: "Source");

            migrationBuilder.DropIndex(
                name: "IX_Quote_Total",
                table: "Quote");

            migrationBuilder.DropIndex(
                name: "IX_Lead_Emoji",
                table: "Lead");

            migrationBuilder.DropIndex(
                name: "IX_Lead_LeadStatusOptionId",
                table: "Lead");

            migrationBuilder.DropIndex(
                name: "IX_Lead_OriginText",
                table: "Lead");

            migrationBuilder.DropIndex(
                name: "IX_Invoice_Balance",
                table: "Invoice");

            migrationBuilder.DropIndex(
                name: "IX_Invoice_CreatedById",
                table: "Invoice");

            migrationBuilder.DropIndex(
                name: "IX_Bill_Balance",
                table: "Bill");

            migrationBuilder.DropIndex(
                name: "IX_Bill_CreatedAt",
                table: "Bill");

            migrationBuilder.DropIndex(
                name: "IX_Bill_CreatedById",
                table: "Bill");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "ProductAttributeValue",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldNullable: true,
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.AlterColumn<string>(
                name: "OriginText",
                table: "Lead",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Emoji",
                table: "Lead",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
