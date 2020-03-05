using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ISProject.Models
{
    public class UserCLS
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int Rol { get; set; }
    }
}