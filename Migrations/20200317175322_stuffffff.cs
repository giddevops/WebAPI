using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GidIndustrial.Gideon.WebApi.Migrations
{
    public partial class stuffffff : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Kittable",
                table: "ProductType",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ProductKitItem",
                columns: table => new
                {
                    CreatedAt = table.Column<DateTime>(nullable: true, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    ParentProductId = table.Column<int>(nullable: false),
                    ChildProductId = table.Column<int>(nullable: false),
                    Quantity = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductKitItem", x => new { x.ParentProductId, x.ChildProductId });
                    table.ForeignKey(
                        name: "FK_ProductKitItem_Product_ChildProductId",
                        column: x => x.ChildProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductKitItem_Product_ParentProductId",
                        column: x => x.ParentProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductKitItem_ChildProductId",
                table: "ProductKitItem",
                column: "ChildProductId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductKitItem");

            migrationBuilder.DropColumn(
                name: "Kittable",
                table: "ProductType");
        }
    }
}
