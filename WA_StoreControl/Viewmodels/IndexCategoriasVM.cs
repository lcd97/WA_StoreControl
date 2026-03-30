using ModelosDB.Inventario;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WA_StoreControl.Utilidades;

namespace WA_StoreControl.ViewModel
{
    public class IndexCategoriasVM
    {
        public IndexCategoriasVM()
        {
            this.Categoria = new Categoria();
            this.SearchCategoriasVM = new SearchCategoriasVM();
        }

        public Categoria Categoria { get; set; } //MODELO
        public SearchCategoriasVM SearchCategoriasVM { get; set; }
    }

    public class SearchCategoriasVM : SearchViewModel
    {
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }
    }
}