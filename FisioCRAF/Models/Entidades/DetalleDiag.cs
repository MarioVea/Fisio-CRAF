using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FisioCRAF.Models.Entidades
{
    public class DetalleDiag
    {
        public int id_DetalleDiag { get; set; }
        public int? id_Diag { get; set; }
        public bool Tipo_Diag { get; set; }
        public string Nombre_Diag { get; set; }
        public int? id_Lesion { get; set; }
        public string RadioGrafia { get; set; }
        public string Descrip_Diag { get; set; }
        public int? id_EscalaDolor { get; set; }
    }
}
