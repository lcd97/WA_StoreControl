using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WA_StoreControl.DTO
{
    public class PersonaDTO
    {
        public int Id { get; set; }
        public string Identificacion { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string FechaNacimiento { get; set; }
        public string NombreComercial { get; set; }
        public string Direccion { get; set; }
        public string Telefono { get; set; }
        public bool EsActivo { get; set; }
        public bool EsPersonaNatural { get; set; }

        public int TipoIdentificacionId { get; set; }
        public string TipoIdentificacion { get; set; }

        public List<DetalleTelefonoDTO> DetallesTelefono { get; set; }
        public List<IdentidadDTO> Identidades { get; set; }
    }
}