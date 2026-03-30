using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WA_StoreControl.Utilidades
{
    public class SystemMessage
    {
        public static string ValidateOperationError => "Error de Validación";
        public static string ServerError => "Se produjo un error al conectar con el servidor";
        public static string CreateSuccessful => "El registro se ha creado correctamente";
        public static string UpdateSuccessful => "El registro se ha actualizado correctamente";
        public static string DeleteSuccessfull => "El registro se ha eliminado exitosamente";
    }
}