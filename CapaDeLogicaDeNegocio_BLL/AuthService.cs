using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
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

        private AuthService()
        {
            _perfilesPorId = new Dictionary<int, PerfilComposite>();
            ConfigurarPerfiles();
        }

        public static AuthService Instance => _instance;

        // configuracion de permisos y perfiles segun la documentacion del proyecto
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
            var verReportes = new PermisoSimple { Nombre = "VerReporte" };
            var generarEstadisticas = new PermisoSimple { Nombre = "GenerarEstadisticas" };
            var generarProyeccion = new PermisoSimple { Nombre = "GenerarProyeccion" };

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
            gerente.Agregar(generarEstadisticas);
            gerente.Agregar(generarProyeccion);
            _perfilesPorId[4] = gerente;
        }

        // metodo para validar credenciales accediendo a la base de datos SQL Server
        public UsuarioDatos? ValidarUsuario(string? username, string? password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password)) return null;

            using (SqlConnection con = new SqlConnection(Conexion.CadenaDeConexion))
            {
                using (SqlCommand cmd = new SqlCommand("SP_ValidarUsuario", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Usuario", username!.Trim());
                    cmd.Parameters.AddWithValue("@Contrasena", password!.Trim());

                    try
                    {
                        con.Open();
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                // verificacion del estado activo del usuario
                                if (Convert.ToBoolean(dr["Activo"]))
                                {
                                    int idPerfil = Convert.ToInt32(dr["IdPerfil"]);
                                    string nombre = dr["Nombre"]?.ToString() ?? string.Empty;
                                    string apellido = dr["Apellido"]?.ToString() ?? string.Empty;
                                    string user = dr["NombreUsuario"]?.ToString() ?? string.Empty;
                                    string rol = ObtenerPerfilPorId(idPerfil)?.Nombre ?? "Desconocido";

                                    // retorno del objeto con informacion del usuario autenticado
                                    return new UsuarioDatos
                                    {
                                        User = user,
                                        Pass = string.Empty, // por seguridad limpieza de credenciales en memoria
                                        NombreReal = $"{nombre} {apellido}".Trim(),
                                        NombreRol = rol,
                                        IdPerfil = idPerfil
                                    };
                                }
                            }
                        }
                    }
                    catch { return null; }
                }
            }
            return null;
        }

        // metodo auxiliar para recuperar el perfil segun id
        public PerfilComposite? ObtenerPerfilPorId(int idPerfil) => _perfilesPorId.GetValueOrDefault(idPerfil);

        // verifica la existencia de permisos en el perfil asignado
        public bool PerfilTienePermiso(int? idPerfil, string nombrePermiso)
        {
            return idPerfil.HasValue && ObtenerPerfilPorId(idPerfil.Value)?.TienePermiso(nombrePermiso) == true;
        }
    }

    // estructura de datos para la transferencia de informacion del usuario
    public class UsuarioDatos
    {
        public string User { get; set; } = string.Empty;
        public string Pass { get; set; } = string.Empty;
        public string NombreReal { get; set; } = string.Empty;
        public string NombreRol { get; set; } = string.Empty;
        public int IdPerfil { get; set; }
    }
}