using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FisioCRAF.Models.Entidades
{
    public class Ejercicio
    {
        public int id_Ejercicio { get; set; }
        public int id_CatEjer { get; set; }
        public string Nombre_Ejer { get; set; }
        public string Imag_Ejer { get; set; }
        public string Descrip_Ejer { get; set; }
    }
}