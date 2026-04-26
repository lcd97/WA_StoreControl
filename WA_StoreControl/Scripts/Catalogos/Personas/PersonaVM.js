class PersonaVM {
    constructor(data) {
        data = data || {};
        const self = this;

        self.Id = ko.observable(data.Id || 0);
        self.Identificacion = ko.observable(data.Identificacion || "").extend({ onlyUpperCase: true });
        self.Nombres = ko.observable(data.Nombres || "").extend({ onlyUpperCase: true });
        self.Apellidos = ko.observable(data.Apellidos || "").extend({ onlyUpperCase: true });
        self.FechaNacimiento = ko.observable(data.FechaNacimiento || new Date().toLocaleDateString('es-ES'));
        self.NombreComercial = ko.observable(data.NombreComercial || "").extend({ onlyUpperCase: true });
        self.Direccion = ko.observable(data.Direccion || "");
        self.Telefono = ko.observable(data.Telefono || "");
        self.EsPersonaNatural = ko.observable(typeof (data.EsPersonaNatural) == "boolean" ? data.EsPersonaNatural : true);
        self.EsActivo = ko.observable(typeof (data.EsActivo) == "boolean" ? data.EsActivo : true);

        self.DetallesTelefono = ko.observableArray(data.DetallesTelefono ? data.DetallesTelefono.map(x => new DetalleTelefonoVM(x)) : []);
        self.Identidades = ko.observableArray(data.Identidades ? data.Identidades.map(x => new IdentidadVM(x)) : []);

        self.NombreCompleto = ko.computed(() => {
            return `${self.EsPersonaNatural() ? (self.Nombres() + ' ' + self.Apellidos()) : self.NombreComercial()}`;
        });

        self.CodigoIdentificacion = ko.computed(() => {
            return (self.Identidades().length > 0
                ? self.Identidades().map(item => {
                    return `${item.DescripcionTipoIdentificacion()} - ${item.Identificacion()}`;
                })
                : ["Sin Identificaciones"]
            ).join("\n");
        });
    }
} 