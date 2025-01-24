using Microsoft.EntityFrameworkCore.Migrations;

namespace GidIndustrial.Gideon.WebApi.Migrations
{
    public partial class ChatMessageUserMention2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ChatMessageUserMention_ChatMessageId",
                table: "ChatMessageUserMention",
                column: "ChatMessageId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMessageUserMention_ChatMessage_ChatMessageId",
                table: "ChatMessageUserMention",
                column: "ChatMessageId",
                principalTable: "ChatMessage",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMessageUserMention_User_UserId",
                table: "ChatMessageUserMention",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatMessageUserMention_ChatMessage_ChatMessageId",
                table: "ChatMessageUserMention");

            migrationBuilder.DropForeignKey(
                name: "FK_ChatMessageUserMention_User_UserId",
                table: "ChatMessageUserMention");

            migrationBuilder.DropIndex(
                name: "IX_ChatMessageUserMention_ChatMessageId",
                table: "ChatMessageUserMention");
        }
    }
}
