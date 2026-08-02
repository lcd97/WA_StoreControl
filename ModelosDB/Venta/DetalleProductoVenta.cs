using ModelosDB.Inventario;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelosDB.Venta
{
    [Table("DetallesProductoVenta", Schema = "ven")]
    public class DetalleProductoVenta
    {
        [Key]
        public int Id { get; set; }

        public int ProductoVentaId { get; set; }

        [Display(Name = "Producto")]
        public int ProductoId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public int Cantidad { get; set; }

        public virtual ProductoVenta ProductoVenta { get; set; }
        public virtual Producto Producto { get; set; }
    }
}
