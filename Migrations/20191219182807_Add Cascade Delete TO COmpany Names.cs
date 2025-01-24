using Microsoft.EntityFrameworkCore.Migrations;

namespace GidIndustrial.Gideon.WebApi.Migrations
{
    public partial class AddCascadeDeleteTOCOmpanyNames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LeadRoutingRuleCompanyName_LeadRoutingRule_LeadRoutingRuleId",
                table: "LeadRoutingRuleCompanyName");

            migrationBuilder.AddForeignKey(
                name: "FK_LeadRoutingRuleCompanyName_LeadRoutingRule_LeadRoutingRuleId",
                table: "LeadRoutingRuleCompanyName",
                column: "LeadRoutingRuleId",
                principalTable: "LeadRoutingRule",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LeadRoutingRuleCompanyName_LeadRoutingRule_LeadRoutingRuleId",
                table: "LeadRoutingRuleCompanyName");

            migrationBuilder.AddForeignKey(
                name: "FK_LeadRoutingRuleCompanyName_LeadRoutingRule_LeadRoutingRuleId",
                table: "LeadRoutingRuleCompanyName",
                column: "LeadRoutingRuleId",
                principalTable: "LeadRoutingRule",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
