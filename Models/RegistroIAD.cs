using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ISProject.Models
{
    public class RegistroIAD
    {
        public int id_iad { get; set; }
        public int id_docente { get; set; }
        public string estado { get; set; }
        public int estado_valor { get; set; }
        public string periodo { get; set; }
        public string carrera { get; set; }
        public int numero_empleado { get; set; }
        public string nombre_docente { get; set; }
        public bool extemporaneous { get; set; }
    }
}