namespace ModelosDB.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class CorreccionRelacionProveedor : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("inv.DetallesEntrada", "FK_inv.DetallesEntrada_gen.Personas_ProveedorId");
            DropIndex("inv.DetallesEntrada", new[] { "Persona_Id" });
            DropColumn("inv.DetallesEntrada", "Persona_Id");
        }

        public override void Down()
        {
            AddColumn("inv.DetallesEntrada", "Persona_Id", c => c.Int());
            CreateIndex("inv.DetallesEntrada", "Persona_Id");
            AddForeignKey("inv.DetallesEntrada", "Persona_Id", "gen.Personas", "Id");
        }
    }
}
