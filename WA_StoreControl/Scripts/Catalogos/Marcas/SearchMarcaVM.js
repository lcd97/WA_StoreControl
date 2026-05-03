class SearchMarcaVM extends SearchViewModel {
    constructor(data) {
        super(data);
        data = data || {};
        const self = this;

        self.Descripcion = ko.observable(data.Descripcion || "");
    }
}