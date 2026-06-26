using CapaDeLogicaDeNegocio_BLL;
using Xunit;

namespace Pruebas_unitarias
{
    public class AuthServiceTests
    {
        [Fact]
        public void Instance_SiempreDevuelveLaMismaInstancia()
        {
            // el singleton debe devolver exactamente el mismo objeto en cada llamada
            var instancia1 = AuthServiceTest.Instance;
            var instancia2 = AuthServiceTest.Instance;

            Assert.Same(instancia1, instancia2);
        }
    }
}
