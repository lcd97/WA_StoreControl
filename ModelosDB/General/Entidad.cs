using ModelosDB.Inventario;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModelosDB.General
{
    [Table("Entidades", Schema = "GEN")]
    public partial class Entidad
    {
        public Entidad()
        {
            this.DetallesEntrada = new HashSet<DetalleEntrada>();
            this.Identidades = new HashSet<Identidad>();
            this.DetallesTelefono = new HashSet<DetalleTelefono>();
        }

        [Key]
        public int Id { get; set; }

        [StringLength(100, ErrorMessage = "La longitud no debe exceder los 100 caracteres")]
        [Display(Name = "Nombres")]
        public string Nombres { get; set; }

        [StringLength(100, ErrorMessage = "La longitud no debe exceder los 100 caracteres")]
        [Display(Name = "Apellidos")]
        public string Apellidos { get; set; }

        [Display(Name = "Fecha de Nacimiento")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime FechaNacimiento { get; set; }

        [StringLength(100, ErrorMessage = "La longitud no debe exceder los 100 caracteres")]
        [Display(Name = "Nombre Comercial")]
        public string NombreComercial { get; set; }

        [StringLength(150, ErrorMessage = "La longitud no debe exceder los 150 caracteres")]
        [Display(Name = "Dirección")]
        public string Direccion { get; set; }

        [Display(Name = "Estado")]
        public bool EsActivo { get; set; }

        //CLASES HIJAS
        public virtual ICollection<DetalleEntrada> DetallesEntrada { get; set; }
        public virtual ICollection<Identidad> Identidades { get; set; }
        public virtual ICollection<DetalleTelefono> DetallesTelefono { get; set; }
    }
}