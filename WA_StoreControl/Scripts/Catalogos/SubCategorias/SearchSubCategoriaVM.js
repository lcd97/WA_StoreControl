class SearchSubCategoriaVM extends SearchViewModel {
    constructor(data) {
        super(data);
        data = data || {};
        const self = this;

        self.CategoriaId = ko.observable(data.CategoriaId || 0);
    }
}