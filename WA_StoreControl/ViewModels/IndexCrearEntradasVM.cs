using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Web;
using WA_StoreControl.DTO;
using WA_StoreControl.Utilidades;

namespace WA_StoreControl.ViewModels
{
    public class IndexCrearEntradasVM
    {
        public IndexCrearEntradasVM()
        {
            this.SearchCrearEntradasVM = new SearchCrearEntradasVM();
            this.Entrada = new EntradaDTO();
        }

        public SearchCrearEntradasVM SearchCrearEntradasVM { get; set; }
        public EntradaDTO Entrada { get; set; }
    }

    public class SearchCrearEntradasVM : SearchViewModel { }
}