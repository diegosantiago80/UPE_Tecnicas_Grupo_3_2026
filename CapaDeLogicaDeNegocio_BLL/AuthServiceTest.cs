using System.Collections.Generic;
using CapaDeEntidades;

namespace CapaDeLogicaDeNegocio_BLL
{
    /// <summary>
    /// Versión de AuthService para pruebas unitarias
    /// Tiene los perfiles hardcodeados, NO intenta conectarse a la BD
    /// Se usa SOLO en el proyecto de pruebas
    /// </summary>
    public class AuthServiceTest
    {
        private static readonly AuthServiceTest _instance = new AuthServiceTest();
        private readonly Dictionary<int, PerfilComposite> _perfilesPorId;

        private AuthServiceTest()
        {
            _perfilesPorId = new Dictionary<int, PerfilComposite>();
            ConfigurarPerfilesDefecto();
        }

        public static AuthServiceTest Instance => _instance;

        /// <summary>
        /// Configura los perfiles hardcodeados para pruebas
        /// </summary>
        private void ConfigurarPerfilesDefecto()
        {
            var gestionarUsuarios = new PermisoSimple { Nombre = "GestionarUsuarios" };
            var configurarPerfiles = new PermisoSimple { Nombre = "ConfigurarPerfiles" };
            var registrarVenta = new PermisoSimple { Nombre = "RegistrarVenta" };
            var gestionarClientes = new PermisoSimple { Nombre = "GestionarClientes" };
            var controlStock = new PermisoSimple { Nombre = "ControlStock" };
            var registrarCompra = new PermisoSimple { Nombre = "RegistrarCompra" };
            var gestionarMedicamentos = new PermisoSimple { Nombre = "GestionarMedicamentos" };
            var gestionarLaboratorios = new PermisoSimple { Nombre = "GestionarLaboratorios" };
            var verReportes = new PermisoSimple { Nombre = "VerReporte" };
            var generarEstadisticas = new PermisoSimple { Nombre = "GenerarEstadisticas" };
            var generarProyeccion = new PermisoSimple { Nombre = "GenerarProyeccion" };

            // Perfil Administrador (id 1)
            var admin = new PerfilComposite { Nombre = "Administrador" };
            admin.Agregar(gestionarUsuarios);
            admin.Agregar(configurarPerfiles);
            _perfilesPorId[1] = admin;

            // Perfil Vendedor (id 2)
            var vendedor = new PerfilComposite { Nombre = "Vendedor" };
            vendedor.Agregar(registrarVenta);
            vendedor.Agregar(gestionarClientes);
            _perfilesPorId[2] = vendedor;

            // Perfil Encargado (id 3)
            var encargado = new PerfilComposite { Nombre = "Encargado" };
            encargado.Agregar(controlStock);
            encargado.Agregar(registrarCompra);
            encargado.Agregar(gestionarMedicamentos);
            encargado.Agregar(gestionarLaboratorios);
            _perfilesPorId[3] = encargado;

            // Perfil Gerente (id 4)
            var gerente = new PerfilComposite { Nombre = "Gerente" };
            gerente.Agregar(verReportes);
            gerente.Agregar(generarEstadisticas);
            gerente.Agregar(generarProyeccion);
            _perfilesPorId[4] = gerente;
        }

        public PerfilComposite? ObtenerPerfilPorId(int idPerfil) => _perfilesPorId.GetValueOrDefault(idPerfil);

        public bool PerfilTienePermiso(int? idPerfil, string nombrePermiso)
        {
            return idPerfil.HasValue && ObtenerPerfilPorId(idPerfil.Value)?.TienePermiso(nombrePermiso) == true;
        }
    }
}
