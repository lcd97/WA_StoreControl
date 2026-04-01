using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ModelosDB.General
{
    [Table("Identidades", Schema = "gen")]
    public class Identidad
    {
        [Key]
        public int Id { get; set; }

        [StringLength(14, MinimumLength = 14, ErrorMessage = "La longitud debe ser de 14 caracteres")]
        [Display(Name = "Identificación")]
        public string Identificacion { get; set; }

        [Display(Name = "Tipo de Identificación")]
        [Required(ErrorMessage = "El campo '{0}' es obligatorio")]
        public int TipoIdentificacionId { get; set; }

        [Required(ErrorMessage = "El campo '{0}' es obligatorio")]
        public int EntidadId { get; set; }

        public virtual Persona Persona { get; set; }
        public virtual TipoIdentificacion TipoIdentificacion { get; set; }
    }
}