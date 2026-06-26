using System;
using System.Collections.Generic;
using Xunit;
using CapaDeLogicaDeNegocio_BLL;
using CapaDeEntidades;

namespace SistemaFarmacia.Tests
{
    // ====================================================================
    // 1. CREAMOS VERSIONES "ESPEJO" EXCLUSIVAS PARA PRUEBAS
    // ====================================================================

    public class TestableProcesadorVentasPorPeriodo : ProcesadorVentasPorPeriodo
    {
        private readonly List<Reporte> _datosSimulados;

        public TestableProcesadorVentasPorPeriodo(DateTime? fechaInicio, DateTime? fechaFin, string vendedor, List<Reporte> datosSimulados)
            : base(fechaInicio, fechaFin, vendedor) // Llama a tu constructor original intacto
        {
            _datosSimulados = datosSimulados;
        }

        // Pisamos el método para que use nuestra lista en memoria y NO el DAL real
        protected override List<Reporte> ObtenerDatos()
        {
            return _datosSimulados;
        }
    }

    public class TestableProcesadorVentasComparativa : ProcesadorVentasComparativa
    {
        private readonly List<Reporte> _datosSimulados;

        public TestableProcesadorVentasComparativa(string medicamento, DateTime? fechaInicio, DateTime? fechaFin, List<Reporte> datosSimulados)
            : base(medicamento, fechaInicio, fechaFin)
        {
            _datosSimulados = datosSimulados;
        }

        protected override List<Reporte> ObtenerDatos()
        {
            return _datosSimulados;
        }
    }

    // ====================================================================
    // 2. EL SET DE PRUEBAS UNITARIAS
    // ====================================================================
    public class ReportesBLLTestsSinModificarBLL
    {
        [Fact]
        public void VentasPorPeriodo_FiltrosTotalmenteVacios_DebeLanzarArgumentException()
        {
            // Arrange
            var datosFalsos = new List<Reporte>();
            // CORREGIDO: Pasamos "null" para las fechas vacías.
            var procesador = new TestableProcesadorVentasPorPeriodo(null, null, string.Empty, datosFalsos);

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => procesador.Procesar());
            // Nota: Asegúrate de que esta palabra esté en el throw original de tu BLL
            Assert.Contains("criterio", ex.Message);
        }

        [Fact]
        public void VentasPorPeriodo_RangoFechasInvertido_DebeLanzarArgumentException()
        {
            // Arrange
            var datosFalsos = new List<Reporte>();
            DateTime inicio = new DateTime(2026, 06, 20);
            DateTime fin = new DateTime(2026, 06, 15);

            // REEMPLAZO: Como ya no hay "Periodo", probamos la lógica de fechas invertidas en Ventas
            var procesador = new TestableProcesadorVentasPorPeriodo(inicio, fin, "Juan", datosFalsos);

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => procesador.Procesar());
            Assert.NotNull(ex); // Simplemente comprobamos que el error salte
        }

        [Fact]
        public void VentasComparativa_RangoFechasInvertido_DebeLanzarArgumentException()
        {
            // Arrange
            var datosFalsos = new List<Reporte>();
            DateTime inicio = new DateTime(2026, 06, 20);
            DateTime fin = new DateTime(2026, 06, 15);

            var procesador = new TestableProcesadorVentasComparativa("Amoxicilina", inicio, fin, datosFalsos);

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => procesador.Procesar());
            Assert.NotNull(ex);
        }

        [Fact]
        public void VentasPorPeriodo_FiltrosCorrectos_DebeAplicarFiltrosEnMemoria()
        {
            // Arrange
            var datosFalsos = new List<Reporte>
            {
                new Reporte { NombreVendedor = "Juan", Fecha = DateTime.Today, Medicamento = "Ibuprofeno" },
                new Reporte { NombreVendedor = "Pedro", Fecha = DateTime.Today, Medicamento = "Paracetamol" }
            };

            // CORREGIDO: Simular un filtro Diario pasando la misma fecha de inicio y fin (o fin null)
            var procesador = new TestableProcesadorVentasPorPeriodo(DateTime.Today, DateTime.Today.AddDays(1).AddTicks(-1), "Juan", datosFalsos);

            // Act
            var resultado = procesador.Procesar();

            // Assert
            Assert.Single(resultado);
            Assert.Equal("Juan", resultado[0].NombreVendedor);
        }
    }
}