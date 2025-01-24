using Microsoft.EntityFrameworkCore.Migrations;

namespace GidIndustrial.Gideon.WebApi.Migrations
{
    public partial class IndexTrackingNumber : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "TrackingNumber",
                table: "IncomingShipment",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_IncomingShipment_TrackingNumber",
                table: "IncomingShipment",
                column: "TrackingNumber");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_IncomingShipment_TrackingNumber",
                table: "IncomingShipment");

            migrationBuilder.AlterColumn<string>(
                name: "TrackingNumber",
                table: "IncomingShipment",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
