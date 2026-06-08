using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FisioCRAF.Models.Entidades
{
    public class DetalleEjerDiag
    {
        public int id_DetalleEjer { get; set; }
        public int? id_Diag { get; set; }
        public int? id_Ejercicio { get; set; }
        public int? id_DetalleEjerLes { get; set; }
        public byte Series { get; set; }
        public byte Repeticiones { get; set; }
    }
}
