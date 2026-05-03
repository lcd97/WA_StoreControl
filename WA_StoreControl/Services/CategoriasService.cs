using ModelosDB.Inventario;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using ModelosDB;
using WA_StoreControl.Utilidades;
using WA_StoreControl.ViewModels;

namespace WA_StoreControl.Services
{
    public class CategoriasService : CRUDBaseService<Categoria>
    {
        private DBStore db;

        public CategoriasService(DBStore db) : base(db ?? new DBStore()) => this.db = db ?? new DBStore();

        public IQueryable<Categoria> GetFilteredOrPaged(SearchCategoriasVM viewModel)
        {
            var query = from d in db.Categorias select d;

            if (!string.IsNullOrEmpty(viewModel.Descripcion))
                query = query.Where(x => x.Descripcion.Contains(viewModel.Descripcion));

            query = PaginateData(query.OrderBy(x => x.Descripcion), viewModel);

            return query.AsNoTracking();
        }

        public string ValidateBeforeCreate(Categoria Categoria)
        {
            if (db.Categorias.Any(x => x.Codigo.Trim().ToLower() == Categoria.Codigo.Trim().ToLower()))
                return string.Format($"{SystemMessage.ValidateOperationError} : Ya existe un código igual. Modifique y vuelva a intentar");

            if (db.Categorias.Any(x => x.Descripcion.Trim().ToLower() == Categoria.Descripcion.Trim().ToLower()))
                return string.Format($"{SystemMessage.ValidateOperationError} : Ya existe una descripción igual. Modifique y vuelva a intentar");

            return string.Empty;
        }

        public string ValidateBeforeUpdate(Categoria Categoria)
        {
            var objeto = db.Categorias.Find(Categoria.Id);

            db.Entry(objeto).State = EntityState.Detached;

            if (objeto != null)
            {
                if (objeto.Codigo.Trim().ToLower() == Categoria.Codigo.Trim().ToLower())
                    return string.Empty;

                return ValidateBeforeCreate(Categoria);
            }

            return string.Format("¡El registro a modificar no existe!");
        }

        public string ValidateBeforeDelete(int id)
        {
            var objeto = db.Categorias.Find(id);

            if (objeto == null)
                return string.Format($"{SystemMessage.ValidateOperationError} : El registro ya no existe, actualice la lista.");

            if (objeto.SubCategorias.Count > 0)
                return string.Format($"{SystemMessage.ValidateOperationError} : El registro no se puede eliminar, debido ha que esta siendo usado por otros registros");

            //Elimina el seguimiento del registro
            db.Entry(objeto).State = EntityState.Detached;

            return string.Empty;
        }
    }
}