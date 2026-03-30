using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WA_StoreControl.Utilidades
{

    /// <summary>
    /// Representa la clase base que puede usarse para retornar resultados de peticiones HTTP al cliente.
    /// </summary>
    public class RequestResult
    {
        /// <summary>
        /// Bandera: Resultado de éxito de la petición.
        /// </summary>
        public bool Success { get; set; }


        /// <summary>
        /// Mensaje a mostrar como resultado de la petición.
        /// </summary>
        public string Message { get; set; }


        /// <summary>
        /// Registro, opcional y útil cuando un método de acción retorna un registro único al cliente.
        /// </summary>
        public object Record { get; set; }


        /// <summary>
        /// Crea una nueva instancia de RequestResult con los párametros especificados.
        /// </summary>
        /// <param name="message">Mensaje a mostrar como resultado de la petición.</param>
        /// <param name="success">Resultado de la petición.</param>
        public RequestResult(string message, bool success = true)
        {
            Success = success;
            Message = message;
        }


        /// <summary>
        /// Crea una nueva instancia de RequestResult con los párametros especificados.
        /// </summary>
        /// <param name="message">Mensaje a mostrar como resultado de la petición.</param>
        /// <param name="success">Resultado de la petición.</param>
        public RequestResult(object record, string message = "", bool success = true)
        {
            Success = success;
            Record = record;
            Message = message;
        }

        public RequestResult()
        {
            Success = true;
        }

        public RequestResult(object record)
        {
            Record = record;
            Success = true;
            Message = "";
        }
    }


    /// <summary>
    /// Representa una clase que puede ser usada para retornar el conjunto de resultados de una petición HTTP.
    /// </summary>
    /// <typeparam name="TRecords">Tipo de datos de la colección a retornar.</typeparam>
    public class RequestCollectionResult<TRecords> : RequestResult
    {

        /// <summary>
        /// Colección de registros a retornar.
        /// </summary>
        public IEnumerable<TRecords> Records { get; set; }


        /// <summary>
        ///  Crea una nueva instancia de RequestCollectionResult con los párametros especificados.
        /// </summary>
        /// <param name="records">Colección de registros a retornar.</param>
        /// <param name="message">Mensaje a mostrar como resultado de la petición [opcional].</param>
        /// <param name="success">Resultado de la petición. Por defecto "true".</param>
        public RequestCollectionResult(IEnumerable<TRecords> records, string message = "", bool success = true) : base(message, success)
        {
            Records = records;
        }


        public RequestCollectionResult() : base()
        {

        }
    }


    /// <summary>
    /// Representa una clase que puede ser usada para retornar un conjunto de resultados paginado de una petición HTTP.
    /// </summary>
    /// <typeparam name="TRecords"></typeparam>
    public class RequestPagedResult<TRecords> : RequestCollectionResult<TRecords>
    {
        public int TotalRecords { get; set; }


        public int TotalPages { get; set; }


        public int Page { get; set; }


        /// <summary>
        ///  Crea una nueva instancia de RequestPagedResult con los párametros especificados.
        /// </summary>
        /// <param name="totalRecords">Total de Registros encontrados en la consulta.</param>
        /// <param name="totalPages">Total de Páginas calculadas.</param>
        /// <param name="page">Página actual a retornar.</param>
        /// <param name="records">Colección de registros a retornar.</param>
        /// <param name="message">Mensaje a mostrar como resultado de la petición [opcional].</param>
        /// <param name="success">Resultado de la petición. Por defecto "true".</param>
        public RequestPagedResult(int totalRecords,
                                  int totalPages,
                                  int page,
                                  IEnumerable<TRecords> records,
                                  string message = "",
                                  bool success = true) : base(records, message)
        {
            Success = success;
            TotalRecords = totalRecords;
            TotalPages = totalPages;
            Page = page;
        }

        public RequestPagedResult() : base()
        {

        }

    }
}