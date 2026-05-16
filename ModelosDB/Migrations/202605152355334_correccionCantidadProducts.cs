namespace ModelosDB.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class correccionCantidadProducts : DbMigration
    {
        public override void Up()
        {
            AlterColumn("inv.DetallesEntrada", "Cantidad", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("inv.DetallesEntrada", "Cantidad", c => c.Double(nullable: false));
        }
    }
}
