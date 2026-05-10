namespace ModelosDB.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AgregandoPrecioEntrada : DbMigration
    {
        public override void Up()
        {
            AddColumn("inv.Entradas", "TotalEntrada", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("inv.Entradas", "TotalEntrada");
        }
    }
}
