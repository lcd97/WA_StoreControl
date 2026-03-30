
// (SOBREESCRITURA) VALIDACION PARA EL FORMATO DE FECHA: DIA/MES/AÑO

//$.validator.addMethod("date", function (value, element) {

//    if (this.optional(element))
//    {
//        return true;
//    }

//    var exito = true;

//    try {
//        $.datepicker.parseDate('dd/mm/yy', value);
//    }
//    catch (err) {
//        exito = false;
//    }

//    return exito;
//});


// REQUIERE DE UN VALOR U OTRO CAMPO 

jQuery.validator.unobtrusive.adapters.add('requiredif', ["campo", "criterio", "tipo"], function (options) {

    options.rules["requiredif"] = {
        campo: options.params.campo,
        criterio: options.params.criterio,
        tipo: options.params.tipo
    };

    options.messages["requiredif"] = options.message;

});

jQuery.validator.addMethod('requiredif', function (value, element, params) {

    var CriterioValido = false;
    var formularioPadre = $(element).parents("form");

    var ValorOtroCampo = $(formularioPadre).find("[name='" + params.campo + "']").val();

    params.tipo = params.tipo.toLowerCase();

    if (params.tipo != "" && params.criterio != "") {
        if (params.tipo == "string" || params.tipo == "bool") {
            CriterioValido = ValorOtroCampo.toString().toLowerCase() == params.criterio.toString().toLowerCase() ? true : false;
        }

        else if (params.tipo == "int") {
            CriterioValido = (parseFloat(ValorOtroCampo.toString()) == parseFloat(params.criterio.toString())) ? true : false;
        }
    }

    else if (params.tipo == "" && params.criterio == "") {
        CriterioValido = (ValorOtroCampo == null || ValorOtroCampo == undefined || ValorOtroCampo == "") ? false : true;
    }

    if (CriterioValido) {
        if (value == null || value == undefined || value == "") {
            return false;
        }
    }

    return true;
});


// COMPARADOR DE DOS CAMPOS (<,<=,>,>=,!=,==) 

jQuery.validator.unobtrusive.adapters.add('comparefield', ["campo", "criteriocomparacion", "tipodato"], function (options) {

    options.rules["comparefield"] = {
        campo: options.params.campo,
        criteriocomparacion: options.params.criteriocomparacion,
        tipodato: options.params.tipodato
    };

    options.messages["comparefield"] = options.message;

});

jQuery.validator.addMethod('comparefield', function (value, element, params) {

    var Exito = false;
    var ValorResultado1 = 0, ValorResultado2 = 0;
    var ValorResultadoFecha1, ValorResultadoFecha2;
    var ValorResultadoTimeSpan1, ValorResultadoTimeSpan2;

    var formularioPadre = $(element).parents("form");
    var ValorOtroCampo = $(formularioPadre).find("[name='" + params.campo + "']").val();

    value = value.toString().replace(/,/g, "");
    ValorOtroCampo = ValorOtroCampo.toString().replace(/,/g, "");

    if (value == "" || ValorOtroCampo == "" || value == undefined || ValorOtroCampo == undefined || value == null || ValorOtroCampo == null) {
        return true;
    }

    switch (params.tipodato.toString())
    {
        case "numero":
            ValorOtroCampo = (ValorOtroCampo == undefined || ValorOtroCampo == null || ValorOtroCampo == "") ? 0 : ValorOtroCampo;
            ValorResultado2 = parseFloat(ValorCampo.ToString());
            ValorResultado1 = parseFloat(value.ToString());
            break;
        case "fecha":
            ValorOtroCampo = (ValorOtroCampo == undefined || ValorOtroCampo == null || ValorOtroCampo == "") ? "" : ValorOtroCampo;

            ValorResultadoFecha1 = new Date(value.toString().split("/").reverse().join("/"));
            ValorResultadoFecha2 = new Date(ValorOtroCampo.toString().split("/").reverse().join("/"));

            ValorResultadoFecha1.setHours(0);
            ValorResultadoFecha1.setMinutes(0);
            ValorResultadoFecha1.setSeconds(0);
            ValorResultadoFecha1.setMilliseconds(0);

            ValorResultadoFecha2.setHours(0);
            ValorResultadoFecha2.setMinutes(0);
            ValorResultadoFecha2.setSeconds(0);
            ValorResultadoFecha2.setMilliseconds(0);
            break;
        case "tiempo":
            ValorOtroCampo = (ValorOtroCampo == undefined || ValorOtroCampo == null || ValorOtroCampo == "") ? "" : ValorOtroCampo;

            var PartesTiempo2 = ValorOtroCampo.split(":");
            PartesTiempo2[2] = (PartesTiempo2[2] == null || PartesTiempo2[2] == undefined) ? 0 : parseInt(PartesTiempo2[2]);

            ValorResultadoTimeSpan2 = new Date(0);
            ValorResultadoTimeSpan2.setHours(parseInt(PartesTiempo2[0]));
            ValorResultadoTimeSpan2.setMinutes(parseInt(PartesTiempo2[1]));
            ValorResultadoTimeSpan2.setSeconds(parseInt(PartesTiempo2[2]));

            var PartesTiempo1 = value.split(":");
            PartesTiempo1[2] = (PartesTiempo1[2] == null || PartesTiempo1[2] == undefined) ? 0 : parseInt(PartesTiempo1[2]);
            ValorResultadoTimeSpan1 = new Date(0);
            ValorResultadoTimeSpan1.setHours(parseInt(PartesTiempo1[0]));
            ValorResultadoTimeSpan1.setMinutes(parseInt(PartesTiempo1[1]));
            ValorResultadoTimeSpan1.setSeconds(parseInt(PartesTiempo1[2]));
            break;
    }

    switch (params.criteriocomparacion.toString())
    {
        case "<":
            switch (params.tipodato.toString()) {
                case "numero":
                    if (ValorResultado1 < ValorResultado2)
                        Exito = true;
                    break;
                case "fecha":
                    if (ValorResultadoFecha1 < ValorResultadoFecha2)
                        Exito = true;
                    break;
                case "tiempo":
                    if (ValorResultadoTimeSpan1 < ValorResultadoTimeSpan2)
                        Exito = true;
                    break;
            }
            break;
        case "<=":
            switch (params.tipodato.toString())
            {
                case "numero":
                    if (ValorResultado1 <= ValorResultado2)
                        Exito = true;
                    break;
                case "fecha":
                    if (ValorResultadoFecha1 <= ValorResultadoFecha2)
                        Exito = true;
                    break;
                case "tiempo":
                    if (ValorResultadoTimeSpan1 <= ValorResultadoTimeSpan2)
                        Exito = true;
                    break;
            }
            break;
        case ">":
            switch (params.tipodato.toString()) {
                case "numero":
                    if (ValorResultado1 > ValorResultado2)
                        Exito = true;
                    break;
                case "fecha":
                    if (ValorResultadoFecha1 > ValorResultadoFecha2)
                        Exito = true;
                    break;
                case "tiempo":
                    if (ValorResultadoTimeSpan1 > ValorResultadoTimeSpan2)
                        Exito = true;
                    break;
            }
            break;
        case ">=":
            switch (params.tipodato.toString()) {
                case "numero":
                    if (ValorResultado1 >= ValorResultado2)
                        Exito = true;
                    break;
                case "fecha":
                    if (ValorResultadoFecha1 >= ValorResultadoFecha2)
                        Exito = true;
                    break;
                case "tiempo":
                    if (ValorResultadoTimeSpan1 >= ValorResultadoTimeSpan2)
                        Exito = true;
                    break;
            }
            break;
        case "!=":
            switch (params.tipodato.toString()) {
                case "numero":
                    if (ValorResultado1 != ValorResultado2)
                        Exito = true;
                    break;
                case "fecha":
                    if (ValorResultadoFecha1 != ValorResultadoFecha2)
                        Exito = true;
                    break;
                case "tiempo":
                    if (ValorResultadoTimeSpan1 != ValorResultadoTimeSpan2)
                        Exito = true;
                    break;
            }
            break;
        case "==":
            switch (params.tipodato.toString()) {
                case "numero":
                    if (ValorResultado1 == ValorResultado2)
                        Exito = true;
                    break;
                case "fecha":
                    if (ValorResultadoFecha1 == ValorResultadoFecha2)
                        Exito = true;
                    break;
                case "tiempo":
                    if (ValorResultadoTimeSpan1 == ValorResultadoTimeSpan2)
                        Exito = true;
                    break;
            }
            break;
    }

    if (!Exito)
        return false;
    else
        return true;
});


// COMPARADOR DE CAMPO FECHA CON RESPECTO A LA FECHA ACTUAL (<,<=,>,>=,!=,==) 

jQuery.validator.unobtrusive.adapters.add('Compararconfechaactual', ["criteriocomparacion"], function (options) {

    options.rules["Compararconfechaactual"] = {
        criteriocomparacion: options.params.criteriocomparacion,
    };

    options.messages["Compararconfechaactual"] = options.message;

});

jQuery.validator.addMethod('Compararconfechaactual', function (value, element, params) {

    var Exito = false;
    var ValorResultadoFecha1, ValorResultadoFecha2;
    value = value.toString().replace(/,/g, "");

    if (isNaN(value) && value.toString().split("/").length == 3) {
        ValorResultadoFecha1 = new Date(value.toString().split("/").reverse().join("/"));
        ValorResultadoFecha2 = new Date();

        ValorResultadoFecha1.setHours(0);
        ValorResultadoFecha1.setMinutes(0);
        ValorResultadoFecha1.setSeconds(0);
        ValorResultadoFecha1.setMilliseconds(0);

        ValorResultadoFecha2.setHours(0);
        ValorResultadoFecha2.setMinutes(0);
        ValorResultadoFecha2.setSeconds(0);
        ValorResultadoFecha2.setMilliseconds(0);

        switch (params.criteriocomparacion) {
            case "<":
                if (ValorResultadoFecha1 < ValorResultadoFecha2)
                    Exito = true;
                break;
            case "<=":
                if (ValorResultadoFecha1 <= ValorResultadoFecha2)
                    Exito = true;
                break;
            case ">":
                if (ValorResultadoFecha1 > ValorResultadoFecha2)
                    Exito = true;
                break;
            case ">=":
                if (ValorResultadoFecha1 >= ValorResultadoFecha2)
                    Exito = true;
                break;
            case "!=":
                if (ValorResultadoFecha1 != ValorResultadoFecha2)
                    Exito = true;
                break;
            case "==":
                if (ValorResultadoFecha1 == ValorResultadoFecha2)
                    Exito = true;
                break;
        }
    }

    if (!Exito)
        return false;
    else
        return true;
});


// VALIDACION FORMATO TIEMPO

jQuery.validator.unobtrusive.adapters.addBool("tiempoformato");

jQuery.validator.addMethod('tiempoformato', function (value, element, params) {

    var Exito = true;
    var TiempoArreglo = value.split(":");
    var hora = parseInt(TiempoArreglo[0]);
    var minuto = parseInt(TiempoArreglo[1]);
    var segundo = (TiempoArreglo[2] == null || TiempoArreglo[2] == undefined)? 0 : parseInt(TiempoArreglo[2]);

    if (hora < 0 || hora > 24)
        Exito = false;
    else if (hora == 24 && (minuto > 0 || segundo > 0))
        Exito = false;
    if (minuto < 0 || minuto > 60)
        Exito = false;
    if (segundo < 0 || segundo > 60)
        Exito = false;

    return Exito;
});



// VALIDACION LISTA DE CHECKBOXS

jQuery.validator.unobtrusive.adapters.addBool("requiere_un_checkbok_seleccion");

jQuery.validator.addMethod('requiere_un_checkbok_seleccion', function (value, element, params) {

    var nameEl = $(element).attr("name");

    return $("[name='" + nameEl + "']:checked").length > 0? true : false;
});