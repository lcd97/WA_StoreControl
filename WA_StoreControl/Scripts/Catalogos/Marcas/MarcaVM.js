class MarcaVM {
    constructor(data) {
        data = data || {};
        const self = this;

        self.Id = ko.observable(data.Id || 0);
        self.Codigo = ko.observable(data.Codigo || "MAR000");
        self.Descripcion = ko.observable(data.Descripcion || "");
        self.EsActivo = ko.observable(typeof (data.EsActivo) == "boolean" ? data.EsActivo : true);
    }
}