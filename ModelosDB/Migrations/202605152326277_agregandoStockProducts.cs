namespace ModelosDB.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class agregandoStockProducts : DbMigration
    {
        public override void Up()
        {
            AddColumn("inv.Productos", "Stock", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("inv.Productos", "Stock");
        }
    }
}
