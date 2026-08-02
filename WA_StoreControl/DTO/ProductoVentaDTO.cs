using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WA_StoreControl.DTO
{
    public class ProductoVentaDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public double PrecioVenta { get; set; }
        public double PrecioMayor { get; set; }
        public double PrecioDescuento { get; set; }
        public bool EsActivo { get; set; }

        public string FechaInicio { get; set; }
        public string FechaFin { get; set; }

        public ICollection<DetalleProductoVentaDTO> DetallesProductoVenta { get; set; }
    }

    public class DetalleProductoVentaDTO
    {
        public int Id { get; set; }
        public int ProductoId { get; set; }
        public int ProductoVentaId { get; set; }
        public int Cantidad { get; set; }

        public string DescripcionProducto { get; set; }
    }
}