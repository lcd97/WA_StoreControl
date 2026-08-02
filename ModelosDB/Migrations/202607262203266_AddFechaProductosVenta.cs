namespace ModelosDB.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFechaProductosVenta : DbMigration
    {
        public override void Up()
        {
            AddColumn("ven.ProductosVenta", "FechaInicio", c => c.DateTime(nullable: false));
            AddColumn("ven.ProductosVenta", "FechaFin", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("ven.ProductosVenta", "FechaFin");
            DropColumn("ven.ProductosVenta", "FechaInicio");
        }
    }
}
