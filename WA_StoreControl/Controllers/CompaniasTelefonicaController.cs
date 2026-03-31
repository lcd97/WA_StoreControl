using AutoMapper;
using ModelosDB.General;
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
    public class CompaniasTelefonicaController : Controller
    {
        private DBStore db;
        private CompaniasTelefonicaService companiasTelefonicaService;

        public CompaniasTelefonicaController()
        {
            this.db = new DBStore();
            this.companiasTelefonicaService = new CompaniasTelefonicaService(db);
        }

        // GET: CompaniasTelefonica
        public ActionResult Index()
        {
            var indexCompanias = new IndexCompaniasTelefonicaVM();

            ViewBag.JsonData = JsonConvert.SerializeObject(indexCompanias);

            return View(indexCompanias);
        }

        [HttpGet]
        public JsonResult GetFilteredOrPaged(SearchCompaniasTelefonicaVM viewModel)
        {
            var records = companiasTelefonicaService.GetFilteredOrPaged(viewModel);
            var recordsMapped = Mapper.Map<ICollection<CompaniaTelefonicaDTO>>(records.ToList());

            var RequestPagedResult = new RequestPagedResult<CompaniaTelefonicaDTO>(viewModel.TotalRecords, viewModel.TotalPages, viewModel.Page, recordsMapped);

            return Json(RequestPagedResult, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create(CompaniaTelefonica CompaniaTelefonica)
        {
            var errorMessage = !ModelState.IsValid ? string.Join(" | ", ModelValidate.GetModelErrorMessages(ModelState)) : companiasTelefonicaService.ValidateBeforeCreate(CompaniaTelefonica);

            if (string.IsNullOrEmpty(errorMessage))
            {
                if (companiasTelefonicaService.Create(CompaniaTelefonica))
                    return Json(new RequestResult(SystemMessage.CreateSuccessful), JsonRequestBehavior.AllowGet);
                else
                    return Json(new RequestResult(SystemMessage.ServerError, false), JsonRequestBehavior.AllowGet);
            }
            else
                return Json(new RequestResult(errorMessage, false), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public JsonResult Edit(CompaniaTelefonica CompaniaTelefonica)
        {
            var errorMessage = !ModelState.IsValid ? string.Join(" | ", ModelValidate.GetModelErrorMessages(ModelState)) : companiasTelefonicaService.ValidateBeforeUpdate(CompaniaTelefonica);
            if (string.IsNullOrEmpty(errorMessage))
            {
                if (string.IsNullOrEmpty(errorMessage))
                {
                    companiasTelefonicaService.Update(CompaniaTelefonica);
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
        public JsonResult Delete([Bind(Include = nameof(CompaniaTelefonica.Id))] CompaniaTelefonica CompaniaTelefonica)
        {
            var errorMessage = companiasTelefonicaService.ValidateBeforeDelete(CompaniaTelefonica.Id);

            if (string.IsNullOrEmpty(errorMessage))
            {
                if (companiasTelefonicaService.Delete(CompaniaTelefonica.Id))
                    return Json(new RequestResult(SystemMessage.DeleteSuccessfull), JsonRequestBehavior.AllowGet);
                else
                    return Json(new RequestResult(SystemMessage.ServerError, false), JsonRequestBehavior.AllowGet);
            }
            else
                return Json(new RequestResult(errorMessage, false), JsonRequestBehavior.AllowGet);
        }
    }
}