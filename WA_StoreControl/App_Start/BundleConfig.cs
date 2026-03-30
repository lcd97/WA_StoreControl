using System.Web;
using System.Web.Optimization;

namespace WA_StoreControl
{
    public class BundleConfig
    {
        // Para obtener más información sobre las uniones, visite https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Utilice la versión de desarrollo de Modernizr para desarrollar y obtener información sobre los formularios.  De esta manera estará
            // para la producción, use la herramienta de compilación disponible en https://modernizr.com para seleccionar solo las pruebas que necesite.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new Bundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));

            // ===================== CSS =====================
            bundles.Add(new StyleBundle("~/Content/ProyectStyle").Include(
                "~/Content/bootstrap.css",
                "~/Content/Plantilla/font-awesome.css",
                "~/Content/Plantilla/custom.css"
            ));

            bundles.Add(new StyleBundle("~/Content/Externos").Include(
                "~/Content/Externos/bootstrap-sweetalert/sweetalert2.css",
                "~/Content/Externos/select2/select2.min.css",
                "~/Content/Externos/Select2/select2-bootstrap5.css",
                "~/Content/Externos/toastr/toastr.min.css",
                "~/Content/Externos/bootstrap-datatable/jquery.dataTables.min.css",
                "~/Content/Externos/smartwizard/smart_wizard_all.min.css",
                "~/Content/Externos/FontAwesome/css/all.css",
                "~/Scripts/Externos/datepicker/jquery-ui.css"
            ));


            // ===================== JS =====================

            // PROYECTO
            bundles.Add(new Bundle("~/bundles/Proyecto").Include(
                "~/Scripts/jquery-3.7.1.js",
                "~/Scripts/jquery.validate.js",
                "~/Scripts/jquery.validate.unobtrusive.js",
                "~/Scripts/Externos/jquery/jquery.validate.custom.js",
                "~/Scripts/knockout/knockout-3.5.1.js",
                "~/Scripts/knockout-mapping/knockout.mapping.js",
                "~/Scripts/ComponentesKO/Shared/Extensions/knockout.custom.js",
                "~/Scripts/Plantilla/bootstrap.bundle.min.js",
                "~/Scripts/Plantilla/custom.js"
            ));

            // EXTERNOS
            bundles.Add(new ScriptBundle("~/bundles/Externos").Include(
                "~/Scripts/Externos/datepicker/jquery-ui.js",
                "~/Scripts/Externos/datepicker-español/datepicker-es.js",
                "~/Scripts/Externos/jquery-mask/jquery.mask.js",
                "~/Scripts/Externos/bootstrap-sweetalert/sweetalert2.js",
                "~/Scripts/Externos/Select2/select2.full.js",
                "~/Scripts/Externos/select2/i18n/es.js",
                "~/Scripts/Externos/toastr/toastr.min.js",
                "~/Scripts/Externos/bootstrap/js/bootstrap-notify.js",
                "~/Scripts/Externos/jquery/jquery.blockUI.js",
                "~/Scripts/Externos/smartwizard/jquery.smartWizard.min.js"
            ));

            // COMPONENTES
            bundles.Add(new ScriptBundle("~/bundles/Componentes").Include(
                "~/Scripts/ComponentesKO/Shared/KnockoutComponents/GridVM.js",
                "~/Scripts/ComponentesKO/Shared/KnockoutComponents/SuperGrid_comp.js",
                "~/Scripts/ComponentesKO/Shared/AppGlobal.js",
                "~/Scripts/ComponentesKO/Shared/AjaxCallService.js",
                "~/Scripts/ComponentesKO/Shared/EnlacesKoCreados.js",
                "~/Scripts/ComponentesKO/Shared/ValidacionesClienteCreadas.js",
                "~/Scripts/ComponentesKO/Shared/CRUDViewModel.js",
                "~/Scripts/ComponentesKO/Shared/Knockout/SearchViewModel.js",
                "~/Scripts/ComponentesKO/Shared/Knockout/PaginationViewModel.js",
                "~/Scripts/ComponentesKO/Shared/KnockoutComponents/Modal.js",
                "~/Scripts/ComponentesKO/Shared/KnockoutComponents/LoadingSpinner.js",
                "~/Scripts/ComponentesKO/Shared/KnockoutComponents/TerceroFinder.js"
            ));

            #region CATALOGOS
            bundles.Add(new Bundle("~/bundles/Categoria").Include(
                "~/Scripts/Catalogos/Categorias/CategoriaVM.js",
                "~/Scripts/Catalogos/Categorias/SearchCategoriaVM.js",
                "~/Scripts/Catalogos/Categorias/IndexCategoriaVM.js"
            ));

            bundles.Add(new Bundle("~/bundles/SubCategoria").Include(
                "~/Scripts/Catalogos/SubCategorias/SubCategoriaVM.js",
                "~/Scripts/Catalogos/Categorias/CategoriaVM.js",
                "~/Scripts/Catalogos/SubCategorias/SearchSubCategoriaVM.js",
                "~/Scripts/Catalogos/SubCategorias/IndexSubCategoriaVM.js"
            ));
            #endregion
        }
    }
}
