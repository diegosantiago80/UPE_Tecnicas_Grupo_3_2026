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
        public static CategoriaBLL CrearCategoriaBLL() => new CategoriaBLL();
        public static VentaBLL CrearVentaBLL() => new VentaBLL();
        public static ClienteBLL CrearClienteBLL() => new ClienteBLL();
        public static UsuarioBLL CrearUsuarioBLL() => new UsuarioBLL();
        public static PerfilBLL CrearPerfilBLL() => new PerfilBLL();
        public static AuditoriaBLL CrearAuditoriaBLL() => new AuditoriaBLL();
        public static ReporteBLL_TemplateMethod CrearReporteBLL() => new ReporteBLL_TemplateMethod();
    }
}
