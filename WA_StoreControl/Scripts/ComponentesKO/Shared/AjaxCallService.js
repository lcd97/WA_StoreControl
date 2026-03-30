(function () {
    if (typeof (AppGlobal) != "undefined") {
        AppGlobal.Ajax = { // Peticion Generica
            CRUD: (params) => ExecuteAjaxCall(params), // Crear algun registro
            Get: (params) => ExecuteAjaxCall(params),       // Extraer todos los registros
            GetFilteredOrPaged: (params) => ExecuteAjaxCall(params), // Extrae todo ls registro paginados
            GetById: (params) => ExecuteAjaxCall(params), // Obtener la información de un registro mediante su id
        };
    }
})();

function ExecuteAjaxCall(params) {
    return $.ajax({
        url: AppGlobal.RootUrl + params.url,
        method: params.method || "Get",
        data: params.data || {},
        dataType: params.dataType || "json",
        beforeSend: params.beforeSend,
        complete: params.complete
    });
};