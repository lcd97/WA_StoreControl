class EntradaVM {
    constructor(data) {
        data = data || {};
        const self = this;

        self.Id = ko.observable(data.Id || 0);
        self.Codigo = ko.observable(data.Codigo || "000");
        self.FechaEntrada = ko.observable(data.FechaEntrada || new Date().toLocaleDateString('es-ES'));
        self.EsActivo = ko.observable(typeof (data.EsActivo) == "boolean" ? data.EsActivo : true);
        self.ProveedorId = ko.observable(data.ProveedorId || 0);
        self.TotalEntrada = ko.observable(data.TotalEntrada || 0);

        self.NombreProveedor = ko.observable(data.NombreProveedor || "");
        self.MotivoAnulacion = ko.observable(data.MotivoAnulacion || "");

        self.DetallesEntrada = ko.observableArray(data.DetallesEntrada ? data.DetallesEntrada.map(x => new DetalleEntrada(x)) : []);

        self.Total = ko.pureComputed(() => {
            return formato.format(self.DetallesEntrada().reduce((suma, item) => {

                let cantidad = parseFloat(item.Cantidad()) || 0;
                let precio = parseFloat(item.Precio()) || 0;

                return suma + (cantidad * precio);

            }, 0));
        });

        self.Total.subscribe((valor) => {
            self.TotalEntrada(parseFloat(valor.replace(/,/g, '')));
        });
    }
}

class DetalleEntrada {
    constructor(data) {
        data = data || this;
        const self = this;

        self.Id = ko.observable(data.Id || 0);
        self.Cantidad = ko.observable(data.Cantidad || 1);
        self.Precio = ko.observable(data.Precio || "");
        self.ProductoId = ko.observable(data.ProductoId || 0);
        self.DescripcionProducto = ko.observable(data.DescripcionProducto || "");
        self.EntradaId = ko.observable(data.EntradaId || 0);

        self.Total = ko.pureComputed(() => { return formato.format(parseFloat(self.Cantidad() * self.Precio())) });
        self.PrecioUnitario = ko.pureComputed(() => { return formato.format(parseFloat(self.Precio())) });
        self.MostrarError = ko.observable(false);

        self.EsInvalido = ko.pureComputed(() => {
            if (!self.MostrarError())
                return false;

            return self.ProductoId() <= 0 ||
                self.Cantidad() <= 0 ||
                self.Precio() <= 0;
        });
    }
}