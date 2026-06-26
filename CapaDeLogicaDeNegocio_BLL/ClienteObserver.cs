using CapaDeEntidades;
using System.Collections.Generic;

namespace CapaDeLogicaDeNegocio_BLL
{
    // patron observer para notificaciones relacionadas a clientes
    // sigue la misma estructura que StockObserver

    // interfaz que deben implementar todos los observadores de cliente
    public interface IClienteObservador
    {
        void Notificar(Cliente cliente);
    }

    // sujeto que mantiene la lista de observadores y los notifica
    public class ClienteSujeto
    {
        private readonly List<IClienteObservador> _observadores = new List<IClienteObservador>();

        public void AgregarObservador(IClienteObservador observador)
        {
            _observadores.Add(observador);
        }

        public void QuitarObservador(IClienteObservador observador)
        {
            _observadores.Remove(observador);
        }

        // notifica a todos los observadores si el cliente es empleado
        public void EvaluarCliente(Cliente cliente)
        {
            if (cliente.EsEmpleado)
            {
                foreach (var obs in _observadores)
                    obs.Notificar(cliente);
            }
        }
    }

    // observador concreto: registra en memoria que se consulto un cliente que es empleado
    // util para auditoria o para mostrar avisos en pantalla
    public class AlertaEmpleadoObservador : IClienteObservador
    {
        private static readonly object _lock = new object();
        public static readonly List<string> AlertasActivas = new List<string>();

        public void Notificar(Cliente cliente)
        {
            string alerta = $"El cliente {cliente.Nombre} {cliente.Apellido} (DNI: {cliente.Dni}) esta registrado como empleado de la farmacia";

            lock (_lock)
            {
                if (!AlertasActivas.Contains(alerta))
                    AlertasActivas.Add(alerta);
            }
        }
    }
}
