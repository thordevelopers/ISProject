using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ISProject.Models
{
    public class HeaderPAADCLS
    {
        public string periodo { set; get; }
        public string nombre { set; get; }
        public int numero_empleado { set; get; }
        public string categoria { set; get; }
        public string cargo { set; get; }
        public int horas_clase { set; get; }
        public int horas_gestion { set; get; }
        public int horas_investigacion { set; get; }
        public int horas_tutorias { set; get; }
        //others
        public List<SelectListItem> cargos { set; get; }
        public int id_paad { get; set; }

    }
}