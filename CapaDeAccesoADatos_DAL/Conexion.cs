namespace CapaDeAccesoADatos_DAL
{
    // centraliza la cadena de conexion del DAL
    // su valor se asigna una sola vez desde Program.cs al arrancar la aplicacion
    public class Conexion
    {
        public static string CadenaDeConexion { get; set; } = string.Empty;
    }
}
