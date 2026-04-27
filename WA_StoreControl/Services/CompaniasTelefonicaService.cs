using ModelosDB.General;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using WA_StoreControl.DTO;
using ModelosDB;
using WA_StoreControl.Utilidades;
using WA_StoreControl.ViewModels;

namespace WA_StoreControl.Services
{
    public class CompaniasTelefonicaService : CRUDBaseService<CompaniaTelefonica>
    {
        private DBStore db;

        public CompaniasTelefonicaService(DBStore db) : base(db ?? new DBStore()) => this.db = db ?? new DBStore();

        public IQueryable<CompaniaTelefonica> GetFilteredOrPaged(SearchCompaniasTelefonicaVM viewModel)
        {
            var query = from d in db.CompaniasTelefonica select d;

            if (!string.IsNullOrEmpty(viewModel.Descripcion))
                query = query.Where(x => x.Descripcion.Contains(viewModel.Descripcion));

            query = PaginateData(query.OrderBy(x => x.Id), viewModel);

            return query.AsNoTracking();
        }

        public string ValidateBeforeCreate(CompaniaTelefonica CompaniaTelefonica)
        {
            if (db.CompaniasTelefonica.Any(x => x.Descripcion.Trim().ToLower() == CompaniaTelefonica.Descripcion.Trim().ToLower()))
                return string.Format($"{SystemMessage.ValidateOperationError} : Ya existe una descripción igual. Modifique y vuelva a intentar");

            return string.Empty;
        }

        public string ValidateBeforeUpdate(CompaniaTelefonica CompaniaTelefonica)
        {
            var objeto = db.CompaniasTelefonica.Find(CompaniaTelefonica.Id);

            db.Entry(objeto).State = EntityState.Detached;

            if (objeto != null)
            {
                if (objeto.Descripcion.Trim().ToLower() == CompaniaTelefonica.Descripcion.Trim().ToLower())
                    return string.Empty;

                return ValidateBeforeCreate(CompaniaTelefonica);
            }

            return string.Format("¡El registro a modificar no existe!");
        }

        public string ValidateBeforeDelete(int id)
        {
            var objeto = db.CompaniasTelefonica.Find(id);

            if (objeto == null)
                return string.Format($"{SystemMessage.ValidateOperationError} : El registro ya no existe, actualice la lista.");

            if (objeto.DetallesTelefono.Count > 0)
                return string.Format($"{SystemMessage.ValidateOperationError} : El registro no se puede eliminar, debido ha que esta siendo usado por otros registros");

            db.Entry(objeto).State = EntityState.Detached;

            return string.Empty;
        }
    }
}