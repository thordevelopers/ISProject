using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ISProject.Models
{
    public class RegistroIAD
    {
        public int id_iad { get; set; }
        public int estado { get; set; }
        public int periodo { get; set; }
        public int carrera { get; set; }
        public int docente { get; set; }
        public int categoria_docente { get; set; }
        public int horas_clase { get; set; }
        public int horas_investigacion { get; set; }
        public int horas_gestion { get; set; }
        public int horas_tutorias { get; set; }
        public int cargo { get; set; }
        public string firma_docente { get; set; }
        public string firma_dir { get; set; }
        public string nombre_docente { get; set; }
        public string comentarios { get; set; }
    }
}