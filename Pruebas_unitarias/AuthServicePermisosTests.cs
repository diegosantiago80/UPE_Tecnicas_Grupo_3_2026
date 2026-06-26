using CapaDeLogicaDeNegocio_BLL;
using Xunit;

namespace Pruebas_unitarias
{
    public class AuthServicePermisosTests
    {
        [Fact]
        public void PerfilTienePermiso_AdminPuedGestionarUsuarios()
        {
            // el perfil Administrador (id 1) debe tener el permiso GestionarUsuarios
            bool resultado = AuthServiceTest.Instance.PerfilTienePermiso(1, "GestionarUsuarios");

            Assert.True(resultado);
        }

        [Fact]
        public void PerfilTienePermiso_VendedorNoPuedeGestionarUsuarios()
        {
            // el perfil Vendedor (id 2) no debe poder gestionar usuarios
            bool resultado = AuthServiceTest.Instance.PerfilTienePermiso(2, "GestionarUsuarios");

            Assert.False(resultado);
        }
    }
}
