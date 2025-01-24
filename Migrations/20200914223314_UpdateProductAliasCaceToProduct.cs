using Microsoft.EntityFrameworkCore.Migrations;

namespace GidIndustrial.Gideon.WebApi.Migrations
{
    public partial class UpdateProductAliasCaceToProduct : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                sql: @"ALTER TRIGGER CacheProductsSearch
ON ProductAlias AFTER INSERT, UPDATE, DELETE  
AS 
BEGIN 
SET NOCOUNT ON; 
declare @productId int = COALESCE((SELECT ProductId from inserted) , (SELECT ProductId from deleted)) 
UPDATE Product 
SET Product.AliasesCache = (SELECT STRING_AGG(REPLACE(REPLACE(REPLACE(REPLACE(ProductAlias.PartNumber,' ',''),'-',''),'(',''),')',''), ' ') FROM ProductAlias WHERE ProductAlias.ProductId = @productId) 
WHERE Product.Id = @productId 
END  
",
                suppressTransaction: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(sql: "DROP TRIGGER CacheProductsSearch", suppressTransaction: true);
        }
    }
}
