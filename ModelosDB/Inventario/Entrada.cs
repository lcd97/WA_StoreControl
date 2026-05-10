using ModelosDB.General;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModelosDB.Inventario
{
    [Table("Entradas", Schema = "inv")]
    public partial class Entrada
    {
        public Entrada()
        {
            this.DetallesEntrada = new HashSet<DetalleEntrada>();
        }

        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo es {0} obligatorio")]
        [StringLength(15, MinimumLength = 3, ErrorMessage = "La longitud debe ser de 3 dígitos")]
        [Display(Name = "Código")]
        public string Codigo { get; set; }

        [Required(ErrorMessage = "El campo es {0} obligatorio")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de entrada")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime FechaEntrada { get; set; }

        [Display(Name = "Estado")]
        public bool EsActivo { get; set; }

        [Display(Name = "Proveedor")]
        public int ProveedorId { get; set; }

        [Required(ErrorMessage = "El campo es {0} obligatorio")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "(0:c2)")]
        [Display(Name = "Total")]
        public double TotalEntrada { get; set; }

        public virtual Persona Proveedor { get; set; }

        //CLASE HIJA
        public virtual ICollection<DetalleEntrada> DetallesEntrada { get; set; }
    }
}