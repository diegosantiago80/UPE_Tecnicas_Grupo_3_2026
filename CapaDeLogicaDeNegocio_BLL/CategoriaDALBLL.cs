using CapaDeAccesoADatos_DAL;
using CapaDeEntidades;
using System.Collections.Generic;

namespace CapaDeLogicaDeNegocio_BLL
{
    public class CategoriaDALBLL
    {
        private readonly CategoriaDAL _categoriaDAL = new CategoriaDAL();

        public List<Categoria> ObtenerTodos()
        {
            return _categoriaDAL.ObtenerTodos();
        }
    }
}
