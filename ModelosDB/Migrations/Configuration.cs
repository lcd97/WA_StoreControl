namespace ModelosDB.Migrations
{
    using ModelosDB.General;
    using ModelosDB.Inventario;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<ModelosDB.DBStore>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(ModelosDB.DBStore context)
        {
            context.Marcas.AddOrUpdate(m => m.Codigo,
               new Marca
               {
                   Codigo = "000001",
                   Descripcion = "Sin Marca",
                   EsActivo = true
               });

            context.TiposIdentificacion.AddOrUpdate(m => m.Descripcion,
               new TipoIdentificacion
               {
                   Descripcion = "Cédula",
                   EsActivo = true
               }, new TipoIdentificacion
               {
                   Descripcion = "RUC",
                   EsActivo = true
               }, new TipoIdentificacion
               {
                   Descripcion = "Pasaporte",
                   EsActivo = true
               });

            context.CompaniasTelefonica.AddOrUpdate(m => m.Descripcion,
              new CompaniaTelefonica
              {
                  Descripcion = "Tigo",
                  EsActivo = true
              }, new CompaniaTelefonica
              {
                  Descripcion = "Claro",
                  EsActivo = true
              });

            context.Categorias.AddOrUpdate(m => m.Codigo,
               new Categoria
               {
                   Codigo = "000001",
                   Descripcion = "Servicios",
                   EsActivo = true
               },
               new Categoria
               {
                   Codigo = "000002",
                   Descripcion = "Joyas",
                   EsActivo = true
               },
               new Categoria
               {
                   Codigo = "000003",
                   Descripcion = "Accesorios",
                   EsActivo = true
               },
               new Categoria
               {
                   Codigo = "000004",
                   Descripcion = "Electrónicos",
                   EsActivo = true
               });

            context.SaveChanges();

            var catServicios = context.Categorias.FirstOrDefault(c => c.Codigo == "000001");
            var catJoyas = context.Categorias.FirstOrDefault(c => c.Codigo == "000002");
            var catAccesorios = context.Categorias.FirstOrDefault(c => c.Codigo == "000003");
            var catElectronicos = context.Categorias.FirstOrDefault(c => c.Codigo == "000004");

            context.SubCategorias.AddOrUpdate(m => m.Codigo,
                new SubCategoria
                {
                    Codigo = "000001",
                    Descripcion = "Logistica",
                    CategoriaId = catJoyas != null ? catJoyas.Id : 1,
                    EsActivo = true
                },
                new SubCategoria
                {
                    Codigo = "000002",
                    Descripcion = "Oro",
                    CategoriaId = catJoyas != null ? catJoyas.Id : 2,
                    EsActivo = true
                },
                new SubCategoria
                {
                    Codigo = "000003",
                    Descripcion = "Plata",
                    CategoriaId = catJoyas != null ? catJoyas.Id : 2,
                    EsActivo = true
                },
                new SubCategoria
                {
                    Codigo = "000004",
                    Descripcion = "Enchapado",
                    CategoriaId = catJoyas != null ? catJoyas.Id : 2,
                    EsActivo = true
                },
                new SubCategoria
                {
                    Codigo = "000005",
                    Descripcion = "Botella de agua",
                    CategoriaId = catJoyas != null ? catJoyas.Id : 3,
                    EsActivo = true
                },
                new SubCategoria
                {
                    Codigo = "000006",
                    Descripcion = "Cargadores Portátiles",
                    CategoriaId = catJoyas != null ? catJoyas.Id : 4,
                    EsActivo = true
                });

            context.SaveChanges();

            var marcaSinMarca = context.Marcas.FirstOrDefault(m => m.Codigo == "000001");
            var subLogistica = context.SubCategorias.FirstOrDefault(s => s.Codigo == "000001");

            context.Productos.AddOrUpdate(m => m.Codigo,
               new Producto
               {
                   Codigo = "000001",
                   SubCategoriaId = subLogistica != null ? subLogistica.Id : 1,
                   MarcaId = marcaSinMarca != null ? marcaSinMarca.Id : 1,
                   Descripcion = "Parqueo",
                   Stock = 0,
                   EsInventariable = false,
                   EsActivo = true
               },
               new Producto
               {
                   Codigo = "000002",
                   SubCategoriaId = subLogistica != null ? subLogistica.Id : 1,
                   MarcaId = marcaSinMarca != null ? marcaSinMarca.Id : 1,
                   Descripcion = "Bebidas",
                   Stock = 0,
                   EsInventariable = false,
                   EsActivo = true
               });

            context.Personas.AddOrUpdate(m => m.EsPersonaNatural,
               new Persona
               {
                   Nombres = "CLIENTE POR ",
                   Apellidos = "DEFECTO",
                   NombreComercial = "CLIENTE POR DEFECTO",
                   EsPersonaNatural = true,
                   EsActivo = true,
                   Direccion = "N/A",
                   FechaNacimiento = DateTime.Today
               });

            context.SaveChanges();
        }
    }
}
