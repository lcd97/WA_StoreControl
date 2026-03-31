class SubCategoriaVM {
    constructor(data) {
        data = data || {};
        const self = this;

        self.Id = ko.observable(data.Id || 0);
        self.Codigo = ko.observable(data.Codigo || "");
        self.Descripcion = ko.observable(data.Descripcion || "");
        self.EsActivo = ko.observable(typeof (data.EsActivo) == "boolean" ? data.EsActivo : true);

        self.CategoriaId = ko.observable(data.CategoriaId || 0);
        self.DescripcionCategoria = ko.observable(data.DescripcionCategoria || "");

        self.CategoriaYSubCategoria = ko.computed(() => {
            return `${self.DescripcionCategoria()} - ${self.Descripcion()}`;
        });
    }
}