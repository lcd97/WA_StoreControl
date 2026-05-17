namespace ModelosDB.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AgregandoEsInventariableProducto : DbMigration
    {
        public override void Up()
        {
            AddColumn("inv.Productos", "EsInventariable", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("inv.Productos", "EsInventariable");
        }
    }
}
