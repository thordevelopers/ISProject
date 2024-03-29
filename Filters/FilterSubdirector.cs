﻿using ISProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ISProject.Filters
{
    public class FilterSubdirector : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            Administrativos admin = (Administrativos)HttpContext.Current.Session["administ"];
            if (admin == null || admin.rol != 2)
                filterContext.Result = new RedirectResult("~/Login/RedirectToHome");
            base.OnActionExecuting(filterContext);
        }
    }
}