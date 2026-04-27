using AutoMapper;
using ModelosDB.Inventario;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WA_StoreControl.DTO;
using WA_StoreControl.Models;
using WA_StoreControl.Services;
using WA_StoreControl.Utilidades;
using WA_StoreControl.ViewModels;

namespace WA_StoreControl.Controllers
{
    public class ProductosController : Controller
    {
        private DBStore db;
        private CategoriasService categoriasService;
        private SubCategoriasService subCategoriasService;
        private ProductosService productosService;
        private MarcasService marcasService;

        public ProductosController()
        {
            this.db = new DBStore();
            this.categoriasService = new CategoriasService(db);
            this.subCategoriasService = new SubCategoriasService(db);
            this.productosService = new ProductosService(db);
            this.marcasService = new MarcasService(db);
        }

        // GET: Productos
        public ActionResult Index()
        {
            var indexProductos = new IndexProductosVM();
            indexProductos.SubCategorias = Mapper.Map<ICollection<SubCategoriaDTO>>(subCategoriasService.GetAll().ToList());
            indexProductos.Categorias = Mapper.Map<ICollection<CategoriaDTO>>(categoriasService.GetAll().ToList());
            indexProductos.Marcas = Mapper.Map<ICollection<MarcaDTO>>(marcasService.GetAll().ToList());

            ViewBag.JsonData = JsonConvert.SerializeObject(indexProductos);

            return View(indexProductos);
        }

        [HttpGet]
        public JsonResult GetFilteredOrPaged(SearchProductosVM viewModel)
        {
            var records = productosService.GetFilteredOrPaged(viewModel);
            var recordsMapped = Mapper.Map<ICollection<ProductoDTO>>(records.ToList());

            var RequestPagedResult = new RequestPagedResult<ProductoDTO>(viewModel.TotalRecords, viewModel.TotalPages, viewModel.Page, recordsMapped);

            return Json(RequestPagedResult, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create(Producto Producto)
        {
            var errorMessage = !ModelState.IsValid ? string.Join(" | ", ModelValidate.GetModelErrorMessages(ModelState)) : productosService.ValidateBeforeCreate(Producto);

            if (string.IsNullOrEmpty(errorMessage))
            {
                if (productosService.Create(Producto))
                    return Json(new RequestResult(SystemMessage.CreateSuccessful), JsonRequestBehavior.AllowGet);
                else
                    return Json(new RequestResult(SystemMessage.ServerError, false), JsonRequestBehavior.AllowGet);
            }
            else
                return Json(new RequestResult(errorMessage, false), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public JsonResult Edit(Producto Producto)
        {
            var errorMessage = !ModelState.IsValid ? string.Join(" | ", ModelValidate.GetModelErrorMessages(ModelState)) : productosService.ValidateBeforeUpdate(Producto);
            if (string.IsNullOrEmpty(errorMessage))
            {
                if (string.IsNullOrEmpty(errorMessage))
                {
                    productosService.Update(Producto);
                    return Json(new RequestResult(SystemMessage.UpdateSuccessful), JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new RequestResult(SystemMessage.ServerError, false), JsonRequestBehavior.AllowGet);
            }
            else
                return Json(new RequestResult(errorMessage, false), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public JsonResult Delete([Bind(Include = nameof(Producto.Id))] Producto Producto)
        {
            var errorMessage = productosService.ValidateBeforeDelete(Producto.Id);

            if (string.IsNullOrEmpty(errorMessage))
            {
                if (productosService.Delete(Producto.Id))
                    return Json(new RequestResult(SystemMessage.DeleteSuccessfull), JsonRequestBehavior.AllowGet);
                else
                    return Json(new RequestResult(SystemMessage.ServerError, false), JsonRequestBehavior.AllowGet);
            }
            else
                return Json(new RequestResult(errorMessage, false), JsonRequestBehavior.AllowGet);
        }
    }
}