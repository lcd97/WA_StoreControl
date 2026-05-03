using ModelosDB.General;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WA_StoreControl.Utilidades;

namespace WA_StoreControl.ViewModels
{
    public class IndexCompaniasTelefonicaVM
    {
        public IndexCompaniasTelefonicaVM()
        {
            this.CompaniaTelefonica = new CompaniaTelefonica();
            this.SearchCompaniasTelefonicaVM = new SearchCompaniasTelefonicaVM();
        }

        public CompaniaTelefonica CompaniaTelefonica { get; set; }
        public SearchCompaniasTelefonicaVM SearchCompaniasTelefonicaVM { get; set; }
    }

    public class SearchCompaniasTelefonicaVM : SearchViewModel
    {
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }
    }
}