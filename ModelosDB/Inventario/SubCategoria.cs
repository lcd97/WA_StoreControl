using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModelosDB.Inventario
{
    [Table("SubCategorias", Schema = "INV")]
    public partial class SubCategoria
    {
        public SubCategoria()
        {
            this.Productos = new HashSet<Producto>();
        }

        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo '{0}' es obligatorio")]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "La longitud debe ser de 3 caracteres")]
        [Display(Name = "Código")]
        public string Codigo { get; set; }

        [Required(ErrorMessage = "El campo '{0}' es obligatorio")]
        [StringLength(100, ErrorMessage = "La longitud no debe exceder los 100 caracteres")]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }

        [Display(Name = "Estado")]
        public bool EsActivo { get; set; }

        [Display(Name = "Categoría")]
        public int CategoriaId { get; set; }

        //CLASES PADRES
        public virtual Categoria Categoria { get; set; }

        //CLASES HIJAS
        public virtual ICollection<Producto> Productos { get; set; }
    }
}