using CapaDeEntidades;
using System.Collections.Generic;
using System.Linq;
using System;


namespace CapaDeAccesoADatos_DAL
{
    public class ReporteDAL
    {
        // En una aplicación real, aquí tendrías tu cadena de conexión (ConnectionString)
        // private readonly string _cadenaConexion = "Server=miServidor;Database=miBD;User Id=miUsuario;Password=miClave;";

        // Método que simula ejecutar un "SELECT * FROM Ventas" en SQL Server
        public List<Reporte> ObtenerTodasLasVentas()
        {
            // Aquí iría tu código de ADO.NET (SqlConnection, SqlCommand, SqlDataReader)
            // o Entity Framework (context.Ventas.ToList())

            // Hemos movido los datos falsos aquí, simulando que son los registros leídos de la tabla de SQL

            //valida
            return new List<Reporte>
            {
                // Un mes con altibajos para Paracetamol
                new Reporte { NombreVendedor = "Carlos Perez", Fecha = DateTime.Today.AddDays(-30), Hora = TimeSpan.Parse("10:30"), Monto = 1200.00m, Medicamento = "Paracetamol" },
                new Reporte { NombreVendedor = "Ana Gomez", Fecha = DateTime.Today.AddDays(-25), Hora = TimeSpan.Parse("14:15"), Monto = 1800.00m, Medicamento = "Paracetamol" },
                new Reporte { NombreVendedor = "Luis Martinez", Fecha = DateTime.Today.AddDays(-15), Hora = TimeSpan.Parse("09:45"), Monto = 900.00m, Medicamento = "Paracetamol" },
                new Reporte { NombreVendedor = "Carlos Perez", Fecha = DateTime.Today.AddDays(-5), Hora = TimeSpan.Parse("16:20"), Monto = 2100.00m, Medicamento = "Paracetamol" },
                new Reporte { NombreVendedor = "Sofia Castro", Fecha = DateTime.Today.AddDays(-2), Hora = TimeSpan.Parse("11:10"), Monto = 1800.00m, Medicamento = "Paracetamol" },

                // Crecimiento constante para Ibuprofeno
                new Reporte { NombreVendedor = "Juan Diaz", Fecha = DateTime.Today.AddDays(-20), Hora = TimeSpan.Parse("12:00"), Monto = 1000.00m, Medicamento = "Ibuprofeno" },
                new Reporte { NombreVendedor = "Ana Gomez", Fecha = DateTime.Today.AddDays(-10), Hora = TimeSpan.Parse("15:30"), Monto = 2500.00m, Medicamento = "Ibuprofeno" },
                new Reporte { NombreVendedor = "Carlos Perez", Fecha = DateTime.Today.AddDays(-1), Hora = TimeSpan.Parse("08:15"), Monto = 3500.00m, Medicamento = "Ibuprofeno" }
            };
        }
    }
}
