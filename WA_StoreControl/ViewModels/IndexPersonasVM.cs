using ModelosDB.General;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WA_StoreControl.DTO;
using WA_StoreControl.Utilidades;

namespace WA_StoreControl.ViewModels
{
    public class IndexPersonasVM
    {
        public IndexPersonasVM()
        {
            this.Persona = new Persona();
            this.TiposIdentificacion = new List<TipoIdentificacionDTO>();
            this.CompaniasTelefonica = new List<CompaniaTelefonicaDTO>();
            this.SearchPersonasVM = new SearchPersonasVM();
        }

        public Persona Persona { get; set; }
        public ICollection<TipoIdentificacionDTO> TiposIdentificacion { get; set; }
        public ICollection<CompaniaTelefonicaDTO> CompaniasTelefonica { get; set; }
        public SearchPersonasVM SearchPersonasVM { get; set; }
    }

    public class SearchPersonasVM : SearchViewModel
    {
        public string Nombres { get; set; }

        [Display(Name = "Identificación")]
        public string Identificacion { get; set; }
    }
}