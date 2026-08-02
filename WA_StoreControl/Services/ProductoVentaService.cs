using ModelosDB;
using ModelosDB.General;
using ModelosDB.Inventario;
using ModelosDB.Venta;
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
    public class ProductoVentaService : CRUDBaseService<ProductoVenta>
    {
        private DBStore db;

        public ProductoVentaService(DBStore db) : base(db ?? new DBStore()) => this.db = db ?? new DBStore();

        public IQueryable<ProductoVenta> GetFilteredOrPaged(SearchProductosVentaVM viewModel)
        {
            var query = from d in db.ProductosVenta where d.EsActivo select d;

            if (!string.IsNullOrEmpty(viewModel.Descripcion))
                query = query.Where(x => x.Nombre.Contains(viewModel.Descripcion)
                                    || x.Descripcion.Contains(viewModel.Descripcion));

            query = PaginateData(query.OrderByDescending(x => x.Nombre), viewModel);

            return query.AsNoTracking();
        }

        public string ValidateBeforeCreate(ProductoVenta ProductoVenta)
        {
            if (ProductoVenta.FechaInicio >= ProductoVenta.FechaFin)
                return string.Format($"{SystemMessage.ValidateOperationError} : La fecha de inicio no puede exceder la fecha final");

            if (ProductoVenta.PrecioVenta <= 0 || ProductoVenta.PrecioMayor <= 0 || ProductoVenta.PrecioDescuento <= 0)
                return string.Format($"{SystemMessage.ValidateOperationError} : Los precios deben ser mayor a 0 ");

            if (ProductoVenta.PrecioVenta < ProductoVenta.PrecioDescuento)
                return string.Format($"{SystemMessage.ValidateOperationError} : Los precios de venta debe ser mayor que el precio de descuento");

            if (ProductoVenta.PrecioVenta < ProductoVenta.PrecioMayor)
                return string.Format($"{SystemMessage.ValidateOperationError} : Los precios de venta debe ser mayor que el precio de mayor");

            if (ProductoVenta.DetallesProductoVenta.Any(x => x.ProductoId <= 0 || x.Cantidad <= 0))
                return string.Format($"{SystemMessage.ValidateOperationError} : Existen detalles de productos inválidos. Intente nuevamente o contactese con el administrador");

            if (ProductoVenta.DetallesProductoVenta.GroupBy(x => x.ProductoId).Any(g => g.Count() > 1))
                return string.Format($"{SystemMessage.ValidateOperationError} : No puede agregar el mismo producto más de una vez. Intente nuevamente.");

            var FormatProducto = PersonaHelper.BuscarCoincidencias(ProductoVenta.Nombre);

            if (db.ProductosVenta.Any(x => x.Nombre.Trim().ToUpper() == FormatProducto.Trim().ToUpper()
            && x.EsActivo
            && x.FechaInicio <= ProductoVenta.FechaFin
            && x.FechaFin >= ProductoVenta.FechaInicio
            ))
                return string.Format($"{SystemMessage.ValidateOperationError} : Ya existe un Producto a vender vigente con el mismo nombre dentro de la fecha especificada");

            return string.Empty;
        }

        public string ValidateBeforeUpdate(ProductoVenta ProductoVenta)
        {
            var objeto = db.ProductosVenta.Find(ProductoVenta.Id);

            db.Entry(objeto).State = EntityState.Detached;

            if (objeto == null)
                return string.Format("¡El registro a modificar no existe!");

            if (ProductoVenta.FechaInicio >= ProductoVenta.FechaFin)
                return string.Format($"{SystemMessage.ValidateOperationError} : La fecha de inicio no puede exceder la fecha final");

            if (ProductoVenta.PrecioVenta <= 0 || ProductoVenta.PrecioMayor <= 0 || ProductoVenta.PrecioDescuento <= 0)
                return string.Format($"{SystemMessage.ValidateOperationError} : Los precios deben ser mayor a 0 ");

            if (ProductoVenta.DetallesProductoVenta.Any(x => x.ProductoId <= 0 || x.Cantidad <= 0))
                return string.Format($"{SystemMessage.ValidateOperationError} : Existen detalles de productos inválidos. Intente nuevamente o contactese con el administrador");

            if (ProductoVenta.PrecioVenta < ProductoVenta.PrecioDescuento)
                return string.Format($"{SystemMessage.ValidateOperationError} : Los precios de venta debe ser mayor que el precio de descuento");

            if (ProductoVenta.PrecioVenta < ProductoVenta.PrecioMayor)
                return string.Format($"{SystemMessage.ValidateOperationError} : Los precios de venta debe ser mayor que el precio de mayor");

            if (ProductoVenta.DetallesProductoVenta.GroupBy(x => x.ProductoId).Any(g => g.Count() > 1))
                return string.Format($"{SystemMessage.ValidateOperationError} : No puede agregar el mismo producto más de una vez. Intente nuevamente.");

            var FormatProducto = PersonaHelper.BuscarCoincidencias(ProductoVenta.Nombre);

            if (db.ProductosVenta.Any(x => x.Nombre.Trim().ToUpper() == FormatProducto.Trim().ToUpper()
            && x.EsActivo
            && x.FechaInicio <= ProductoVenta.FechaFin
            && x.FechaFin >= ProductoVenta.FechaInicio
            && x.Id != ProductoVenta.Id
            ))
                return string.Format($"{SystemMessage.ValidateOperationError} : Ya existe un Producto a vender vigente con el mismo nombre dentro de la fecha especificada");

            return string.Empty;
        }

        public string ValidateBeforeDelete(int id)
        {
            var objeto = db.ProductosVenta.Find(id);

            if (objeto == null)
                return string.Format($"{SystemMessage.ValidateOperationError} : El registro ya no existe, actualice la lista.");

            db.Entry(objeto).State = EntityState.Detached;

            return string.Empty;
        }

        public bool Update(ProductoVenta ProductoVenta, out string ErrorMessage)
        {
            ErrorMessage = string.Empty;

            try
            {
                var productosDB = db.ProductosVenta.Include(x => x.DetallesProductoVenta).FirstOrDefault(x => x.Id == ProductoVenta.Id);

                db.Entry(productosDB).CurrentValues.SetValues(ProductoVenta);

                foreach (var detalleDB in productosDB.DetallesProductoVenta.ToList())
                {
                    if (!ProductoVenta.DetallesProductoVenta.Any(i => i.Id == detalleDB.Id))
                        db.DetallesProductoVenta.Remove(detalleDB);
                }

                foreach (var detalle in ProductoVenta.DetallesProductoVenta)
                {
                    var existe = productosDB.DetallesProductoVenta
                        .FirstOrDefault(i => i.Id == detalle.Id);

                    if (existe != null)
                        db.Entry(existe).CurrentValues.SetValues(detalle);
                    else
                    {
                        detalle.ProductoVentaId = productosDB.Id;
                        productosDB.DetallesProductoVenta.Add(detalle);
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

        public new bool Delete(int ProductoVentaId)
        {
            try
            {
                var productoVenta = db.ProductosVenta
                    .Include(x => x.DetallesProductoVenta)
                    .FirstOrDefault(x => x.Id == ProductoVentaId);

                foreach (var detalle in productoVenta.DetallesProductoVenta.ToList())
                {
                    db.DetallesProductoVenta.Remove(detalle);
                }

                db.ProductosVenta.Remove(productoVenta);

                return db.SaveChanges() > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}