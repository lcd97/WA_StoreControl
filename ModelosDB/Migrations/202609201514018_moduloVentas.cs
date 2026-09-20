namespace ModelosDB.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class moduloVentas : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "ven.DetallesVenta",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Cantidad = c.Int(nullable: false),
                        PrecioUnitario = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TipoPrecio = c.Int(nullable: false),
                        Subtotal = c.Decimal(nullable: false, precision: 18, scale: 2),
                        VentaId = c.Int(nullable: false),
                        ProductoVentaId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("ven.ProductosVenta", t => t.ProductoVentaId)
                .ForeignKey("ven.Ventas", t => t.VentaId)
                .Index(t => t.VentaId)
                .Index(t => t.ProductoVentaId);
            
            CreateTable(
                "ven.Ventas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Codigo = c.String(nullable: false, maxLength: 15),
                        FechaVenta = c.DateTime(nullable: false),
                        EsActivo = c.Boolean(nullable: false),
                        ClienteId = c.Int(nullable: false),
                        Subtotal = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Descuento = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalVenta = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MotivoAnulacion = c.String(maxLength: 250),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("gen.Personas", t => t.ClienteId)
                .Index(t => t.ClienteId);
            
            AddColumn("ven.ProductosVenta", "EsDescuentoActivo", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("ven.DetallesVenta", "VentaId", "ven.Ventas");
            DropForeignKey("ven.Ventas", "ClienteId", "gen.Personas");
            DropForeignKey("ven.DetallesVenta", "ProductoVentaId", "ven.ProductosVenta");
            DropIndex("ven.Ventas", new[] { "ClienteId" });
            DropIndex("ven.DetallesVenta", new[] { "ProductoVentaId" });
            DropIndex("ven.DetallesVenta", new[] { "VentaId" });
            DropColumn("ven.ProductosVenta", "EsDescuentoActivo");
            DropTable("ven.Ventas");
            DropTable("ven.DetallesVenta");
        }
    }
}
