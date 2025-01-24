using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GidIndustrial.Gideon.WebApi.Migrations
{
    public partial class addscannerLabelVariableValue : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ScannerLabelVariableValue",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    ScannerLabelTypeVariableId = table.Column<int>(nullable: false),
                    ScannerLabelId = table.Column<int>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScannerLabelVariableValue", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScannerLabelVariableValue_ScannerLabel_ScannerLabelId",
                        column: x => x.ScannerLabelId,
                        principalTable: "ScannerLabel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ScannerLabelVariableValue_ScannerLabelId",
                table: "ScannerLabelVariableValue",
                column: "ScannerLabelId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScannerLabelVariableValue");
        }
    }
}
