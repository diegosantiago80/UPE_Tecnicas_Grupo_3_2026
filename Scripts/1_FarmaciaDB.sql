-- Crea la base de datos solo si no existe
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'FarmaciaDB')
BEGIN
    CREATE DATABASE FarmaciaDB;
END
GO

USE FarmaciaDB;
GO

-- ---------------------------------------------
-- ESTRUCTURA DE LAS TABLAS
-- ---------------------------------------------

-- Tabla Perfil (roles del sistema)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Perfil')
BEGIN
    CREATE TABLE Perfil (
        IdPerfil INT IDENTITY(1,1) PRIMARY KEY,
        PermisosAsignados VARCHAR(500) NULL,
        Descripcion VARCHAR(60) NOT NULL
    );
END;

-- Tabla Persona (datos personales)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Persona')
BEGIN
    CREATE TABLE Persona (
        IdPersona INT IDENTITY(1,1) PRIMARY KEY,
        DNI VARCHAR(20) UNIQUE NOT NULL,
        Nombre VARCHAR(45) NOT NULL,
        Apellido VARCHAR(45) NOT NULL,
        Telefono VARCHAR(30) NOT NULL,
        Email VARCHAR(45) NOT NULL
    );
END;

-- Tabla Usuario (cuentas de acceso del personal)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Usuario')
BEGIN
    CREATE TABLE Usuario (
        IdUsuario INT IDENTITY(1,1) PRIMARY KEY,
        IdPersona INT NOT NULL UNIQUE,
        NombreUsuario VARCHAR(45) UNIQUE NOT NULL,
        Contrasena VARCHAR(255) NOT NULL,
        IdPerfil INT NOT NULL,
        Activo BIT DEFAULT 1,
        
        -- Claves foraneas y restricciones
        CONSTRAINT FK_Usuario_Persona FOREIGN KEY (IdPersona) REFERENCES Persona(IdPersona),
        CONSTRAINT FK_Usuario_Perfil FOREIGN KEY (IdPerfil) REFERENCES Perfil(IdPerfil)
    );
END;
GO

-- Tabla de Auditoria (registros de los usuarios)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Auditoria')
BEGIN
    CREATE TABLE Auditoria (
        IdAuditoria INT IDENTITY(1,1) PRIMARY KEY,
        FechaHora DATETIME NOT NULL DEFAULT GETDATE(),
        IdUsuario INT NOT NULL,
        Modulo VARCHAR(60) NOT NULL,
        Accion VARCHAR(255) NOT NULL,
        CONSTRAINT FK_Auditoria_Usuario FOREIGN KEY (IdUsuario) REFERENCES Usuario(IdUsuario)
    );
END;
GO

-- ---------------------------------------------
-- CARGA DE DATOS INICIALES (SEED DATA)
-- ---------------------------------------------

-- Insercion de los 4 perfiles iniciales
IF NOT EXISTS (SELECT 1 FROM Perfil WHERE IdPerfil = 1)
BEGIN
    SET IDENTITY_INSERT Perfil ON;
    INSERT INTO Perfil (IdPerfil, Descripcion) VALUES 
    (1, 'Administrador'),
    (2, 'Vendedor'),
    (3, 'Encargado'),
    (4, 'Gerente');
    SET IDENTITY_INSERT Perfil OFF;
END;

-- Insercion de datos asociadas a los empleados
IF NOT EXISTS (SELECT 1 FROM Persona WHERE DNI = '99999991')
BEGIN
    INSERT INTO Persona (DNI, Nombre, Apellido, Telefono, Email) VALUES
    ('99999991', 'Kiara', 'Poloni', '11223344', 'kiara.poloni@upe.edu.ar'),
    ('99999992', 'Victoria', 'Mac Kenzie', '11556677', 'victoria.mackenzie@upe.edu.ar'),
    ('99999993', 'Diego', 'Santiago', '11889900', 'diego.santiago@upe.edu.ar'),
    ('99999994', 'Cristian', 'López', '11443322', 'cristian.lopez@upe.edu.ar');
END;

-- Insercion de usuarios del sistema mapeados a sus perfiles y datos
IF NOT EXISTS (SELECT 1 FROM Usuario WHERE NombreUsuario = 'admin')
BEGIN
    INSERT INTO Usuario (IdPersona, NombreUsuario, Contrasena, IdPerfil, Activo) VALUES
    ((SELECT IdPersona FROM Persona WHERE DNI = '99999991'), 'admin', CONVERT(VARCHAR(255), HASHBYTES('SHA2_256', '123'), 2), 1, 1),
    ((SELECT IdPersona FROM Persona WHERE DNI = '99999992'), 'vendedor', CONVERT(VARCHAR(255), HASHBYTES('SHA2_256', '123'), 2), 2, 1),
    ((SELECT IdPersona FROM Persona WHERE DNI = '99999993'), 'deposito', CONVERT(VARCHAR(255), HASHBYTES('SHA2_256', '123'), 2), 3, 1),
    ((SELECT IdPersona FROM Persona WHERE DNI = '99999994'), 'gerente', CONVERT(VARCHAR(255), HASHBYTES('SHA2_256', '123'), 2), 4, 1);
END;
GO

-----------------------------------------------------------------------
-- Insercion de datos de ejemplo para auditoria (fechas fijas, segundos en 00)
INSERT INTO Auditoria (FechaHora, IdUsuario, Modulo, Accion) VALUES
  ('2026-06-13 14:30:00', 1, 'Gestión de Usuarios', 'Modificación del usuario: admin'),
  ('2026-06-14 09:12:00', 2, 'Registrar Venta', 'Registró la venta N° 11 por $1200,00'),
  ('2026-06-12 16:05:00', 3, 'Nueva compra', 'Registró la compra N° 1 por $450,00'),
  ('2026-06-15 11:00:00', 4, 'Reporte de estadísticas', 'Consultó estadísticas de medicamentos');
GO

-- ---------------------------------------------
-- CREACION DE STORED PROCEDURES (SPs)
-- ---------------------------------------------

-- SP para validacion de credenciales en el Login
CREATE OR ALTER PROCEDURE SP_ValidarUsuario
    @Usuario VARCHAR(45),
    @Contrasena VARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Se encripta la contraseña recibida para compararla con la base de datos
    DECLARE @ContrasenaHash VARCHAR(255) = CONVERT(VARCHAR(255), HASHBYTES('SHA2_256', @Contrasena), 2);
    
    SELECT 
        u.IdUsuario,
        u.NombreUsuario,
        u.IdPerfil,
        u.Activo,
        p.DNI,
        p.Nombre,
        p.Apellido,
        p.Email
    FROM Usuario u
    INNER JOIN Persona p ON u.IdPersona = p.IdPersona 
    WHERE u.NombreUsuario = @Usuario 
      AND u.Contrasena = @ContrasenaHash 
      AND u.Activo = 1; 
END;
GO

-- SP para recuperar la descripcion del perfil asignado
CREATE OR ALTER PROCEDURE SP_ObtenerPerfilPorId
    @IdPerfil INT 
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT IdPerfil, Descripcion 
    FROM Perfil 
    WHERE IdPerfil = @IdPerfil;
END;
GO

-- SP para obtener todos los usuarios (para el index)
CREATE OR ALTER PROCEDURE SP_ObtenerUsuarios
AS
BEGIN
    SET NOCOUNT ON;
    
    -- INNER JOIN para traer los datos del usuario y de su persona asociada
    SELECT 
        u.IdUsuario, 
        u.NombreUsuario, 
        u.Contrasena, 
        u.IdPerfil, 
        u.Activo,
        p.IdPersona, 
        p.DNI, 
        p.Nombre, 
        p.Apellido, 
        p.Telefono, 
        p.Email
    FROM Usuario u
    INNER JOIN Persona p ON u.IdPersona = p.IdPersona;
END;
GO

-- SP para crear un nuevo usuario 
CREATE OR ALTER PROCEDURE SP_AgregarUsuario
    @DNI VARCHAR(20),
    @Nombre VARCHAR(45),
    @Apellido VARCHAR(45),
    @Telefono VARCHAR(30),
    @Email VARCHAR(45),
    @NombreUsuario VARCHAR(45),
    @Contrasena VARCHAR(255),
    @IdPerfil INT
AS
BEGIN

SET NOCOUNT ON;

IF LEN(@Nombre) NOT BETWEEN 2 AND 15 OR @Nombre LIKE '%[^A-Za-zÀ-ÿ ]%'
BEGIN
    RAISERROR('El nombre ingresado no es valido', 16, 1);
    RETURN;
END

IF LEN(@Apellido) NOT BETWEEN 2 AND 15 OR @Apellido LIKE '%[^A-Za-zÀ-ÿ ]%'
BEGIN
    RAISERROR('El apellido ingresado no es valido', 16, 1);
    RETURN;
END

IF LEN(@DNI) NOT BETWEEN 7 AND 8 OR @DNI LIKE '%[^0-9]%'
BEGIN
    RAISERROR('El DNI ingresado no es valido', 16, 1);
    RETURN;
END

IF LEN(@Telefono) NOT BETWEEN 8 AND 15 OR @Telefono LIKE '%[^0-9]%'
    BEGIN
        RAISERROR('El telefono ingresado no es valido', 16, 1);
        RETURN;
END

IF LEN(@Email) NOT BETWEEN 5 AND 50 OR @Email NOT LIKE '%@%.%' OR @Email LIKE '% %'
BEGIN
    RAISERROR('El email ingresado no es valido', 16, 1);
    RETURN;
END

IF LEN(@NombreUsuario) NOT BETWEEN 5 AND 10 OR @NombreUsuario LIKE '%[^A-Za-z0-9]%'
BEGIN
    RAISERROR('El nombre de usuario ingresado no es valido', 16, 1);
    RETURN;
END
    
    -- Transaccion para asegurar que se guarden ambas tablas o ninguna
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Inserta primero en la tabla Persona
        DECLARE @IdPersonaGenerado INT;
        INSERT INTO Persona (DNI, Nombre, Apellido, Telefono, Email)
        VALUES (@DNI, @Nombre, @Apellido, @Telefono, @Email);
        
        -- Captura el ID de la persona que se acaba de crear
        SET @IdPersonaGenerado = SCOPE_IDENTITY();
        
        -- Encripta la contraseña
        DECLARE @ContrasenaHash VARCHAR(255) = CONVERT(VARCHAR(255), HASHBYTES('SHA2_256', @Contrasena), 2);
        
        -- Inserta en usuario usando el ID de la persona creada
        INSERT INTO Usuario (IdPersona, NombreUsuario, Contrasena, IdPerfil, Activo)
        VALUES (@IdPersonaGenerado, @NombreUsuario, @ContrasenaHash, @IdPerfil, 1);
        
        -- Si todo salio bien confirma los cambios
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        -- Si hubo un error deshace todo
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO

-- SP para modificar un usuario existente
CREATE OR ALTER PROCEDURE SP_ModificarUsuario
    @IdUsuario INT,
    @DNI VARCHAR(20),
    @Nombre VARCHAR(45),
    @Apellido VARCHAR(45),
    @Telefono VARCHAR(30),
    @Email VARCHAR(45),
    @NombreUsuario VARCHAR(45),
    @Contrasena VARCHAR(255),
    @IdPerfil INT
AS
BEGIN

SET NOCOUNT ON;

IF LEN(@Nombre) NOT BETWEEN 2 AND 15 OR @Nombre LIKE '%[^A-Za-zÀ-ÿ ]%'
BEGIN
    RAISERROR('El nombre ingresado no es valido', 16, 1);
    RETURN;
END

IF LEN(@Apellido) NOT BETWEEN 2 AND 15 OR @Apellido LIKE '%[^A-Za-zÀ-ÿ ]%'
BEGIN
    RAISERROR('El apellido ingresado no es valido', 16, 1);
    RETURN;
END

IF LEN(@DNI) NOT BETWEEN 7 AND 8 OR @DNI LIKE '%[^0-9]%'
BEGIN
    RAISERROR('El DNI ingresado no es valido', 16, 1);
    RETURN;
END

IF LEN(@Telefono) NOT BETWEEN 8 AND 15 OR @Telefono LIKE '%[^0-9]%'
    BEGIN
        RAISERROR('El telefono ingresado no es valido', 16, 1);
        RETURN;
END

IF LEN(@Email) NOT BETWEEN 5 AND 50 OR @Email NOT LIKE '%@%.%' OR @Email LIKE '% %'
BEGIN
    RAISERROR('El email ingresado no es valido', 16, 1);
    RETURN;
END

IF LEN(@NombreUsuario) NOT BETWEEN 5 AND 10 OR @NombreUsuario LIKE '%[^A-Za-z0-9]%'
BEGIN
    RAISERROR('El nombre de usuario ingresado no es valido', 16, 1);
    RETURN;
END
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        DECLARE @IdPersonaAsociada INT = (SELECT IdPersona FROM Usuario WHERE IdUsuario = @IdUsuario);
        
        -- Actualiza los datos personales
        UPDATE Persona
        SET DNI = @DNI, Nombre = @Nombre, Apellido = @Apellido, Telefono = @Telefono, Email = @Email
        WHERE IdPersona = @IdPersonaAsociada;
        
        -- Actualiza el Usuario
        UPDATE Usuario
        SET NombreUsuario = @NombreUsuario,
            IdPerfil = @IdPerfil,
            -- Si recibe una contraseña nueva, la encripta
            -- Si esta vacia, deja la que ya estaba en la base de datos
            Contrasena = CASE 
                            WHEN LTRIM(RTRIM(@Contrasena)) <> '' 
                            THEN CONVERT(VARCHAR(255), HASHBYTES('SHA2_256', @Contrasena), 2)
                            ELSE Contrasena 
                         END
        WHERE IdUsuario = @IdUsuario;
        
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO

-- SP para obtener un usuario por id
CREATE OR ALTER PROCEDURE SP_ObtenerUsuarioPorId
    @IdUsuario INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        u.IdUsuario,
        u.NombreUsuario,
        u.Contrasena,
        u.IdPerfil,
        u.Activo,
        p.IdPersona,
        p.DNI,
        p.Nombre,
        p.Apellido,
        p.Telefono,
        p.Email
    FROM Usuario u
    INNER JOIN Persona p ON u.IdPersona = p.IdPersona
    WHERE u.IdUsuario = @IdUsuario;
END;
GO

-- SP para validar unicidad de nombre de usuario y dni antes de agregar o modificar
-- retorna Resultado = 0 con un mensaje si hay conflicto, Resultado = 1 si todo esta libre
CREATE OR ALTER PROCEDURE SP_ValidarUnicidadUsuario
    @NombreUsuario VARCHAR(45),
    @Dni           VARCHAR(20),
    @IdUsuario     INT = 0   -- 0 en alta, el id real en modificacion
AS
BEGIN
    SET NOCOUNT ON;

    -- nombre de usuario ya usado por otro usuario activo
    IF EXISTS (
        SELECT 1 FROM Usuario u
        WHERE u.NombreUsuario = @NombreUsuario
          AND u.Activo = 1
          AND u.IdUsuario <> @IdUsuario
    )
    BEGIN
        SELECT 0 AS Resultado, 'El nombre de usuario ya existe, por favor elija otro' AS Mensaje;
        RETURN;
    END;

    -- nombre de usuario pertenece a un usuario dado de baja
    IF EXISTS (
        SELECT 1 FROM Usuario u
        WHERE u.NombreUsuario = @NombreUsuario
          AND u.Activo = 0
          AND u.IdUsuario <> @IdUsuario
    )
    BEGIN
        SELECT 0 AS Resultado, 'Ese nombre de usuario no es valido' AS Mensaje;
        RETURN;
    END;

    -- dni ya registrado por otro usuario activo
    IF EXISTS (
        SELECT 1 FROM Usuario u
        INNER JOIN Persona p ON u.IdPersona = p.IdPersona
        WHERE p.DNI = @Dni
          AND u.Activo = 1
          AND u.IdUsuario <> @IdUsuario
    )
    BEGIN
        SELECT 0 AS Resultado, 'El DNI ingresado ya se encuentra registrado en el sistema' AS Mensaje;
        RETURN;
    END;

    -- dni pertenece a un usuario dado de baja
    IF EXISTS (
        SELECT 1 FROM Usuario u
        INNER JOIN Persona p ON u.IdPersona = p.IdPersona
        WHERE p.DNI = @Dni
          AND u.Activo = 0
          AND u.IdUsuario <> @IdUsuario
    )
    BEGIN
        SELECT 0 AS Resultado, 'Ese numero de DNI no es valido' AS Mensaje;
        RETURN;
    END;

    SELECT 1 AS Resultado, '' AS Mensaje;
END;
GO

-- SP para eliminar un usuario (baja logica)
CREATE OR ALTER PROCEDURE SP_EliminarUsuario
    @IdUsuario INT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Cambia el estado
    UPDATE Usuario
    SET Activo = 0
    WHERE IdUsuario = @IdUsuario;
END;
GO

-----------------------------------------------------------------------

-- SP para recuperar perfiles
CREATE OR ALTER PROCEDURE SP_RecuperarPerfiles
AS
BEGIN
    SET NOCOUNT ON;
    SELECT IdPerfil, Descripcion, ISNULL(PermisosAsignados, '') AS PermisosAsignados 
    FROM Perfil ORDER BY Descripcion;
END;
GO

-- SP para obtener perfil por ID
CREATE OR ALTER PROCEDURE SP_ObtenerPerfilPorId
    @IdPerfil INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT IdPerfil, Descripcion, ISNULL(PermisosAsignados, '') AS PermisosAsignados 
    FROM Perfil WHERE IdPerfil = @IdPerfil;
END;
GO

-- SP para agregar un perfil
CREATE OR ALTER PROCEDURE SP_AgregarPerfil
    @Descripcion VARCHAR(60),
    @PermisosAsignados VARCHAR(500)
AS
BEGIN

SET NOCOUNT ON;

IF LEN(@Descripcion) NOT BETWEEN 1 AND 15 OR @Descripcion LIKE '%[^A-Za-z0-9 ]%'
BEGIN
    SELECT 0 AS Resultado, 'La descripción ingresada no es valida' AS Mensaje;
    RETURN;
END

    INSERT INTO Perfil (Descripcion, PermisosAsignados) VALUES (@Descripcion, @PermisosAsignados);
    SELECT 1 AS Resultado, '¡Perfil registrado exitosamente!' AS Mensaje;
END;
GO

-- SP para modificar un perfil
CREATE OR ALTER PROCEDURE SP_ModificarPerfil
    @IdPerfil INT,
    @Descripcion VARCHAR(60),
    @PermisosAsignados VARCHAR(500)
AS
BEGIN

SET NOCOUNT ON;

IF LEN(@Descripcion) NOT BETWEEN 1 AND 15 OR @Descripcion LIKE '%[^A-Za-z0-9 ]%'
BEGIN
    SELECT 0 AS Resultado, 'La descripción ingresada no es valida' AS Mensaje;
    RETURN;
END

    UPDATE Perfil 
    SET Descripcion = @Descripcion, PermisosAsignados = @PermisosAsignados 
    WHERE IdPerfil = @IdPerfil;
    SELECT 1 AS Resultado, '¡Perfil modificado exitosamente!' AS Mensaje;
END;
GO

-- SP para eliminar un perfil
CREATE OR ALTER PROCEDURE SP_EliminarPerfil
    @IdPerfil INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Movemos los usuarios inactivos al perfil administrador (IdPerfil = 1) para liberar la clave foranea
        -- Activo = 0 para que no pueden loguearse al sistema
        UPDATE Usuario 
        SET IdPerfil = 1 
        WHERE IdPerfil = @IdPerfil AND Activo = 0;

        DELETE FROM Perfil WHERE IdPerfil = @IdPerfil;
        
        COMMIT TRANSACTION;
        SELECT 1 AS Resultado, '¡Perfil eliminado exitosamente!' AS Mensaje;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        SELECT 0 AS Resultado, 'No se pudo eliminar el perfil por un conflicto en la base de datos.' AS Mensaje;
    END CATCH
END;
GO

-- SP para validar unicidad de la descripcion del perfil y el id del perfil antes de agregar o modificar
CREATE OR ALTER PROCEDURE SP_ValidarUnicidadPerfil
    @Descripcion VARCHAR(60),
    @IdPerfil INT = 0
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM Perfil WHERE Descripcion = @Descripcion AND IdPerfil <> @IdPerfil)
        SELECT 0 AS Resultado, '¡ALERTA! El perfil descrito ya existe.' AS Mensaje;
    ELSE
        SELECT 1 AS Resultado, '' AS Mensaje;
END;
GO

-- SP para verificar si un perfil esta en uso
CREATE OR ALTER PROCEDURE SP_PerfilEnUso
    @IdPerfil INT
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM Usuario WHERE IdPerfil = @IdPerfil AND Activo = 1)
        SELECT 1 AS Resultado;
    ELSE
        SELECT 0 AS Resultado;
END;
GO

-----------------------------------------------------------------------

-- Le asignamos los permisos que tiene cada perfil
UPDATE Perfil 
SET PermisosAsignados = 'GestionarUsuarios,ConfigurarPerfiles,ConsultarLog' 
WHERE IdPerfil = 1;

UPDATE Perfil 
SET PermisosAsignados = 'RegistrarVenta,GestionarClientes' 
WHERE IdPerfil = 2;

UPDATE Perfil 
SET PermisosAsignados = 'ControlStock,RegistrarCompra,GestionarMedicamentos,GestionarLaboratorios' 
WHERE IdPerfil = 3;

UPDATE Perfil 
SET PermisosAsignados = 'VerReporte,GenerarEstadisticas,GenerarProyeccion' 
WHERE IdPerfil = 4;
GO

---------------------------------------------------------------------
-- SP para registrar logs
CREATE OR ALTER PROCEDURE SP_RegistrarLogAuditoria
    @IdUsuario INT,
    @Modulo VARCHAR(60),
    @Accion VARCHAR(255)
AS
BEGIN
    INSERT INTO Auditoria (IdUsuario, Modulo, Accion)
    VALUES (@IdUsuario, @Modulo, @Accion);
END;
GO

-- SP para recuperar logs
CREATE OR ALTER PROCEDURE SP_RecuperarLogsAuditoria
    @FechaDesde DATE = NULL,
    @FechaHasta DATE = NULL,
    @Usuario VARCHAR(45) = NULL,
    @Modulo VARCHAR(60) = NULL
AS
BEGIN
    SELECT a.FechaHora, u.NombreUsuario AS Usuario, p.Descripcion AS Perfil, a.Modulo, a.Accion
    FROM Auditoria a
    INNER JOIN Usuario u ON a.IdUsuario = u.IdUsuario
    INNER JOIN Perfil p ON u.IdPerfil = p.IdPerfil
    WHERE (@FechaDesde IS NULL OR CAST(a.FechaHora AS DATE) >= @FechaDesde)
      AND (@FechaHasta IS NULL OR CAST(a.FechaHora AS DATE) <= @FechaHasta)
      AND (@Usuario IS NULL OR u.NombreUsuario LIKE '%' + @Usuario + '%')
      AND (@Modulo IS NULL OR a.Modulo = @Modulo)
    ORDER BY a.FechaHora DESC;
END;
GO
