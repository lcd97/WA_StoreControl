

ko.bindingHandlers.grid = {
    update: function (element, valueAccessor, allBindings) {

        // SE OBTIENE EL OBJETO GRID NECESARIO PARA CONTRUIR EL GRID
        var ObjGrid = ko.toJS(valueAccessor());

        // SE CREA LA ESTRUCTURA INTERNA DE ELEMENTOS DEL GRID
        $(element).append("<thead><tr></tr></thead><tbody></tbody>");

        // SE RECORRE EL ARREGLO DE DATOS DE COLUMNA Y SE AGREGA LA PROPIEDAD PARA ESPECIFICAR LA CLASE DE ALINEACIÓN DE LA CELDA
        ObjGrid.DatosColumnas = $.map(ObjGrid.DatosColumnas, function (obj, i) {

            obj.ClaseAlineacionColumna = "";
            obj.AlineacionColumna = (obj.AlineacionColumna != undefined) ? obj.AlineacionColumna.trim().toLowerCase() : obj.AlineacionColumna;

            switch (obj.AlineacionColumna) {
                case "izquierda":
                    obj.ClaseAlineacionColumna = "text-left";
                    break;
                case "centro":
                    obj.ClaseAlineacionColumna = "text-center";
                    break;
                case "derecha":
                    obj.ClaseAlineacionColumna = "text-right";
                    break;
            }

            return obj;
        });

        // SE CREAN LOS ENCABEZADOS DE COLUMNAS ESPECIFICADOS EN EL OBJETO GRID
        var EncabezadosGrid = $.map(ObjGrid.DatosColumnas, function (obj, i) {

            var ClaseOcultar = (obj.OcultarColumna == true) ? "d-none" : "";
            var Clases = " text-center " + ClaseOcultar;

            return "<th class='" + Clases.trim() + "'>" + obj.TextoColumna + "</th>";

        }).join("");

        // SE AGREGA EL HTML DE LOS ENCABEZADOS DE COLUMNAS EN LA ESTRUCTURA INTERNA DE CABECERA DEL GRID
        $(element).find("thead tr").append(EncabezadosGrid);

        var FilasGrid = "";
        var CeldaGrid = "";

        // SE CREAN LAS CELDAS DE LAS FILAS ESPECIFICADOS EN EL OBJETO GRID Y ADEMAS SE AGREGAN LAS PROPIEDADES DE ESTILOS DE LAS CELDAS
        ObjGrid.DatosFilas.forEach(function (objf, i) {

            FilasGrid += "<tr>";
            CeldaGrid = "";

            ObjGrid.DatosColumnas.forEach(function (objc, ii) {

                var ClaseOcultar = (objc.OcultarColumna == true) ? "d-none" : "";
                var EstiloAncho = (objc.AnchoColumna != undefined && objc.AnchoColumna != "") ? "style='width:" + objc.AnchoColumna + "'" : "";

                var Clases = " " + objc.ClaseAlineacionColumna + " " + ClaseOcultar;
                var ValorFormato = (typeof objc.FormatoColumna === 'function') ? objc.FormatoColumna(objf) : undefined;
                var ValorCelda = (ValorFormato == undefined) ? ((objc.CampoColumna == "") ? "" : objf[objc.CampoColumna]) : ValorFormato;

                CeldaGrid += "<td class='" + Clases.trim() + "' " + EstiloAncho + ">" + ValorCelda + "</td>";
            });

            FilasGrid += CeldaGrid + "</tr>";

        });

        // SE AGREGA EL HTML DE LAS FILAS CON SUS CELDAS EN LA ESTRUCTURA INTERNA DEL CUERPO DEL GRID
        $(element).find("tbody").append(FilasGrid);

    }
};

ko.bindingHandlers.checkedTabLink = {
    init: function (element, valueAccessor, allBindings) {

        var Valor = valueAccessor();
        if (Valor() == null || Valor() == undefined) {
            var href = $('a[data-toggle="tab"].active').attr("href").replace("#", "");
            Valor(href);
        }
    },
    update: function (element, valueAccessor, allBindings) {

        var Valor = valueAccessor();
        $(element).parents(".nav-tabs").find("a[href='#" + Valor() + "']").tab('show');

        $('a[data-toggle="tab"]').on('shown.bs.tab', function (event) {
            var href = $(event.target).attr("href").replace("#", "");
            Valor(href);
        });
    }
};

//Custom Binding para cambiar el comportamiento del binding html que no aplica bindings a los elementos inyectados al DOM...
ko.bindingHandlers.htmlBound = {
    init: function () {
        return { controlsDescendantBindings: true };
    },
    update: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
        ko.utils.setHtml(element, valueAccessor());
        ko.applyBindingsToDescendants(bindingContext, element);
    }
};

ko.bindingHandlers.SlideVisible = {
    init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
        var obj = ko.toJS(valueAccessor());
        $(element).toggle(obj.Visible);
    },
    update: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
        var obj = ko.toJS(valueAccessor());

        if (obj.Visible == true)
            $(element).slideDown(obj.Duracion || 400);
        else
            $(element).slideUp(obj.Duracion || 400);
    }
};

ko.bindingHandlers.select2Value = {
    init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
        var Prop = valueAccessor();
        var valor = Prop();
        valor = valor == 0 ? null : valor; 
        $(element).val(Prop()); 
        $(element).trigger('change');
    },
    update: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
        var Prop = valueAccessor();
        var valor = Prop();
        valor = valor == 0 ? null : valor; 

        // Evento de cambio del select
        $(element).on("change", function () {
            valor = $(this).val();
            Prop(valor);
        });

        $(element).val(valor);
        $(element).trigger('change');
        Prop(valor);
    }
};