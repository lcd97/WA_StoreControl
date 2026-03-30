
ko.components.register('limite-caracteres-componente', {
    viewModel: {
        createViewModel: function (params, componentInfo) {

            var self;

            //#region === VIEWMODELS ===

            function LimiteCaracteresVM() {
                self = this;

                self.Valor = params.Valor();
                self.LimiteCaracteres = params.LimiteCaracteres;

                self.ResultadoCaracteresPermitidos = ko.pureComputed(function () {
                    var LongitudCampo = (self.Valor == null) ? 0 : self.Valor.length;
                    return self.LimiteCaracteres - LongitudCampo;
                });
                self.ClaseLimiteColor = ko.pureComputed(function () {
                    return (self.ResultadoCaracteresPermitidos() > 25) ? "color-inicial" : "color-final";
                });
            }

            //#endregion

            return LimiteCaracteresVM;
        }
    },
    template: { cargadorVista: "ComponentesKo/_LimiteCaracteres", esParcial: true }
});


