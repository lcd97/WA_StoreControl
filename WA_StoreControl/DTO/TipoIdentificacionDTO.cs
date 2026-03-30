using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WA_StoreControl.DTO
{
    public class TipoIdentificacionDTO
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public bool EsActivo { get; set; }
    }
}