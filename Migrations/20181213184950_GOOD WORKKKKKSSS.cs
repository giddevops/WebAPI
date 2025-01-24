using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GidIndustrial.Gideon.WebApi.Migrations
{
    public partial class GOODWORKKKKKSSS : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LeadRoutingRuleCompanyName",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    LeadRoutingRuleId = table.Column<int>(nullable: true),
                    CompanyName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeadRoutingRuleCompanyName", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LeadRoutingRuleCompanyName_LeadRoutingRule_LeadRoutingRuleId",
                        column: x => x.LeadRoutingRuleId,
                        principalTable: "LeadRoutingRule",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LeadRoutingRuleCompanyName_LeadRoutingRuleId",
                table: "LeadRoutingRuleCompanyName",
                column: "LeadRoutingRuleId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LeadRoutingRuleCompanyName");
        }
    }
}
