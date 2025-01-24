using Microsoft.EntityFrameworkCore.Migrations;

namespace GidIndustrial.Gideon.WebApi.Migrations
{
    public partial class AddFulltextIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // You need to instal sql server developer edition and then you have to do a custom install to get full text searching capabilities
            migrationBuilder.Sql(
                sql: "CREATE FULLTEXT CATALOG ftCatalog AS DEFAULT;",
                suppressTransaction: true);
            migrationBuilder.Sql(
                sql: "CREATE FULLTEXT INDEX ON Product (Description, ShortDescription) KEY INDEX PK_Product;",
                suppressTransaction: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
