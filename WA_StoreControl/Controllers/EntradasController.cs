using AutoMapper;
using ModelosDB;
using ModelosDB.Inventario;
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
    public class EntradasController : Controller
    {
        private DBStore db;
        private EntradasService entradaService;
        private ProductosService productosService;
        private PersonasService personasService;

        public EntradasController()
        {
            db = new DBStore();
            entradaService = new EntradasService(db);
            productosService = new ProductosService(db);
            personasService = new PersonasService(db);
        }

        // GET: Entradas
        public ActionResult Index()
        {
            var entradaViewModel = new IndexEntradasVM();
            ViewBag.JsonData = JsonConvert.SerializeObject(entradaViewModel);

            return View(entradaViewModel);
        }

        [HttpGet]
        public JsonResult GetFilteredOrPaged(SearchEntradasVM viewModel)
        {
            var records = entradaService.GetFilteredOrPaged(viewModel);
            var recordsMapped = Mapper.Map<ICollection<EntradaDTO>>(records.ToList());

            var RequestPagedResult = new RequestPagedResult<EntradaDTO>(viewModel.TotalRecords, viewModel.TotalPages, viewModel.Page, recordsMapped);

            return Json(RequestPagedResult, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CrearEntrada(int EntradaId = 0)
        {
            var entradaViewModel = new IndexCrearEntradasVM();
            entradaViewModel.Entrada = entradaService.ObtenerParaCrearOClonar(EntradaId);

            ViewBag.JsonData = JsonConvert.SerializeObject(entradaViewModel);

            return View(entradaViewModel);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create(Entrada Entrada)
        {
            var Message = !ModelState.IsValid ? string.Join(" | ", ModelValidate.GetModelErrorMessages(ModelState)) : entradaService.ValidateBeforeCreate(Entrada);

            if (string.IsNullOrEmpty(Message))
            {
                if (entradaService.Create(Entrada, out Message))
                    return Json(new RequestResult(Message), JsonRequestBehavior.AllowGet);
                else
                    return Json(new RequestResult(Message, false), JsonRequestBehavior.AllowGet);
            }
            else
                return Json(new RequestResult(Message, false), JsonRequestBehavior.AllowGet);
        }

        //[ValidateAntiForgeryToken]
        public ActionResult DetalleEntrada(int EntradaId = 0)
        {
            var entradaViewModel = new IndexCrearEntradasVM();
            entradaViewModel.Entrada = Mapper.Map<EntradaDTO>(entradaService.Find(EntradaId));

            ViewBag.JsonData = JsonConvert.SerializeObject(entradaViewModel);

            return View(entradaViewModel);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public JsonResult AnularEntrada([Bind(Include = nameof(Entrada.Id))] Entrada Entrada, string Motivo)
        {
            var Message = entradaService.ValidateBeforeDelete(Entrada.Id);

            if (string.IsNullOrEmpty(Message))
            {
                if (entradaService.AnularEntrada(Entrada, Motivo, out Message))
                    return Json(new RequestResult(Message), JsonRequestBehavior.AllowGet);
                else
                    return Json(new RequestResult(Message, false), JsonRequestBehavior.AllowGet);
            }
            else
                return Json(new RequestResult(Message, false), JsonRequestBehavior.AllowGet);
        }

        public JsonResult BusquedaProveedor(string nombre)
        {
            return Json(new RequestResult(personasService.BusquedaProveedor(nombre)), JsonRequestBehavior.AllowGet);
        }

        public JsonResult BusquedaProductos(string producto)
        {
            return Json(new RequestResult(productosService.BusquedaProducto(producto).ToList()), JsonRequestBehavior.AllowGet);
        }
    }
}