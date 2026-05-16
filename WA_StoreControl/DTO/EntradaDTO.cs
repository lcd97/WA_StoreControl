using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WA_StoreControl.DTO
{
    public class EntradaDTO
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public string FechaEntrada { get; set; }
        public int ProveedorId { get; set; }
        public bool EsActivo { get; set; }
        public float TotalEntrada { get; set; }
        public string MotivoAnulacion { get; set; }

        public string NombreProveedor { get; set; }

        public List<DetalleEntradaDTO> DetallesEntrada { get; set; }
    }

    public class DetalleEntradaDTO
    {
        public int Id { get; set; }
        public double Cantidad { get; set; }
        public double Precio { get; set; }
        public int EntradaId { get; set; }
        public int ProductoId { get; set; }
        public string DescripcionProducto { get; set; }
    }
}