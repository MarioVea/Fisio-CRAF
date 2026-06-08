using FisioCRAF.Models;
using FisioCRAF.Models.Entidades;
using FisioCRAF.Models.Services;
using System.Collections.Generic;
using System;

class Program {
    static void Main() {
        try {
            var ds = new DiagnosticoService();
            ds.GuardarDiagnostico(1, null, new List<DetalleDiag> { new DetalleDiag { Tipo_Diag = true, Nombre_Diag = "test", id_Lesion = 1, Descrip_Diag = "test", id_EscalaDolor = 7 } }, new List<DetalleEjerDiag> { new DetalleEjerDiag { id_Ejercicio = 1, Series = 2, Repeticiones = 12 } }, new List<DetalleCH> { new DetalleCH { Imag_Musculo = "", Nombre_Musculo = "Cuádriceps", Descripcion_DiagCH = "test" } }, new List<int?> { 1 });
        } catch (Exception ex) {
            Console.WriteLine(ex.ToString());
        }
    }
}
