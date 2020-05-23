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
        /* Esta accion se manda llamar cuando se quiere validar las credenciales de una cuenta
         * Esta cuenta validad que la contrasena corresponda correctamente al correo
         * Recibe las credenciales
         * Regresa un booleano con el resultado de la autenticacion*/
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