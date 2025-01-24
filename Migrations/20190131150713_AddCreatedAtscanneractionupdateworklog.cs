using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GidIndustrial.Gideon.WebApi.Migrations
{
    public partial class AddCreatedAtscanneractionupdateworklog : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "CreatedById",
                table: "ScannerActionUpdateWorkLog",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "ScannerActionUpdateWorkLog",
                nullable: true,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTime));

            migrationBuilder.AddColumn<string>(
                name: "PaymentRefNum",
                table: "QuickBooksPayment",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentRefNum",
                table: "QuickBooksPayment");

            migrationBuilder.AlterColumn<int>(
                name: "CreatedById",
                table: "ScannerActionUpdateWorkLog",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "ScannerActionUpdateWorkLog",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true,
                oldDefaultValueSql: "GETUTCDATE()");
        }
    }
}
