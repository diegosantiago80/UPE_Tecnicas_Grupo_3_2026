using CapaDeEntidades;
using CapaDeLogicaDeNegocio_BLL;
using System;
using System.Collections.Generic;
using Xunit;

namespace Pruebas_unitarias
{
    public class VentaBLLTests
    {
        private readonly VentaBLL _ventaBLL = BLLFactory.CrearVentaBLL();

        [Fact]
        public void RegistrarVenta_SinDetalle_LanzaExcepcion()
        {
            // si la lista de detalle esta vacia, la BLL debe rechazar la venta
            var excepcion = Assert.Throws<Exception>(() =>
                _ventaBLL.RegistrarVenta(1, 1, 100m, new List<DetalleVenta>())
            );

            Assert.Contains("no tiene productos", excepcion.Message);
        }

        [Fact]
        public void RegistrarVenta_DetalleNulo_LanzaExcepcion()
        {
            // detalle null tambien debe ser rechazado
            var excepcion = Assert.Throws<Exception>(() =>
                _ventaBLL.RegistrarVenta(1, 1, 100m, null)
            );

            Assert.Contains("no tiene productos", excepcion.Message);
        }
    }
}
