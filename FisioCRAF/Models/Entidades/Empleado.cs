using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FisioCRAF.Models.Entidades
{
    public class Empleado
    {
        public int id_Emp { get; set; }
        public string Clave_Emp { get; set; }
        public string Nombre_Emp { get; set; }
        public string ApellidoP_Emp { get; set; }
        public string ApellidoM_Emp { get; set; }
        public int tipo_Emp { get; set; }
        public string Tipo_Emp { get; set; }
        public string Contraseña { get; set; }
        public string Telefono_Emp { get; set; }
        public string CedulaProfesional { get; set; }
        public int id_Esp { get; set; }
        public string nombre_Esp { get; set; }
        public int estatus_Emp { get; set; }
        public string Estatus_Emp { get; set; }

    }
}