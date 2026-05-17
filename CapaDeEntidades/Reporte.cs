using System;
using System.Collections.Generic;
using System.Text;

namespace CapaDeEntidades
{
    // Capa de Entidades (Entity Layer) / DTOs
    // Su propósito es definir los "moldes" o clases que representan las tablas de la base de datos.
    // Esta es la única capa que puede ser referenciada por todas las demás (Datos, Lógica y Presentación).
    public class Reporte
    {
        public string NombreVendedor { get; set; }
        public DateTime Fecha { get; set; }
        public TimeSpan Hora { get; set; }
        public decimal Monto { get; set; }
        public string Medicamento { get; set; }
    }
}
