namespace GeneralStoreAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedProductIDProperty : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Transactions", "ProductSKU", "dbo.Products");
            DropIndex("dbo.Transactions", new[] { "ProductSKU" });
            RenameColumn(table: "dbo.Transactions", name: "ProductSKU", newName: "ProductID");
            DropPrimaryKey("dbo.Products");
            AddColumn("dbo.Products", "ProductID", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.Transactions", "ProductID", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.Products", "ProductID");
            CreateIndex("dbo.Transactions", "ProductID");
            AddForeignKey("dbo.Transactions", "ProductID", "dbo.Products", "ProductID", cascadeDelete: true);
            DropColumn("dbo.Products", "ProductSKU");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Products", "ProductSKU", c => c.String(nullable: false, maxLength: 128));
            DropForeignKey("dbo.Transactions", "ProductID", "dbo.Products");
            DropIndex("dbo.Transactions", new[] { "ProductID" });
            DropPrimaryKey("dbo.Products");
            AlterColumn("dbo.Transactions", "ProductID", c => c.String(maxLength: 128));
            DropColumn("dbo.Products", "ProductID");
            AddPrimaryKey("dbo.Products", "ProductSKU");
            RenameColumn(table: "dbo.Transactions", name: "ProductID", newName: "ProductSKU");
            CreateIndex("dbo.Transactions", "ProductSKU");
            AddForeignKey("dbo.Transactions", "ProductSKU", "dbo.Products", "ProductSKU");
        }
    }
}
