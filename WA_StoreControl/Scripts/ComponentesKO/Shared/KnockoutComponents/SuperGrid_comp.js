
ko.components.register('super-grid-componente', {
    viewModel: {
        createViewModel: function (params, componentInfo) {

            class SuperGridVM {
                constructor() {

                    const self = this;

                    //#region Propiedades

                    self.gridVM = params.gridVM == undefined ? new GridVM() : params.gridVM;
                    self.gridVM.DatosColumnasVisibles = ko.pureComputed(() => {
                        return self.gridVM.DatosColumnas().filter((obj) => { return obj.OcultarColumna() });
                    });
                    self.paginacionSuperGridVM = new PaginacionSuperGridVM(self.gridVM);
                    self.gridVM.EjecutarCargarDatos(EjecutarCargarDatos);

                    self.ListaCantidadPorPagina = ko.observableArray();
                    self.VerBusquedaPorPagina = ko.observable(false);
                    self.VerColumnas = ko.observable(false);

                    //#endregion


                    //#region Suscripciones     
                    
                    self.gridVM.DatosFilas.subscribe(() => {
                        if (self.gridVM.CantidadTotalRegistros() > 0 && self.paginacionSuperGridVM.ListaPagina().length == 0)
                            LlenarListadoPaginas();
                        else if (self.gridVM.CantidadTotalRegistros() > 0)
                            ReCrearNavPaginacion();
                    });

                    //#endregion


                    //#region Funciones Públicas

                    self.SeleccionRegistrosPorPagina = (obj, e) => {
                        $(e.currentTarget).blur();
                        EjecutarCargarDatos();
                    };

                    self.DigitarBusquedaPorPagina = (obj, e) => {
                        let OrdenCol = $(e.currentTarget).parents("th").attr("data-ordencol");
                        let ValorBusqueda = e.currentTarget.value;
                        ValorBusqueda = ValorBusqueda.toLowerCase();
                        ValorBusqueda = ValorBusqueda.replace("á", "a").replace("é", "e").replace("í", "i").replace("ó", "o").replace("ú", "u");

                        $(".grid-ko tbody tr td").each((i, el) => {
                            let OrdenEl = $(el).attr("data-ordencol");

                            if (OrdenCol == OrdenEl)
                            {
                                let ValorEl = $(el).find("span").first().html().trim();
                                ValorEl = ValorEl.toLowerCase();
                                ValorEl = ValorEl.replace("á", "a").replace("é", "e").replace("í", "i").replace("ó", "o").replace("ú", "u");

                                if (ValorEl.includes(ValorBusqueda) && ValorBusqueda.trim() != "")
                                    $(el).addClass("seleccion-busqueda-pagina");
                                else
                                    $(el).removeClass("seleccion-busqueda-pagina");
                            }
                        });
                    };

                    //#endregion


                    //#region Funciones Privadas

                    function LlenarListadoPaginas()
                    {
                        let CantidadDePaginas = Math.ceil(self.gridVM.CantidadTotalRegistros() / self.gridVM.CantidadRegistrosPorPagina());
                        let LimitePaginas = 5;

                        for (let p = 1; p <= ((CantidadDePaginas <= LimitePaginas) ? CantidadDePaginas : LimitePaginas); p++)
                            self.paginacionSuperGridVM.ListaPagina.push(new PaginaSuperGridVM(p.toString(), (p == 1? "active" : "")));

                        if (CantidadDePaginas > 0)
                        {
                            self.paginacionSuperGridVM.ListaPagina.unshift(new PaginaSuperGridVM("<<", ""));
                            self.paginacionSuperGridVM.ListaPagina.push(new PaginaSuperGridVM(">>", ""));
                        }
                    }

                    function ReCrearNavPaginacion()
                    {
                        let CantidadDePaginas = Math.ceil(self.gridVM.CantidadTotalRegistros() / self.gridVM.CantidadRegistrosPorPagina());
                        let PaginaMinima = 1;
                        let PaginaMaxima = CantidadDePaginas;
                        let LimitePaginas = 5;

                        if (CantidadDePaginas > PaginaMinima)
                        {
                            var Mitad = (self.paginacionSuperGridVM.ListaPagina().length == 7) ? parseInt(self.paginacionSuperGridVM.ListaPagina()[3].ValorPagina) : 0;
                            var LimiteDiferencia = 2;
                            var PrimeraPagina = parseInt(self.paginacionSuperGridVM.ListaPagina()[1].ValorPagina);
                            var UltimaPagina = parseInt(self.paginacionSuperGridVM.ListaPagina()[self.paginacionSuperGridVM.ListaPagina().length - 2].ValorPagina);

                            if (self.paginacionSuperGridVM.ListaPagina().length == 7 && self.paginacionSuperGridVM.NumeroPaginaActual() > Mitad && UltimaPagina != PaginaMaxima) {
                                var respaldo = [];

                                if (self.paginacionSuperGridVM.NumeroPaginaActual() == PaginaMaxima) {
                                    var limite = PaginaMaxima - LimitePaginas + 1;
                                    limite = (limite < PaginaMinima) ? PaginaMinima : limite

                                    for (var p = limite; p <= PaginaMaxima; p++) 
                                        respaldo.push(new PaginaSuperGridVM(p, (p == PaginaMaxima ? "active" : "")));

                                    respaldo.unshift(new PaginaSuperGridVM("<<", ""));
                                    respaldo.push(new PaginaSuperGridVM(">>", ""));

                                }
                                else
                                {
                                    var diferencia = (self.paginacionSuperGridVM.NumeroPaginaActual() - Mitad);
                                    diferencia = (diferencia > LimiteDiferencia) ? LimiteDiferencia : diferencia;
                                    diferencia = ((UltimaPagina + diferencia <= CantidadDePaginas) ? diferencia : diferencia - 1);

                                    self.paginacionSuperGridVM.ListaPagina().forEach(function (vp, i) {
                                        var NuevaPagina = parseInt(vp.ValorPagina) + diferencia;
                                        NuevaPagina = (isNaN(NuevaPagina)) ? vp.ValorPagina : NuevaPagina.toString();
                                        var ClasePaginaActiva = (self.paginacionSuperGridVM.NumeroPaginaActual() == NuevaPagina) ? "active" : "";
                                        respaldo.push(new PaginaSuperGridVM(NuevaPagina, ClasePaginaActiva));
                                    });
                                }

                                self.paginacionSuperGridVM.ListaPagina.removeAll();
                                self.paginacionSuperGridVM.ListaPagina(respaldo);

                            }
                            else if (self.paginacionSuperGridVM.ListaPagina().length == 7 && self.paginacionSuperGridVM.NumeroPaginaActual() < Mitad && PrimeraPagina != PaginaMinima) {
                                var respaldo = [];

                                if (self.paginacionSuperGridVM.NumeroPaginaActual() == PaginaMinima) {
                                    var limite = PaginaMinima + CantidadDePaginas - 1;
                                    limite = (limite > LimitePaginas) ? LimitePaginas : limite

                                    for (var p = PaginaMinima; p <= limite; p++)
                                        respaldo.push(new PaginaSuperGridVM(p, (p == PaginaMinima ? "active" : "")));

                                    respaldo.unshift(new PaginaSuperGridVM("<<", ""));
                                    respaldo.push(new PaginaSuperGridVM(">>", ""));
                                }
                                else
                                {
                                    var diferencia = (Mitad - self.paginacionSuperGridVM.NumeroPaginaActual());
                                    diferencia = (diferencia > LimiteDiferencia) ? LimiteDiferencia : diferencia;
                                    diferencia = ((PrimeraPagina - diferencia >= 1) ? diferencia : diferencia - 1);

                                    self.paginacionSuperGridVM.ListaPagina().forEach(function (vp, i) {
                                        var NuevaPagina = parseInt(vp.ValorPagina) - diferencia;
                                        NuevaPagina = (isNaN(NuevaPagina)) ? vp.ValorPagina : NuevaPagina.toString();
                                        var ClasePaginaActiva = (self.NumeroPaginaActual() == NuevaPagina) ? "active" : "";
                                        respaldo.push(new PaginaSuperGridVM(NuevaPagina, ClasePaginaActiva));
                                    });
                                }

                                self.paginacionSuperGridVM.ListaPagina.removeAll();
                                self.paginacionSuperGridVM.ListaPagina(respaldo);
                            }
                            else {
                                var respaldo = [];

                                self.paginacionSuperGridVM.ListaPagina().forEach(function (vp, i) {
                                    var ClasePaginaActiva = (self.paginacionSuperGridVM.NumeroPaginaActual() == vp.ValorPagina) ? "active" : "";
                                    respaldo.push(new PaginaSuperGridVM(vp.ValorPagina, ClasePaginaActiva));
                                });

                                self.paginacionSuperGridVM.ListaPagina.removeAll();
                                self.paginacionSuperGridVM.ListaPagina(respaldo);
                            }
                        }
                        else
                            self.paginacionSuperGridVM.ListaPagina.removeAll();

                    }

                    function EjecutarCargarDatos() {
                        self.paginacionSuperGridVM.NumeroPaginaActual(1);
                        self.paginacionSuperGridVM.ListaPagina.removeAll();
                        self.paginacionSuperGridVM.RealizarCargarDatosPorRuta();
                    }

                    function LlenarListaCantidadRegistrosPorPagina() {

                        for (let i = 10; i <= 50; i = i + 10)
                            self.ListaCantidadPorPagina.push({ Cantidad: i });
                    }

                    //#endregion


                    //#region INICIO

                    if (self.gridVM.CantidadRegistrosPorPagina() > 0)
                        LlenarListaCantidadRegistrosPorPagina();

                    if (self.gridVM.CantidadRegistrosPorPagina() > 0)
                        LlenarListadoPaginas();

                    //#endregion

                }
            }

            class PaginacionSuperGridVM {
                constructor(gridVM) {

                    const self = this;

                    //#region Propiedades

                    self.gridVM = gridVM;
                    self.NumeroPaginaActual = ko.observable(1);
                    self.ListaPagina = ko.observableArray([]);

                    self.TextoRegistrosPiePagina = ko.pureComputed(function () {
                        var texto = "";

                        if (self.ListaPagina().length > 0)
                        {
                            var RegistroInicio = (((self.NumeroPaginaActual() * self.gridVM.CantidadRegistrosPorPagina()) - self.gridVM.CantidadRegistrosPorPagina()) + 1);
                            var RegistroFinal = (self.NumeroPaginaActual() * self.gridVM.CantidadRegistrosPorPagina());
                            RegistroFinal = (RegistroFinal > self.gridVM.CantidadTotalRegistros()) ? self.gridVM.CantidadTotalRegistros() : RegistroFinal;

                            texto = "De " + RegistroInicio.toLocaleString("es-NI") + " a " + RegistroFinal.toLocaleString("es-NI") + " registros de un total de " + self.gridVM.CantidadTotalRegistros().toLocaleString("es-NI") + " registros.";
                        }

                        return texto;
                    });

                    //#endregion


                    //#region Suscripciones                    

                    //#endregion


                    //#region Funciones Públicas

                    self.IrPaginaGrid = (pagina) =>
                    {
                        let ValorPaginaAnterior = self.NumeroPaginaActual();
                        let PaginaSeleccionada = 0;

                        switch (pagina.ValorPagina) {
                            case "<<":
                                PaginaSeleccionada = 1;
                                break;

                            case ">>":
                                var CantidadDePaginas = Math.ceil(self.gridVM.CantidadTotalRegistros() / self.gridVM.CantidadRegistrosPorPagina());
                                PaginaSeleccionada = CantidadDePaginas;
                                break;

                            default:
                                PaginaSeleccionada = parseInt(pagina.ValorPagina);
                                break;
                        }

                        self.NumeroPaginaActual(PaginaSeleccionada);

                        if (ValorPaginaAnterior != PaginaSeleccionada)
                            self.RealizarCargarDatosPorRuta();
                    }

                    self.RealizarCargarDatosPorRuta = () => {

                        window.GifCargaGrid(".cont-grid-ko", true);
                        var Parametros = JSON.parse(JSON.stringify(self.gridVM.ParametrosCargaDeDatos()));
                        Parametros.DatosFilas = [];

                        // SE AGREGAN PROPIEDADES PARA LA PAGINACIÓN
                        Parametros.CantidadRegistrosPorPagina = self.gridVM.CantidadRegistrosPorPagina();
                        Parametros.Pagina = self.NumeroPaginaActual();

                        $.getJSON(self.gridVM.UrlCargaDatos(), Parametros, function (rs) {
                            if (rs != undefined) {
                                self.gridVM.CantidadTotalRegistros(rs.CantidadTotalRegistros);
                                self.gridVM.DatosFilas(((rs.DatosFilas == null) ? [] : rs.DatosFilas));
                            }
                        }).always(function () {
                            window.ActivarTooltip();

                            setTimeout(function () {
                                window.GifCargaGrid(".cont-grid-ko", false);
                            }, 300);
                        });
                    }

                    //#endregion


                    //#region Funciones Privadas

                    //#endregion
                }
            }

            class PaginaSuperGridVM {
                constructor(ValorPagina, ClasePaginaActiva) {

                    const self = this;

                    //#region Propiedades

                    self.ValorPagina = ValorPagina || "";
                    self.ClasePaginaActiva = ClasePaginaActiva || ""

                    //#endregion


                    //#region Funciones Privadas

                    //#endregion
                }
            }


            // RETORNO
            return new SuperGridVM();
        }
    },
    template: { cargadorVista: "ComponentesKo/_SuperGrid", esParcial: true }
});



