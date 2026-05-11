using CapaDeEntidades;
using System.Collections.Generic;
using System.Linq;

namespace CapaDeAccesoADatos_DAL
{
    // mientras no haya bd la persistencia se simula en memoria
    // en la proxima entrega cada metodo va a ejecutar un sp
    public class MedicamentoDAL
    {
        // static para que el stock persista entre requests
        private static List<Medicamento> _stockEnMemoria = new List<Medicamento>
        {
            new Medicamento(1, "Paracetamol", "500mg - Analgésico", 500.00m, 300.00m, 50, 10, false, 1, true),
            new Medicamento(2, "Ibuprofeno", "600mg - Antiinflamatorio", 800.00m, 450.00m, 5, 15, false, 1, true),
            new Medicamento(3, "Amoxicilina", "1g - Antibiótico", 1200.00m, 700.00m, 20, 5, true, 2, true),
            new Medicamento(4, "Loratadina", "10mg - Antialérgico", 450.00m, 200.00m, 3, 10, false, 1, true),
            new Medicamento(5, "Aspirina", "100mg - Preventivo", 300.00m, 150.00m, 100, 20, false, 1, true)
        };

        public List<Medicamento> ObtenerTodos()
        {
            return _stockEnMemoria;
        }

        public List<Medicamento> ObtenerCriticos()
        {
            return _stockEnMemoria.Where(m => m.TieneStockCritico).ToList();
        }

        public bool ActualizarStockPorIngreso(List<Medicamento> itemsComprados)
        {
            try
            {
                foreach (var item in itemsComprados)
                {
                    var medEnStock = _stockEnMemoria.FirstOrDefault(m => m.IdMedicamento == item.IdMedicamento);
                    if (medEnStock != null)
                    {
                        medEnStock.StockActual += item.StockActual;
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool ActualizarStockPorVenta(List<Medicamento> itemsVendidos)
        {
            try
            {
                foreach (var item in itemsVendidos)
                {
                    var medEnStock = _stockEnMemoria.FirstOrDefault(m => m.IdMedicamento == item.IdMedicamento);
                    if (medEnStock != null)
                    {
                        medEnStock.StockActual -= item.StockActual;
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
