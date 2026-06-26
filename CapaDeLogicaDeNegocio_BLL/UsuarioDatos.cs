namespace CapaDeLogicaDeNegocio_BLL
{
    // datos del usuario autenticado que se guardan en sesion
    public class UsuarioDatos
    {
        public int IdUsuario { get; set; }
        public string Ingreso { get; set; } = string.Empty;
        public string Clave { get; set; } = string.Empty;
        public string NombreReal { get; set; } = string.Empty;
        public string NombreRol { get; set; } = string.Empty;
        public int IdPerfil { get; set; }
        public string Dni { get; set; } = string.Empty;
    }
}
