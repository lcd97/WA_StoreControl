class SearchEntradaVM extends SearchViewModel {
    constructor(data) {
        super(data);
        data = data || {};
        const self = this;

        self.FechaDesde = ko.observable(data.FechaDesde || "");
        self.FechaHasta = ko.observable(data.FechaHasta || "");
    }
}