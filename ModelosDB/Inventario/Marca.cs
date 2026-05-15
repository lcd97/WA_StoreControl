using ModelosDB.Interfaces;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModelosDB.Inventario
{
    [Table("Marcas", Schema = "inv")]
    public partial class Marca : ICodeEntity
    {
        public Marca()
        {
            this.Productos = new HashSet<Producto>();
        }

        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo '{0}' es obligatorio")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "La longitud debe ser de 6 caracteres")]
        [Display(Name = "Código")]
        public string Codigo { get; set; }

        [Required(ErrorMessage = "El campo '{0}' es obligatorio")]
        [StringLength(100, ErrorMessage = "La longitud no debe exceder los 100 caracteres")]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }

        [Display(Name = "Estado")]
        public bool EsActivo { get; set; }

        public virtual ICollection<Producto> Productos { get; set; }
    }
}
