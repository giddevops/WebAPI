using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GidIndustrial.Gideon.WebApi.Migrations
{
    public partial class AddConditionalFormatters : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ViewConditionalFormatter",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ViewId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    FormatType = table.Column<string>(nullable: true),
                    FormatValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ViewConditionalFormatter", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ViewConditionalFormatter_View_ViewId",
                        column: x => x.ViewId,
                        principalTable: "View",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ViewConditionalFormatterCondition",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ViewConditionalFormatterId = table.Column<int>(nullable: false),
                    FieldName = table.Column<string>(nullable: true),
                    FieldType = table.Column<string>(nullable: true),
                    ViewFilterCondition = table.Column<string>(nullable: true),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ViewConditionalFormatterCondition", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ViewConditionalFormatterCondition_ViewConditionalFormatter_ViewConditionalFormatterId",
                        column: x => x.ViewConditionalFormatterId,
                        principalTable: "ViewConditionalFormatter",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuthenticationToken_CreatedAt",
                table: "AuthenticationToken",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ViewConditionalFormatter_ViewId",
                table: "ViewConditionalFormatter",
                column: "ViewId");

            migrationBuilder.CreateIndex(
                name: "IX_ViewConditionalFormatterCondition_ViewConditionalFormatterId",
                table: "ViewConditionalFormatterCondition",
                column: "ViewConditionalFormatterId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ViewConditionalFormatterCondition");

            migrationBuilder.DropTable(
                name: "ViewConditionalFormatter");

            migrationBuilder.DropIndex(
                name: "IX_AuthenticationToken_CreatedAt",
                table: "AuthenticationToken");
        }
    }
}
