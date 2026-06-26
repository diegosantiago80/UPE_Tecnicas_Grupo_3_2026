using System.Collections.Generic;

namespace CapaDePresentacion_Web.Models
{
    public class CompraRequestDTO
    {
        public int IdLaboratorio { get; set; }
        public List<ItemCompraDTO> Items { get; set; } = new List<ItemCompraDTO>();
    }

    public class ItemCompraDTO
    {
        public int IdMedicamento { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
    }
}
