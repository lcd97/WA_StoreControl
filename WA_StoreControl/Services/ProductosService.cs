using AutoMapper;
using ModelosDB;
using ModelosDB.Inventario;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using WA_StoreControl.Controllers;
using WA_StoreControl.DTO;
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

            query = PaginateData(query.OrderBy(x => x.Codigo).ThenBy(x => x.Descripcion), viewModel);

            return query.AsNoTracking();
        }

        public virtual string ValidateBeforeCreate(Producto Producto)
        {
            var producto = PersonaHelper.BuscarCoincidencias(Producto.Descripcion);

            if (!Producto.EsInventariable && Producto.Stock != 0)
                return string.Format($"{SystemMessage.ValidateOperationError} : Un producto no inventariable no debe tener stock. Modifique y vuelva a intentar");

            if (db.Productos.Any(x => x.Codigo.Trim().ToLower() == Producto.Codigo.Trim().ToLower()))
                return string.Format($"{SystemMessage.ValidateOperationError} : Ya existe un código igual. Modifique y vuelva a intentar");

            if (db.Productos.AsNoTracking().AsEnumerable().Any(x => PersonaHelper.BuscarCoincidencias(x.Descripcion).Trim().ToLower() == producto.Trim().ToLower()
                                                                    && x.MarcaId == Producto.MarcaId
                                                                    && x.SubCategoriaId == Producto.SubCategoriaId))
                return string.Format($"{SystemMessage.ValidateOperationError} : Ya existe un producto con la misma descripción, marca y subcategoría. Modifique y vuelva a intentar");

            return string.Empty;
        }

        public virtual string ValidateBeforeUpdate(Producto Producto)
        {
            var producto = PersonaHelper.BuscarCoincidencias(Producto.Descripcion);

            if (!Producto.EsInventariable && Producto.Stock != 0)
                return string.Format($"{SystemMessage.ValidateOperationError} : Un producto no inventariable no debe tener stock. Modifique y vuelva a intentar");

            var existente = db.Productos.Find(Producto.Id);

            if (existente != null && existente.Stock != Producto.Stock)
                return string.Format($"{SystemMessage.ValidateOperationError} : El stock no se puede modificar al editar un producto. Modifique y vuelva a intentar");

            if (db.Productos.Any(x => x.Codigo.Trim().ToLower() == Producto.Codigo.Trim().ToLower() && x.Id != Producto.Id))
                return string.Format($"{SystemMessage.ValidateOperationError} : Ya existe un código igual. Modifique y vuelva a intentar");

            if (db.Productos.AsNoTracking().AsEnumerable().Any(x => PersonaHelper.BuscarCoincidencias(x.Descripcion).Trim().ToLower() == producto.Trim().ToLower()
                                                                    && x.MarcaId == Producto.MarcaId
                                                                    && x.SubCategoriaId == Producto.SubCategoriaId
                                                                    && x.Id != Producto.Id))
                return string.Format($"{SystemMessage.ValidateOperationError} : Ya existe un producto con la misma descripción, marca y subcategoría. Modifique y vuelva a intentar");

            return string.Empty;
        }

        public virtual string ValidateBeforeDelete(int id)
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

        public List<ProductoDTO> BusquedaProducto(string producto = "")
        {
            var FormatProducto = PersonaHelper.BuscarCoincidencias(producto);

            var productos = db.Productos.Where(x => (x.Descripcion.Trim().Contains(FormatProducto.Trim())
                    || x.Marca.Descripcion.Trim().Contains(FormatProducto.Trim())) && x.EsActivo).ToList();

            return Mapper.Map<ICollection<ProductoDTO>>(productos).ToList();
        }

        public List<ProductoDTO> BusquedaProductoEnStock(string producto = "")
        {
            var FormatProducto = PersonaHelper.BuscarCoincidencias(producto);

            var productos = db.Productos.Where(x => (x.Descripcion.Trim().Contains(FormatProducto.Trim())
                    || x.Marca.Descripcion.Trim().Contains(FormatProducto.Trim()))
                    && x.EsActivo
                    && (x.Stock > 0 || !x.EsInventariable)
                    ).ToList();

            return Mapper.Map<ICollection<ProductoDTO>>(productos).ToList();
        }
    }
}