using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ISProject.Models;
namespace ISProject.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Login(UserCLS user)
        {
            if (!ModelState.IsValid)
            {
                return View(user);
            }
            else
            {
                USERS user_db;
                using (var db= new DB_PAAD_IADEntities())
                {
                    user_db = db.USERS.Where(p => p.EMAIL==user.Email && p.PASSWORD==user.Password).FirstOrDefault();
                }
                if(user_db==null)
                {
                    return View(user);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine(user_db.ID);
                    Session["id"] = user_db.ID;
                    user = null;
                    return RedirectToAction("Index", "Home");
                }
                
            }
            
        }
    }
}