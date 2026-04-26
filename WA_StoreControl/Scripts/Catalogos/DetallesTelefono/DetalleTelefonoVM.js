class DetalleTelefonoVM {
    constructor(data) {
        data = data || {};
        const self = this;

        self.Id = ko.observable(data.Id || 0);
        self.NumeroTelefonico = ko.observable(data.NumeroTelefonico || "");
        self.CompaniaTelefonicaId = ko.observable(data.CompaniaTelefonicaId || 0);
        self.PersonaId = ko.observable(data.PersonaId || 0);

        self.DescripcionCompania = ko.observable(data.DescripcionCompania || "");
    }
}