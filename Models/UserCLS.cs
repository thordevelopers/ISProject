using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ISProject.Models
{
    public class UserCLS
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100,ErrorMessage = "Maximo 100 caracteres")]
        public string Email { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "Maximo 50 caracteres")]
        public string Password { get; set; }
        public int Rol { get; set; }
    }
}