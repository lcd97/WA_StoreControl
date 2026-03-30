class CRUDViewModel {
    constructor(data) {
        data = data || {};
        const self = this;

        self.Action = ko.observable(data.Action || "");

        self.AntiForgeryToken = ko.observable(data.AntiForgeryToken);

        self.SaveButtonText = ko.computed(function () {
            let result = "Guardar";
            if (self.Action().toLowerCase() === "create") {
                result = "Guardar";

            } else if (self.Action().toLowerCase() === "edit" || self.Action().toLowerCase() === "close") {
                result = "Guardar Cambios";

            } else if (self.Action().toLowerCase() === "delete") {
                result = "Eliminar";

            } else if (self.Action().toLowerCase() === "desactivar") {
                result = "Desactivar";

            }
            else if (self.Action().toLowerCase() === "verify") {
                result = "Guardar Verificación";

            } else if (self.Action().toLowerCase() === "unverify") {
                result = "Desverificar";
            }

            return result;
        });

        self.SaveButtonIconClass = ko.computed(function () {
            let result = "fas fa-save";
            if (self.Action().toLowerCase() === "create" || self.Action().toLowerCase() === "edit" || self.Action().toLowerCase() === "close") {
                result = "fas fa-save";

            } else if (self.Action().toLowerCase() === "delete") {
                result = "fas fa-trash-alt";

            } else if (self.Action().toLowerCase() === "verify") {
                result = "fas fa-check";

            } else if (self.Action().toLowerCase() === "unverify") {
                result = "fas fa-unlock-alt";

            } else if (self.Action().toLowerCase() === "desactivar") {
                result = "fa fa-ban";
            }

            return result;
        });

        self.SaveButtonBackgroundClass = ko.computed(function () {
            let result = "btn btn-info";
            if (self.Action().toLowerCase() === "create") {
                result = "btn btn-primary";

            } else if (self.Action().toLowerCase() === "edit") {
                result = "btn btn-success";

            } else if (self.Action().toLowerCase() === "delete" || self.Action().toLowerCase() === "unverify" || self.Action().toLowerCase() === "close" || self.Action().toLowerCase() === "desactivar") {
                result = "btn btn-danger";
            }

            return result;
        });

        self.DataViewModel = ko.observable(data.DataViewModel); //ViewModel del Modelo Principal...

        self.ProcessingAction = ko.observable(false);

        self.DisableUserInput = ko.observable(false);

        self.ModelName = ko.observable(data.ModelName || "");

        self.ModalHeaderTitle = ko.observable("");

        self.ModalBackgroundColorClass = ko.computed(() => {
            let result = "bg-info text-dark";
            if (self.Action().toLowerCase() === "create") {
                result = "bg-primary text-white";
                self.ModalHeaderTitle("Crear " + self.ModelName());
                self.DisableUserInput(true);
            }
            else if (self.Action().toLowerCase() === "edit") {
                result = "bg-success text-white";
                self.ModalHeaderTitle("Editar " + self.ModelName());
                self.DisableUserInput(true);
            }
            else if (self.Action().toLowerCase() === "delete") {
                result = "bg-danger text-white";
                self.ModalHeaderTitle("Eliminar " + self.ModelName());
            }
            else if (self.Action().toLowerCase() === "desactivar") {
                result = "bg-danger text-white";
                self.ModalHeaderTitle("Desactivar " + self.ModelName());
            }
            else if (self.Action().toLowerCase() === "details") {
                result = "bg-secondary text-white";
                self.ModalHeaderTitle("Detalles " + self.ModelName());
            }
            else if (self.Action().toLowerCase() === "verify") {
                result = "bg-info text-dark";
                self.ModalHeaderTitle("Verificar " + self.ModelName());

            } else if (self.Action().toLowerCase() === "unverify") {
                result = "bg-danger text-white";
                self.ModalHeaderTitle("Desverificar " + self.ModelName());

            } else if (self.Action().toLowerCase() === "close") {
                result = "bg-danger text-white";
                self.ModalHeaderTitle("Cerrar " + self.ModelName());
            } else if (self.Action().toLowerCase() === "") {
                result = "bg-primary text-white";
                self.ModalHeaderTitle(self.ModelName());
            } else {
                self.ModalHeaderTitle(self.ModelName())
            }

            return result;
        });

        self.DeleteWarningMessage = ko.computed(function () {
            return self.Action().toLowerCase() === "delete" ? "¿Esta seguro/a que desea eliminar este registro?" : null;
        });

    };
};