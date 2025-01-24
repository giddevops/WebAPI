using Microsoft.EntityFrameworkCore.Migrations;

namespace GidIndustrial.Gideon.WebApi.Migrations
{
    public partial class addfieldtype : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FieldType",
                table: "ViewFilter",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FieldType",
                table: "ViewDisplayField",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FieldType",
                table: "ViewFilter");

            migrationBuilder.DropColumn(
                name: "FieldType",
                table: "ViewDisplayField");
        }
    }
}
