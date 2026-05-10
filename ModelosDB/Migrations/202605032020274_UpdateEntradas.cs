namespace ModelosDB.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateEntradas : DbMigration
    {
        public override void Up()
        {
            DropIndex("inv.DetallesEntrada", new[] { "ProveedorId" });
            RenameColumn(table: "inv.DetallesEntrada", name: "ProveedorId", newName: "Persona_Id");
            AddColumn("inv.Entradas", "ProveedorId", c => c.Int(nullable: false));
            AlterColumn("inv.DetallesEntrada", "Persona_Id", c => c.Int());
            CreateIndex("inv.DetallesEntrada", "Persona_Id");
            CreateIndex("inv.Entradas", "ProveedorId");
            AddForeignKey("inv.Entradas", "ProveedorId", "gen.Personas", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("inv.Entradas", "ProveedorId", "gen.Personas");
            DropIndex("inv.Entradas", new[] { "ProveedorId" });
            DropIndex("inv.DetallesEntrada", new[] { "Persona_Id" });
            AlterColumn("inv.DetallesEntrada", "Persona_Id", c => c.Int(nullable: false));
            DropColumn("inv.Entradas", "ProveedorId");
            RenameColumn(table: "inv.DetallesEntrada", name: "Persona_Id", newName: "ProveedorId");
            CreateIndex("inv.DetallesEntrada", "ProveedorId");
        }
    }
}
