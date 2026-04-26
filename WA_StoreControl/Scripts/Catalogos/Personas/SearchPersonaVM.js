class SearchPersonaVM extends SearchViewModel {
    constructor(data) {
        super(data);
        data = data || {};
        const self = this;

        self.Nombres = ko.observable(data.Nombres || "");
        self.Identificacion = ko.observable(data.Identificacion || "");
    }
} 