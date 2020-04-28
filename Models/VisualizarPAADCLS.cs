using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ISProject.Models
{
    public class VisualizarPAADCLS
    {
        public int id_paad { get; set; }
        public string estado { get; set; }
        public string periodo { get; set; }
        public string carrera { get; set; }
        public int numero_empleado { get; set; }
        public string nombre_docente { get; set; }
        public string categoria_docente { get; set; }
        public int horas_clase { get; set; }
        public int horas_investigacion { get; set; }
        public int horas_gestion { get; set; }
        public int horas_tutorias { get; set; }
        public string cargo { get; set; }
        public string firma_docente { get; set; }
        public string firma_director { get; set; }
        public List<ActivityCLS> activities { get; set; }
    }
}