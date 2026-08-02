using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelosDB.Venta
{
    [Table("ProductosVenta", Schema = "ven")]
    public class ProductoVenta
    {
        public ProductoVenta()
        {
            this.DetallesProductoVenta = new HashSet<DetalleProductoVenta>();
        }

        [Key]
        public int Id { get; set; }

        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "El campo '{0}' es obligatorio")]
        [StringLength(250, ErrorMessage = "La longitud debe no debe exceder los 250 caracteres")]
        public string Nombre { get; set; }

        [Display(Name = "Descripción")]
        [Required(ErrorMessage = "El campo '{0}' es obligatorio")]
        [StringLength(250, ErrorMessage = "La longitud debe no debe exceder los 250 caracteres")]
        public string Descripcion { get; set; }

        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "(0:c2)")]
        [Display(Name = "Precio Venta")]
        public double PrecioVenta { get; set; }

        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "(0:c2)")]
        [Display(Name = "Precio Mayor")]
        public double PrecioMayor { get; set; }

        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "(0:c2)")]
        [Display(Name = "Precio en Descuento")]
        public double PrecioDescuento { get; set; }

        [Display(Name = "Estado")]
        public bool EsActivo { get; set; }

        [Display(Name = "Fecha Inicio Vigencia")]
        [DataType(DataType.DateTime)]
        public DateTime FechaInicio { get; set; }

        [Display(Name = "Fecha Fin Vigencia")]
        [DataType(DataType.DateTime)]
        public DateTime FechaFin { get; set; }

        public virtual ICollection<DetalleProductoVenta> DetallesProductoVenta { get; set; }
    }
}
