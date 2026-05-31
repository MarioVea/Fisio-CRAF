using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FisioCRAF.Models.Entidades
{
    public class Lesion
    {
        public int id_Lesion { get; set; }
        public int id_TipoLes { get; set; }
        public string Nombre_TipoLes { get; set; }
        public string Nom_les { get; set; }
        public string Grado { get; set; }
        public string Descrip_Les { get; set; }
    }
}