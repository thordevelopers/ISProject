﻿using ISProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ISProject.Filters
{
    public class FilterCoordinador: ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            Docentes doc = (Docentes)HttpContext.Current.Session["user"];
            if (doc == null || doc.rol != 2)
                filterContext.Result = new RedirectResult("~/Login/RedirectToHome");
            base.OnActionExecuting(filterContext);
        }
    }
}