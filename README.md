Primera entrega de trabajo practico integrador 

Breve descripcion del proyecto 

## Arquitectura y Patrones de Diseño

### Estructura de 4 Capas

El proyecto está organizado en cuatro capas claramente separadas:

**CapaDeEntidades**: Define los modelos de dominio (Usuario, Cliente, Medicamento, Venta, Compra, etc.). Cada entidad cuenta con dos constructores: uno sin parámetros para crear instancias en tiempo de ejecución, y otro parametrizado para mapear datos desde la base de datos.

**CapaDeLogicaDeNegocio_BLL**: Contiene los servicios que implementan la lógica de negocio. Aquí residen las validaciones, reglas de negocio y orquestación entre capas. AuthService gestiona autenticación y permisos.

**CapaDeAccesoDatos_DAL**: Maneja la comunicación con la base de datos exclusivamente a través de procedimientos almacenados. No contiene SQL embebido. 
  ACLARACION: En esta primera etapa aun no contiene la conexion a la DB.

**CapaDePresentacion**: Controllers y Views que manejan las solicitudes HTTP y presentan la interfaz al usuario según su rol y permisos.

### Patrones Implementados

**Singleton**: El AuthService es una instancia única en toda la aplicación. Se inicializa automáticamente al cargar la clase. Administra la autenticación de usuarios y la configuración de perfiles.

**Composite**: Los perfiles (PerfilComposite) y permisos (PermisoSimple) forman una estructura jerárquica. Cada perfil puede contener múltiples permisos, permitiendo consultas simples sobre qué acciones tiene autorizado cada rol.

**Strategy**: Los cálculos de descuento implementan diferentes estrategias según el tipo de cliente. Cada estrategia encapsula un algoritmo de descuento distinto (descuento por obra social, descuento particular) permitiendo cambiar el comportamiento en tiempo de ejecución sin modificar el código cliente.

### Relaciones entre Entidades

Las entidades principales están relacionadas de forma relacional: Venta contiene múltiples DetalleVenta, Compra contiene múltiples DetalleCompra, Medicamento está categorizado y puede requerir receta. Esta estructura refleja la realidad del negocio y prepara el modelo para una base de datos relacional.

