using ModelosDB;
using ModelosDB.Inventario;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using WA_StoreControl.Controllers;
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

            query = PaginateData(query.OrderBy(x => x.Codigo).ThenBy(x => x.Categoria.Descripcion).ThenBy(x => x.Descripcion), viewModel);

            return query.AsNoTracking();
        }

        public virtual string ValidateBeforeCreate(SubCategoria SubCategoria)
        {
            var subCategoria = PersonaHelper.BuscarCoincidencias(SubCategoria.Descripcion);

            if (db.SubCategorias.Any(x => x.Codigo.Trim().ToLower() == SubCategoria.Codigo.Trim().ToLower()))
                return string.Format($"{SystemMessage.ValidateOperationError} : Ya existe un código igual. Modifique y vuelva a intentar");

            if (db.SubCategorias.AsNoTracking().AsEnumerable().Any(x => x.CategoriaId == SubCategoria.CategoriaId
                                                                        && PersonaHelper.BuscarCoincidencias(x.Descripcion).Trim().ToLower() == subCategoria.Trim().ToLower()))
                return string.Format($"{SystemMessage.ValidateOperationError} : Ya existe una subcategoría con la misma descripción en la categoría seleccionada. Modifique y vuelva a intentar");

            return string.Empty;
        }

        public virtual string ValidateBeforeUpdate(SubCategoria SubCategoria)
        {
            var subCategoria = PersonaHelper.BuscarCoincidencias(SubCategoria.Descripcion);

            if (db.SubCategorias.Any(x => x.Codigo.Trim().ToLower() == SubCategoria.Codigo.Trim().ToLower() && x.Id != SubCategoria.Id))
                return string.Format($"{SystemMessage.ValidateOperationError} : Ya existe un código igual. Modifique y vuelva a intentar");

            if (db.SubCategorias.AsNoTracking().AsEnumerable().Any(x => x.CategoriaId == SubCategoria.CategoriaId
                                                                        && PersonaHelper.BuscarCoincidencias(x.Descripcion).Trim().ToLower() == subCategoria.Trim().ToLower()
                                                                        && x.Id != SubCategoria.Id))
                return string.Format($"{SystemMessage.ValidateOperationError} : Ya existe una subcategoría con la misma descripción en la categoría seleccionada. Modifique y vuelva a intentar");

            return string.Empty;
        }

        public virtual string ValidateBeforeDelete(int id)
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