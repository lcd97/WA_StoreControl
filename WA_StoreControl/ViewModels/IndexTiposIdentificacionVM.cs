using ModelosDB.General;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WA_StoreControl.Utilidades;

namespace WA_StoreControl.ViewModels
{
    public class IndexTiposIdentificacionVM
    {
        public IndexTiposIdentificacionVM()
        {
            this.TipoIdentificacion = new TipoIdentificacion();
            this.SearchTiposIdentificacionVM = new SearchTiposIdentificacionVM();
        }

        public TipoIdentificacion TipoIdentificacion { get; set; }
        public SearchTiposIdentificacionVM SearchTiposIdentificacionVM { get; set; }
    }

    public class SearchTiposIdentificacionVM : SearchViewModel
    {
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }
    }
}