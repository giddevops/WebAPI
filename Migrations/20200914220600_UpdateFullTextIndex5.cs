using Microsoft.EntityFrameworkCore.Migrations;

namespace GidIndustrial.Gideon.WebApi.Migrations
{
    public partial class UpdateFullTextIndex5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.Sql(sql: @"DROP FULLTEXT INDEX ON Product ", suppressTransaction: true);
            migrationBuilder.Sql(
                sql: "CREATE FULLTEXT INDEX ON Product (GidPartNumberNoSpecialChars, PartNumberNoSpecialChars, AliasesCache, Description, ShortDescription) KEY INDEX PK_Product;",
                suppressTransaction: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
