
ko.components.register('etiqueta-rol', {
    viewModel: {
        createViewModel: function (params, componentInfo) {

            var self;

            //#region === VIEWMODELS ===

            function EtiquetaRolVM() {
                self = this;

                self.EtiquetaRol = params.EtiquetaRol;
                self.ClaseIconoRol = params.ClaseIconoRol;
            }

            //#endregion

            return EtiquetaRolVM;
        }
    },
    template: { cargadorVista: "ComponentesKo/_EtiquetaRol", esParcial: true }
});