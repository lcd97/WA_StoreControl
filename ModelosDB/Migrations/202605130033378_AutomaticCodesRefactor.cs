namespace ModelosDB.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AutomaticCodesRefactor : DbMigration
    {
        public override void Up()
        {
            AlterColumn("inv.Categorias", "Codigo", c => c.String(nullable: false, maxLength: 6));
            AlterColumn("inv.SubCategorias", "Codigo", c => c.String(nullable: false, maxLength: 6));
            AlterColumn("inv.Productos", "Codigo", c => c.String(nullable: false, maxLength: 6));
            AlterColumn("inv.Marcas", "Codigo", c => c.String(nullable: false, maxLength: 6));
        }
        
        public override void Down()
        {
            AlterColumn("inv.Marcas", "Codigo", c => c.String(nullable: false, maxLength: 3));
            AlterColumn("inv.Productos", "Codigo", c => c.String(nullable: false, maxLength: 3));
            AlterColumn("inv.SubCategorias", "Codigo", c => c.String(nullable: false, maxLength: 3));
            AlterColumn("inv.Categorias", "Codigo", c => c.String(nullable: false, maxLength: 3));
        }
    }
}
