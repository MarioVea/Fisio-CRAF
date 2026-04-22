using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace FisioCRAF.Models
{
    public class Conexion
    {
        public string conexion()
        {
            string conex = "";
            string rutacompleta = @"C:\conexion\FisioCRAF.txt";
            using (StreamReader file = new StreamReader(rutacompleta))
            {
                conex = @"" + file.ReadToEnd();
                file.Close();
            }
            return conex;
        }
    }
}