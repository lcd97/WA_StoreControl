class IndexTipoIdentificacionVM {
    constructor(data) {
        data = data || {};
        const self = this;

        //#region PROPIEDADES PRINCIPALES
        self.TiposIdentificacion = ko.observableArray(data.TiposIdentificacion ? data.TiposIdentificacion.map(x => new TipoIdentificacionVM(x)) : []);
        self.TipoIdentificacion = ko.observable(new TipoIdentificacionVM());
        self.PeticionEnCurso = ko.observable(null);

        self.LoadingRegistros = ko.observable(false);

        self.Action = ko.observable("");
        self.bodyTemplate = ko.observable({});
        self.SearchViewModel = ko.observable(new SearchTipoIdentificacionVM({ ...data.SearchTiposIdentificacionVM, RecordsPerPage: 10 } || {}));

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
        self.GetFilteredOrPaged = () => {
            GetFilteredOrPaged();
        };

        self.CleanFilter = () => {
            self.SearchViewModel().Descripcion("");

            self.GetFilteredOrPaged();
        };

        self.ShowModal = function (data, action) {

            self.TipoIdentificacion(new TipoIdentificacionVM(ko.toJS(data || {})));

            self.bodyTemplate(new CRUDViewModel({
                Action: action,
                DataViewModel: self.TipoIdentificacion,
                ModelName: "Tipo Identificación"
            }));

            self.ModalViewModel().ModalHeaderViewModel().ModalTitle(self.bodyTemplate().ModalHeaderTitle()).BackgroundColorClass(self.bodyTemplate().ModalBackgroundColorClass());
            self.ModalViewModel().ModalBodyViewModel().ModalBodyTemplate({
                name: "CRUD-TipoIdentificacion-Template",
                data: self.bodyTemplate(),
                afterRender: AppGlobal.ParseDynamicContent
            });
            self.ModalViewModel().BootstrapInstance().show();
        };

        self.SaveData = function (formCRUD, data) {
            let TipoIdentificacion = ko.toJS(data) || {};
            let url = "TiposIdentificacion/" + self.bodyTemplate().Action();
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
                    data: { TipoIdentificacion },
                    method: "POST",
                    beforeSend: beforeSendCallBack,
                    complete: completeCallBack,
                }).done(successCallBack).fail(errorCallBack);
            }
        };
        //#endregion

        //#region FUNCIONES PRIVADAS
        function GetFilteredOrPaged() {
            let url = "TiposIdentificacion/GetFilteredOrPaged/";

            var successCallBack = (response) => {
                if (response.Success)
                    self.TiposIdentificacion(response.Records ? response.Records.map(x => new TipoIdentificacionVM(x)) : []);
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
                url: "TiposIdentificacion/GetFilteredOrPaged",
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

    let root = new IndexTipoIdentificacionVM(dataRoot);

    ko.applyBindings(root);
    root.GetFilteredOrPaged();
});