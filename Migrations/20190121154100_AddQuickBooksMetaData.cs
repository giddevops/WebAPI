using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GidIndustrial.Gideon.WebApi.Migrations
{
    public partial class AddQuickBooksMetaData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "QuickBooksPayment",
                columns: table => new
                {
                    RecordId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UnappliedAmt = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    ProcessPayment = table.Column<bool>(nullable: false),
                    domain = table.Column<string>(nullable: true),
                    sparse = table.Column<bool>(nullable: false),
                    Id = table.Column<string>(nullable: false),
                    SyncToken = table.Column<string>(nullable: true),
                    TotalAmt = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    TxnDate = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuickBooksPayment", x => x.RecordId);
                    table.UniqueConstraint("AK_QuickBooksPayment_Id", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QuickBooksPayment");
        }
    }
}
