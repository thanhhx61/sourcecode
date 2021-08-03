namespace OnlineShop.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class createDB : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Category",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        Name = c.String(maxLength: 250),
                        Link = c.String(maxLength: 250, fixedLength: true),
                        Status = c.Boolean(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Product",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        Product_Name = c.String(maxLength: 250),
                        Metatitle = c.String(maxLength: 250, fixedLength: true),
                        Promotion_Price = c.Decimal(precision: 18, scale: 0),
                        Price = c.Decimal(precision: 18, scale: 0),
                        Image = c.String(maxLength: 250),
                        Quantity = c.Int(),
                        Category_ID = c.Long(),
                        Status = c.Boolean(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Category", t => t.Category_ID)
                .Index(t => t.Category_ID);
            
            CreateTable(
                "dbo.sysdiagrams",
                c => new
                    {
                        diagram_id = c.Int(nullable: false, identity: true),
                        name = c.String(nullable: false, maxLength: 128),
                        principal_id = c.Int(nullable: false),
                        version = c.Int(),
                        definition = c.Binary(),
                    })
                .PrimaryKey(t => t.diagram_id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Product", "Category_ID", "dbo.Category");
            DropIndex("dbo.Product", new[] { "Category_ID" });
            DropTable("dbo.sysdiagrams");
            DropTable("dbo.Product");
            DropTable("dbo.Category");
        }
    }
}
