using ModelosDB.General;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using ModelosDB;
using WA_StoreControl.Services;

namespace WA_StoreControl.Controllers
{
    public static class PersonaHelper
    {
        public static string BuscarCoincidencias(string nombre)
        {
            nombre = nombre.ToUpper();

            var normalizedString = nombre.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = Char.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                    stringBuilder.Append(c);
            }

            nombre = stringBuilder.ToString().Normalize(NormalizationForm.FormC);

            nombre = nombre.Replace(",", "");

            nombre = Regex.Replace(nombre, @"\s+", " ");

            return nombre.Trim();
        }
    }
}