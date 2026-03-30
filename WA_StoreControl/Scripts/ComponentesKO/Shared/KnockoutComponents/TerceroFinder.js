//Registrar custom binding para inicializar el componente de modal bootstrap...
ko.bindingHandlers.bsModalTerceroFinder = {
    init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
        let options = ko.unwrap(valueAccessor());
        let modal = new bootstrap.Modal(element, options); //Crear la instancia del modal...
        bindingContext.$data.ModalBootstrapInstance(modal); //Actualizar la instancia del viewModel a la actual instancia creada...

        element.addEventListener('hidden.bs.modal', function (event) {
            bindingContext.$component().ClearSearchFilters();
            bindingContext.$data.SetSearchType(0);
        })
    }
};

class SearchTerceroViewModel extends SearchViewModel {
    constructor(data) {
        super(data);
        data = data || {};
        const self = this;

        self.Nombres = ko.observable(data.Nombres || "");
        self.Apellidos = ko.observable(data.Apellidos || "");
        self.Identidad = ko.observable(data.Identidad || "");
        self.SoloEmpleados = ko.observable(data.SoloEmpleados || false);
        self.FiltrarDominioUnidad = ko.observable(typeof (data.FiltrarDominioUnidad) === "boolean" ? true : false);
    }
}

class TerceroFinderViewModel {
    constructor(data) {
        data = data || {};
        const self = this;

        if (!data.add)
            throw new Error('La función callback "add" no se ha especificado.');

        self.TerceroFinderId = ko.observable(data.TerceroFinderId || "defaultTerceroFinderId");
        self.ModalComponentOptions = ko.observable(data.ModalComponentOptions || { backdrop: "static", });
        self.ModalBootstrapInstance = ko.observable();
        self.ModalTitle = ko.observable(data.ModalTitle || "Buscador de Terceros");
        self.ModalSizeClass = ko.observable(data.ModalSizeClass)
        self.ModalBackgroundColorClass = ko.observable(data.ModalBackgroundColorClass || "bg-primary text-white");
        self.ModalHeaderClass = ko.computed(function () {
            return "modal-header " + self.ModalBackgroundColorClass();
        });

        self.EmpleadoInfo = ko.observable("");

        self.ModalHeaderIconClass = ko.observable(data.ModalHeaderIconClass || "fa fa-search");
        self.SearchTabsId = ko.observable(data.SearchTabsId || "FinderSearchTabs");
        self.SearchTabQuickId = ko.observable(data.SearchTabQuickId || "SearchTabQuick");//Para el tab de busqueda rapida
        self.SearchTabAdvancedId = ko.observable(data.SearchTabAdvancedId || "SearchTabAdvanced");//Para el tab de busqueda avanzada
        self.SearchTabsContentId = ko.observable(data.SearchTabsContentId || "FinderSearchPanes");//Para el tab principal donde se renderiza el contenido
        self.SearchQuickTabContentId = ko.observable(data.SearchQuickTabContentId + "-" + self.TerceroFinderId() || "FinderQuickSearchTabContent" + "-" + self.TerceroFinderId());//Para la renderizacion del contenido de busqueda rapida
        self.SearchAdvancedTabContentId = ko.observable(data.SearchAdvancedTabContentId + "-" + self.TerceroFinderId() || "FinderAdvancedSearchTabContent" + "-" + self.TerceroFinderId()); // para la renderizacion del contenido de busqueda avanzada
        self.SearchViewModel = ko.observable(data.SearchViewModel || new SearchTerceroViewModel());
        self.PaginationViewModel = ko.observable(new PaginationViewModel({
            TotalPages: self.SearchViewModel().TotalPages,
            CurrentPage: self.SearchViewModel().Page,
            TotalDisplayedPages: 5,
            OnCurrentPageChange: GetFilteredOrPaged
        }));

        self.LoadingAjaxCurrentRequest = ko.observable(null);
        self.LoadingRegistros = ko.observable(false);
        self.terceros = ko.observableArray(data.terceros || []);

        self.add = data.add;

        //#region Funciones del viewmodel
        self.GetFilteredOrPaged = () => {
            if (self.SearchViewModel().Nombres().trim() != ""
                || self.SearchViewModel().Apellidos().trim() != ""
                || self.SearchViewModel().Identidad().trim() != ""
                || self.SearchViewModel().SearchString().trim() != ""
            ) {
                self.PaginationViewModel().TriggerOnCurrentPageChange(false);
                self.SearchViewModel().Page(1);
                GetFilteredOrPaged();
            } else
                AppGlobal.ShowNotification({
                    element: "body",
                    message: "Ingrese una de las opciones del filtro",
                    type: "danger",
                    icon: "fas fa-exclamation-triangle",
                    animateEnter: "animated bounceIn",
                    animateExit: "animated bounceOut"
                })
        };

        self.SetSearchType = (searchType) => {
            if (typeof (searchType) === 'number')
                self.SearchViewModel().SearchType(searchType);
        };

        self.addTercero = (item, event) => {
            self.add(item);
            self.ModalBootstrapInstance().hide();
        };

        self.ClearSearchFilters = () => {
            self.SearchViewModel()
                .SearchString("")
                .Nombres("")
                .Apellidos("")
                .Identidad("")

            if (!(self.SearchViewModel().SoloEmpleados()))
                self.SearchViewModel().SoloEmpleados(false);

            self.terceros([]);//Limpiar los items de cuentas encontrados en busquedas anteriores
        };

        self.ClearSearchThenFilter = () => {
            self.ClearSearchFilters();
            self.GetFilteredOrPaged();
        };

        self.ShowEmpleadoFinderCRUD = () => {
            self.ModalBootstrapInstance().show();
        }

        self.GetEmpleado = (FilterCodigoDeEmpleado) => {
            GetEmpleado(FilterCodigoDeEmpleado);
        }
        //#endregion

        //#region Funciones js
        function GetFilteredOrPaged() {
            var beforeSend = (jqXHR) => {  // Petición en curso
                if (self.LoadingAjaxCurrentRequest()) //verifica si hay otra petición para abortar
                    self.LoadingAjaxCurrentRequest().abort();

                self.LoadingAjaxCurrentRequest(jqXHR);
                self.LoadingRegistros(true);
            };

            var error = () => { // Error de la peticion
                AppGlobal.ShowNotification({
                    element: "body",
                    message: AppGlobal.Messages.AjaxRequestError,
                    type: "danger",
                    icon: "fas fa-exclamation-triangle",
                    animateEnter: "animated bounceIn",
                    animateExit: "animated bounceOut"
                });
            };

            var success = (response) => { // Respuesta de la petición
                if (response.Success) {
                    self.terceros(response.Records || []);

                    ////Se debe evitar que actualizar la pagina actua con la pagina en la response del servidor
                    ////Dispare nuevamente esta peticion y genere un ciclo infinito...
                    self.PaginationViewModel().TriggerOnCurrentPageChange(false);
                    self.SearchViewModel().TotalRecords(response.TotalRecords).TotalPages(response.TotalPages).Page(response.Page);
                }
                else {
                    AppGlobal.ShowNotification({
                        element: "body",
                        message: response.Message,
                        type: "danger",
                        icon: "fas fa-exclamation-triangle",
                        animateEnter: "animated bounceIn",
                        animateExit: "animated bounceOut"
                    });
                }
            };

            AppGlobal.Ajax.GetFilteredOrPaged({
                url: "FiltroEmpleado/GetFilteredOrPaged",
                data: ko.toJS(self.SearchViewModel),
                beforeSend: beforeSend,
                complete: () => { self.LoadingRegistros(false); self.LoadingAjaxCurrentRequest(null); },
            }).done(success).fail(error);
        }

        function GetEmpleado(FilterCodigoDeEmpleado) {
            var beforeSend = (jqXHR) => {  // Peticion en curso
                if (self.LoadingAjaxCurrentRequest()) //verifica si hay otra peticion para abortar
                    self.LoadingAjaxCurrentRequest().abort();

                self.LoadingAjaxCurrentRequest(jqXHR);
                self.LoadingRegistros(true);
            };

            var success = (res) => { // Respuesta de la peticion
                if (res.Success) {

                    if (res.Record) {
                        //self.SubsidioVM().EmpleadoId(res.Record.Id); // Asigna el ID del empleado buscado
                        self.EmpleadoInfo(res.Record.NombreYApellido);
                        return res.Record.Id; // Asigna el ID del empleado buscado
                    }
                    else {
                        //self.SubsidioVM().EmpleadoId(0);                        
                        self.EmpleadoInfo("No se encontró ningún empleado con el código ingresado");
                        return 0;
                    }
                }
                else
                    AppGlobal.Messages.ShowNotifyError(res.Message);
            };

            var error = () => (jqXHR, statusText) => {
                if (statusText !== "abort")
                    AppGlobal.Messages.ShowNotifyError();
            };

            AppGlobal.Ajax.Get({
                url: "FiltroEmpleado/GetEmpleado",
                data: { codigoEmpleado: ko.toJS(FilterCodigoDeEmpleado) },//Asignacion de parametros
                beforeSend: beforeSend,
                complete: () => { self.LoadingAjaxCurrentRequest(false); self.LoadingAjaxCurrentRequest(null); },
            }).done(success).fail(error);
        }
        //#endregion
    }
}


ko.components.register("ko-tercero-finder", {
    template: { element: "tercero-finder-template" },
    viewModel: function (params) {
        return params.viewModel;
    }
});