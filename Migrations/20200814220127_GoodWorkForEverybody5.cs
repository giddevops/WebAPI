using Microsoft.EntityFrameworkCore.Migrations;

namespace GidIndustrial.Gideon.WebApi.Migrations
{
    public partial class GoodWorkForEverybody5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProjectedProfits",
                table: "SalesOrder",
                newName: "ProjectedProfit");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProjectedProfit",
                table: "SalesOrder",
                newName: "ProjectedProfits");
        }
    }
}
