$(function(){

    HabilitarBotonImpresion();
 
});



function HabilitarBotonImpresion(MostrarBotonImpresion)
{
    
    var print = false;
    _raiz = ObtenerRaiz();
    // navigator.appName = (Netscape, Microsoft Internet Explorer)


    if (navigator.appName != "Microsoft Internet Explorer")
    {
        $("body").append("<div id='Actualizar' class='vista-actualizacion '> <div class='contenedor-gif'> <div class='img-impresora'></div> <div class='gif-imprimiendo'></div> </div> <div> <h1 class='textofirefox'> Presione \"Ctrl + P\" para Finalizar la Impresión </h1> </div> </div>");

        $("form").append("<div id='ContenedorPopupPdf' style='display:none'>" +
        "<div class='contd-pdf'>" +
        "<b class='btn-cerrar-iframe-pdf'> X </b>" +
        "<div class=contd-iframe-pdf>" +
        "<iframe id='IframeReporte' name='IframeReporte' class='ocultar-iframe tamañoiframPDF' runat='server'></iframe>" +
        "<div class='imgSeñalizacion'></div> <h3 class='ayudaimpresion'> Click! en la \"Flecha\", luego en \"Imprimir\" para imprimir el informe. </h3>" +
        "</div>" +
        "</div>" +
        "</div>");

         showPrintButton();
        
    }

    $("body").on("click", "#btnPrint", function () {
        printPDF();
    });


    $(".btn-cerrar-iframe-pdf").on("click", function () {

        $("#ContenedorPopupPdf").fadeOut('slow',function () {

            $("#IframeReporte").toggleClass("ocultar-iframe");
            $("#ContenedorPopupPdf").toggleClass("PopupPDF");

        });

    });


    $("#IframeReporte").on("load", function () {

        if (print) {

            if (navigator.userAgent.search("Firefox") >= 0) {
                $("#IframeReporte").toggleClass("ocultar-iframe");
                $("#ContenedorPopupPdf").toggleClass("PopupPDF");
                $("#Actualizar").hide();
                $("#ContenedorPopupPdf").fadeIn('slow');
            }
            else {
                window.frames['IframeReporte'].focus();
                $("#Actualizar").hide();
                window.frames["IframeReporte"].print();
                $("#IframeReporte").contents().html("");
            }
        }

        print = false;

    });


    $("#Actualizar").on("click", function () {

        window.frames['IframeReporte'].focus();

    });


    function printPDF() {

        var DireccionReporte = $("#frmReportViewer").attr("action");
        var DireccionGeneral = DireccionReporte.split("?")[0];

        $("#Actualizar").fadeIn('slow');
        $("#frmReportViewer").attr("action", _raiz + "/Reports/"+ DireccionGeneral + "?Imprimir=1");
        $("#frmReportViewer").trigger("submit");
        $("#frmReportViewer").attr("action", DireccionReporte);

        print = true;
    }


    function showPrintButton() {
        //var tablaActualizar = ($("table[title='Actualizar']").length > 0) ? $("table[title='Actualizar']") : $("table[title='Refresh']");

        //var table = tablaActualizar;
        //var parentTable = $(table).parents('table');
        //var parentDiv = $(parentTable).parents('div').parents('div').first();

        //if (parentDiv.find("input[value='Print']").length == 0) {
        //    parentDiv.append('<table cellpadding="0" cellspacing="0" toolbarspacer="true" class="tabla-btnPrint"><tbody><tr><td></td></tr></tbody></table>');
        //    parentDiv.append('<div id="customDiv" class="div-btnPrint"><table cellpadding="0" cellspacing="0"><tbody><tr><td><span id="btnPrint" class="btn-Print HighlightDiv"><span class="glyphui glyphui-print"></span></span></td></tr></tbody></table></div>'); //<img src="' + _raiz + '/Content/images/VistaPreviaImpresion.png" alt="Print Report" title="Print Report" width="25px" height="25px" class="img-btnPrint"/>
        //}ToolbarExport
        $('.ToolbarBack').after('<div class="ToolbarPrint WidgetSet" style="font-family:Verdana;font-size:8pt;vertical-align:top;display:inline-block;">' +
                '<table cellpadding="0" cellspacing="0" style="display:inline;"><tbody><tr>'+
                '<td height="28px"><div ><div id="btnPrint"  class="NormalButton" style=""><table title="Imprimir">' +
                '<tbody><tr><td><span  class="glyphui glyphui-print"></span></td>'+
                 '</tr></tbody> </table></div></div></td> </tr> </tbody></table></div>');
    }

}//Fin Funcion HabilitarBotonImpresion

    
function ObtenerRaiz() {

        var RaizGlobal;
        var Ruta = location.host;

        if (Ruta.indexOf("localhost") >= 0 && Ruta.indexOf(":") >= 0) {
            RaizGlobal = '';
        }
        else {
            var NombreRuta = window.location.pathname;
            var DirectorioVirtual = NombreRuta.split('/');
            RaizGlobal = DirectorioVirtual[1];
            RaizGlobal = '/' + RaizGlobal;
        }

        return RaizGlobal;

    }