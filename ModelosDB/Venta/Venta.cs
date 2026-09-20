using ModelosDB.General;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModelosDB.Venta
{
    [Table("Ventas", Schema = "ven")]
    public partial class Venta
    {
        public Venta()
        {
            this.DetallesVenta = new HashSet<DetalleVenta>();
        }

        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo es {0} obligatorio")]
        [StringLength(15, MinimumLength = 15, ErrorMessage = "La longitud debe ser de 15 caracteres")]
        [Display(Name = "Código")]
        public string Codigo { get; set; }

        [Required(ErrorMessage = "El campo es {0} obligatorio")]
        [DataType(DataType.DateTime)]
        [Display(Name = "Fecha de venta")]
        public DateTime FechaVenta { get; set; }

        [Display(Name = "Estado")]
        public bool EsActivo { get; set; }

        [Display(Name = "Cliente")]
        public int ClienteId { get; set; }

        [Required(ErrorMessage = "El campo es {0} obligatorio")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "(0:c2)")]
        [Display(Name = "Subtotal")]
        public decimal Subtotal { get; set; }

        [Required(ErrorMessage = "El campo es {0} obligatorio")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "(0:c2)")]
        [Display(Name = "Descuento")]
        public decimal Descuento { get; set; }

        [Required(ErrorMessage = "El campo es {0} obligatorio")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "(0:c2)")]
        [Display(Name = "Total")]
        public decimal TotalVenta { get; set; }

        [Display(Name = "Motivo de anulación")]
        [StringLength(250, ErrorMessage = "La longitud debe ser de 250 caracteres")]
        public string MotivoAnulacion { get; set; }

        public virtual Persona Cliente { get; set; }

        //CLASE HIJA
        public virtual ICollection<DetalleVenta> DetallesVenta { get; set; }
    }
}