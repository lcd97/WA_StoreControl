using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModelosDB.Venta
{
    [Table("DetallesVenta", Schema = "ven")]
    public partial class DetalleVenta
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Cantidad")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public int Cantidad { get; set; }

        [Required(ErrorMessage = "El campo es {0} obligatorio")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "(0:c2)")]
        [Display(Name = "Precio Unitario")]
        public decimal PrecioUnitario { get; set; }

        [Display(Name = "Tipo de Precio")]
        public TipoPrecio TipoPrecio { get; set; }

        [Required(ErrorMessage = "El campo es {0} obligatorio")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "(0:c2)")]
        [Display(Name = "Subtotal")]
        public decimal Subtotal { get; set; }

        //DEFINCION DE FK
        public int VentaId { get; set; }
        public int ProductoVentaId { get; set; }

        //CLASE PADRE
        public virtual Venta Venta { get; set; }
        public virtual ProductoVenta ProductoVenta { get; set; }
    }
}