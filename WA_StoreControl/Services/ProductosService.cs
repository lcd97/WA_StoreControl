using ModelosDB.Inventario;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using WA_StoreControl.Models;
using WA_StoreControl.Utilidades;
using WA_StoreControl.ViewModels;

namespace WA_StoreControl.Services
{
    public class ProductosService : CRUDBaseService<Producto>
    {
        private DBStore db;

        public ProductosService(DBStore db) : base(db ?? new DBStore()) => this.db = db ?? new DBStore();

        public IQueryable<Producto> GetFilteredOrPaged(SearchProductosVM viewModel)
        {
            var query = from d in db.Productos select d;

            if (!string.IsNullOrEmpty(viewModel.Descripcion))
                query = query.Where(x => x.Descripcion.Contains(viewModel.Descripcion) || x.Marca.Descripcion.Contains(viewModel.Descripcion));

            if (viewModel.SubCategoriaId > 0)
                query = query.Where(x => x.SubCategoriaId == viewModel.SubCategoriaId);

            if (viewModel.CategoriaId > 0)
                query = query.Where(x => x.SubCategoria.CategoriaId == viewModel.CategoriaId);

            query = PaginateData(query.OrderBy(x => x.Id), viewModel);

            return query.AsNoTracking();
        }

        public string ValidateBeforeCreate(Producto Producto)
        {
            if (db.Productos.Any(x => x.Codigo.Trim().ToLower() == Producto.Codigo.Trim().ToLower()))
                return string.Format($"{SystemMessage.ValidateOperationError} : Ya existe un código igual. Modifique y vuelva a intentar");

            if (db.Productos.Any(x => x.Descripcion.Trim().ToLower() == Producto.Descripcion.Trim().ToLower() && x.MarcaId == Producto.MarcaId))
                return string.Format($"{SystemMessage.ValidateOperationError} : Ya existe una descripción igual. Modifique y vuelva a intentar");

            return string.Empty;
        }

        public string ValidateBeforeUpdate(Producto Producto)
        {
            var objeto = db.Productos.Find(Producto.Id);

            db.Entry(objeto).State = EntityState.Detached;

            if (objeto != null)
            {
                if (objeto.Codigo.Trim().ToLower() == Producto.Codigo.Trim().ToLower())
                    return string.Empty;

                if (objeto.Descripcion.Trim().ToLower() == Producto.Descripcion.Trim().ToLower() && objeto.MarcaId == Producto.MarcaId)
                    return string.Empty;

                return ValidateBeforeCreate(Producto);
            }

            return string.Format("¡El registro a modificar no existe!");
        }

        public string ValidateBeforeDelete(int id)
        {
            var objeto = db.Productos.Find(id);

            if (objeto == null)
                return string.Format($"{SystemMessage.ValidateOperationError} : El registro ya no existe, actualice la lista.");

            if (objeto.DetallesEntrada.Count > 0)
                return string.Format($"{SystemMessage.ValidateOperationError} : El registro no se puede eliminar, debido ha que esta siendo usado por otros registros");

            //Elimina el seguimiento del registro
            db.Entry(objeto).State = EntityState.Detached;

            return string.Empty;
        }
    }
}