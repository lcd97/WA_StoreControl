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
                   Codigo = "001",
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
                   Codigo = "001",
                   Descripcion = "Joyas",
                   EsActivo = true
               },
               new Categoria
               {
                   Codigo = "002",
                   Descripcion = "Accesorios",
                   EsActivo = true
               },
               new Categoria
               {
                   Codigo = "003",
                   Descripcion = "Electrónicos",
                   EsActivo = true
               });

            context.SubCategorias.AddOrUpdate(m => m.Codigo,
                new SubCategoria
                {
                    Codigo = "001",
                    Descripcion = "Oro",
                    CategoriaId = 1,
                    EsActivo = true
                },
                new SubCategoria
                {
                    Codigo = "002",
                    Descripcion = "Plata",
                    CategoriaId = 1,
                    EsActivo = true
                },
                new SubCategoria
                {
                    Codigo = "003",
                    Descripcion = "Enchapado",
                    CategoriaId = 1,
                    EsActivo = true
                },
                new SubCategoria
                {
                    Codigo = "004",
                    Descripcion = "Botella de agua",
                    CategoriaId = 2,
                    EsActivo = true
                },
                new SubCategoria
                {
                    Codigo = "005",
                    Descripcion = "Cargadores Portátiles",
                    CategoriaId = 3,
                    EsActivo = true
                });
        }
    }
}
