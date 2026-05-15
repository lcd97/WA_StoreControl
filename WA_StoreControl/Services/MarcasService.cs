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
    public class MarcasService : CRUDBaseService<Marca>
    {
        private DBStore db;

        public MarcasService(DBStore db) : base(db ?? new DBStore()) => this.db = db ?? new DBStore();

        public IQueryable<Marca> GetFilteredOrPaged(SearchMarcasVM viewModel)
        {
            var query = from d in db.Marcas select d;

            if (!string.IsNullOrEmpty(viewModel.Descripcion))
                query = query.Where(x => x.Descripcion.Contains(viewModel.Descripcion));

            query = PaginateData(query.OrderBy(x => x.Codigo), viewModel);

            return query.AsNoTracking();
        }

        public string ValidateBeforeCreate(Marca Marca)
        {
            if (db.Marcas.Any(x => x.Codigo.Trim().ToLower() == Marca.Codigo.Trim().ToLower()))
                return string.Format($"{SystemMessage.ValidateOperationError} : Ya existe un código igual. Modifique y vuelva a intentar");

            if (db.Marcas.Any(x => x.Descripcion.Trim().ToLower() == Marca.Descripcion.Trim().ToLower()))
                return string.Format($"{SystemMessage.ValidateOperationError} : Ya existe una descripción igual. Modifique y vuelva a intentar");

            return string.Empty;
        }

        public string ValidateBeforeUpdate(Marca Marca)
        {

            if (db.Marcas.Any(x => x.Codigo.Trim().ToLower() == Marca.Codigo.Trim().ToLower() && x.Id != Marca.Id))
                return string.Format($"{SystemMessage.ValidateOperationError} : Ya existe un código igual. Modifique y vuelva a intentar");

            if (db.Marcas.Any(x => x.Descripcion.Trim().ToLower() == Marca.Descripcion.Trim().ToLower() && x.Id != Marca.Id))
                return string.Format($"{SystemMessage.ValidateOperationError} : Ya existe una descripción igual. Modifique y vuelva a intentar");

            return string.Empty;

        }

        public string ValidateBeforeDelete(int id)
        {
            var objeto = db.Marcas.Find(id);

            if (objeto == null)
                return string.Format($"{SystemMessage.ValidateOperationError} : El registro ya no existe, actualice la lista.");

            if (objeto.Codigo == "0000001")
                return string.Format($"{SystemMessage.ValidateOperationError} : El registro no se puede eliminar debido a que es un registro predeterminado");

            if (objeto.Productos.Count > 0)
                return string.Format($"{SystemMessage.ValidateOperationError} : El registro no se puede eliminar, debido ha que esta siendo usado por otros registros");

            //Elimina el seguimiento del registro
            db.Entry(objeto).State = EntityState.Detached;

            return string.Empty;
        }
    }
}