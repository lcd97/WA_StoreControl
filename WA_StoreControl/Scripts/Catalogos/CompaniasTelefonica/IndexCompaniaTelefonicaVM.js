class IndexCompaniaTelefonicaVM {
    constructor(data) {
        data = data || {};
        const self = this;

        //#region PROPIEDADES PRINCIPALES
        self.CompaniasTelefonica = ko.observableArray(data.CompaniasTelefonica ? data.CompaniasTelefonica.map(x => new CompaniaTelefonicaVM(x)) : []);
        self.CompaniaTelefonica = ko.observable(new CompaniaTelefonicaVM());
        self.PeticionEnCurso = ko.observable(null);

        self.LoadingRegistros = ko.observable(false);

        self.Action = ko.observable("");
        self.bodyTemplate = ko.observable({});
        self.SearchViewModel = ko.observable(new SearchCompaniaTelefonicaVM({ ...data.SearchCompaniasTelefonicaVM, RecordsPerPage: 10 } || {}));

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

            self.CompaniaTelefonica(new CompaniaTelefonicaVM(ko.toJS(data || {})));

            self.bodyTemplate(new CRUDViewModel({
                Action: action,
                DataViewModel: self.CompaniaTelefonica,
                ModelName: "Compañías Telefónica"
            }));

            self.ModalViewModel().ModalHeaderViewModel().ModalTitle(self.bodyTemplate().ModalHeaderTitle()).BackgroundColorClass(self.bodyTemplate().ModalBackgroundColorClass());
            self.ModalViewModel().ModalBodyViewModel().ModalBodyTemplate({
                name: "CRUD-CompaniaTelefonica-Template",
                data: self.bodyTemplate(),
                afterRender: AppGlobal.ParseDynamicContent
            });
            self.ModalViewModel().BootstrapInstance().show();
        };

        self.SaveData = function (formCRUD, data) {
            let CompaniaTelefonica = ko.toJS(data) || {};
            let url = "CompaniasTelefonica/" + self.bodyTemplate().Action();
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
                    data: { CompaniaTelefonica },
                    method: "POST",
                    beforeSend: beforeSendCallBack,
                    complete: completeCallBack,
                }).done(successCallBack).fail(errorCallBack);
            }
        };
        //#endregion

        //#region FUNCIONES PRIVADAS
        function GetFilteredOrPaged() {
            let url = "CompaniasTelefonica/GetFilteredOrPaged/";

            var successCallBack = (response) => {
                if (response.Success)
                    self.CompaniasTelefonica(response.Records ? response.Records.map(x => new CompaniaTelefonicaVM(x)) : []);
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
                url: "CompaniasTelefonica/GetFilteredOrPaged",
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

    let root = new IndexCompaniaTelefonicaVM(dataRoot);

    ko.applyBindings(root);
    root.GetFilteredOrPaged();
});