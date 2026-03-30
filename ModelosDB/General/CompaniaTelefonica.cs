using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ModelosDB.General
{
    [Table("CompaniaTelefonica", Schema = "GEN")]
    public class CompaniaTelefonica
    {
        public CompaniaTelefonica()
        {
            this.DetallesTelefono = new HashSet<DetalleTelefono>();
        }

        [Key]
        public int Id { get; set; }

        [StringLength(80, ErrorMessage = "La longitud no debe exceder los 80 caracteres")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }

        [Display(Name = "Estado")]
        public bool EsActivo { get; set; }

        public virtual ICollection<DetalleTelefono> DetallesTelefono { get; set; }
    }
}