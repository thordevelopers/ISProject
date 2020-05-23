using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ISProject.Models
{
    public class MessageCLS
    {
        public int id_message { set; get; }
        public int paad { set; get; }
        public int iad { set; get; }
        public int tipo { set; get; }
        public string mensaje { set; get; }
        //extra
        public string tipo_nombre { set; get; }
    }
}