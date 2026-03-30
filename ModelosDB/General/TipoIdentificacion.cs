using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ModelosDB.General
{
    [Table("TiposIdentificacion", Schema = "GEN")]
    public partial class TipoIdentificacion
    {
        public TipoIdentificacion()
        {
            this.Identidades = new HashSet<Identidad>();
        }

        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo '{0}' es obligatorio")]
        [StringLength(100, ErrorMessage = "La longitud no debe exceder los 100 caracteres")]
        [Display(Name = "Tipo Identificación")]
        public string Descripcion { get; set; }

        [Display(Name = "Estado")]
        public bool EsActivo { get; set; }

        //CLASES HIJAS
        public virtual ICollection<Identidad> Identidades { get; set; }
    }
}