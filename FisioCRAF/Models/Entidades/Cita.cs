using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FisioCRAF.Models.Entidades
{
    public class Cita
    {

        public int id_Cita { get; set; }
        public int Folio { get; set; }
        public int id_Pac { get; set; }
        public string Motivo_Cita { get; set; }
        public DateTime Fecha_Cita { get; set; }
        public TimeSpan Hora_Cita { get; set; }
        public int id_Emp { get; set; }
        public int Estatus_Cita { get; set; }

    }
}