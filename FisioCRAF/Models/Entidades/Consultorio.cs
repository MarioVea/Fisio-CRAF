using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FisioCRAF.Models.Entidades
{
    public class Consultorio
    {
        public int id_Consul { get; set; }
        public string Nombre_Consul { get; set; }
        public string Telefono_Consul { get; set; }
        public string Direccion { get; set; }
    }
}
