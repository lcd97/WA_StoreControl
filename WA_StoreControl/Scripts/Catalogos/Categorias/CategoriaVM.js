class CategoriaVM {
    constructor(data) {
        data = data || {};
        const self = this;

        //TRABAJAN CON LOS CAMPOS DEL DTO
        self.Id = ko.observable(data.Id || 0);
        self.Codigo = ko.observable(data.Codigo || "");
        self.Descripcion = ko.observable(data.Descripcion || "");
        self.EsActivo = ko.observable(typeof (data.EsActivo) == "boolean" ? data.EsActivo : true);
    }
}