using ModelosDB.General;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModelosDB.Inventario
{
    [Table("DetallesEntrada", Schema = "inv")]
    public partial class DetalleEntrada
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo es {0} obligatorio")]
        [Display(Name = "Cantidad")]
        public int Cantidad { get; set; }

        [Required(ErrorMessage = "El campo es {0} obligatorio")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "(0:c2)")]
        [Display(Name = "Precio")]
        public double Precio { get; set; }

        //DEFINCION DE FK
        public int EntradaId { get; set; }
        public int ProductoId { get; set; }

        //CLASE PADRE
        public virtual Entrada Entrada { get; set; }
        public virtual Producto Producto { get; set; }
    }
}