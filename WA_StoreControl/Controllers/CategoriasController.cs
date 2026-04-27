using AutoMapper;
using ModelosDB.Inventario;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WA_StoreControl.DTO;
using ModelosDB;
using WA_StoreControl.Services;
using WA_StoreControl.Utilidades;
using WA_StoreControl.ViewModels;

namespace WA_StoreControl.Controllers
{
    public class CategoriasController : Controller
    {
        private DBStore db;
        private CategoriasService acService;

        public CategoriasController()
        {
            db = new DBStore();
            acService = new CategoriasService(db);
        }

        // GET: Categorias
        public ActionResult Index()
        {
            var acViewModel = new IndexCategoriasVM();
            ViewBag.JsonData = JsonConvert.SerializeObject(acViewModel);

            return View(acViewModel);
        }

        [HttpGet]
        public JsonResult GetFilteredOrPaged(SearchCategoriasVM viewModel)
        {
            var records = acService.GetFilteredOrPaged(viewModel);
            var recordsMapped = Mapper.Map<ICollection<CategoriaDTO>>(records.ToList());

            var RequestPagedResult = new RequestPagedResult<CategoriaDTO>(viewModel.TotalRecords, viewModel.TotalPages, viewModel.Page, recordsMapped);

            return Json(RequestPagedResult, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create(Categoria Categoria)
        {
            var errorMessage = !ModelState.IsValid ? string.Join(" | ", ModelValidate.GetModelErrorMessages(ModelState)) : acService.ValidateBeforeCreate(Categoria);

            if (string.IsNullOrEmpty(errorMessage))
            {
                if (acService.Create(Categoria))
                    return Json(new RequestResult(SystemMessage.CreateSuccessful), JsonRequestBehavior.AllowGet);
                else
                    return Json(new RequestResult(SystemMessage.ServerError, false), JsonRequestBehavior.AllowGet);
            }
            else
                return Json(new RequestResult(errorMessage, false), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public JsonResult Edit(Categoria Categoria)
        {
            var errorMessage = !ModelState.IsValid ? string.Join(" | ", ModelValidate.GetModelErrorMessages(ModelState)) : acService.ValidateBeforeUpdate(Categoria);
            if (string.IsNullOrEmpty(errorMessage))
            {
                if (string.IsNullOrEmpty(errorMessage))
                {
                    acService.Update(Categoria);
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
        public JsonResult Delete([Bind(Include = nameof(Categoria.Id))] Categoria Categoria)
        {
            var errorMessage = acService.ValidateBeforeDelete(Categoria.Id);

            if (string.IsNullOrEmpty(errorMessage))
            {
                if (acService.Delete(Categoria.Id))
                    return Json(new RequestResult(SystemMessage.DeleteSuccessfull), JsonRequestBehavior.AllowGet);
                else
                    return Json(new RequestResult(SystemMessage.ServerError, false), JsonRequestBehavior.AllowGet);
            }
            else
                return Json(new RequestResult(errorMessage, false), JsonRequestBehavior.AllowGet);
        }
    }
}