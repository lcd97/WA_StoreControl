using AutoMapper;
using ModelosDB;
using ModelosDB.Inventario;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.ModelBinding;
using WA_StoreControl.DTO;
using WA_StoreControl.Utilidades;
using WA_StoreControl.ViewModels;

namespace WA_StoreControl.Services
{
    public class EntradasService : CRUDBaseService<Entrada>
    {
        private DBStore db;

        public EntradasService(DBStore db) : base(db ?? new DBStore()) => this.db = db ?? new DBStore();

        public IQueryable<Entrada> GetFilteredOrPaged(SearchEntradasVM viewModel)
        {
            var query = from d in db.Entradas select d;

            if (!string.IsNullOrEmpty(viewModel.FechaDesde))
                query = query.Where(x => x.FechaEntrada >= DateTime.Parse(viewModel.FechaDesde));

            if (!string.IsNullOrEmpty(viewModel.FechaHasta))
                query = query.Where(x => x.FechaEntrada <= DateTime.Parse(viewModel.FechaHasta));

            query = PaginateData(query.OrderBy(x => x.Codigo), viewModel);

            return query.AsNoTracking();
        }

        public string ValidateBeforeCreate(Entrada Entrada)
        {
            if (Entrada.DetallesEntrada.Any(x => x.ProductoId <= 0 || x.Cantidad <= 0 || x.Precio <= 0))
                return string.Format($"{SystemMessage.ValidateOperationError} : Existen detalles de productos inválidos. Intente nuevamente o contactese con el administrador");

            return string.Empty;
        }

        public string ValidateBeforeUpdate(Entrada Entrada)
        {
            var objeto = db.Entradas.Find(Entrada.Id);

            db.Entry(objeto).State = EntityState.Detached;

            if (objeto != null)
            {
                if (objeto.Codigo.Trim().ToLower() == Entrada.Codigo.Trim().ToLower())
                    return string.Empty;

                return ValidateBeforeCreate(Entrada);
            }

            return string.Format("¡El registro a modificar no existe!");
        }

        public string ValidateBeforeDelete(int id)
        {
            var objeto = db.Entradas.Find(id);

            if (objeto == null)
                return string.Format($"{SystemMessage.ValidateOperationError} : El registro ya no existe, actualice la lista.");

            //Elimina el seguimiento del registro
            db.Entry(objeto).State = EntityState.Detached;

            return string.Empty;
        }

        public string GenerarNuevoNumeroEntrada()
        {
            string tipo = "ENT";
            string puntoEmision = ConfigurationManager.AppSettings["PuntoEmision"];

            var siguienteNumero = db.Database.SqlQuery<long>("SELECT NEXT VALUE FOR inv.Seq_EntradasInventario").First();

            string secuencial = siguienteNumero.ToString().PadLeft(9, '0');

            return $"{tipo}{puntoEmision}{secuencial}";
        }

        public bool Create(Entrada Entrada, out string Message)
        {
            Message = string.Empty;

            try
            {
                Entrada.Codigo = GenerarNuevoNumeroEntrada();
                db.Entradas.Add(Entrada);

                var almacenado = db.SaveChanges() > 0;

                Message = almacenado
                    ? string.Format($"{SystemMessage.CreateSuccessful} : Se ha almacenado correctamente el registro de entrada con numeración {Entrada.Codigo}")
                    : string.Format($"{SystemMessage.ValidateOperationError} : Se ha generado un error al crear el registro, intente nuevamente o consulte con el administrador");

                return almacenado;

            }
            catch (Exception ex)
            {
                Message = string.Format($"{SystemMessage.ValidateOperationError} : Ha ocurrido un error al crear el registro : {ex.ToString()}");
                return false;
            }
        }

        public bool AnularEntrada(Entrada Entrada, out string Message)
        {
            Message = string.Empty;

            try
            {
                var EntradaDB = db.Entradas.Find(Entrada.Id);

                EntradaDB.EsActivo = false;

                db.Entry(EntradaDB).State = EntityState.Modified;

                var almacenado = db.SaveChanges() > 0;

                Message = almacenado
                    ? string.Format($"{SystemMessage.UpdateSuccessful} : Se ha anulado correctamente el registro de entrada con numeración {Entrada.Codigo}")
                    : string.Format($"{SystemMessage.ValidateOperationError} : Se ha generado un error al anular el registro, intente nuevamente o consulte con el administrador");

                return almacenado;

            }
            catch (Exception ex)
            {
                Message = string.Format($"{SystemMessage.ValidateOperationError} : Ha ocurrido un error al anular el registro : {ex.ToString()}");
                return false;
            }
        }

        public EntradaDTO ObtenerParaCrearOClonar(int id)
        {
            if (id <= 0)
                return new EntradaDTO();

            var entrada = Mapper.Map<EntradaDTO>(db.Entradas.Find(id));

            entrada.Id = 0;
            entrada.EsActivo = true;

            foreach (var detalle in entrada.DetallesEntrada)
            {
                detalle.Id = 0;
                detalle.EntradaId = 0;
            }

            return entrada;
        }

    }
}