using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WA_StoreControl.DTO
{
    public class IdentidadDTO
    {
        public int Id { get; set; }
        public string Identificacion { get; set; }
        public int TipoIdentificacionId { get; set; }
        public int PersonaId { get; set; }

        public string DescripcionTipoIdentificacion { get; set; }
    }
}