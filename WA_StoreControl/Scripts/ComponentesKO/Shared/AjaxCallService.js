(function () {
    if (typeof (AppGlobal) != "undefined") {
        AppGlobal.Ajax = { // Peticion Generica
            CRUD: (params) => ExecuteAjaxCall(params), // Crear algun registro
            Get: (params) => ExecuteAjaxCall(params),       // Extraer todos los registros
            GetFilteredOrPaged: (params) => ExecuteAjaxCall(params), // Extrae todo ls registro paginados
            GetById: (params) => ExecuteAjaxCall(params), // Obtener la información de un registro mediante su id
        };

        // Alias global histórico para compatibilidad con código que utiliza `Ajax.*`
        try {
            window.Ajax = AppGlobal.Ajax;
        } catch (e) {
            console.warn("No se pudo exponer el alias global Ajax:", e);
        }
    } else
        console.log("Error en el app global");
})();

function buildRequestUrl(relativeUrl) {
    // Si ya es una URL absoluta, retornar tal cual
    if (!relativeUrl) return (AppGlobal && AppGlobal.RootUrl) || "/";
    if (/^(?:https?:)?\/\//i.test(relativeUrl)) return relativeUrl;

    var root = (AppGlobal && AppGlobal.RootUrl) ? AppGlobal.RootUrl.toString() : "/";
    // Asegurar que root termina con una sola barra
    root = root.endsWith("/") ? root : root + "/";
    // Quitar barras iniciales de la parte relativa
    relativeUrl = relativeUrl.replace(/^\/+/, "");
    return root + relativeUrl;
}

function ExecuteAjaxCall(params) {
    return $.ajax({
        url: buildRequestUrl(params.url),
        method: params.method || "Get",
        data: params.data || {},
        dataType: params.dataType || "json",
        beforeSend: params.beforeSend,
        complete: params.complete
    });
};