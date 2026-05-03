using AutoMapper;
using ModelosDB.Inventario;
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
    public class SubCategoriasController : Controller
    {
        private DBStore db;
        private SubCategoriasService subCategoriaService;
        private CategoriasService categoriaService;

        public SubCategoriasController()
        {
            this.db = new DBStore();
            this.subCategoriaService = new SubCategoriasService(db);
            this.categoriaService = new CategoriasService(db);
        }

        // GET: SubCategorias
        public ActionResult Index()
        {
            var subCategoriaVM = new IndexSubCategoriasVM();
            subCategoriaVM.Categorias = Mapper.Map<ICollection<CategoriaDTO>>(categoriaService.GetAll().ToList());

            ViewBag.JsonData = JsonConvert.SerializeObject(subCategoriaVM);

            return View(subCategoriaVM);
        }

        [HttpGet]
        public JsonResult GetFilteredOrPaged(SearchSubCategoriasVM viewModel)
        {
            var records = subCategoriaService.GetFilteredOrPaged(viewModel);
            var recordsMapped = Mapper.Map<ICollection<SubCategoriaDTO>>(records.ToList());

            var RequestPagedResult = new RequestPagedResult<SubCategoriaDTO>(viewModel.TotalRecords, viewModel.TotalPages, viewModel.Page, recordsMapped);

            return Json(RequestPagedResult, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create(SubCategoria SubCategoria)
        {
            var errorMessage = !ModelState.IsValid ? string.Join(" | ", ModelValidate.GetModelErrorMessages(ModelState)) : subCategoriaService.ValidateBeforeCreate(SubCategoria);

            if (string.IsNullOrEmpty(errorMessage))
            {
                if (subCategoriaService.Create(SubCategoria))
                    return Json(new RequestResult(SystemMessage.CreateSuccessful), JsonRequestBehavior.AllowGet);
                else
                    return Json(new RequestResult(SystemMessage.ServerError, false), JsonRequestBehavior.AllowGet);
            }
            else
                return Json(new RequestResult(errorMessage, false), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public JsonResult Edit(SubCategoria SubCategoria)
        {
            var errorMessage = !ModelState.IsValid ? string.Join(" | ", ModelValidate.GetModelErrorMessages(ModelState)) : subCategoriaService.ValidateBeforeUpdate(SubCategoria);
            if (string.IsNullOrEmpty(errorMessage))
            {
                if (string.IsNullOrEmpty(errorMessage))
                {
                    subCategoriaService.Update(SubCategoria);
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
        public JsonResult Delete([Bind(Include = nameof(SubCategoria.Id))] SubCategoria SubCategoria)
        {
            var errorMessage = subCategoriaService.ValidateBeforeDelete(SubCategoria.Id);

            if (string.IsNullOrEmpty(errorMessage))
            {
                if (subCategoriaService.Delete(SubCategoria.Id))
                    return Json(new RequestResult(SystemMessage.DeleteSuccessfull), JsonRequestBehavior.AllowGet);
                else
                    return Json(new RequestResult(SystemMessage.ServerError, false), JsonRequestBehavior.AllowGet);
            }
            else
                return Json(new RequestResult(errorMessage, false), JsonRequestBehavior.AllowGet);
        }
    }
}