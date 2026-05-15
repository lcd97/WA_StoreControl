using ModelosDB.Interfaces;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModelosDB.Inventario
{
    [Table("Categorias", Schema = "inv")]
    public partial class Categoria : ICodeEntity
    {
        public Categoria()
        {
            this.SubCategorias = new HashSet<SubCategoria>();
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

        //CLASES HIJAS
        public virtual ICollection<SubCategoria> SubCategorias { get; set; }
    }
}