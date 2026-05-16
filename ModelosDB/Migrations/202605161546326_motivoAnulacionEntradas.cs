namespace ModelosDB.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class motivoAnulacionEntradas : DbMigration
    {
        public override void Up()
        {
            AddColumn("inv.Entradas", "MotivoAnulacion", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("inv.Entradas", "MotivoAnulacion");
        }
    }
}
