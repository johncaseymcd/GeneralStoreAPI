namespace GeneralStoreAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RefactoredCart : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Carts", "Product_ProductID", c => c.Int());
            AddColumn("dbo.Products", "Cart_CartID", c => c.Int());
            CreateIndex("dbo.Carts", "Product_ProductID");
            CreateIndex("dbo.Products", "Cart_CartID");
            AddForeignKey("dbo.Carts", "Product_ProductID", "dbo.Products", "ProductID");
            AddForeignKey("dbo.Products", "Cart_CartID", "dbo.Carts", "CartID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Products", "Cart_CartID", "dbo.Carts");
            DropForeignKey("dbo.Carts", "Product_ProductID", "dbo.Products");
            DropIndex("dbo.Products", new[] { "Cart_CartID" });
            DropIndex("dbo.Carts", new[] { "Product_ProductID" });
            DropColumn("dbo.Products", "Cart_CartID");
            DropColumn("dbo.Carts", "Product_ProductID");
        }
    }
}
