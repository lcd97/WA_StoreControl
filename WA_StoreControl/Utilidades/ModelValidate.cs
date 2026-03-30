using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace WA_StoreControl.Utilidades
{
    public static class ModelValidate
    {
        public static IEnumerable<string> GetModelErrorMessages(ModelStateDictionary modelStateDictionary)
        {
            return modelStateDictionary.Values.SelectMany(c => c.Errors.Select(x => x.ErrorMessage));
        }
    }

    public class CustomHandleErrorAttribute : FilterAttribute, IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            var isAjaxRequest = filterContext.RequestContext.HttpContext.Request.IsAjaxRequest();

            if (isAjaxRequest)
            {
                filterContext.Result = new JsonResult()
                {
                    Data = new RequestResult(SystemMessage.ServerError, success: false),
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
            //else
            //{
            //    filterContext.Result = new ViewResult()
            //    {
            //        ViewName = ConfigApplicationKey.ErrorViewPath
            //    };
            //}

            filterContext.ExceptionHandled = true;
        }
    }

}