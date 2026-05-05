using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FisioCRAF.Models
{
    public class Paciente
    {
        public int id_Pac { get; set; } 
        public string Nombre_Pac { get; set; }
        public string ApellidoP_Pac { get; set; }
        public string ApellidoM_Pac { get; set; }
        public string Telefono_Pac { get; set; }
        public DateTime Fecha_Nacimiento { get; set; }
        public string Genero { get; set; }
    }
}