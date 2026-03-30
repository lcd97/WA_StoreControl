

//#region === VIEWMODELS ===

function GridVM() {

    var self_grid = this;

    self_grid.DatosColumnas = ko.observableArray();
    self_grid.DatosFilas = ko.observableArray();
    self_grid.CantidadRegistrosPorPagina = ko.observable(10);
    self_grid.CantidadTotalRegistros = ko.observable(0);
    self_grid.UrlCargaDatos = ko.observable("");
    self_grid.ParametrosCargaDeDatos = ko.observable();
    self_grid.EjecutarCargarDatos = ko.observable();
    self_grid.CuerpoTablaScroll = ko.observable(false);
    self_grid.AutoCargaDatos = ko.observable(false);

    self_grid.OcultarUtilidadGrid = ko.observable(false);
    self_grid.OcultarUtilidadGridBusquedaPorPagina = ko.observable(false);
    self_grid.OcultarUtilidadGridMostrarColumnas = ko.observable(false);
}

function GridColumnaVM(_CampoColumna, _TextoColumna, _AlineacionColumna, _AnchoColumna, _OcultarColumna, _FormatoColumna, _WordBreak, _TieneSumatoria, _AliasCampoSumatoria) {

    var self_col = this;

    self_col.CampoColumna = ko.observable(_CampoColumna);
    self_col.TextoColumna = ko.observable(_TextoColumna);
    self_col.AlineacionColumna = ko.observable(_AlineacionColumna); 
    self_col.AnchoColumna = ko.observable(_AnchoColumna);
    self_col.OcultarColumna = ko.observable(_OcultarColumna || true);
    self_col.FormatoColumna = ko.observable(_FormatoColumna);
    self_col.WordBreak = ko.observable(((_WordBreak == undefined) ? false : _WordBreak));
    self_col.TieneSumatoria = ko.observable(_TieneSumatoria);
    self_col.AliasCampoSumatoria = ko.observable(_AliasCampoSumatoria);

    self_col.ColumnaSumatoria = function (ListaFila, Col) {

        var CampoColumna = (Col.AliasCampoSumatoria() != null && Col.AliasCampoSumatoria() != undefined && Col.AliasCampoSumatoria() != "") ? Col.AliasCampoSumatoria() : Col.CampoColumna();

        var ListaCampoSumatoria = ListaFila.map(function (obj) {
            return obj[CampoColumna];
        }); 

        var Sumatoria = ListaCampoSumatoria.reduce(function (previo, actual) { return previo + ((!isNaN(actual))? actual : 0) }, 0);

        return parseFloat(Sumatoria).toLocaleString("es");
    };

    self_col.ClaseAlineacionColumna = ko.pureComputed(function () {

        var ClaseAlineacionColumna = "";
        self_col.AlineacionColumna((self_col.AlineacionColumna() != undefined) ? self_col.AlineacionColumna().trim().toLowerCase() : self_col.AlineacionColumna());

        switch (self_col.AlineacionColumna()) {
            case "izquierda":
                ClaseAlineacionColumna = "text-start";
                break;
            case "centro":
                ClaseAlineacionColumna = "text-center";
                break;
            case "derecha":
                ClaseAlineacionColumna = "text-end";
                break;
            default:
                ClaseAlineacionColumna = "text-start";
        }

        return ClaseAlineacionColumna;
    });

    self_col.ClaseWordBreak = ko.pureComputed(function () { return ((self_col.WordBreak()) ? "word-break" : ""); });
}

    //#endregion



