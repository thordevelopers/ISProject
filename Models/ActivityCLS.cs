using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ISProject.Models
{
    public class ActivityCLS
    {
        public int id { get; set; }
        [Display(Name = "Actividad")]
        [Required]
        [StringLength(150, ErrorMessage = "Maximo 150 caracteres")]
        public string actividad { get; set; }
        [Display(Name = "Produccion")]
        [Required]
        [StringLength(300, ErrorMessage = "Maximo 300 caracteres")]
        public string produccion { get; set; }
        [Display(Name = "Lugar")]
        [Required]
        [StringLength(150, ErrorMessage = "Maximo 150 caracteres")]
        public string lugar { get; set; }
        [Display(Name = "Avance")]
        [Required]
        [Range(0,99,ErrorMessage = "Fuera de rango (0 - 100)")]
        public int porcentaje { get; set; }
        public bool cacei { get; set; }
        public bool cuerpo_academico { get; set; }
        public bool iso { get; set; }
        public int id_paad { get; set; }
        public int id_iad { get; set; }
    }
}