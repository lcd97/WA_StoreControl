
ko.components.register('buscador-autocompletado-componente', {
    viewModel: {
        createViewModel: function (params, componentInfo) {

            var self;
            var ultimaSemilla;
            var ControlTiempoBusqueda;

            //#region === VIEWMODELS ===

            function BuscadorAutocompletadoVM() {
                self = this;

                self.modoAdicionalAutocompletado = ko.observable((params.modoAdicionalAutocompletado == null || undefined) ? false : params.modoAdicionalAutocompletado);
                self.textoBuscar = (params.textoBuscar == null || params.textoBuscar == undefined) ? ko.observable() : params.textoBuscar;
                self.url = params.url;
                self.inicioCantidadCaracter = (params.inicioCantidadCaracter == null || undefined) ? 5 : params.inicioCantidadCaracter;
                self.autoBusqueda = (params.autoBusqueda == null || undefined) ? true : params.autoBusqueda;
                self.indicaciones = (params.indicaciones == null || undefined)? ko.observable("Sin indicaciones") : params.indicaciones;
                self.parametros = (params.parametros == null || undefined)? {} : params.parametros;
                self.textoPrimario = params.textoPrimario;
                self.textoSecundario = params.textoSecundario;
                self.aplicaMarcaCoincidenciaPrimario = (params.aplicaMarcaCoincidenciaPrimario == null || undefined) ? true : params.aplicaMarcaCoincidenciaPrimario;
                self.aplicaMarcaCoincidenciaSecundario = (params.aplicaMarcaCoincidenciaSecundario == null || undefined) ? false : params.aplicaMarcaCoincidenciaSecundario;
                self.cantidadMaximaResultados = (params.cantidadMaximaResultados == null || undefined) ? 200 : params.cantidadMaximaResultados;
                self.atributoName = (params.atributoName == null || undefined) ? "" : params.atributoName;
                self.tieneElFoco = (params.tieneElFoco == null || undefined) ? ko.observable(false) : params.tieneElFoco;

                self.realizarBusqueda = (self.tieneElFoco() == null || undefined) ? ko.observable() : params.realizarBusqueda;

                self.eventoSeleccionResultado = (params.eventoSeleccionResultado == null || undefined) ? null : params.eventoSeleccionResultado;

                if (params.realizarBusqueda != undefined)
                    self.realizarBusqueda(RealizarBusqueda);

                self.esVisible = ko.observable(false);
                self.forzarVisible = ko.observable(false);
                self.listaResultados = ko.observableArray();
                self.cantidadResultados = ko.observable(0);

                //#region === EVENTOS ===

                self.verIndicaciones = function () {
                    self.forzarVisible(true);
                    swal({
                        title: "Indicaciones",
                        text: "<div class='ko-autocompletado-indicaciones'>" + self.indicaciones() + "</div>",
                        type: "info",
                        html: true,
                        showCancelButton: false,
                        confirmButtonClass: "btn-info",
                        confirmButtonText: "Entendido"
                    }, function () {
                        self.forzarVisible(false);
                        setTimeout(function () {
                            self.tieneElFoco(true);
                        }, 300);
                        
                    });                   
                };

                self.seleccionarResultado = function (obj) {

                    if (self.textoBuscar() != null && self.textoBuscar() != undefined)
                    {
                        if (self.modoAdicionalAutocompletado)
                            self.textoBuscar("");
                        else
                            self.textoBuscar(obj[self.textoPrimario]);
                    }

                    params.AsignarValorAdicionales = (params.AsignarValorAdicionales == null || params.AsignarValorAdicionales == undefined) ? [] : params.AsignarValorAdicionales;

                    params.AsignarValorAdicionales.forEach(function (ova, i) {
                        ova.clave(obj[ova.valor]);
                    });

                    if (params.eventoSeleccionResultado != null && typeof params.eventoSeleccionResultado === "function")
                    {
                        params.eventoSeleccionResultado();
                    }
                };

                self.EventoTecla = function (obj, e) {

                    var tecla = e.keyCode || e.which;        

                    if (tecla == 13) // TECLA ENTER
                    {
                        var cantidadCaracteres = (self.textoBuscar() == null || self.textoBuscar() == undefined) ? 0 : self.textoBuscar().length;
                        if (cantidadCaracteres >= self.inicioCantidadCaracter)
                            RealizarBusqueda();
                    }
                };

                //#endregion

                //#region === SUSCRIPCIONES ===

                self.textoBuscar.subscribe(function (texto) {

                    clearTimeout(ControlTiempoBusqueda);

                    var cantidadCaracteres = (texto == null || undefined) ? 0 : texto.length;

                    if (cantidadCaracteres >= self.inicioCantidadCaracter && self.tieneElFoco() && self.autoBusqueda)
                    {
                        ControlTiempoBusqueda = setTimeout(function () { 
                            RealizarBusqueda();
                        }, 800);
                    }
                    else if (cantidadCaracteres < self.inicioCantidadCaracter)
                    {
                        self.cantidadResultados(0);
                        self.listaResultados([]);
                    }

                });

                self.tieneElFoco.subscribe(function (valor) {

                    if (valor)
                    {
                        if (self.textoBuscar() != "" && self.textoBuscar() != null && self.textoBuscar() != undefined)
                        {
                            var cantidadCaracteres = (self.textoBuscar() == null || self.textoBuscar() == undefined) ? 0 : self.textoBuscar().length;

                            if (cantidadCaracteres >= self.inicioCantidadCaracter && self.tieneElFoco())
                                RealizarBusqueda();
                            else if (cantidadCaracteres < self.inicioCantidadCaracter)
                            {
                                self.cantidadResultados(0);
                                self.listaResultados([]);
                            }
                        }

                        self.esVisible(true);
                    }
                    else
                    {
                        setTimeout(function () {
                            if (!self.forzarVisible())
                                self.esVisible(false);
                        }, 300);
                    }
                });

                //#endregion
            }

            //#endregion

            //#region === FUNCIONES ===

            function RealizarBusqueda()
            {
                var selectorGifCarga = (self.modoAdicionalAutocompletado) ? ".ko-buscador-autocompletado-lista" : ".ko-buscador-autocompletado-resultado";
                window.GifCargaGrid(selectorGifCarga, true);
                self.tieneElFoco(true);
                var parametros_json = ko.toJS(self.parametros);
                parametros_json.TextoBusqueda = self.textoBuscar();
                parametros_json.CantidadMaximaResultados = self.cantidadMaximaResultados;

                $.ajax({
                    type: "GET",
                    url: window.ObtenerDominioApp() + self.url,
                    data: parametros_json,
                    beforeSend: function (jqXHR, settings) {
                        settings.url = settings.url + "&semilla=" + Date.now();
                    },
                    success: function (rs)
                    {
                        if ((rs.semilla > ultimaSemilla || rs.semilla == undefined) || ultimaSemilla == undefined)
                        {
                            // Se asigna la ultima semilla de respuesta.
                            ultimaSemilla = rs.semilla;

                            var datos = (rs.datos == undefined) ? rs : rs.datos;

                            if (datos.length > self.cantidadMaximaResultados)
                                datos = datos.slice(0, self.cantidadMaximaResultados);

                            self.cantidadResultados(datos.length);
                            ConstruirLista(datos, parametros_json.TextoBusqueda);
                        } 
                    },
                    complete: function () {
                        window.GifCargaGrid(selectorGifCarga, false);
                    }
                });
            }

            function ConstruirLista(Lista, TextoBusqueda) {

                var arrayTextBusqueda = TextoBusqueda.toString().trim().split(" ");
                var arrayTextoBusquedaReg = [];

                arrayTextBusqueda.forEach(function (texto, i) {
                    arrayTextoBusquedaReg.push(new RegExp("(" + texto + ")", "ig"));
                });

                self.listaResultados(
                    Lista.map(function (obj)
                    {
                        var primero = obj[self.textoPrimario].toString();

                        if (self.aplicaMarcaCoincidenciaPrimario)
                        {
                            arrayTextoBusquedaReg.forEach(function (textoReg, i) {
                                primero = primero.replace(textoReg, "<strong>$1</strong>");
                            });  
                        }

                        var segundo = "";

                        if (self.textoSecundario != null && self.textoSecundario != undefined)
                            segundo = obj[self.textoSecundario].toString();

                        if (self.aplicaMarcaCoincidenciaSecundario)
                        {
                            arrayTextoBusquedaReg.forEach(function (textoReg, i) {
                                segundo = segundo.replace(textoReg, "<strong>$1</strong>");
                            }); 
                            
                        }

                        obj.Primero = primero;
                        obj.Segundo = segundo;

                        return obj;
                    })
                );
            }

            //#endregion

            var buscadorAutocompletadoVM = new BuscadorAutocompletadoVM();

            if(self.modoAdicionalAutocompletado())
                $(".ko-buscador-autocompletado").addClass("ko-buscador-autocompletado-adicional");

            return buscadorAutocompletadoVM;
        }
    },
    template: { cargadorVista: "ComponentesKo/_BuscadorAutocompletado", esParcial: true }
});