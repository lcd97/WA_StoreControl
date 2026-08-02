using AutoMapper;
using ModelosDB;
using ModelosDB.Inventario;
using ModelosDB.Venta;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using WA_StoreControl.DTO;
using WA_StoreControl.Services;
using WA_StoreControl.Utilidades;
using WA_StoreControl.ViewModels;

namespace WA_StoreControl.Controllers
{
    public class ProductosVentaController : Controller
    {
        private DBStore db;
        private ProductoVentaService productoVentaService;
        private ProductosService productosService;

        public ProductosVentaController()
        {
            this.db = new DBStore();
            this.productoVentaService = new ProductoVentaService(db);
            this.productosService = new ProductosService(db);
        }

        // GET: ProductosVenta
        public ActionResult Index()
        {
            var productoVentaVM = new IndexProductoVentaVM();
            ViewBag.JsonData = JsonConvert.SerializeObject(productoVentaVM);

            return View(productoVentaVM);
        }

        [HttpGet]
        public JsonResult GetFilteredOrPaged(SearchProductosVentaVM viewModel)
        {
            var records = productoVentaService.GetFilteredOrPaged(viewModel);
            var recordsMapped = Mapper.Map<ICollection<ProductoVentaDTO>>(records.ToList());

            var RequestPagedResult = new RequestPagedResult<ProductoVentaDTO>(viewModel.TotalRecords, viewModel.TotalPages, viewModel.Page, recordsMapped);

            return Json(RequestPagedResult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult BusquedaProductoEnStock(string producto)
        {
            return Json(new RequestResult(productosService.BusquedaProductoEnStock(producto).ToList()), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create(ProductoVenta ProductoVenta)
        {
            var errorMessage = !ModelState.IsValid ? string.Join(" | ", ModelValidate.GetModelErrorMessages(ModelState)) : productoVentaService.ValidateBeforeCreate(ProductoVenta);

            if (string.IsNullOrEmpty(errorMessage))
            {
                if (productoVentaService.Create(ProductoVenta))
                    return Json(new RequestResult(SystemMessage.CreateSuccessful), JsonRequestBehavior.AllowGet);
                else
                    return Json(new RequestResult(SystemMessage.ServerError, false), JsonRequestBehavior.AllowGet);
            }
            else
                return Json(new RequestResult(errorMessage, false), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public JsonResult Edit(ProductoVenta ProductoVenta)
        {
            var errorMessage = !ModelState.IsValid ? string.Join(" | ", ModelValidate.GetModelErrorMessages(ModelState)) : productoVentaService.ValidateBeforeUpdate(ProductoVenta);
            if (string.IsNullOrEmpty(errorMessage))
            {
                productoVentaService.Update(ProductoVenta, out errorMessage);
                return Json(new RequestResult(SystemMessage.UpdateSuccessful), JsonRequestBehavior.AllowGet);
            }
            else
                return Json(new RequestResult(errorMessage, false), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public JsonResult Delete([Bind(Include = nameof(ProductoVenta.Id))] ProductoVenta ProductoVenta)
        {
            var errorMessage = productoVentaService.ValidateBeforeDelete(ProductoVenta.Id);

            if (string.IsNullOrEmpty(errorMessage))
            {
                if (productoVentaService.Delete(ProductoVenta.Id))
                    return Json(new RequestResult(SystemMessage.DeleteSuccessfull), JsonRequestBehavior.AllowGet);
                else
                    return Json(new RequestResult(SystemMessage.ServerError, false), JsonRequestBehavior.AllowGet);
            }
            else
                return Json(new RequestResult(errorMessage, false), JsonRequestBehavior.AllowGet);
        }
    }
}