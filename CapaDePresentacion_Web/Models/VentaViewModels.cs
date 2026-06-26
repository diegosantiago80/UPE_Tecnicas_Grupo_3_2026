using System.Collections.Generic;

namespace CapaDePresentacion_Web.Models
{
    public class VentaRequestDTO
    {
        public string DniCliente { get; set; } = string.Empty;
        public string TipoCobro { get; set; } = string.Empty;
        public List<ItemVentaDTO> Lineas { get; set; } = new List<ItemVentaDTO>();
    }

    public class ItemVentaDTO
    {
        public int IdMedicamento { get; set; }
        public int Cantidad { get; set; }
        public decimal Precio { get; set; }
    }
}
