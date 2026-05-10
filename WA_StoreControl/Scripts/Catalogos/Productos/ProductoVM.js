class ProductoVM {
    constructor(data) {
        data = data || {};
        const self = this;

        self.Id = ko.observable(data.Id || 0);
        self.Codigo = ko.observable(data.Codigo || "");
        self.Descripcion = ko.observable(data.Descripcion || "");
        self.DescripcionMarca = ko.observable(data.DescripcionMarca || "");
        self.SubCategoriaId = ko.observable(data.SubCategoriaId || 0);
        self.DescripcionCategoria = ko.observable(data.DescripcionCategoria || 0);
        self.DescripcionSubCategoria = ko.observable(data.DescripcionSubCategoria || 0);
        self.EsActivo = ko.observable(typeof (data.EsActivo) == "boolean" ? data.EsActivo : true);

        self.MarcaId = ko.observable(data.MarcaId || 1);

        self.CategoriaYSubCategoria = ko.computed(() => {
            return `${self.DescripcionCategoria()} - ${self.DescripcionSubCategoria()}`;
        });

        self.DescripcionProducto = ko.observable(data.DescripcionProducto || "");
    }
}