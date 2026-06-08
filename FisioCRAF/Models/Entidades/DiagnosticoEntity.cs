using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FisioCRAF.Models.Entidades
{
    public class DiagnosticoEntity
    {
        public int id_Diag { get; set; }
        public int Num_Diag { get; set; }
        public int? id_Cita { get; set; }
        public int? id_Consul { get; set; }
    }
}
