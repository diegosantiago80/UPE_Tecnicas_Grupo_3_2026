using System;
using CapaDeAccesoADatos_DAL;
using CapaDeEntidades;

namespace CapaDeLogicaDeNegocio_BLL
{
    public class ClienteBLL
    {
        private readonly ClienteDAL _clienteDAL = new ClienteDAL();

        public Cliente? BuscarCliente(string dni)
        {
            if (string.IsNullOrEmpty(dni)) return null;
            return _clienteDAL.BuscarPorDni(dni);
        }

        public bool ModificarCliente(Cliente cliente)
        {
            return _clienteDAL.Actualizar(cliente);
        }

        public bool CrearCliente(Cliente nuevoCliente)
        {
            if (string.IsNullOrEmpty(nuevoCliente.Nombre)) return false;

            // si ya existe como cliente activo, no permitir duplicado
            var existente = _clienteDAL.BuscarPorDni(nuevoCliente.Dni);
            if (existente != null)
                throw new Exception($"El DNI {nuevoCliente.Dni} ya está registrado como cliente ({existente.Nombre} {existente.Apellido}).");

            // si el DNI ya existe en Persona (es empleado del sistema) usamos el SP de vinculacion
            // que reutiliza la Persona existente y setea EsEmpleado = 1 automaticamente
            if (_clienteDAL.PersonaExistePorDni(nuevoCliente.Dni))
                return _clienteDAL.VincularEmpleadoComoCliente(nuevoCliente);

            // persona completamente nueva: flujo normal
            return _clienteDAL.Crear(nuevoCliente);
        }
    }
}
