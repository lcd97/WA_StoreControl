using AutoMapper;
using ModelosDB.General;
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
    public class PersonasController : Controller
    {
        private DBStore db = new DBStore();
        private PersonasService personasService;
        private TiposIdentificacionService tiposIdentificacionService;
        private CompaniasTelefonicaService companiasTelefonicaService;

        public PersonasController()
        {
            this.db = new DBStore();
            this.personasService = new PersonasService(db);
            this.tiposIdentificacionService = new TiposIdentificacionService(db);
            this.companiasTelefonicaService = new CompaniasTelefonicaService(db);
        }

        // GET: Persona
        public ActionResult Index()
        {
            var indexPersonasVM = new IndexPersonasVM();
            indexPersonasVM.CompaniasTelefonica = Mapper.Map<ICollection<CompaniaTelefonicaDTO>>(companiasTelefonicaService.GetAll());
            indexPersonasVM.TiposIdentificacion = Mapper.Map<ICollection<TipoIdentificacionDTO>>(tiposIdentificacionService.GetAll());

            ViewBag.JsonData = JsonConvert.SerializeObject(indexPersonasVM);

            return View(indexPersonasVM);
        }

        [HttpGet]
        public JsonResult GetFilteredOrPaged(SearchPersonasVM viewModel)
        {
            var records = personasService.GetFilteredOrPaged(viewModel);
            var recordsMapped = Mapper.Map<ICollection<PersonaDTO>>(records.ToList());

            var RequestPagedResult = new RequestPagedResult<PersonaDTO>(viewModel.TotalRecords, viewModel.TotalPages, viewModel.Page, recordsMapped);

            return Json(RequestPagedResult, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create(Persona Persona)
        {
            var errorMessage = !ModelState.IsValid ? string.Join(" | ", ModelValidate.GetModelErrorMessages(ModelState)) : personasService.ValidateBeforeCreate(Persona);

            if (string.IsNullOrEmpty(errorMessage))
            {
                if (personasService.Create(Persona, out errorMessage))
                    return Json(new RequestResult(SystemMessage.CreateSuccessful), JsonRequestBehavior.AllowGet);
                else
                    return Json(new RequestResult(errorMessage, false), JsonRequestBehavior.AllowGet);
            }
            else
                return Json(new RequestResult(errorMessage, false), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public JsonResult Edit(Persona Persona)
        {
            var errorMessage = !ModelState.IsValid ? string.Join(" | ", ModelValidate.GetModelErrorMessages(ModelState)) : personasService.ValidateBeforeUpdate(Persona);
            if (string.IsNullOrEmpty(errorMessage))
            {
                if (string.IsNullOrEmpty(errorMessage))
                {
                    personasService.Update(Persona, out errorMessage);
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
        public JsonResult Delete(Persona Persona)
        {
            var errorMessage = personasService.ValidateBeforeDelete(Persona.Id);

            if (string.IsNullOrEmpty(errorMessage))
            {
                if (personasService.Delete(Persona.Id))
                    return Json(new RequestResult(SystemMessage.DeleteSuccessfull), JsonRequestBehavior.AllowGet);
                else
                    return Json(new RequestResult(SystemMessage.ServerError, false), JsonRequestBehavior.AllowGet);
            }
            else
                return Json(new RequestResult(errorMessage, false), JsonRequestBehavior.AllowGet);
        }

        public JsonResult PuedeAgregarIdentidad(string Identificacion, int TipoIdentificacionId, int PersonaId = 0, int Id = 0)
        {
            return Json(new RequestResult("", personasService.PuedeAgregarIdentidad(Identificacion, TipoIdentificacionId, PersonaId, Id)), JsonRequestBehavior.AllowGet);
        }
    }
}