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
    public class MarcasController : Controller
    {
        private DBStore db;
        private MarcasService marcaService;

        public MarcasController()
        {
            db = new DBStore();
            marcaService = new MarcasService(db);
        }

        // GET: Marcas
        public ActionResult Index()
        {
            var acViewModel = new IndexMarcasVM();
            ViewBag.JsonData = JsonConvert.SerializeObject(acViewModel);

            return View(acViewModel);
        }

        [HttpGet]
        public JsonResult GetFilteredOrPaged(SearchMarcasVM viewModel)
        {
            var records = marcaService.GetFilteredOrPaged(viewModel);
            var recordsMapped = Mapper.Map<ICollection<MarcaDTO>>(records.ToList());

            var RequestPagedResult = new RequestPagedResult<MarcaDTO>(viewModel.TotalRecords, viewModel.TotalPages, viewModel.Page, recordsMapped);

            return Json(RequestPagedResult, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create(Marca Marca)
        {
            var errorMessage = !ModelState.IsValid ? string.Join(" | ", ModelValidate.GetModelErrorMessages(ModelState)) : marcaService.ValidateBeforeCreate(Marca);

            if (string.IsNullOrEmpty(errorMessage))
            {
                if (marcaService.Create(Marca))
                    return Json(new RequestResult(SystemMessage.CreateSuccessful), JsonRequestBehavior.AllowGet);
                else
                    return Json(new RequestResult(SystemMessage.ServerError, false), JsonRequestBehavior.AllowGet);
            }
            else
                return Json(new RequestResult(errorMessage, false), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public JsonResult Edit(Marca Marca)
        {
            var errorMessage = !ModelState.IsValid ? string.Join(" | ", ModelValidate.GetModelErrorMessages(ModelState)) : marcaService.ValidateBeforeUpdate(Marca);
            if (string.IsNullOrEmpty(errorMessage))
            {
                if (string.IsNullOrEmpty(errorMessage))
                {
                    marcaService.Update(Marca);
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
        public JsonResult Delete([Bind(Include = nameof(Marca.Id))] Marca Marca)
        {
            var errorMessage = marcaService.ValidateBeforeDelete(Marca.Id);

            if (string.IsNullOrEmpty(errorMessage))
            {
                if (marcaService.Delete(Marca.Id))
                    return Json(new RequestResult(SystemMessage.DeleteSuccessfull), JsonRequestBehavior.AllowGet);
                else
                    return Json(new RequestResult(SystemMessage.ServerError, false), JsonRequestBehavior.AllowGet);
            }
            else
                return Json(new RequestResult(errorMessage, false), JsonRequestBehavior.AllowGet);
        }
    }
}