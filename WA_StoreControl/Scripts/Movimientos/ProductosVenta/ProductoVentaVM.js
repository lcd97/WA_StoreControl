class ProductoVentaVM {
    constructor(data) {
        data = data || {};
        const self = this;

        self.Id = ko.observable(data.Id || 0);
        self.Nombre = ko.observable(data.Nombre || "").extend({ onlyUpperCase: true });
        self.Descripcion = ko.observable(data.Descripcion || "").extend({ onlyUpperCase: true });
        self.PrecioVenta = ko.observable(data.PrecioVenta || 0);
        self.PrecioMayor = ko.observable(data.PrecioMayor || 0);
        self.PrecioDescuento = ko.observable(data.PrecioDescuento || 0);
        self.FechaInicio = ko.observable(data.FechaInicio || new Date().toLocaleDateString('es-ES'));
        self.FechaFin = ko.observable(data.FechaFin || new Date().toLocaleDateString('es-ES'));
        self.EsActivo = ko.observable(typeof (data.EsActivo) == "boolean" ? data.EsActivo : true);
        self.EsDescuentoActivo = ko.observable(typeof (data.EsDescuentoActivo) == "boolean" ? data.EsDescuentoActivo : true);

        self.DetallesProductoVenta = ko.observableArray(data.DetallesProductoVenta ?
            data.DetallesProductoVenta.map(x => new DetalleProductoVenta(x)) : []);

        self.PrecioVentaText = ko.pureComputed(() => {
            return formato.format(parseFloat(self.PrecioVenta()) ?? 0);
        });

        self.PrecioMayorText = ko.pureComputed(() => {
            return formato.format(parseFloat(self.PrecioMayor()) ?? 0);
        });

        self.PrecioDescuentoText = ko.pureComputed(() => {
            return formato.format(parseFloat(self.PrecioDescuento()) ?? 0);
        });
    }
}

class DetalleProductoVenta {
    constructor(data) {
        data = data || {};
        const self = this;

        self.Id = ko.observable(data.Id || 0);
        self.ProductoId = ko.observable(data.ProductoId || 0);
        self.ProductoVentaId = ko.observable(data.ProductoVentaId || 0);
        self.Cantidad = ko.observable(data.Cantidad || 1);
        self.DescripcionProducto = ko.observable(data.DescripcionProducto || "");
        self.MostrarError = ko.observable(false);

        self.EsInvalido = ko.pureComputed(() => {
            if (!self.MostrarError())
                return false;

            return self.ProductoId() <= 0 ||
                self.Cantidad() <= 0
        });
    }
}