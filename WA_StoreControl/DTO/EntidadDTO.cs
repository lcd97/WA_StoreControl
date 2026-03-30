using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WA_StoreControl.DTO
{
    public class EntidadDTO
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
        public string Descripcion { get; set; }

        public int TipoIdentificacionId { get; set; }
        public string TipoIdentificacion { get; set; }
    }
}