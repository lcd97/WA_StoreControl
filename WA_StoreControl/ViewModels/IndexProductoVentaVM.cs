using ModelosDB.Venta;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WA_StoreControl.Utilidades;

namespace WA_StoreControl.ViewModels
{
    public class IndexProductoVentaVM
    {
        public IndexProductoVentaVM()
        {
            this.SearchProductosVentaVM = new SearchProductosVentaVM();
            this.ProductoVenta = new ProductoVenta();
        }

        public ProductoVenta ProductoVenta { get; set; }
        public SearchProductosVentaVM SearchProductosVentaVM { get; set; }
    }

    public class SearchProductosVentaVM : SearchViewModel
    {
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }
    }
}