using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ModelosDB.General
{
    [Table("DetallesTelefono", Schema = "gen")]
    public class DetalleTelefono
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [StringLength(8, ErrorMessage = "La longitud no debe exceder los 8 caracteres")]
        [Display(Name = "Número")]
        public string NumeroTelefonico { get; set; }

        [Display(Name = "Compañía")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public int CompaniaTelefonicaId { get; set; }

        public int PersonaId { get; set; }

        public virtual Persona Persona { get; set; }
        public virtual CompaniaTelefonica CompaniaTelefonica { get; set; }
    }
}