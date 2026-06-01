namespace CapaDeLogicaDeNegocio_BLL
{
    // patron factory: centraliza la creacion de servicios de la capa de negocio
    // los controllers no instancian los bll directamente, los piden a la factory
    // esto facilita el mantenimiento y el testing porque hay un solo lugar donde
    // se decide como se construye cada servicio
    public static class BLLFactory
    {
        public static MedicamentoBLL CrearMedicamentoBLL() => new MedicamentoBLL();
        public static LaboratorioBLL CrearLaboratorioBLL() => new LaboratorioBLL();
        public static CompraBLL CrearCompraBLL() => new CompraBLL();
        public static CategoriaDALBLL CrearCategoriaBLL() => new CategoriaDALBLL();
        public static VentaBLL CrearVentaBLL() => new VentaBLL();
        public static ClienteBLL CrearClienteBLL() => new ClienteBLL();
    }
}
