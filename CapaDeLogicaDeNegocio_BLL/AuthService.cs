using System;
using System.Collections.Generic;
using System.Linq;
using CapaDeEntidades;

namespace CapaDeLogicaDeNegocio_BLL
{
    // singleton: una sola instancia para toda la app
    // ademas arma los perfiles (composite) que se usan para los permisos
    public class AuthService
    {
        private static AuthService _instance = null;
        private static readonly object _lock = new object();

        private readonly List<UsuarioDatos> _usuarios;
        private readonly Dictionary<int, PerfilComposite> _perfilesPorId;

        private AuthService()
        {
            _usuarios = new List<UsuarioDatos>();
            _perfilesPorId = new Dictionary<int, PerfilComposite>();
            ConfigurarPerfiles();
            ConfigurarUsuarios();
        }

        public static AuthService Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null) _instance = new AuthService();
                    return _instance;
                }
            }
        }

        // los nombres de permiso van en espejo con los cu de la documentacion del grupo
        // cada rol arranca con minimo privilegio, en la proxima entrega esto tiene que salir de la bd
        private void ConfigurarPerfiles()
        {
            var gestionarUsuarios = new PermisoSimple { Nombre = "GestionarUsuarios" };
            var configurarPerfiles = new PermisoSimple { Nombre = "ConfigurarPerfiles" };
            var registrarVenta = new PermisoSimple { Nombre = "RegistrarVenta" };
            var gestionarClientes = new PermisoSimple { Nombre = "GestionarClientes" };
            var controlStock = new PermisoSimple { Nombre = "ControlStock" };
            var registrarCompra = new PermisoSimple { Nombre = "RegistrarCompra" };
            var gestionarMedicamentos = new PermisoSimple { Nombre = "GestionarMedicamentos" };
            var gestionarLaboratorios = new PermisoSimple { Nombre = "GestionarLaboratorios" };
            var verReportes = new PermisoSimple { Nombre = "VerReportes" };

            // admin: solo modulo de seguridad (abm usuarios + perfiles)
            var admin = new PerfilComposite { Nombre = "Administrador" };
            admin.Agregar(gestionarUsuarios);
            admin.Agregar(configurarPerfiles);
            _perfilesPorId[1] = admin;

            // vendedor: ventas + clientes
            var vendedor = new PerfilComposite { Nombre = "Vendedor" };
            vendedor.Agregar(registrarVenta);
            vendedor.Agregar(gestionarClientes);
            _perfilesPorId[2] = vendedor;

            // encargado de deposito: stock + compras + abm de medicamentos y laboratorios
            var encargado = new PerfilComposite { Nombre = "Encargado" };
            encargado.Agregar(controlStock);
            encargado.Agregar(registrarCompra);
            encargado.Agregar(gestionarMedicamentos);
            encargado.Agregar(gestionarLaboratorios);
            _perfilesPorId[3] = encargado;

            // gerente: solo reportes
            var gerente = new PerfilComposite { Nombre = "Gerente" };
            gerente.Agregar(verReportes);
            _perfilesPorId[4] = gerente;
        }

        private void ConfigurarUsuarios()
        {
            // NombreReal aparece en el saludo del home, NombreRol en el badge del menu
            _usuarios.Add(new UsuarioDatos { User = "admin", Pass = "123", NombreReal = "Kiara Poloni", NombreRol = "Administrador", IdPerfil = 1 });
            _usuarios.Add(new UsuarioDatos { User = "vendedor", Pass = "123", NombreReal = "Victoria Mac Kenzie", NombreRol = "Vendedor", IdPerfil = 2 });
            _usuarios.Add(new UsuarioDatos { User = "deposito", Pass = "123", NombreReal = "Diego Santiago", NombreRol = "Depósito", IdPerfil = 3 });
            _usuarios.Add(new UsuarioDatos { User = "gerente", Pass = "123", NombreReal = "Gerente Farmacia UPE", NombreRol = "Gerente", IdPerfil = 4 });
        }

        public UsuarioDatos ValidarUsuario(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password)) return null;

            return _usuarios.FirstOrDefault(u =>
                u.User.Trim().Equals(username.Trim(), StringComparison.OrdinalIgnoreCase) &&
                u.Pass.Trim() == password.Trim());
        }

        public PerfilComposite ObtenerPerfilPorId(int idPerfil)
        {
            return _perfilesPorId.TryGetValue(idPerfil, out var perfil) ? perfil : null;
        }

        // atajo para que la vista pregunte permisos sin tener que armar el perfil a mano
        public bool PerfilTienePermiso(int? idPerfil, string nombrePermiso)
        {
            if (idPerfil == null) return false;
            var perfil = ObtenerPerfilPorId(idPerfil.Value);
            return perfil != null && perfil.TienePermiso(nombrePermiso);
        }
    }

    public class UsuarioDatos
    {
        public string User { get; set; }
        public string Pass { get; set; }
        public string NombreReal { get; set; }
        public string NombreRol { get; set; }
        public int IdPerfil { get; set; }
    }
}
