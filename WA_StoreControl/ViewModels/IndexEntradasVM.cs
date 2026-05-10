using ModelosDB.Inventario;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WA_StoreControl.Utilidades;

namespace WA_StoreControl.ViewModels
{
    public class IndexEntradasVM
    {
        public IndexEntradasVM()
        {
            this.SearchEntradasVM = new SearchEntradasVM();
        }

        public SearchEntradasVM SearchEntradasVM { get; set; }
    }

    public class SearchEntradasVM : SearchViewModel
    {
        [Display(Name = "Fecha desde")]
        public string FechaDesde { get; set; }

        [Display(Name = "Fecha hasta")]
        public string FechaHasta { get; set; }
    }
}