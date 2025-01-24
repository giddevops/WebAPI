using Microsoft.EntityFrameworkCore.Migrations;

namespace GidIndustrial.Gideon.WebApi.Migrations
{
    public partial class RemoveParentAndChildPartTypesFromScannerAction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScannerActionRelatePieceParts_ScannerLabelType_ChildPartScannerLabelTypeId",
                table: "ScannerActionRelatePieceParts");

            migrationBuilder.DropForeignKey(
                name: "FK_ScannerActionRelatePieceParts_ScannerLabelType_ParentPartScannerLabelTypeId",
                table: "ScannerActionRelatePieceParts");

            migrationBuilder.DropIndex(
                name: "IX_ScannerActionRelatePieceParts_ChildPartScannerLabelTypeId",
                table: "ScannerActionRelatePieceParts");

            migrationBuilder.DropIndex(
                name: "IX_ScannerActionRelatePieceParts_ParentPartScannerLabelTypeId",
                table: "ScannerActionRelatePieceParts");

            migrationBuilder.DropColumn(
                name: "ChildPartScannerLabelTypeId",
                table: "ScannerActionRelatePieceParts");

            migrationBuilder.DropColumn(
                name: "ParentPartScannerLabelTypeId",
                table: "ScannerActionRelatePieceParts");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ChildPartScannerLabelTypeId",
                table: "ScannerActionRelatePieceParts",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ParentPartScannerLabelTypeId",
                table: "ScannerActionRelatePieceParts",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScannerActionRelatePieceParts_ChildPartScannerLabelTypeId",
                table: "ScannerActionRelatePieceParts",
                column: "ChildPartScannerLabelTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ScannerActionRelatePieceParts_ParentPartScannerLabelTypeId",
                table: "ScannerActionRelatePieceParts",
                column: "ParentPartScannerLabelTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ScannerActionRelatePieceParts_ScannerLabelType_ChildPartScannerLabelTypeId",
                table: "ScannerActionRelatePieceParts",
                column: "ChildPartScannerLabelTypeId",
                principalTable: "ScannerLabelType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ScannerActionRelatePieceParts_ScannerLabelType_ParentPartScannerLabelTypeId",
                table: "ScannerActionRelatePieceParts",
                column: "ParentPartScannerLabelTypeId",
                principalTable: "ScannerLabelType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
