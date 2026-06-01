using System;
using System.Collections.Generic;
using System.Text;

namespace CapaDeAccesoADatos_DAL
{
    // Clase estatica que centraliza la cadena de conexión del DAL
    // Su valor se asigna una sola vez desde Program.cs al arrancar la aplicación
    // Lee de appsettings.json mediante IConfiguration
    public class Conexion
    {
        // Se inicializa vacia
        public static string CadenaDeConexion = string.Empty;
    }
}