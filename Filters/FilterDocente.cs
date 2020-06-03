using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ISProject.Models;

namespace ISProject.Filters
{
    public class FilterDocente:ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            Docentes doc = (Docentes)HttpContext.Current.Session["docente"];
            if (doc == null)
                filterContext.Result = new RedirectResult("~/Login/RedirectToHome");
            base.OnActionExecuting(filterContext);
        }
    }
}