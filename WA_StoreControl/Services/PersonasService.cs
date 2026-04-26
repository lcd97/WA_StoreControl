using Microsoft.Ajax.Utilities;
using ModelosDB.General;
using ModelosDB.Inventario;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls.WebParts;
using WA_StoreControl.Controllers;
using WA_StoreControl.Models;
using WA_StoreControl.Utilidades;
using WA_StoreControl.ViewModels;
using WebGrease.Css.Extensions;

namespace WA_StoreControl.Services
{
    public class PersonasService : CRUDBaseService<Persona>
    {
        public PersonasService(DBStore db) : base(db ?? new DBStore()) => this.db = db ?? new DBStore();

        public IQueryable<Persona> GetFilteredOrPaged(SearchPersonasVM viewModel)
        {
            var query = from d in db.Personas select d;

            if (!string.IsNullOrEmpty(viewModel.Nombres))
                query = query.Where(x => x.Nombres.Contains(viewModel.Nombres) || x.Apellidos.Contains(viewModel.Nombres) || x.NombreComercial.Contains(viewModel.Nombres));

            if (!string.IsNullOrEmpty(viewModel.Identificacion))
            {
                var identidades = db.Identidades.Where(x => x.Identificacion.Contains(viewModel.Identificacion)).Select(x => x.PersonaId).ToList();
                query = query.Where(x => identidades.Contains(x.Id));
            }

            query = PaginateData(query.OrderBy(x => x.Id), viewModel);

            return query.AsNoTracking();
        }

        public string ValidateBeforeCreate(Persona Persona)
        {
            if (Persona.EsPersonaNatural && (string.IsNullOrEmpty(Persona.Nombres.Trim()) || string.IsNullOrEmpty(Persona.Apellidos.Trim())))
                return string.Format($"{SystemMessage.ValidateOperationError} : Para personas naturales, los campos de nombres y apellidos son obligatorios. Modifique y vuelva a intentar");

            if (Persona.EsPersonaNatural && Persona.FechaNacimiento == null)
                return string.Format($"{SystemMessage.ValidateOperationError} : Para personas naturales, el campo de fecha de nacimiento es obligatorio. Modifique y vuelva a intentar");

            if (Persona.EsPersonaNatural && Persona.FechaNacimiento > DateTime.Now)
                return string.Format($"{SystemMessage.ValidateOperationError} : Para personas naturales, el campo de fecha de nacimiento no puede ser una fecha futura. Modifique y vuelva a intentar");

            if (!Persona.EsPersonaNatural && string.IsNullOrEmpty(Persona.NombreComercial))
                return string.Format($"{SystemMessage.ValidateOperationError} : Para personas jurídicas, el campo de nombre comercial es obligatorio. Modifique y vuelva a intentar");

            var nombre = PersonaHelper.BuscarCoincidencias(string.Concat(Persona.Nombres, " ", Persona.Apellidos));

            if (db.Personas.Any(x => string.Concat(x.Nombres, " ", x.Apellidos) == nombre || x.NombreComercial == nombre))
                return string.Format($"{SystemMessage.ValidateOperationError} : Ya existe un registro con los nombres ingresados. Modifique y vuelva a intentar");

            return string.Empty;
        }

        public string ValidateBeforeUpdate(Persona Persona)
        {
            var objeto = db.Personas.Find(Persona.Id);
            var nombre = PersonaHelper.BuscarCoincidencias(string.Concat(Persona.Nombres, " ", Persona.Apellidos));

            db.Entry(objeto).State = EntityState.Detached;

            if (objeto != null)
            {
                if (Persona.EsPersonaNatural && (string.IsNullOrEmpty(Persona.Nombres.Trim()) || string.IsNullOrEmpty(Persona.Apellidos.Trim())))
                    return string.Format($"{SystemMessage.ValidateOperationError} : Para personas naturales, los campos de nombres y apellidos son obligatorios. Modifique y vuelva a intentar");

                if (Persona.EsPersonaNatural && Persona.FechaNacimiento == null)
                    return string.Format($"{SystemMessage.ValidateOperationError} : Para personas naturales, el campo de fecha de nacimiento es obligatorio. Modifique y vuelva a intentar");

                if (Persona.EsPersonaNatural && Persona.FechaNacimiento > DateTime.Now)
                    return string.Format($"{SystemMessage.ValidateOperationError} : Para personas naturales, el campo de fecha de nacimiento no puede ser una fecha futura. Modifique y vuelva a intentar");

                if (!Persona.EsPersonaNatural && string.IsNullOrEmpty(Persona.NombreComercial))
                    return string.Format($"{SystemMessage.ValidateOperationError} : Para personas jurídicas, el campo de nombre comercial es obligatorio. Modifique y vuelva a intentar");

                if (db.Personas.Any(x => (string.Concat(x.Nombres, " ", x.Apellidos) == nombre.Trim().ToUpper() || x.NombreComercial == nombre) && x.Id != Persona.Id))
                    return string.Format($"{SystemMessage.ValidateOperationError} : Ya existe un registro con los nombres ingresados. Modifique y vuelva a intentar");

                return string.Empty;
            }

            return string.Format("¡El registro a modificar no existe!");
        }

        public string ValidateBeforeDelete(int id)
        {
            var objeto = db.Personas.Find(id);

            if (objeto == null)
                return string.Format($"{SystemMessage.ValidateOperationError} : El registro ya no existe, actualice la lista.");

            if (db.Personas.Find(id).Id == 1)
                return string.Format($"{SystemMessage.ValidateOperationError} : El registro no se puede eliminar debido a que es el cliente por defecto");

            if (objeto.DetallesEntrada.Count > 0)
                return string.Format($"{SystemMessage.ValidateOperationError} : El registro no se puede eliminar, debido ha que esta siendo usado por otros registros");

            db.Entry(objeto).State = EntityState.Detached;

            return string.Empty;
        }

        public bool Create(Persona Persona, out string ErrorMessage)
        {
            var Almacenado = false;
            ErrorMessage = string.Empty;

            try
            {
                db.Personas.Add(Persona);
                Almacenado = db.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ha ocurrido un error al crear el registro : {ex.ToString()}";
            }

            return Almacenado;
        }

        public bool Update(Persona Persona, out string ErrorMessage)
        {
            ErrorMessage = string.Empty;

            try
            {
                var personaDB = db.Personas
                    .Include(x => x.Identidades)
                    .Include(x => x.DetallesTelefono)
                    .FirstOrDefault(x => x.Id == Persona.Id);

                db.Entry(personaDB).CurrentValues.SetValues(Persona);

                foreach (var identidadDB in Persona.Identidades.ToList())
                {
                    if (!Persona.Identidades.Any(i => i.Id == identidadDB.Id))
                        db.Identidades.Remove(identidadDB);
                }

                foreach (var identidad in Persona.Identidades)
                {
                    var existe = personaDB.Identidades
                        .FirstOrDefault(i => i.Id == identidad.Id);

                    if (existe != null)
                        db.Entry(existe).CurrentValues.SetValues(identidad);
                    else
                    {
                        identidad.PersonaId = personaDB.Id;
                        personaDB.Identidades.Add(identidad);
                    }
                }

                foreach (var telefonoDB in personaDB.DetallesTelefono.ToList())
                {
                    if (!Persona.DetallesTelefono.Any(i => i.Id == telefonoDB.Id))
                        db.DetallesTelefono.Remove(telefonoDB);
                }

                foreach (var telefono in Persona.DetallesTelefono)
                {
                    var existe = personaDB.DetallesTelefono
                        .FirstOrDefault(i => i.Id == telefono.Id);

                    if (existe != null)
                        db.Entry(existe).CurrentValues.SetValues(telefono);
                    else
                    {
                        telefono.PersonaId = personaDB.Id;
                        personaDB.DetallesTelefono.Add(telefono);
                    }
                }

                return db.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ha ocurrido un error al crear el registro || {ex.ToString()}";
                return false;
            }
        }

        public bool PuedeAgregarIdentidad(string Identificacion, int TipoIdentificacionId, int PersonaId, int Id)
        {
            return !db.Identidades.Any(x =>
                        x.Identificacion.ToUpper() == Identificacion
                       && x.Id != Id &&
                       (x.PersonaId != PersonaId
                       || (x.PersonaId == PersonaId && x.TipoIdentificacionId == TipoIdentificacionId)
                       ));
        }

        public new bool Delete(int PersonaId)
        {
            try
            {
                var PersonaDB = db.Personas
                    .Include(x => x.DetallesTelefono)
                    .Include(x => x.Identidades)
                    .FirstOrDefault(x => x.Id == PersonaId);

                foreach (var identidad in PersonaDB.Identidades.ToList())
                {
                    db.Identidades.Remove(identidad);
                }

                foreach (var telefono in PersonaDB.DetallesTelefono.ToList())
                {
                    db.DetallesTelefono.Remove(telefono);
                }

                db.Personas.Remove(PersonaDB);

                return db.SaveChanges() > 1;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}