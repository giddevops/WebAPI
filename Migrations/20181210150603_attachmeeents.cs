using Microsoft.EntityFrameworkCore.Migrations;

namespace GidIndustrial.Gideon.WebApi.Migrations
{
    public partial class attachmeeents : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IncomingShipmentAttachment",
                columns: table => new
                {
                    AttachmentId = table.Column<int>(nullable: false),
                    IncomingShipmentId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncomingShipmentAttachment", x => new { x.IncomingShipmentId, x.AttachmentId });
                    table.ForeignKey(
                        name: "FK_IncomingShipmentAttachment_Attachment_AttachmentId",
                        column: x => x.AttachmentId,
                        principalTable: "Attachment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IncomingShipmentAttachment_IncomingShipment_IncomingShipmentId",
                        column: x => x.IncomingShipmentId,
                        principalTable: "IncomingShipment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IncomingShipmentAttachment_AttachmentId",
                table: "IncomingShipmentAttachment",
                column: "AttachmentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IncomingShipmentAttachment");
        }
    }
}
