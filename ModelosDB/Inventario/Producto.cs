using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModelosDB.Inventario
{
    [Table("Productos", Schema = "inv")]
    public partial class Producto
    {
        public Producto()
        {
            this.DetallesEntrada = new HashSet<DetalleEntrada>();
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

        //LLAVES FORANEAS
        [Display(Name = "SubCategoría")]
        public int SubCategoriaId { get; set; }

        [Display(Name = "Marca")]
        public int MarcaId { get; set; }

        //CLASES PADRES
        public virtual SubCategoria SubCategoria { get; set; }
        public virtual Marca Marca { get; set; }

        //CLASES HIJAS
        public virtual ICollection<DetalleEntrada> DetallesEntrada { get; set; }
    }
}