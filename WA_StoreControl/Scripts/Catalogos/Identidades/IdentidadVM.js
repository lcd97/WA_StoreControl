class IdentidadVM {
    constructor(data) {
        data = data || {};
        const self = this;

        self.Id = ko.observable(data.Id || 0);
        self.Identificacion = ko.observable(data.Identificacion || "").extend({ onlyUpperCase: true });
        self.TipoIdentificacionId = ko.observable(data.TipoIdentificacionId || 0);
        self.PersonaId = ko.observable(data.PersonaId || 0);

        self.DescripcionTipoIdentificacion = ko.observable(data.DescripcionTipoIdentificacion || "");
    }
}