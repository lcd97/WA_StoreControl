class SearchProductoVM extends SearchViewModel {
    constructor(data) {
        super(data);
        data = data || {};
        const self = this;

        self.CategoriaId = ko.observable(data.CategoriaId || 0);
        self.SubCategoriaId = ko.observable(data.SubCategoriaId || 0);
        self.Descripcion = ko.observable(data.Descripcion || "");
    }
}