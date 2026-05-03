using ModelosDB.Inventario;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WA_StoreControl.Utilidades;

namespace WA_StoreControl.ViewModels
{
    public class IndexMarcasVM
    {
        public IndexMarcasVM()
        {
            this.Marca = new Marca();
            this.SearchMarcasVM = new SearchMarcasVM();
        }

        public Marca Marca { get; set; }
        public SearchMarcasVM SearchMarcasVM { get; set; }
    }

    public class SearchMarcasVM : SearchViewModel
    {
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }
    }
}