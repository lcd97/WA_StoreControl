class IndexProductoVentaVM {
    constructor(data) {
        data = data || {};
        const self = this;

        //#region PROPIEDADES PRINCIPALES
        self.ProductosVenta = ko.observableArray(data.ProductosVenta ? data.ProductosVenta.map(x => new ProductoVentaVM(x)) : []);
        self.ProductoVenta = ko.observable(new ProductoVentaVM());
        self.PeticionEnCurso = ko.observable(null);

        self.LoadingRegistros = ko.observable(true);

        self.bodyTemplate = ko.observable({});
        self.SearchViewModel = ko.observable(new SearchProductoVentaVM({ ...data.SearchProductosVentaVM, RecordsPerPage: 10 } || {}));

        self.PaginationViewModel = ko.observable(new PaginationViewModel({
            TotalPages: self.SearchViewModel().TotalPages,
            CurrentPage: self.SearchViewModel().Page,
            TotalDisplayedPages: 5,
            OnCurrentPageChange: GetFilteredOrPaged
        }));

        self.ModalViewModel = ko.observable(new ModalViewModel({
            ComponentOptions: { backdrop: "static" },
            ModalHeaderViewModel: new ModalHeaderViewModel(),
            ModalBodyViewModel: new ModalBodyViewModel(),
            ModalSizeClass: "modal-xl"
        }));

        self.StepNumber = ko.observable();

        self.SmartwizardOptions = AppGlobal.SmartWizardOptions({
            StepSaveButton: 2,
            StepSaveButtonCallBack: 'SaveData'
        });

        self.ProductosConfig = (item) => {
            return {
                url: "/ProductosVenta/BusquedaProductoEnStock",
                queryParam: 'producto',
                recordKey: 'Record',
                textFields: ['DescripcionProducto'],
                placeholder: 'Buscar producto...',
                value: item.ProductoId,
                selectedItem: item.ProductoId() > 0
                    ? {
                        id: item.ProductoId(),
                        text: item.DescripcionProducto()
                    }
                    : null
            };
        };
        //#endregion

        //#region FUNCIONES PUBLICAS
        self.GetFilteredOrPaged = () => {
            GetFilteredOrPaged();
        };

        self.CleanFilter = () => {
            self.SearchViewModel().Descripcion("");

            self.GetFilteredOrPaged();
        };

        self.AgregarProducto = () => {
            self.ProductoVenta().DetallesProductoVenta.push(new DetalleProductoVenta());
        };

        self.DeleteProducto = (producto) => {
            self.ProductoVenta().DetallesProductoVenta.remove(producto);
        };

        self.DetallesValidos = () => {
            let detallesValidos = true;

            self.ProductoVenta().DetallesProductoVenta().forEach(element => {
                element.MostrarError(true);
                detallesValidos = !element.EsInvalido();
            });

            return detallesValidos;
        }

        self.ShowModal = function (data, action) {
            self.ProductoVenta(new ProductoVentaVM(ko.toJS(data || {})));

            self.bodyTemplate(new CRUDViewModel({
                Action: action,
                DataViewModel: self.ProductoVenta,
                ModelName: "Productos de Venta"
            }));

            self.ModalViewModel().ModalHeaderViewModel().ModalTitle(self.bodyTemplate().ModalHeaderTitle()).BackgroundColorClass(self.bodyTemplate().ModalBackgroundColorClass());
            self.ModalViewModel().ModalBodyViewModel().ModalBodyTemplate({
                name: "CRUD-ProductoVenta-Template",
                data: self.bodyTemplate(),
                afterRender: AppGlobal.ParseDynamicContent
            });

            $('#smartwizard').smartWizard("goToStep", 0);
            self.ModalViewModel().BootstrapInstance().show();
        };

        self.SaveData = function () {

            const form = document.getElementById("formCRUD");

            let ProductoVenta = ko.toJS(self.ProductoVenta) || {};
            $.validator.unobtrusive.parse($(formCRUD));

            if (self.ProductoVenta().DetallesProductoVenta().length <= 0)
                AppGlobal.validateMessage("error", "No ha agregado registros de productos.", "Reintentar");
            else if (!self.DetallesValidos()) {
                AppGlobal.validateMessage("warning", "Existen registros de productos vacios, elimine o corrija antes de guardar", "Reintentar");
            } else if ($(formCRUD).valid()) {
                SaveData(ProductoVenta);
            }
        };
        //#endregion

        //#region FUNCIONES PRIVADAS
        function GetFilteredOrPaged() {
            let url = "ProductosVenta/GetFilteredOrPaged/";

            var successCallBack = (response) => {
                if (response.Success) {
                    self.PaginationViewModel().TriggerOnCurrentPageChange(false);
                    self.SearchViewModel().TotalRecords(response.TotalRecords).TotalPages(response.TotalPages).Page(response.Page);
                    self.ProductosVenta(response.Records ? response.Records.map(x => new ProductoVentaVM(x)) : []);
                }
            }

            var errorCallBack = (jqXHR, statusText) => {
                if (statusText !== "abort")
                    AppGlobal.Messages.ShowNotifyError();
            }

            var beforeSendCallBack = (jqXHR) => {
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
                url: "ProductosVenta/GetFilteredOrPaged",
                data: ko.toJS(self.SearchViewModel),
                method: "GET",
                beforeSend: beforeSendCallBack,
                complete: completeCallBack,
            }).done(successCallBack).fail(errorCallBack);
        }

        function SaveData(ProductoVenta) {
            let url = "ProductosVenta/" + self.bodyTemplate().Action();
            let token = $('input[name="__RequestVerificationToken"]').val();

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

            var errorCallBack = (jqXHR, statusText) => {
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
                data: { ProductoVenta },
                method: "POST",
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

    let root = new IndexProductoVentaVM(dataRoot);

    ko.applyBindings(root);
    root.GetFilteredOrPaged();
});