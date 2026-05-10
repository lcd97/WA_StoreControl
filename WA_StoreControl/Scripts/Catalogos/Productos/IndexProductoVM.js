class IndexProductoVM {
    constructor(data) {
        data = data || {};
        const self = this;

        //#region PROPIEDADES PRINCIPALES
        self.Productos = ko.observableArray(data.Productos ? data.Productos.map(x => new ProductoVM(x)) : []);
        self.Producto = ko.observable(new ProductoVM());
        self.PeticionEnCurso = ko.observable(null);

        self.Categorias = ko.observableArray(data.Categorias ? data.Categorias.map(x => new CategoriaVM(x)) : []);
        self.CategoriasYSub = ko.observableArray(data.CategoriasYSub ? data.CategoriasYSub.map(x => new SubCategoriaVM(x)) : []);

        self.SubCategorias = ko.observableArray([]);
        self.Marcas = ko.observableArray(data.Marcas ? data.Marcas.map(x => new MarcaVM(x)) : []);

        self.LoadingRegistros = ko.observable(false);

        self.Action = ko.observable("");
        self.bodyTemplate = ko.observable({});
        self.SearchViewModel = ko.observable(new SearchProductoVM({ ...data.SearchProductosVM, RecordsPerPage: 10 } || {}));

        self.PaginationViewModel = ko.observable(new PaginationViewModel({
            TotalPages: self.SearchViewModel().TotalPages,
            CurrentPage: self.SearchViewModel().Page,
            TotalDisplayedPages: 5,
            OnCurrentPageChange: GetFilteredOrPaged
        }));

        self.ModalViewModel = ko.observable(new ModalViewModel({
            ComponentOptions: { backdrop: "static" },
            ModalHeaderViewModel: new ModalHeaderViewModel(),
            ModalBodyViewModel: new ModalBodyViewModel()
        }));
        //#endregion

        //#region FUNCIONES PUBLICAS
        self.SearchViewModel().CategoriaId.subscribe(function (value) {
            if (value > 0)
                CargarSubCategoria(value);
            else
                self.SubCategorias([]);
        });

        self.GetFilteredOrPaged = () => {
            GetFilteredOrPaged();
        };

        self.CleanFilter = () => {
            self.SearchViewModel()
                .CategoriaId(0)
                .SubCategoriaId(0)
                .Descripcion("");

            self.GetFilteredOrPaged();
        };

        self.ShowModal = function (data, action) {
            self.Producto(new ProductoVM(ko.toJS(data || {})));

            self.bodyTemplate(new CRUDViewModel({
                Action: action,
                DataViewModel: self.Producto,
                ModelName: "Producto"
            }));

            self.ModalViewModel().ModalHeaderViewModel().ModalTitle(self.bodyTemplate().ModalHeaderTitle()).BackgroundColorClass(self.bodyTemplate().ModalBackgroundColorClass());
            self.ModalViewModel().ModalBodyViewModel().ModalBodyTemplate({
                name: "CRUD-Producto-Template",
                data: self.bodyTemplate(),
                afterRender: AppGlobal.ParseDynamicContent
            });
            self.ModalViewModel().BootstrapInstance().show();
        };

        self.SaveData = function (formCRUD, data) {
            let Producto = ko.toJS(data) || {};
            let url = "Productos/" + self.bodyTemplate().Action();
            let token = $('input[name="__RequestVerificationToken"]').val();
            $.validator.unobtrusive.parse($(formCRUD));

            if ($(formCRUD).valid()) {
                var beforeSendCallBack = (jqXHR) => {
                    if (self.PeticionEnCurso())
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
                    data: { Producto },
                    method: "POST",
                    beforeSend: beforeSendCallBack,
                    complete: completeCallBack,
                }).done(successCallBack).fail(errorCallBack);
            }
        };
        //#endregion

        //#region FUNCIONES PRIVADAS
        function GetFilteredOrPaged() {
            let url = "Productos/GetFilteredOrPaged/";

            var successCallBack = (response) => {
                if (response.Success)
                    self.Productos(response.Records ? response.Records.map(x => new ProductoVM(x)) : []);
            }

            var errorCallBack = (response) => (jqXHR, statusText) => {
                if (statusText !== "abort")
                    AppGlobal.Messages.ShowNotifyError();
            }

            var beforeSendCallBack = () => (jqXHR) => {
                if (self.PeticionEnCurso())
                    self.PeticionEnCurso().abort();

                self.PeticionEnCurso(jqXHR);
                self.LoadingRegistros(true);
            }

            var completeCallBack = () => {
                self.LoadingRegistros(false);
                self.PeticionEnCurso(null);
            }

            Ajax.GetFilteredOrPaged({
                url: "Productos/GetFilteredOrPaged",
                data: ko.toJS(self.SearchViewModel),
                method: "GET",
                beforeSend: beforeSendCallBack,
                complete: completeCallBack,
            }).done(successCallBack).fail(errorCallBack);
        }

        function CargarSubCategoria(CategoriaId) {
            var successCallBack = (response) => {
                if (response.Success)
                    self.SubCategorias(response.Record ? response.Record.map(x => new SubCategoriaVM(x)) : []);
            }

            var errorCallBack = (response) => (jqXHR, statusText) => {
                if (statusText !== "abort")
                    AppGlobal.Messages.ShowNotifyError();
            }

            var beforeSendCallBack = () => (jqXHR) => {
                if (self.PeticionEnCurso())
                    self.PeticionEnCurso().abort();

                self.PeticionEnCurso(jqXHR);
                self.LoadingRegistros(true);
            }

            var completeCallBack = () => {
                self.LoadingRegistros(false);
                self.PeticionEnCurso(null);
            }

            Ajax.GetFilteredOrPaged({
                url: "Productos/CargarSubCategoria",
                data: { CategoriaId },
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

    let root = new IndexProductoVM(dataRoot);

    ko.applyBindings(root);
    root.GetFilteredOrPaged();
});