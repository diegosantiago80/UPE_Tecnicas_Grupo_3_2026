using System.Collections.Generic;
using CapaDeEntidades;
using CapaDeAccesoADatos_DAL;

namespace CapaDeLogicaDeNegocio_BLL
{
    // singleton: una sola instancia para toda la app
    // ademas arma los perfiles (composite) que se usan para los permisos
    public class AuthService
    {
        private static readonly AuthService _instance = new AuthService();
        private readonly Dictionary<int, PerfilComposite> _perfilesPorId;
        private readonly AutenticacionDAL _autenticacionDAL = new AutenticacionDAL();

        private AuthService()
        {
            _perfilesPorId = new Dictionary<int, PerfilComposite>();
            ConfigurarPerfiles();
        }

        public static AuthService Instance => _instance;

        // invocado desde la capa de negocio cuando se guarda o elimina un perfil
        public void RecargarPerfilesDesdeBaseDeDatos()
        {
            ConfigurarPerfiles();
        }

        // lee los datos de la BD y mapea dinamicamente los componentes del patron composite
        private void ConfigurarPerfiles()
        {
            _perfilesPorId.Clear();
            var perfilDal = new PerfilDAL();
            var perfilesBD = perfilDal.ObtenerTodos();

            foreach (var perf in perfilesBD)
            {
                // crea la raiz del composite con la descripcion del rol
                var composite = new PerfilComposite { Nombre = perf.Descripcion };

                // string de permisos separados por comas
                if (!string.IsNullOrWhiteSpace(perf.PermisosAsignados))
                {
                    var permisos = perf.PermisosAsignados.Split(',');
                    foreach (var p in permisos)
                    {
                        if (!string.IsNullOrWhiteSpace(p))
                        {
                            // agrega cada permiso como una hoja simple dentro del composite
                            composite.Agregar(new PermisoSimple { Nombre = p.Trim() });
                        }
                    }
                }
                _perfilesPorId[perf.IdPerfil] = composite;
            }
        }

        // delega la validacion de credenciales al DAL y arma el objeto de sesion
        public UsuarioDatos? ValidarUsuario(string? ingreso, string? clave)
        {
            if (string.IsNullOrWhiteSpace(ingreso) || string.IsNullOrWhiteSpace(clave)) return null;

            var resultado = _autenticacionDAL.ValidarCredenciales(ingreso, clave);

            if (!resultado.encontrado || !resultado.activo) return null;

            string rol = ObtenerPerfilPorId(resultado.idPerfil)?.Nombre ?? "Desconocido";

            return new UsuarioDatos
            {
                IdUsuario  = resultado.idUsuario,
                Ingreso       = resultado.nombreUsuario,
                Clave       = string.Empty,
                NombreReal = $"{resultado.nombre} {resultado.apellido}".Trim(),
                NombreRol  = rol,
                IdPerfil   = resultado.idPerfil,
                Dni        = resultado.dni
            };
        }

        public PerfilComposite? ObtenerPerfilPorId(int idPerfil) => _perfilesPorId.GetValueOrDefault(idPerfil);

        public bool PerfilTienePermiso(int? idPerfil, string nombrePermiso)
        {
            return idPerfil.HasValue && ObtenerPerfilPorId(idPerfil.Value)?.TienePermiso(nombrePermiso) == true;
        }
    }
}
