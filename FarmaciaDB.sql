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