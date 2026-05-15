using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ModelosDB.Interfaces
{
    public interface ICodeEntity
    {
        int Id { get; set; }
        string Codigo { get; set; }
    }
}