
ko.components.register('indicador-estado-solicitud', {
    viewModel: {
        createViewModel: function (params, componentInfo) {

            var self;

            //#region === VIEWMODELS ===

            function IndicadorEstadoSolicitudVM() {
                self = this;

                self.ListaEstadoSolicitud = params.ListaEstadoSolicitud;
                self.DetalleEstadoSolicitudEvento = params.DetalleEstadoSolicitudEvento;
                self.ListaIndicadorEstadoSolicitud = self.ListaEstadoSolicitud().map(function (obj, i) {

                    if (self.DetalleEstadoSolicitudEvento().length > 0) {
                        var activo = self.DetalleEstadoSolicitudEvento().some(function (o, ii) { return o.EstadoSolicitudEventoId() == obj.Item1() });
                        var claseIcono = "";
                        var claseOrden = "";
                        var claseColor = "";

                        switch (obj.Item2()) {
                            case "001":
                                claseIcono = "fas fa-file-signature";
                                claseOrden = "estado-" + obj.Item2();
                                claseColor = "text-warning";
                                break;
                            case "002":
                                claseIcono = "fas fa-file-export";
                                claseOrden = "estado-" + obj.Item2();
                                claseColor = "text-primary";
                                break;
                            case "003":
                                claseIcono = "fas fa-file-upload";
                                claseOrden = "estado-" + obj.Item2();
                                claseColor = "text-info";
                                break;
                            case "004":
                                claseIcono = "fas fa-file-alt";
                                claseOrden = "estado-" + obj.Item2();
                                claseColor = "text-success";
                                break;
                            case "005":
                                claseIcono = "fas fa-file-excel";
                                claseOrden = "estado-" + obj.Item2();
                                claseColor = "text-danger";
                                break;
                        }

                        var indesta = new IndicadorEstadoSolicitud(obj.Item1(), obj.Item2(), obj.Item3(), activo, claseIcono, claseOrden, claseColor);

                        return indesta;
                    }
                    else
                        return [];
                });                
            }

            function IndicadorEstadoSolicitud(EstadoSolicitudId, Codigo, DescEstado, Activo, ClaseIcono, ClaseOrden, ClaseColor) {
                this.EstadoSolicitudId = EstadoSolicitudId;
                this.Codigo = Codigo;
                this.DescEstado = DescEstado;
                this.Activo = Activo;
                this.ClaseIcono = ClaseIcono;
                this.ClaseOrden = ClaseOrden;
                this.ClaseColor = ClaseColor;
            }

            //#endregion

            return IndicadorEstadoSolicitudVM;
        }
    },
    template: { cargadorVista: "ComponentesKo/_IndicadorEstadoSolicitud", esParcial: true }
});