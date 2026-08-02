namespace ModelosDB.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class modifyTableProductoVenta : DbMigration
    {
        public override void Up()
        {
            AddColumn("ven.ProductosVenta", "EsActivo", c => c.Boolean(nullable: false));
            DropColumn("ven.ProductosVenta", "Cantidad");
        }
        
        public override void Down()
        {
            AddColumn("ven.ProductosVenta", "Cantidad", c => c.Int(nullable: false));
            DropColumn("ven.ProductosVenta", "EsActivo");
        }
    }
}
