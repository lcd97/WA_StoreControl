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
    public class IndexProductosVM
    {
        public IndexProductosVM()
        {
            this.Producto = new Producto();
            this.SearchProductosVM = new SearchProductosVM();
            this.Marcas = new List<MarcaDTO>();
            this.CategoriasYSub = new List<SubCategoriaDTO>();
        }

        public Producto Producto { get; set; }
        public SearchProductosVM SearchProductosVM { get; set; }
        public ICollection<CategoriaDTO> Categorias { get; set; }
        public ICollection<SubCategoriaDTO> CategoriasYSub { get; set; }
        public ICollection<MarcaDTO> Marcas { get; set; }
    }

    public class SearchProductosVM : SearchViewModel
    {
        [Display(Name = "SubCategoría")]
        public int SubCategoriaId { get; set; }

        [Display(Name = "Categoría")]
        public int CategoriaId { get; set; }

        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }
    }
}