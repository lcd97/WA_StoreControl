namespace ModelosDB.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "inv.Categorias",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Codigo = c.String(nullable: false, maxLength: 3),
                    Descripcion = c.String(nullable: false, maxLength: 100),
                    EsActivo = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "inv.SubCategorias",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Codigo = c.String(nullable: false, maxLength: 3),
                    Descripcion = c.String(nullable: false, maxLength: 100),
                    EsActivo = c.Boolean(nullable: false),
                    CategoriaId = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("inv.Categorias", t => t.CategoriaId)
                .Index(t => t.CategoriaId);

            CreateTable(
                "inv.Productos",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Codigo = c.String(nullable: false, maxLength: 3),
                    Descripcion = c.String(nullable: false, maxLength: 100),
                    EsActivo = c.Boolean(nullable: false),
                    SubCategoriaId = c.Int(nullable: false),
                    MarcaId = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("inv.Marcas", t => t.MarcaId)
                .ForeignKey("inv.SubCategorias", t => t.SubCategoriaId)
                .Index(t => t.SubCategoriaId)
                .Index(t => t.MarcaId);

            CreateTable(
                "inv.DetallesEntrada",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Cantidad = c.Double(nullable: false),
                    Precio = c.Double(nullable: false),
                    EntradaId = c.Int(nullable: false),
                    ProveedorId = c.Int(nullable: false),
                    ProductoId = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("inv.Entradas", t => t.EntradaId)
                .ForeignKey("inv.Productos", t => t.ProductoId)
                .ForeignKey("gen.Personas", t => t.ProveedorId)
                .Index(t => t.EntradaId)
                .Index(t => t.ProveedorId)
                .Index(t => t.ProductoId);

            CreateTable(
                "inv.Entradas",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Codigo = c.String(nullable: false, maxLength: 3),
                    FechaEntrada = c.DateTime(nullable: false),
                    EsActivo = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "gen.Personas",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Nombres = c.String(maxLength: 100),
                    Apellidos = c.String(maxLength: 100),
                    FechaNacimiento = c.DateTime(nullable: false),
                    NombreComercial = c.String(maxLength: 100),
                    Direccion = c.String(maxLength: 150),
                    EsActivo = c.Boolean(nullable: false),
                    EsPersonaNatural = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "gen.DetallesTelefono",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    NumeroTelefonico = c.String(nullable: false, maxLength: 8),
                    CompaniaTelefonicaId = c.Int(nullable: false),
                    PersonaId = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("gen.CompaniasTelefonica", t => t.CompaniaTelefonicaId)
                .ForeignKey("gen.Personas", t => t.PersonaId)
                .Index(t => t.CompaniaTelefonicaId)
                .Index(t => t.PersonaId);

            CreateTable(
                "gen.CompaniasTelefonica",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Descripcion = c.String(nullable: false, maxLength: 80),
                    EsActivo = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "gen.Identidades",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Identificacion = c.String(maxLength: 14),
                    TipoIdentificacionId = c.Int(nullable: false),
                    PersonaId = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("gen.Personas", t => t.PersonaId)
                .ForeignKey("gen.TiposIdentificacion", t => t.TipoIdentificacionId)
                .Index(t => t.TipoIdentificacionId)
                .Index(t => t.PersonaId);

            CreateTable(
                "gen.TiposIdentificacion",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Descripcion = c.String(nullable: false, maxLength: 100),
                    EsActivo = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "inv.Marcas",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Codigo = c.String(nullable: false, maxLength: 3),
                    Descripcion = c.String(nullable: false, maxLength: 100),
                    EsActivo = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.Id);

        }

        public override void Down()
        {
            DropForeignKey("inv.Productos", "SubCategoriaId", "inv.SubCategorias");
            DropForeignKey("inv.Productos", "MarcaId", "inv.Marcas");
            DropForeignKey("gen.Identidades", "TipoIdentificacionId", "gen.TiposIdentificacion");
            DropForeignKey("gen.Identidades", "PersonaId", "gen.Personas");
            DropForeignKey("gen.DetallesTelefono", "PersonaId", "gen.Personas");
            DropForeignKey("gen.DetallesTelefono", "CompaniaTelefonicaId", "gen.CompaniasTelefonica");
            DropForeignKey("inv.DetallesEntrada", "ProveedorId", "gen.Personas");
            DropForeignKey("inv.DetallesEntrada", "ProductoId", "inv.Productos");
            DropForeignKey("inv.DetallesEntrada", "EntradaId", "inv.Entradas");
            DropForeignKey("inv.SubCategorias", "CategoriaId", "inv.Categorias");
            DropIndex("gen.Identidades", new[] { "PersonaId" });
            DropIndex("gen.Identidades", new[] { "TipoIdentificacionId" });
            DropIndex("gen.DetallesTelefono", new[] { "PersonaId" });
            DropIndex("gen.DetallesTelefono", new[] { "CompaniaTelefonicaId" });
            DropIndex("inv.DetallesEntrada", new[] { "ProductoId" });
            DropIndex("inv.DetallesEntrada", new[] { "ProveedorId" });
            DropIndex("inv.DetallesEntrada", new[] { "EntradaId" });
            DropIndex("inv.Productos", new[] { "MarcaId" });
            DropIndex("inv.Productos", new[] { "SubCategoriaId" });
            DropIndex("inv.SubCategorias", new[] { "CategoriaId" });
            DropTable("inv.Marcas");
            DropTable("gen.TiposIdentificacion");
            DropTable("gen.Identidades");
            DropTable("gen.CompaniasTelefonica");
            DropTable("gen.DetallesTelefono");
            DropTable("gen.Personas");
            DropTable("inv.Entradas");
            DropTable("inv.DetallesEntrada");
            DropTable("inv.Productos");
            DropTable("inv.SubCategorias");
            DropTable("inv.Categorias");
        }
    }
}
