﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ISProject.Models
{
    public class RegistroPAAD
    {
        public int id_paad { get; set; }
        public string estado { get; set; }
        public string periodo { get; set; }
        public string carrera { get; set; }
        public int numero_empleado { get; set; }
        public string nombre_docente { get; set; }
        public string categoria_docente { get; set; }
        public string cargo { get; set; }
    }
}