using Rotativa;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ISProject.Controllers
{
    public class PDFController : Controller
    {
        // GET: PDF
        public ActionResult Index()
        {
            return View();
        }
        
        public ActionResult ToPDF()
        {
            return new ViewAsPdf("ToPDF") {
                PageOrientation = Rotativa.Options.Orientation.Landscape
            };
        }
    }
}