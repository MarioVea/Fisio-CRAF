using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace FisioCRAF.Models.Entidades
{
    public class Tratamiento
    {
        public int id_Trata { get; set; }
        [Required(ErrorMessage = "El campo Nombre de Tratamiento es obligatorio.")]
        [StringLength(50, ErrorMessage = "El Nombre del Tratamiento no puede exceder los 50 caracteres.")]
        public string Nombre_Trata { get; set; }

        [Required(ErrorMessage = "El campo Descripción de Tratamiento es obligatorio.")]
        [StringLength(300, ErrorMessage = "La Descripción del Tratamiento no puede exceder los 300 caracteres.")]
        public string Descrip_Trata { get; set; }
    }
}
