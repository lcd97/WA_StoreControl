using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Helpers;

namespace WA_StoreControl.Utilidades
{
    public abstract class SearchViewModel
    {
        #region Constructors
        public SearchViewModel()
        {
            Page = 1;
            SortDirection = SortDirection.Ascending;
            RecordsPerPage = 10;
            SearchType = SearchType.Quick;
            SortColumns = new Dictionary<string, string>();
            ExcludedIds = new List<int>();
        }

        public SearchViewModel(int recordsPerPage, string sortColumn, Dictionary<string, string> sortColumns)
        {
            Page = 1;
            SortDirection = SortDirection.Ascending;
            SearchType = SearchType.Quick;
            RecordsPerPage = recordsPerPage;
            SortColumn = sortColumn;
            SortColumns = sortColumns;
            ExcludedIds = new List<int>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Página actual solicitada.
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// Nombre de la columna por la cual se deben ordenar los datos que se presentaran en la lista.
        /// </summary>
        [Display(Name = "Ordenar por")]
        public string SortColumn { get; set; }

        /// <summary>
        /// Diccionario que contiene los pares clave-valor de las columnas por las que es posible ordenar 
        /// los resultados de búsqueda.
        /// </summary>
        public Dictionary<string, string> SortColumns { get; set; }


        /// <summary>
        /// Dirección de ordenamiento de los datos. Asc o Desc.
        /// </summary>
        [Display(Name = "Dirección")]
        public SortDirection SortDirection { get; set; }


        /// <summary>
        /// Total número de registros que se recuperaron
        /// </summary>
        public int TotalRecords { get; set; }

        /// <summary>
        /// Número de registros por página
        /// </summary>
        [Display(Name = "Total/Página")]
        [Range(minimum: 1, maximum: 50, ErrorMessage = "El valor del campo {0} debe estar entre {1} y {2}.")]
        public int RecordsPerPage { get; set; }

        /// <summary>
        /// Número total de páginas obtenidos en la búsqueda
        /// </summary>
        [Display(Name = "Total páginas")]
        public int TotalPages { get; set; }


        /// <summary>
        /// Cadena de texto utilizada para filtrar los datos en modo "Búsqueda rápida"
        /// </summary>
        [Display(Name = "Palabras clave")]
        public string SearchString { get; set; }


        public SearchType SearchType { get; set; }


        public bool Paginate { get; set; }


        public List<int> ExcludedIds { get; set; }


        #endregion
    }

    public enum SearchType
    {
        Quick = 0,
        Advanced = 1
    }
}