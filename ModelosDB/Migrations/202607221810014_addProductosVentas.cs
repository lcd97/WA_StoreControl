namespace ModelosDB.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addProductosVentas : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "ven.DetallesProductoVenta",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProductoVentaId = c.Int(nullable: false),
                        ProductoId = c.Int(nullable: false),
                        Cantidad = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("inv.Productos", t => t.ProductoId)
                .ForeignKey("ven.ProductosVenta", t => t.ProductoVentaId)
                .Index(t => t.ProductoVentaId)
                .Index(t => t.ProductoId);
            
            CreateTable(
                "ven.ProductosVenta",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nombre = c.String(nullable: false, maxLength: 250),
                        Descripcion = c.String(nullable: false, maxLength: 250),
                        PrecioVenta = c.Double(nullable: false),
                        PrecioMayor = c.Double(nullable: false),
                        PrecioDescuento = c.Double(nullable: false),
                        Cantidad = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("ven.DetallesProductoVenta", "ProductoVentaId", "ven.ProductosVenta");
            DropForeignKey("ven.DetallesProductoVenta", "ProductoId", "inv.Productos");
            DropIndex("ven.DetallesProductoVenta", new[] { "ProductoId" });
            DropIndex("ven.DetallesProductoVenta", new[] { "ProductoVentaId" });
            DropTable("ven.ProductosVenta");
            DropTable("ven.DetallesProductoVenta");
        }
    }
}
