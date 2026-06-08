using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FisioCRAF.Models.Entidades
{
    public class DetalleCH
    {
        public int id_DetalleCH { get; set; }
        public int? id_Diag { get; set; }
        public string Imag_Musculo { get; set; }
        public string Nombre_Musculo { get; set; }
        public int? id_DetalleTrata { get; set; }
        public string Descripcion_DiagCH { get; set; }
    }
}
