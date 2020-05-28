using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ISProject.Models
{
    public class InfoIADCLS
    {
        public int id_iad { set; get; }
        public int id_paad { set; get; }
        public int status_value { set; get; }
        public string status_name { set; get; }
        public bool active { set; get; }
        public bool isdirector { set; get; }
    }
}