using ModelosDB.General;
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
    public class TiposIdentificacionService : CRUDBaseService<TipoIdentificacion>
    {
        private DBStore db;

        public TiposIdentificacionService(DBStore db) : base(db ?? new DBStore()) => this.db = db ?? new DBStore();

        public IQueryable<TipoIdentificacion> GetFilteredOrPaged(SearchTiposIdentificacionVM viewModel)
        {
            var query = from d in db.TiposIdentificacion select d;

            if (!string.IsNullOrEmpty(viewModel.Descripcion))
                query = query.Where(x => x.Descripcion.Contains(viewModel.Descripcion));

            query = PaginateData(query.OrderBy(x => x.Id), viewModel);

            return query.AsNoTracking();
        }

        public string ValidateBeforeCreate(TipoIdentificacion TipoIdentificacion)
        {
            if (db.TiposIdentificacion.Any(x => x.Descripcion.Trim().ToLower() == TipoIdentificacion.Descripcion.Trim().ToLower()))
                return string.Format($"{SystemMessage.ValidateOperationError} : Ya existe una descripción igual. Modifique y vuelva a intentar");

            return string.Empty;
        }

        public string ValidateBeforeUpdate(TipoIdentificacion TipoIdentificacion)
        {
            var objeto = db.TiposIdentificacion.Find(TipoIdentificacion.Id);

            db.Entry(objeto).State = EntityState.Detached;

            if (objeto != null)
            {
                if (objeto.Descripcion.Trim().ToLower() == TipoIdentificacion.Descripcion.Trim().ToLower())
                    return string.Empty;

                return ValidateBeforeCreate(TipoIdentificacion);
            }

            return string.Format("¡El registro a modificar no existe!");
        }

        public string ValidateBeforeDelete(int id)
        {
            var objeto = db.TiposIdentificacion.Find(id);

            if (objeto == null)
                return string.Format($"{SystemMessage.ValidateOperationError} : El registro ya no existe, actualice la lista.");

            if (objeto.Identidades.Count > 0)
                return string.Format($"{SystemMessage.ValidateOperationError} : El registro no se puede eliminar, debido ha que esta siendo usado por otros registros");

            db.Entry(objeto).State = EntityState.Detached;

            return string.Empty;
        }

    }
}