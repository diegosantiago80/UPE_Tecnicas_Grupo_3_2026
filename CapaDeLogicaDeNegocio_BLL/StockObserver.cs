using CapaDeEntidades;
using System.Collections.Generic;

namespace CapaDeLogicaDeNegocio_BLL
{
    // patron observer: permite que distintas partes del sistema reaccionen
    // cuando el stock de un medicamento cae al nivel critico
    // el sujeto (StockSujeto) notifica a todos sus observadores registrados

    // interfaz que deben implementar todos los observadores de stock
    public interface IStockObservador
    {
        void Notificar(Medicamento medicamento);
    }

    // sujeto que mantiene la lista de observadores y los notifica
    public class StockSujeto
    {
        private readonly List<IStockObservador> _observadores = new List<IStockObservador>();

        public void AgregarObservador(IStockObservador observador)
        {
            _observadores.Add(observador);
        }

        public void QuitarObservador(IStockObservador observador)
        {
            _observadores.Remove(observador);
        }

        // notifica a todos los observadores si el medicamento esta en stock critico
        public void EvaluarStock(Medicamento medicamento)
        {
            if (medicamento.TieneStockCritico)
            {
                foreach (var obs in _observadores)
                    obs.Notificar(medicamento);
            }
        }
    }

    // observador concreto: registra una alerta de stock critico en memoria
    // en una entrega posterior esto podria escribir en una tabla de alertas en la bd
    public class AlertaStockObservador : IStockObservador
    {
        // lock para acceso seguro desde multiples requests concurrentes
        private static readonly object _lock = new object();
        public static readonly List<string> AlertasActivas = new List<string>();

        public void Notificar(Medicamento medicamento)
        {
            string alerta = $"Stock critico: {medicamento.Nombre} tiene {medicamento.StockActual} unidades (minimo: {medicamento.StockMinimo})";

            // evitar alertas duplicadas para el mismo medicamento
            lock (_lock)
            {
                if (!AlertasActivas.Contains(alerta))
                    AlertasActivas.Add(alerta);
            }
        }
    }
}
