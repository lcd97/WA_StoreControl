
ko.components.register('filtro-documentos-componente', {
    viewModel: {
        createViewModel: function (params, componentInfo) {

            var self;
            var DatosEnviar = null;

            //#region === VIEWMODELS ===

            function FiltroDocumentosVM() {

                self = this;
                self.Eventos = new EventosVM();
                self.ClaveDocumentoModuloId = params.ClaveDocumentoModuloId;
                self.ModeloDeVista = params.filtroDocumento;
                self.ModeloDeVista.CmbClaveDocumentoPorDocumentoActual = ko.pureComputed(function () {

                    var ListaClaveDocumento;

                    if (self.ModeloDeVista.ClaveDocumentoPantallaId() > 0) {
                        if (self.ModeloDeVista.ClaveDocumentoPantallaId() == 4)
                            ListaClaveDocumento = self.ModeloDeVista.CmbClaveDocumento().filter(function (f) { return f.Item1() == self.ModeloDeVista.ClaveDocumentoPantallaId() || f.Item3() == true })
                        else
                            ListaClaveDocumento = self.ModeloDeVista.CmbClaveDocumento().filter(function (f) { return f.Item1() == self.ModeloDeVista.ClaveDocumentoPantallaId() });
                    }
                    else
                        ListaClaveDocumento = self.ModeloDeVista.CmbClaveDocumento();

                    return ListaClaveDocumento;
                });
                self.ModeloDeVista.CmbPeriodoEjercicio = ko.pureComputed(function () { return self.ModeloDeVista.CmbPeriodo().filter(function (f) { return f.Item3() == self.ModeloDeVista.EjercicioId() }) });

                //#region === PARAMETROS PARA EL COMPONENTE DE BUSCADOR AUTOCOMPLEADO ===

                    var indicacionesTercero_txt = `<ul>
                    <li>Solo se buscarán coincidencias con el <strong>código de identidad</strong> o con parte del <strong>nombre o apellido</strong>.</li>
                    <li>Solo se iniciará a realizar la búsqueda apartir del cuarto caracter digitado.</li>
                    <li>Solo se mostrarán coincidencias con el <strong>código de identidad</strong> cuando coincida completamente.</li>
                    <li>Solo se obtendrán como máximo un total de 200 registros.</li>
                    <li>Entre más específico sea el texto a buscar menos coincidencias para seleccionar y además la búsqueda tardará menos tiempo.</li>
                    </u>`;
                    self.ModeloDeVista.indicacionesTerceroBuscadorAutocompletado = ko.observable(indicacionesTercero_txt);
                    self.ModeloDeVista.tieneElFocoTerceroAutocompletado = ko.observable(false);
                    self.ModeloDeVista.MostrarBuscadorTerceroAutocompletado = function () {
                        self.ModeloDeVista.tieneElFocoTerceroAutocompletado(true);
                    };

                //#endregion

                self.OpcionCheckTab = self.ModeloDeVista.OpcionCheckTab;
                self.OpcionCheckTab.subscribe(function () {

                    self.OpcionBusquedaRapida("SecuenciaDoc");
                    self.OpcionBusquedaAvanzada([]);

                    var propiedades = self.OpcionBusquedaRapidaFiltros().concat(self.OpcionBusquedaAvanzadaFiltros()).concat(["EjercicioId"]);
                    LimpiarModelo(propiedades);
                });

                self.OpcionBusquedaRapidaFiltros = ko.observableArray(["SecuenciaDoc", "NumeroDocumento", "NumeroCheque"]);
                self.OpcionBusquedaRapida = self.ModeloDeVista.OpcionBusquedaRapida;
                self.OpacidadCampoSecuenciaDoc = ko.observable(1);
                self.OpacidadCampoNumeroDocumento = ko.observable(0);
                self.OpacidadCampoNumeroCheque = ko.observable(0);
                self.HabilitarCampoSecuenciaDoc = ko.observable(false);
                self.HabilitarCampoNumeroDocumento = ko.observable(true);
                self.HabilitarCampoNumeroCheque = ko.observable(true);
                self.OpcionBusquedaRapida.subscribe(function (value) {

                    self.OpacidadCampoSecuenciaDoc(0);
                    self.OpacidadCampoNumeroDocumento(0);
                    self.OpacidadCampoNumeroCheque(0);
                    self.HabilitarCampoSecuenciaDoc(true);
                    self.HabilitarCampoNumeroDocumento(true);
                    self.HabilitarCampoNumeroCheque(true);

                    switch (self.OpcionBusquedaRapida()) {
                        case "SecuenciaDoc":
                            self.OpacidadCampoSecuenciaDoc(1);
                            self.HabilitarCampoSecuenciaDoc(false);
                            break;

                        case "NumeroDocumento":
                            self.OpacidadCampoNumeroDocumento(1);
                            self.HabilitarCampoNumeroDocumento(false);
                            break;

                        case "NumeroCheque":
                            self.OpacidadCampoNumeroCheque(1);
                            self.HabilitarCampoNumeroCheque(false);
                            break;
                    }

                    var FiltrosNoCheck = self.OpcionBusquedaRapidaFiltros().filter(function (v, i) {
                        return value != v;
                    });

                    LimpiarModelo(FiltrosNoCheck);

                });

                self.OpcionBusquedaAvanzadaFiltros = ko.observableArray(["PeriodoId", "FechaDocumento", "ClaveDocumentoId", "CuentaBancoId", "TerceroId", "NombreCompletoTercero", "FormaPagoId", "OficinaContableId", "Descripcion"]);
                self.OpcionBusquedaAvanzada = self.ModeloDeVista.OpcionBusquedaAvanzada;
                self.OpcionBusquedaAvanzada.subscribe(function (value) {

                    var FiltrosNoCheck = self.OpcionBusquedaAvanzadaFiltros().filter(function (v, i) {
                        return !value.includes(v);
                    });

                    LimpiarModelo(FiltrosNoCheck);
                });

                // OBTIENE EL MODELO DE VISTA INICIAL, POR DEFECTO
                if (self.ModeloDeVista.ModeloDeVistaInicial() == null || self.ModeloDeVista.ModeloDeVistaInicial() == "")
                    ObtenerModeloDeVistaInicial();
                else
                {
                    ObtenerDatosEnviar();
                    EjecutarFiltro();
                }
            }

            function EventosVM() {

                var self_evento = this;

                self_evento.RealizarFiltro = function () {

                    // SE OBTIENE LOS DATOS A ENVIAR
                    ObtenerDatosEnviar();

                    // SE EJECUTA FILTRO DE DOCUMENTOS
                    EjecutarFiltro();
                };
            }

            //#endregion

            //#region === FUNCIONES ===

            function ObtenerModeloDeVistaInicial() {

                setTimeout(function () {
                    var modelo = JSON.parse(JSON.stringify(ko.toJS(self.ModeloDeVista)));
                    delete modelo.CmbEjercicio;
                    delete modelo.CmbPeriodo;
                    delete modelo.CmbClaveDocumento;
                    delete modelo.CmbClaveDocumentoPorDocumentoActual;
                    delete modelo.CmbPeriodoEjercicio;

                    delete modelo.CmbCuentaBanco;
                    delete modelo.CmbFormaPago;
                    delete modelo.CmbOficinaContable;

                    delete modelo.__ko_mapping__;

                    self.ModeloDeVista.ModeloDeVistaInicial(JSON.stringify(modelo));
                }, 1500);
            }

            function LimpiarModelo(ListaPropiedades) {
               
                if (self.ModeloDeVista.ModeloDeVistaInicial() !== "")
                {
                    var ModeloDeVistaInicial = JSON.parse(self.ModeloDeVista.ModeloDeVistaInicial());
                    ListaPropiedades.forEach(function (v, i)
                    {
                        if (v === "FechaDocumento") {
                            self.ModeloDeVista[v + "I"](ModeloDeVistaInicial[v + "I"]);
                            self.ModeloDeVista[v + "F"](ModeloDeVistaInicial[v + "F"]);
                        }
                        else {
                            self.ModeloDeVista[v](ModeloDeVistaInicial[v]);
                        }
                    });
                }
            }

            function ObtenerDatosEnviar()
            {
                DatosEnviar = JSON.parse(JSON.stringify(ko.toJS(self.ModeloDeVista)));

                //SE BORRAN PROPIEDADES QUE NO SE UTILIZARAN PARA LOS FILTROS DE DOCUMENTOS
                delete DatosEnviar.CmbEjercicio;
                delete DatosEnviar.CmbPeriodo;
                delete DatosEnviar.CmbClaveDocumento;
                delete DatosEnviar.CmbClaveDocumentoPorDocumentoActual;
                delete DatosEnviar.CmbPeriodoEjercicio;

                delete DatosEnviar.CmbCuentaBanco;
                delete DatosEnviar.CmbFormaPago;
                delete DatosEnviar.CmbOficinaContable;

                delete DatosEnviar.ModeloDeVistaInicial;
                delete DatosEnviar.__ko_mapping__;

                if (DatosEnviar.OpcionCheckTab == "BusquedaRapida")
                {
                    DatosEnviar.EjercicioId = (DatosEnviar.EjercicioId > 0) ? 0 : DatosEnviar.EjercicioId;
                    DatosEnviar.PeriodoId = (DatosEnviar.PeriodoId > 0) ? 0 : DatosEnviar.PeriodoId;
                    DatosEnviar.ClaveDocumentoId = (DatosEnviar.ClaveDocumentoId > 0) ? 0 : DatosEnviar.ClaveDocumentoId;

                    DatosEnviar.CuentaBancoId = (DatosEnviar.CuentaBancoId > 0) ? 0 : DatosEnviar.CuentaBancoId;
                    DatosEnviar.FormaPagoId = (DatosEnviar.FormaPagoId > 0) ? 0 : DatosEnviar.FormaPagoId;
                    DatosEnviar.OficinaContableId = (DatosEnviar.OficinaContableId > 0) ? 0 : DatosEnviar.OficinaContableId;

                } else
                {
                    if (DatosEnviar.OpcionBusquedaAvanzada.includes("PeriodoId") == false)
                        DatosEnviar.PeriodoId = (DatosEnviar.PeriodoId > 0) ? 0 : DatosEnviar.PeriodoId;
                    if (DatosEnviar.OpcionBusquedaAvanzada.includes("ClaveDocumentoId") == false)
                        DatosEnviar.ClaveDocumentoId = (DatosEnviar.ClaveDocumentoId > 0) ? 0 : DatosEnviar.ClaveDocumentoId;

                    if (DatosEnviar.OpcionBusquedaAvanzada.includes("CuentaBancoId") == false)
                        DatosEnviar.CuentaBancoId = (DatosEnviar.CuentaBancoId > 0) ? 0 : DatosEnviar.CuentaBancoId;
                    if (DatosEnviar.OpcionBusquedaAvanzada.includes("FormaPagoId") == false)
                        DatosEnviar.FormaPagoId = (DatosEnviar.FormaPagoId > 0) ? 0 : DatosEnviar.FormaPagoId;
                    if (DatosEnviar.OpcionBusquedaAvanzada.includes("OficinaContableId") == false)
                        DatosEnviar.OficinaContableId = (DatosEnviar.OficinaContableId > 0) ? 0 : DatosEnviar.OficinaContableId;
                }
            }

            function EjecutarFiltro()
            {
                //SE ESTABLECE LOS PARAMETROS SI ES CARGA DE DATOS DEL SERVIDOR
                if (params.UrlCargaDatos() != "" && params.UrlCargaDatos() != null) {
                    params.ParametrosCargaDeDatos(DatosEnviar);
                    (params.EjecutarCargarDatos())();
                }
            }

            //#endregion
            

            var filtroDocumentos = new FiltroDocumentosVM();

            // SE ESTABLECE EL CONTROL DATEPICKER A LOS CAMPOS MARCADOS COMO CALENDARIOS
            window.EstablecerDatePicker();

            return filtroDocumentos;
        }
    },
    template: { cargadorVista: "ComponentesKo/_FiltroDocumentos", esParcial: true }
});





    
