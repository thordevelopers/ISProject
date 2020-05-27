using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Web;
using System.Web.Mvc;
using ISProject.Models;

namespace ISProject.Controllers
{
    public class AuthenticationController : Controller
    {
        // GET: Authentication
        public ActionResult Index()
        {
            return View();
        }

        public bool AuthenticateCredentials(string email, string password)
        {
            using (var db = new DB_PAAD_IADEntities())
            {
                if (db.USERS.Where(p => p.EMAIL == email && p.PASSWORD == password).Count() <= 0)
                    return false;
            }
            return true;
        }
    }
}