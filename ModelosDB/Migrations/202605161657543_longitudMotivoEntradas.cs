namespace ModelosDB.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class longitudMotivoEntradas : DbMigration
    {
        public override void Up()
        {
            AlterColumn("inv.Entradas", "MotivoAnulacion", c => c.String(maxLength: 250));
        }
        
        public override void Down()
        {
            AlterColumn("inv.Entradas", "MotivoAnulacion", c => c.String());
        }
    }
}
