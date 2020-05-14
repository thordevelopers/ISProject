using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ISProject.Models
{
    public class AuthenticationCLS
    {
        //Campos principales de autenticacion
        [Display(Name ="Usuario")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        [Required]
        public string email { set; get; }
        [Display(Name ="Contraseña")]
        [DataType(DataType.Password)]
        [Required]
        public string password { set; get; }
        public string message { set; get; }
    }
}