class IndexEntradaVM {
    constructor(data) {
        data = data || {};
        const self = this;

        //#region PROPIEDADES PRINCIPALES
        self.Entradas = ko.observableArray(data.Entradas ? data.Entradas.map(x => new EntradaVM(x)) : []);
        self.PeticionEnCurso = ko.observable(null);
        self.LoadingRegistros = ko.observable(false);

        self.SearchViewModel = ko.observable(new SearchEntradaVM({ ...data.SearchEntradasVM, RecordsPerPage: 10 } || {}));

        self.PaginationViewModel = ko.observable(new PaginationViewModel({
            TotalPages: self.SearchViewModel().TotalPages,
            CurrentPage: self.SearchViewModel().Page,
            TotalDisplayedPages: 5,
            OnCurrentPageChange: GetFilteredOrPaged
        }));
        //#endregion

        //#region FUNCIONES PUBLICAS
        self.GetFilteredOrPaged = () => {
            GetFilteredOrPaged();
        };

        self.CleanFilter = () => {
            self.SearchViewModel()
                .FechaDesde("")
                .FechaHasta("");

            self.GetFilteredOrPaged();
        };

        self.CrearEntrada = () => {
            window.open("/Entradas/CrearEntrada", "_self");
        };

        self.AnularEntrada = (entrada) => {
            AppGlobal.AjaxMessage("warning", "¿Deseas anular este registro?\nUna vez anulado no se tomará en cuenta en el stock de productos")
                .then((result) => {
                    if (result.isConfirmed)
                        AnularEntrada(entrada);
                });
        };

        self.ClonarEntrada = (Entrada) => {
            window.open(`/Entradas/CrearEntrada?EntradaId=${Entrada.Id()}`, "_self");
        };

        self.DetalleEntrada = (Entrada) => {
            window.open(`/Entradas/DetalleEntrada?EntradaId=${Entrada.Id()}`, "_self");
        };
        //#endregion

        //#region FUNCIONES PRIVADAS
        function GetFilteredOrPaged() {
            var successCallBack = (response) => {
                if (response.Success)
                    self.Entradas(response.Records ? response.Records.map(x => new EntradaVM(x)) : []);
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
                url: "Entradas/GetFilteredOrPaged",
                data: ko.toJS(self.SearchViewModel),
                method: "GET",
                beforeSend: beforeSendCallBack,
                complete: completeCallBack,
            }).done(successCallBack).fail(errorCallBack);
        }

        function AnularEntrada(Entrada) {
            var successCallBack = (response) => {
                if (response.Success) {
                    GetFilteredOrPaged();
                    AppGlobal.validateMessage("success", response.Message);
                }
                else
                    AppGlobal.validateMessage("error", response.Message);
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

            Ajax.CRUD({
                url: "Entradas/AnularEntrada",
                data: { Entrada },
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

    let root = new IndexEntradaVM(dataRoot);
    ko.applyBindings(root);
    root.GetFilteredOrPaged();
});