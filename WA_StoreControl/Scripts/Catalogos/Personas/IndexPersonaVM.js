class IndexPersonaVM {
    constructor(data) {
        data = data || {};
        const self = this;

        //#region PROPIEDADES PRINCIPALES
        self.Personas = ko.observableArray(data.Personas ? data.Personas.map(x => new PersonaVM(x)) : []);
        self.Persona = ko.observable(new PersonaVM());

        self.TiposIdentificacion = ko.observableArray(data.TiposIdentificacion ? data.TiposIdentificacion.map(x => new TipoIdentificacionVM(x)) : []);
        self.CompaniasTelefonica = ko.observableArray(data.CompaniasTelefonica ? data.CompaniasTelefonica.map(x => new CompaniaTelefonicaVM(x)) : []);

        self.Identidades = ko.observable(new IdentidadVM());
        self.Telefonos = ko.observable(new DetalleTelefonoVM());

        self.PeticionEnCurso = ko.observable(null);

        self.LoadingRegistros = ko.observable(true);
        self.LoadingTelefonos = ko.observable(false);
        self.LoadingIdentidades = ko.observable(false);

        self.Action = ko.observable("");
        self.bodyTemplate = ko.observable({});
        self.SearchViewModel = ko.observable(new SearchPersonaVM({ ...data.SearchPersonasVM, RecordsPerPage: 10 } || {}));

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
            ModalSizeClass: "modal-lg"
        }));

        self.StepNumber = ko.observable();

        self.SmartwizardOptions = AppGlobal.SmartWizardOptions({
            StepSaveButton: 0,
            StepSaveButtonCallBack: 'SaveData'
        });
        //#endregion

        //#region FUNCIONES PUBLICAS
        self.AddIdentidad = function () {
            let identificacion = self.TiposIdentificacion()
                .find(x => ko.unwrap(x.Id) == ko.unwrap(self.Identidades().TipoIdentificacionId()));

            let { Identificacion, TipoIdentificacionId, Id } = ko.toJS(self.Identidades());
            let PersonaId = ko.toJS(self.Persona().Id());

            if (!Identificacion || TipoIdentificacionId <= 0) {
                AppGlobal.validateMessage("warning",
                    "Ingrese una tipo de identificación y código antes de agregar",
                    "Reintentar");
                return;
            }

            if (TipoIdentificacionId == 1) {
                if (!esIdentidadValida()) {
                    AppGlobal.validateMessage("warning", "El código de identidad cédula no tiene un formato correcto", "Reintentar");
                    return;
                }
            }

            let existeLocal = self.Persona().Identidades()
                .some(x => ko.unwrap(x.TipoIdentificacionId) == TipoIdentificacionId);

            if (existeLocal) {
                AppGlobal.validateMessage("warning",
                    `Ya existe una ${ko.unwrap(identificacion.Descripcion())} registrada.`,
                    "Reintentar");
                return;
            }

            PuedeAgregarIdentidad(Identificacion, TipoIdentificacionId, PersonaId, Id)
                .then(function (puede) {
                    if (!puede) {
                        AppGlobal.validateMessage("warning",
                            "El número de identidad ya se encuentra registrado.",
                            "Reintentar");
                        return;
                    }

                    self.Persona().Identidades.push(new IdentidadVM({
                        Id: self.Identidades().Id(),
                        Identificacion: Identificacion,
                        TipoIdentificacionId: TipoIdentificacionId,
                        DescripcionTipoIdentificacion: identificacion.Descripcion(),
                        PersonaId: PersonaId
                    }));

                    self.ClearIdentidad();
                });
        };

        self.AddTelefono = () => {

            if (self.Telefonos().NumeroTelefonico().length != 8)
                return AppGlobal.validateMessage("warning", "El número telefónico debe contener 8 dígitos", "Reintentar");

            let companias = self.CompaniasTelefonica().find(x => ko.unwrap(x.Id) == ko.unwrap(self.Telefonos().CompaniaTelefonicaId()));

            if (self.Telefonos().NumeroTelefonico() != "" && self.Telefonos().CompaniaTelefonicaId() > 0) {
                let telefonoExistente = self.Persona().DetallesTelefono().find(x => ko.unwrap(x.NumeroTelefonico) == ko.unwrap(self.Telefonos().NumeroTelefonico()));

                if (telefonoExistente)
                    AppGlobal.validateMessage("warning", "Ya existe un número telefónico ingresado igual", "Reintentar");
                else {
                    self.Persona().DetallesTelefono.push(new DetalleTelefonoVM({
                        Id: self.Telefonos().Id(),
                        NumeroTelefonico: self.Telefonos().NumeroTelefonico(),
                        CompaniaTelefonicaId: self.Telefonos().CompaniaTelefonicaId(),
                        DescripcionCompania: companias.Descripcion(),
                        PersonaId: self.Persona().Id()
                    }));

                    self.ClearTelefono();
                }
            }
            else
                AppGlobal.validateMessage("warning", "Ingrese una compañía telefónica y número de teléfono antes de agregar", "Reintentar");
        };

        self.EditIdentidad = (data) => {
            self.Identidades(new IdentidadVM(ko.toJS(data)));

            self.Persona().Identidades.remove(data);
        };

        self.EditTelefono = (data) => {
            self.Telefonos(new DetalleTelefonoVM(ko.toJS(data)));

            self.Persona().DetallesTelefono.remove(data);
        };

        self.DeleteIdentidad = (data) => {
            AppGlobal.AjaxMessage("warning", "¿Deseas eliminar este registro?")
                .then((result) => {
                    if (result.isConfirmed) {
                        self.Persona().Identidades.remove(data);
                        AppGlobal.validateMessage("success", "Se eliminó correctamente la identificación", "Aceptar");
                    }
                });
        };

        self.DeleteTelefono = (data) => {
            AppGlobal.AjaxMessage("warning", "¿Deseas eliminar este registro?")
                .then((result) => {
                    if (result.isConfirmed) {
                        self.Persona().DetallesTelefono.remove(data);
                        AppGlobal.validateMessage("success", "Se eliminó correctamente el teléfono", "Aceptar");
                    }
                });
        };

        self.ClearIdentidad = () => {
            self.Identidades(new IdentidadVM());
        };

        self.ClearTelefono = () => {
            self.Telefonos(new DetalleTelefonoVM());
        };

        self.GetFilteredOrPaged = () => {
            GetFilteredOrPaged();
        };

        self.CleanFilter = () => {
            self.SearchViewModel()
                .Identificacion("")
                .Nombres("");

            self.GetFilteredOrPaged();
        };

        self.ShowModal = function (data, action) {
            self.Persona(new PersonaVM(ko.toJS(data || {})));

            self.Identidades(new IdentidadVM());
            self.Telefonos(new DetalleTelefonoVM());

            self.bodyTemplate(new CRUDViewModel({
                Action: action,
                DataViewModel: self.Persona,
                ModelName: "Persona"
            }));

            self.ModalViewModel().ModalHeaderViewModel().ModalTitle(self.bodyTemplate().ModalHeaderTitle()).BackgroundColorClass(self.bodyTemplate().ModalBackgroundColorClass());
            self.ModalViewModel().ModalBodyViewModel().ModalBodyTemplate({
                name: "CRUD-Persona-Template",
                data: self.bodyTemplate(),
                afterRender: AppGlobal.ParseDynamicContent
            });

            $('#smartwizard').smartWizard("goToStep", 0);
            self.ModalViewModel().BootstrapInstance().show();
        };

        self.SaveData = function () {
            let action = ko.toJS(self.bodyTemplate().Action());

            if (action == 'Delete') {
                AppGlobal.AjaxMessage("warning", "¿Está seguro que desea eliminar este registro? Una vez eliminado no podrá recuperarlo.")
                    .then((result) => {
                        if (result.isConfirmed) {
                            SaveData();
                        }
                    });
            } else
                SaveData();

        };

        //#endregion

        //#region FUNCIONES PRIVADAS
        function GetFilteredOrPaged() {
            let url = "Personas/GetFilteredOrPaged/";

            var successCallBack = (response) => {
                if (response.Success)
                    self.Personas(response.Records ? response.Records.map(x => new PersonaVM(x)) : []);
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
                url: "Personas/GetFilteredOrPaged",
                data: ko.toJS(self.SearchViewModel),
                method: "GET",
                beforeSend: beforeSendCallBack,
                complete: completeCallBack,
            }).done(successCallBack).fail(errorCallBack);
        }

        function PuedeAgregarIdentidad(Identificacion, TipoIdentificacionId, PersonaId, Id) {
            return Ajax.GetFilteredOrPaged({
                url: "Personas/PuedeAgregarIdentidad",
                data: { Identificacion, TipoIdentificacionId, PersonaId, Id },
                method: "GET",
                beforeSend: function (jqXHR) {
                    if (self.PeticionEnCurso())
                        self.PeticionEnCurso().abort();

                    self.PeticionEnCurso(jqXHR);
                    self.LoadingIdentidades(true);
                },
                complete: function () {
                    self.LoadingIdentidades(false);
                    self.PeticionEnCurso(null);
                }
            })
                .then(function (response) {
                    return response && response.Success;
                })
                .catch(function (error) {
                    AppGlobal.Messages.ShowNotifyError();
                    return false;
                });
        }

        function SaveData() {
            let Persona = ko.toJS(self.Persona)
            let url = "Personas/" + self.bodyTemplate().Action();
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
                data: { Persona },
                method: "POST",
                beforeSend: beforeSendCallBack,
                complete: completeCallBack,
            }).done(successCallBack).fail(errorCallBack);
        }

        function esIdentidadValida() {
            const tipo = self.Identidades().TipoIdentificacionId();
            const valor = (self.Identidades().Identificacion() || "").trim();

            switch (tipo) {

                case 1:
                    if (!/^\d{13}[A-Z]$/.test(valor)) {
                        return false;
                    }
                    break;
                default:
                    return true;
            }

            return true;
        }
        //#endregion
    }
}

$(() => {
    var dataRoot = JSON.parse($("#JsonData").val());
    $("#JsonData").remove();

    let root = new IndexPersonaVM(dataRoot);
    root.GetFilteredOrPaged();
    ko.applyBindings(root);
});