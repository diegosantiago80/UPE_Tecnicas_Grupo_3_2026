using CapaDeEntidades;
using Xunit;

namespace Pruebas_unitarias
{
    public class PermisosCompositeTests
    {
        [Fact]
        public void PerfilComposite_TienePermiso_DetectaPermisoAsignado()
        {
            // un perfil con un permiso asignado debe reconocerlo correctamente
            var permiso = new PermisoSimple();
            permiso.Nombre = "RegistrarVenta";

            var perfil = new PerfilComposite();
            perfil.Nombre = "Vendedor";
            perfil.Agregar(permiso);

            bool resultado = perfil.TienePermiso("RegistrarVenta");

            Assert.True(resultado);
        }

        [Fact]
        public void PerfilComposite_TienePermiso_RechazaPermisoNoAsignado()
        {
            // un perfil no debe tener permisos que no se le asignaron
            var permiso = new PermisoSimple();
            permiso.Nombre = "RegistrarVenta";

            var perfil = new PerfilComposite();
            perfil.Nombre = "Vendedor";
            perfil.Agregar(permiso);

            bool resultado = perfil.TienePermiso("GestionarUsuarios");

            Assert.False(resultado);
        }
    }
}
