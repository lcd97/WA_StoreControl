namespace ModelosDB.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModificacionCodigoEntrada : DbMigration
    {
        public override void Up()
        {
            AlterColumn("inv.Entradas", "Codigo", c => c.String(nullable: false, maxLength: 15));
        }
        
        public override void Down()
        {
            AlterColumn("inv.Entradas", "Codigo", c => c.String(nullable: false, maxLength: 3));
        }
    }
}
