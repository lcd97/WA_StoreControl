class IndexCrearEntradaVM {
    constructor(data) {
        data = data || {};
        const self = this;

        //#region PROPIEDADES PRINCIPALES
        self.Entrada = ko.observable(new EntradaVM(data.Entrada));

        self.PeticionEnCurso = ko.observable(null);

        self.ElementosVacios = ko.observable(false);

        self.ProveedorConfig = (item) => {
            return {
                url: "BusquedaProveedor",
                queryParam: 'nombre',
                recordKey: 'Record',
                textFields: ['NombreComercial', 'Identificacion'],
                minimumInputLength: 5,
                placeholder: 'Buscar proveedor...',

                value: item.ProveedorId,
                selectedItem: item.ProveedorId() > 0
                    ? {
                        id: item.ProveedorId(),
                        text: item.NombreProveedor()
                    }
                    : null
            };
        };

        self.ProductosConfig = (item) => {
            return {
                url: "BusquedaProductos",
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

        self.AgregarProducto = () => {
            self.Entrada().DetallesEntrada.push(new DetalleEntrada());
        };

        self.DeleteProducto = (producto) => {
            self.Entrada().DetallesEntrada.remove(producto);
        };

        self.Regresar = () => {
            window.open("/Entradas/Index", "_self");
        };

        self.DetallesValidos = () => {
            let detallesValidos = true;

            self.Entrada().DetallesEntrada().forEach(element => {

                element.MostrarError(true);
                detallesValidos = !element.EsInvalido();
            });

            return detallesValidos;
        }

        self.SaveData = function (formCRUD, data) {
            let Entrada = ko.toJS(data) || {};
            $.validator.unobtrusive.parse($(formCRUD));

            if (self.Entrada().DetallesEntrada().length <= 0)
                AppGlobal.validateMessage("error", "No ha agregado registros de productos.", "Reintentar");
            else if (!self.DetallesValidos()) {
                AppGlobal.validateMessage("warning", "Existen registros de productos vacios, elimine o corrija antes de almacenar", "Reintentar");
            } else if ($(formCRUD).valid()) {
                SaveData(Entrada);
            }
        };

        //#endregion

        //#region FUNCIONES PRIVADAS
        function SaveData(Entrada) {
            let token = $('input[name="__RequestVerificationToken"]').val();

            var beforeSendCallBack = (jqXHR) => {
                if (self.PeticionEnCurso())
                    self.PeticionEnCurso().abort();

                self.PeticionEnCurso(jqXHR);
                AppGlobal.startLoad();
            }

            var successCallBack = (response) => {
                if (response.Success) {
                    AppGlobal.Messages.ShowNotifyCorrect(response.Message);
                    self.Entrada(new EntradaVM());
                } else
                    AppGlobal.Messages.ShowNotifyError(response.Message);
            }

            var errorCallBack = () => (jqXHR, statusText) => {
                if (statusText !== "abort") {
                    AppGlobal.Messages.ShowNotifyError();
                }
            };

            var completeCallBack = () => {
                self.PeticionEnCurso(null);
                AppGlobal.endLoad();
            }

            Ajax.CRUD({
                url: "Entradas/Create",
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

    let root = new IndexCrearEntradaVM(dataRoot);
    ko.applyBindings(root);
});