using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace WA_StoreControl.DTO
{
    public class ProductoDTO
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public string DescripcionMarca { get; set; }
        public bool EsActivo { get; set; }
        public int Stock { get; set; }

        public int SubCategoriaId { get; set; }

        public string DescripcionCategoria { get; set; }
        public string DescripcionSubCategoria { get; set; }
        public int MarcaId { get; set; }

        public string DescripcionProducto { get; set; }
    }
}