using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WA_StoreControl.DTO
{
    public class DetalleTelefonoDTO
    {
        public int Id { get; set; }
        public int NumeroTelefonico { get; set; }
        public int CompaniaTelefonicaId { get; set; }
        public int PersonaId { get; set; }

        public string DescripcionCompania { get; set; }
    }
}