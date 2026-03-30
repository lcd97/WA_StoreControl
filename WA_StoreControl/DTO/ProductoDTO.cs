using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WA_StoreControl.DTO
{
    public class ProductoDTO
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public string Marca { get; set; }
        public bool EsActivo { get; set; }

        public int SubCategoriaId { get; set; }

        public string DescripCategoria { get; set; }
    }
}