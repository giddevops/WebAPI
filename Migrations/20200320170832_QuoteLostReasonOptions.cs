using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GidIndustrial.Gideon.WebApi.Migrations
{
    public partial class QuoteLostReasonOptions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "OpportunityLost",
                table: "Quote",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "QuoteLostReasonOptionId",
                table: "Quote",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "QuoteLostReasonOption",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    Value = table.Column<string>(nullable: true),
                    Locked = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuoteLostReasonOption", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QuoteLostReasonOption");

            migrationBuilder.DropColumn(
                name: "OpportunityLost",
                table: "Quote");

            migrationBuilder.DropColumn(
                name: "QuoteLostReasonOptionId",
                table: "Quote");
        }
    }
}
