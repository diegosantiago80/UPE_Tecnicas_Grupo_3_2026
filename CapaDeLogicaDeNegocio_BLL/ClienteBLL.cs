using CapaDeAccesoADatos_DAL;
using CapaDeEntidades;
using Microsoft.Data.SqlClient;

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

            return _clienteDAL.Crear(nuevoCliente);
        }
    }
}
