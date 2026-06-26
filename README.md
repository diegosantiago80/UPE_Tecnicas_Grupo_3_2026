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

- **Singleton** — `AuthService`: instancia unica para login y gestion de sesion (obligatorio)
- **Composite** — `PermisosComposite`: estructura jerarquica de permisos por perfil (obligatorio)
- **Factory** — `BLLFactory`: creacion centralizada de servicios de la BLL (patron creacional)
- **Template Method** — `ReporteBLL`: esqueleto del algoritmo de reportes, cada tipo de reporte implementa los pasos concretos (patron de comportamiento)
- **Strategy** — `ICalculadorDescuento`: calculo de descuentos segun tipo de cliente (Particular, ObraSocial, Empleado, EmpleadoObraSocial)
- **Observer** — `StockObserver` y `ClienteObserver`: notificaciones de stock critico y eventos de cliente
- **Filtro de autorizacion** — `SesionRequeridaAttribute`: control de acceso por rol en todos los controllers, evita acceso directo por URL

---

## Instalacion

### Requisitos previos
- Visual Studio 2022 o superior
- SQL Server (local o instancia remota)
- .NET 10 SDK

### Pasos

**1. Ejecutar los scripts SQL en orden** (carpeta `Scripts/`) sobre una base de datos `FarmaciaDB` nueva:

```
Scripts/1_FarmaciaDB.sql                 -- Crea la BD, tablas base y usuarios del sistema
Scripts/2_FarmaciaDB_Script.sql          -- Tablas Cliente, Venta, DetalleVenta y SPs
Scripts/3_ FarmaciaDB_Script.sql         -- Tablas Categoria, Laboratorio, Medicamento, Compra y SPs
Scripts/4_FarmaciaDB_Script.sql          -- SPs de reportes del Gerente
Scripts/5_FarmaciaDB_SeedData_Extra.sql  -- Datos adicionales de prueba
Scripts/6_FarmaciaDB_SeedData_Test.sql   -- (Opcional) 10 clientes y 10 ventas de prueba
Scripts/7_Farmacia_DB.Script_sql.sql     -- Script complementario
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
│   ├── Filters/                     -- Filtro SesionRequerida (autorizacion por rol)
│   ├── Models/                      -- ViewModels
│   ├── Views/
│   └── wwwroot/
├── Pruebas_unitarias/               -- Tests unitarios (xUnit)
├── Documentacion/                   -- Diagramas, especificaciones y entregables
└── Scripts/                         -- Scripts SQL numerados en orden de ejecucion
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

## Pruebas unitarias

| Archivo | Clase bajo prueba |
|---|---|
| `AuthServiceTests.cs` | `AuthService` (Singleton) |
| `AuthServicePermisosTests.cs` | `AuthService` — validacion de permisos por rol |
| `PermisosCompositeTests.cs` | `PermisosComposite` (Composite) |
| `ReporteBLLTests.cs` | `ReporteBLL` (Template Method) |
| `VentaBLLTests.cs` | `VentaBLL` — flujo de venta |
| `DescuentoTests.cs` | Estrategias de descuento (Strategy) |

---

## Entregables

| Entrega | Estado |
|---|---|
| 1ra entrega — Analisis y Diseno | Completada |
| 2da entrega — Integracion BD | Completada |
| 3ra entrega — Final | Completada |

