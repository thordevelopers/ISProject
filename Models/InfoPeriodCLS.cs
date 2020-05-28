using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ISProject.Models
{
    public class InfoPeriodCLS
    {
        public int id_period { set; get; }
        public bool on_time_paad { set; get; }
        public bool is_close_paad { set; get; }
        public bool on_time_iad { set; get; }
        public bool is_close { set; get; }
    }
}