using AutoMapper;
using ModelosDB.General;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WA_StoreControl.DTO;
using WA_StoreControl.Models;
using WA_StoreControl.Services;
using WA_StoreControl.Utilidades;
using WA_StoreControl.ViewModels;

namespace WA_StoreControl.Controllers
{
    public class TiposIdentificacionController : Controller
    {
        private DBStore db;
        private TiposIdentificacionService tiposIdentificacionService;

        public TiposIdentificacionController()
        {
            this.db = new DBStore();
            this.tiposIdentificacionService = new TiposIdentificacionService(db);
        }

        // GET: TiposIdentificacion
        public ActionResult Index()
        {
            var indexTiposIdentificacionVM = new IndexTiposIdentificacionVM();

            ViewBag.JsonData = JsonConvert.SerializeObject(indexTiposIdentificacionVM);

            return View(indexTiposIdentificacionVM);
        }

        [HttpGet]
        public JsonResult GetFilteredOrPaged(SearchTiposIdentificacionVM viewModel)
        {
            var records = tiposIdentificacionService.GetFilteredOrPaged(viewModel);
            var recordsMapped = Mapper.Map<ICollection<TipoIdentificacionDTO>>(records.ToList());

            var RequestPagedResult = new RequestPagedResult<TipoIdentificacionDTO>(viewModel.TotalRecords, viewModel.TotalPages, viewModel.Page, recordsMapped);

            return Json(RequestPagedResult, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create(TipoIdentificacion tipoIdentificacion)
        {
            var errorMessage = !ModelState.IsValid ? string.Join(" | ", ModelValidate.GetModelErrorMessages(ModelState)) : tiposIdentificacionService.ValidateBeforeCreate(tipoIdentificacion);

            if (string.IsNullOrEmpty(errorMessage))
            {
                if (tiposIdentificacionService.Create(tipoIdentificacion))
                    return Json(new RequestResult(SystemMessage.CreateSuccessful), JsonRequestBehavior.AllowGet);
                else
                    return Json(new RequestResult(SystemMessage.ServerError, false), JsonRequestBehavior.AllowGet);
            }
            else
                return Json(new RequestResult(errorMessage, false), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public JsonResult Edit(TipoIdentificacion tipoIdentificacion)
        {
            var errorMessage = !ModelState.IsValid ? string.Join(" | ", ModelValidate.GetModelErrorMessages(ModelState)) : tiposIdentificacionService.ValidateBeforeUpdate(tipoIdentificacion);
            if (string.IsNullOrEmpty(errorMessage))
            {
                if (string.IsNullOrEmpty(errorMessage))
                {
                    tiposIdentificacionService.Update(tipoIdentificacion);
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
        public JsonResult Delete([Bind(Include = nameof(tipoIdentificacion.Id))] TipoIdentificacion tipoIdentificacion)
        {
            var errorMessage = tiposIdentificacionService.ValidateBeforeDelete(tipoIdentificacion.Id);

            if (string.IsNullOrEmpty(errorMessage))
            {
                if (tiposIdentificacionService.Delete(tipoIdentificacion.Id))
                    return Json(new RequestResult(SystemMessage.DeleteSuccessfull), JsonRequestBehavior.AllowGet);
                else
                    return Json(new RequestResult(SystemMessage.ServerError, false), JsonRequestBehavior.AllowGet);
            }
            else
                return Json(new RequestResult(errorMessage, false), JsonRequestBehavior.AllowGet);
        }
    }
}