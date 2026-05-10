class IndexDetalleEntradaVM {
    constructor(data) {
        data = data || {};
        const self = this;

        //#region PROPIEDADES PRINCIPALES
        self.Entrada = ko.observable(new EntradaVM(data.Entrada));
        //#endregion

        //#region FUNCIONES PUBLICAS
        self.Regresar = () => {
            window.open("/Entradas/Index", "_self");
        };
        //#endregion
    }
}

$(() => {
    var dataRoot = JSON.parse($("#JsonData").val());
    $("#JsonData").remove();

    let root = new IndexDetalleEntradaVM(dataRoot);
    ko.applyBindings(root);
});