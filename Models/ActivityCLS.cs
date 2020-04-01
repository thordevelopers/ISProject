using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ISProject.Models
{
    public class ActivityCLS
    {
        public int Id { get; set; }
        [Required]
        [StringLength(150, ErrorMessage = "Maximo 150 caracteres")]
        public string actividad { get; set; }
        [Required]
        [StringLength(300, ErrorMessage = "Maximo 300 caracteres")]
        public string produccion { get; set; }
        [Required]
        [StringLength(150, ErrorMessage = "Maximo 150 caracteres")]
        public string lugar { get; set; }
        [Required]
        public int porcentaje_inicial { get; set; }
        [Required]
        public int porcentaje_final { get; set; }
        [Required]
        public bool cacei { get; set; }
        [Required]
        public bool cuerpo_academico { get; set; }
        [Required]
        public bool iso { get; set; }
        public int id_paad { get; set; }
        public int id_iad { get; set; }
    }
}