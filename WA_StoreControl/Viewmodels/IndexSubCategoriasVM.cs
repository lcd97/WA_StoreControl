using ModelosDB.Inventario;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WA_StoreControl.DTO;
using WA_StoreControl.Utilidades;

namespace WA_StoreControl.ViewModels
{
    public class IndexSubCategoriasVM
    {
        public IndexSubCategoriasVM()
        {
            this.Categorias = new List<CategoriaDTO>();
            this.SearchSubCategoriasVM = new SearchSubCategoriasVM();
            this.SubCategoria = new SubCategoria();
        }

        public SubCategoria SubCategoria { get; set; }
        public ICollection<CategoriaDTO> Categorias { get; set; }
        public SearchSubCategoriasVM SearchSubCategoriasVM { get; set; }
    }

    public class SearchSubCategoriasVM : SearchViewModel
    {
        [Display(Name = "Categorías")]
        public int CategoriaId { get; set; }
    }
}