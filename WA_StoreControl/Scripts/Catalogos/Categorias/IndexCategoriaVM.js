class IndexCategoriaVM {
    constructor(data) {
        data = data || {};
        const self = this;

        //#region PROPIEDADES PRINCIPALES
        self.Categorias = ko.observableArray(data.Categorias ? data.Categorias.map(x => new CategoriaVM(x)) : []);
        self.Categoria = ko.observable(new CategoriaVM());//OBTIENE UNA CATEGORIA DE LA TABLA
        self.PeticionEnCurso = ko.observable(null);//CANCELAR MULTIPLES PETICIONES

        self.LoadingRegistros = ko.observable(false);

        self.Action = ko.observable("");//CAPTURAR LA ACTION
        self.bodyTemplate = ko.observable({}); //Data De Modal
        self.SearchViewModel = ko.observable(new SearchCategoriaVM({ ...data.SearchCategoriasVM, RecordsPerPage: 10 } || {})); // Propiedades de configuracion para la paginacion

        self.PaginationViewModel = ko.observable(new PaginationViewModel({ //Instancia del ViewModel de paginacion...
            TotalPages: self.SearchViewModel().TotalPages, //EL total de paginas se envia como referencia para controlarlo desde este nivel...
            CurrentPage: self.SearchViewModel().Page, //Igualmente la pagina actual se pasa como referencia...
            TotalDisplayedPages: 5, //Numero de links de paginas mostrados a la vez...
            OnCurrentPageChange: GetFilteredOrPaged //Callback al paginar...
        }));

        //Viemodels De Modal
        self.ModalViewModel = ko.observable(new ModalViewModel({ //ViewModel para el Componente modal...
            ComponentOptions: { backdrop: "static" },
            ModalHeaderViewModel: new ModalHeaderViewModel(),
            ModalBodyViewModel: new ModalBodyViewModel()
        }));
        //#endregion

        //#region FUNCIONES PUBLICAS
        self.GetFilteredOrPaged = () => {
            GetFilteredOrPaged();
        };

        self.CleanFilter = () => {
            self.SearchViewModel().Descripcion("");

            self.GetFilteredOrPaged();
        };

        self.ShowModal = function (data, action) {

            self.Categoria(new CategoriaVM(ko.toJS(data || {})));

            self.bodyTemplate(new CRUDViewModel({
                Action: action,
                DataViewModel: self.Categoria,
                ModelName: "Categoria"
            }));

            self.ModalViewModel().ModalHeaderViewModel().ModalTitle(self.bodyTemplate().ModalHeaderTitle()).BackgroundColorClass(self.bodyTemplate().ModalBackgroundColorClass());
            self.ModalViewModel().ModalBodyViewModel().ModalBodyTemplate({
                name: "CRUD-Categoria-Template",
                data: self.bodyTemplate(),
                afterRender: AppGlobal.ParseDynamicContent
            });
            self.ModalViewModel().BootstrapInstance().show();
        };

        self.SaveData = function (formCRUD, data) { // Funcion Para El CRUD
            let Categoria = ko.toJS(data) || {};
            let url = "Categorias/" + self.bodyTemplate().Action(); //url de accion a realizar
            let token = $('input[name="__RequestVerificationToken"]').val(); //Token 
            $.validator.unobtrusive.parse($(formCRUD)); // Reaplica las validaciones

            if ($(formCRUD).valid()) { //Validar datos del formulario
                var beforeSendCallBack = (jqXHR) => {
                    if (self.PeticionEnCurso()) //verifica si hay otra peticion para abortar
                        self.PeticionEnCurso().abort();

                    self.PeticionEnCurso(jqXHR);
                    self.bodyTemplate().ProcessingAction(true);
                    self.LoadingRegistros(true);
                }

                var successCallBack = (response) => {
                    if (response.Success) {
                        self.GetFilteredOrPaged();
                        self.ModalViewModel().BootstrapInstance().hide();
                        AppGlobal.Messages.ShowNotifyCorrect(response.Message);
                    } else
                        AppGlobal.Messages.ShowNotifyError(response.Message);
                }

                var errorCallBack = () => (jqXHR, statusText) => {
                    if (statusText !== "abort") {
                        AppGlobal.Messages.ShowNotifyError();
                    }
                };

                var completeCallBack = () => {
                    self.bodyTemplate().ProcessingAction(false);
                    self.PeticionEnCurso(null);
                    self.LoadingRegistros(false);
                }

                Ajax.CRUD({
                    url: url,
                    data: { Categoria },
                    method: "POST",
                    beforeSend: beforeSendCallBack,
                    complete: completeCallBack,
                }).done(successCallBack).fail(errorCallBack);
            }
        };
        //#endregion

        //#region FUNCIONES PRIVADAS
        function GetFilteredOrPaged() {
            let url = "Categorias/GetFilteredOrPaged/";

            var successCallBack = (response) => {
                if (response.Success)
                    self.Categorias(response.Records ? response.Records.map(x => new CategoriaVM(x)) : []);
            }

            var errorCallBack = (response) => (jqXHR, statusText) => {
                if (statusText !== "abort") {
                    Messages.TimerMessages({
                        icon: "error",
                        title: response.Message
                    });
                }
            }

            var beforeSendCallBack = () => (jqXHR) => {
                if (self.PeticionEnCurso()) //verifica si hay otra peticion para abortar
                    self.PeticionEnCurso().abort();

                self.PeticionEnCurso(jqXHR);
                self.LoadingRegistros(true);
            }

            var completeCallBack = () => {
                self.LoadingRegistros(false);
                self.PeticionEnCurso(null);
            }

            Ajax.GetFilteredOrPaged({
                url: "Categorias/GetFilteredOrPaged",
                data: ko.toJS(self.SearchViewModel),
                method: "GET",
                beforeSend: beforeSendCallBack,
                complete: completeCallBack,
            }).done(successCallBack).fail(errorCallBack);
        }
        //#endregion
    }
}

$(() => {
    var dataRoot = JSON.parse($("#JsonData").val());
    $("#JsonData").remove();

    let root = new IndexCategoriaVM(dataRoot);

    ko.applyBindings(root);
    root.GetFilteredOrPaged();
});