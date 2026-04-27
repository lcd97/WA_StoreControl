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
    public class SubCategoriasService : CRUDBaseService<SubCategoria>
    {
        private DBStore db;

        public SubCategoriasService(DBStore db) : base(db ?? new DBStore()) => this.db = db ?? new DBStore();

        public IQueryable<SubCategoria> GetFilteredOrPaged(SearchSubCategoriasVM viewModel)
        {
            var query = from d in db.SubCategorias select d;

            if (viewModel.CategoriaId > 0)
                query = query.Where(x => x.CategoriaId == viewModel.CategoriaId);

            query = PaginateData(query.OrderBy(x => x.CategoriaId).ThenBy(x => x.Descripcion), viewModel);

            return query.AsNoTracking();
        }

        public string ValidateBeforeCreate(SubCategoria SubCategoria)
        {
            if (db.SubCategorias.Any(x => x.Codigo.Trim().ToLower() == SubCategoria.Codigo.Trim().ToLower()))
                return string.Format($"{SystemMessage.ValidateOperationError} : Ya existe un código igual. Modifique y vuelva a intentar");

            if (db.SubCategorias.Any(x => x.Descripcion.Trim().ToLower() == SubCategoria.Descripcion.Trim().ToLower()))
                return string.Format($"{SystemMessage.ValidateOperationError} : Ya existe una descripción igual. Modifique y vuelva a intentar");

            return string.Empty;
        }

        public string ValidateBeforeUpdate(SubCategoria SubCategoria)
        {
            var objeto = db.SubCategorias.Find(SubCategoria.Id);

            db.Entry(objeto).State = EntityState.Detached;

            if (objeto != null)
            {
                if (objeto.Codigo.Trim().ToLower() == SubCategoria.Codigo.Trim().ToLower())
                    return string.Empty;

                return ValidateBeforeCreate(SubCategoria);
            }

            return string.Format("¡El registro a modificar no existe!");
        }

        public string ValidateBeforeDelete(int id)
        {
            var objeto = db.SubCategorias.Find(id);

            if (objeto == null)
                return string.Format($"{SystemMessage.ValidateOperationError} : El registro ya no existe, actualice la lista.");

            if (objeto.Productos.Count > 0)
                return string.Format($"{SystemMessage.ValidateOperationError} : El registro no se puede eliminar, debido ha que esta siendo usado por otros registros");

            //Elimina el seguimiento del registro
            db.Entry(objeto).State = EntityState.Detached;

            return string.Empty;
        }

    }
}