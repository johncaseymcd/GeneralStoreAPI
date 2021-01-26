namespace GeneralStoreAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatedProductSKUProperty : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "ProductSKU", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Products", "ProductSKU");
        }
    }
}
