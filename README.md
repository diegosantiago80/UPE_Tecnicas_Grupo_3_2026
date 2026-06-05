# UPE_Tecnicas_GrupoX_2026
# Sistema de Gestion de Farmacia — Grupo 3

**Materia:** Tecnicas de Programacion  
**Profesor:** Ing. Chimuris, Andres  
**Institucion:** UPE — 2026  
**Repositorio:** [UPE_Tecnicas_Grupo_3_2026](https://github.com/diegosantiago80/UPE_Tecnicas_Grupo_3_2026)

---

## Integrantes

| Nombre | Modulo | Rama |
|---|---|---|
| Diego Santiago | Stock / BD / Infraestructura | `feature/stock` |
| Victoria Mac Kenzie | Ventas + Clientes | `feature/ventas` |
| Cristian Lopez | Reportes del Gerente | `feature/gerencia` |
| Kiara Poloni | Usuarios (ABM) + Login | `feature/login` |

---

## Descripcion

Sistema administrativo de compras y ventas de productos para una farmacia. Permite gestionar usuarios con distintos roles, clientes, medicamentos, ventas y reportes gerenciales.

---

## Arquitectura — 4 Capas

| Capa | Proyecto | Responsabilidad |
|---|---|---|
| **DAL** | `CapaDeAccesoADatos_DAL` | Acceso a BD exclusivamente mediante stored procedures. Sin SQL embebido. |
| **Entidades** | `CapaDeEntidades` | Modelos y clases de dominio (Usuario, Cliente, Medicamento, Venta, etc.) |
| **BLL** | `CapaDeLogicaDeNegocio_BLL` | Logica de negocio, validaciones y servicios |
| **Web** | `CapaDePresentacion_Web` | Controllers, Views y configuracion ASP.NET Core MVC |

---

## Tecnologias

- **Backend:** C# / ASP.NET Core MVC (.NET 10)
- **Base de datos:** SQL Server
- **Acceso a datos:** ADO.NET con stored procedures
- **Frontend:** Razor Views + Bootstrap 5

---

## Roles del sistema

| Perfil | Permisos principales |
|---|---|
| **Administrador** | Alta de usuarios y perfiles |
| **Vendedor** | Gestion de clientes, registro de ventas |
| **Encargado de Stock** | Gestion de medicamentos, laboratorios, compras, alertas de stock minimo |
| **Gerente** | Reportes de ventas, estadisticas de medicamentos, proyecciones |

**Usuarios de prueba** (contrasena: `123` para todos):

| Usuario | Perfil |
|---|---|
| `admin` | Administrador |
| `vendedor` | Vendedor |
| `deposito` | Encargado de Stock |
| `gerente` | Gerente |

---

## Patrones de diseno implementados

- **Singleton** — `AuthService`: instancia unica para login y gestion de sesion
- **Composite** — `PermisosComposite`: estructura jerarquica de permisos por perfil
- **Factory** — `BLLFactory`: creacion centralizada de servicios de la BLL
- **Template Method** — `ProcesadorDeReportesTemplate`: esqueleto del algoritmo de reportes con implementaciones concretas por tipo
- **Strategy** — `ICalculadorDescuento`: calculo de descuentos segun obra social del cliente
- **Observer** — `StockObserver`: notificaciones de stock critico
- **Proxy** — `ReporteDALProxy`: control de acceso y cache para la capa de reportes

---

## Instalacion

### Requisitos previos
- Visual Studio 2022 o superior
- SQL Server (local o instancia remota)
- .NET 10 SDK

### Pasos

**1. Ejecutar los scripts SQL en orden** sobre una base de datos `FarmaciaDB` nueva:

```
1. FarmaciaDB.sql                    -- Crea la BD, tablas base y usuarios del sistema
2. FarmaciaDB_Script_B_Victoria.sql  -- Tablas Cliente, Venta, DetalleVenta y SPs
3. FarmaciaDB_Script_C.sql           -- Tablas Categoria, Laboratorio, Medicamento, Compra y SPs
4. FarmaciaDB_Script_Gerente.sql     -- SPs de reportes del Gerente
5. FarmaciaDB_SeedData_Test.sql      -- (Opcional) Datos de prueba: 10 clientes y 10 ventas
```

**2. Configurar la cadena de conexion** en `CapaDePresentacion_Web/appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=TU_SERVIDOR;Database=FarmaciaDB;Trusted_Connection=True;"
}
```

**3. Compilar y ejecutar** desde Visual Studio (F5) o con:

```bash
cd CapaDePresentacion_Web
dotnet run
```

---

## Estructura de carpetas

```
TrabajoPracticoIntegrador_Upe2026_Grupo3/
├── CapaDeAccesoADatos_DAL/          -- DAL: stored procedures, conexion
├── CapaDeEntidades/                 -- Modelos de dominio
├── CapaDeLogicaDeNegocio_BLL/       -- Logica de negocio y patrones
│   └── Estrategias/                 -- Patron Strategy (descuentos)
├── CapaDePresentacion_Web/          -- ASP.NET Core MVC
│   ├── Controllers/
│   ├── Views/
│   └── wwwroot/
├── FarmaciaDB.sql
├── FarmaciaDB_Script_B_Victoria.sql
├── FarmaciaDB_Script_C.sql
├── FarmaciaDB_Script_Gerente.sql
└── FarmaciaDB_SeedData_Test.sql
```

---

## Convenciones de codigo

| Elemento | Convencion | Ejemplo |
|---|---|---|
| Atributos privados | Prefijo `_` | `_nombre`, `_idUsuario` |
| Metodos que recuperan datos | Prefijo `Obtener` / `Recuperar` | `ObtenerUsuarios()` |
| Variables y metodos | camelCase | `totalVenta`, `calcularDescuento()` |
| Clases | PascalCase | `UsuarioDAL`, `VentaBLL` |
| Comentarios | Minusculas, sin iconos | `// validar que el usuario tenga permisos` |
| Idioma del codigo | Ingles para nombres, espanol para comentarios | — |

---

## Ramas

| Rama | Contenido |
|---|---|
| `main` | Version estable integrada |
| `feature/stock` | Modulo de stock, BD e infraestructura compartida |
| `feature/ventas` | Modulo de ventas y clientes |
| `feature/gerencia` | Modulo de reportes del gerente |
| `feature/login` | Modulo de usuarios ABM y login |

---

## Entregables

| Entrega | Estado |
|---|---|
| 1ra entrega — Analisis y Diseno | Completada |
| 2da entrega — Integracion BD | En curso |
| 3ra entrega — Final | Pendiente |
