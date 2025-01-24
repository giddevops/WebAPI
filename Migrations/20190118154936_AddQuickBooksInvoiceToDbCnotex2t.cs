using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GidIndustrial.Gideon.WebApi.Migrations
{
    public partial class AddQuickBooksInvoiceToDbCnotex2t : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_QuickBooksInvoice",
                table: "QuickBooksInvoice");

            migrationBuilder.DropColumn(
                name: "InvoiceId",
                table: "QuickBooksInvoice");

            migrationBuilder.AddColumn<int>(
                name: "RecordId",
                table: "QuickBooksInvoice",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_QuickBooksInvoice_Id",
                table: "QuickBooksInvoice",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_QuickBooksInvoice",
                table: "QuickBooksInvoice",
                column: "RecordId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_QuickBooksInvoice_Id",
                table: "QuickBooksInvoice");

            migrationBuilder.DropPrimaryKey(
                name: "PK_QuickBooksInvoice",
                table: "QuickBooksInvoice");

            migrationBuilder.DropColumn(
                name: "RecordId",
                table: "QuickBooksInvoice");

            migrationBuilder.AddColumn<int>(
                name: "InvoiceId",
                table: "QuickBooksInvoice",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_QuickBooksInvoice",
                table: "QuickBooksInvoice",
                column: "Id");
        }
    }
}
