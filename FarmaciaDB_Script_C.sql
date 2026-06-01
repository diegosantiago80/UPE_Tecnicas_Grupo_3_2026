-- =============================================================
-- Script modulo C - Diego
-- Tablas: Categoria, Laboratorio, Medicamento, Compra, DetalleCompra
-- Stored Procedures para cada operacion
-- Ejecutar sobre FarmaciaDB (script de Kiara debe estar ejecutado primero)
-- =============================================================

USE FarmaciaDB;
GO

-- ---------------------------------------------
-- ESTRUCTURA DE LAS TABLAS
-- ---------------------------------------------

-- Tabla Categoria
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Categoria')
BEGIN
    CREATE TABLE Categoria (
        IdCategoria  INT IDENTITY(1,1) PRIMARY KEY,
        Nombre       VARCHAR(100) NOT NULL,
        Descripcion  VARCHAR(300) NULL
    );
END;

-- Tabla Laboratorio
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Laboratorio')
BEGIN
    CREATE TABLE Laboratorio (
        IdLaboratorio INT IDENTITY(1,1) PRIMARY KEY,
        RazonSocial   VARCHAR(200) NOT NULL,
        Cuit          VARCHAR(15)  NOT NULL,
        Telefono      VARCHAR(20)  NULL,
        Email         VARCHAR(100) NULL,
        Direccion     VARCHAR(200) NULL,
        Activo        BIT DEFAULT 1 NOT NULL
    );
END;

-- Tabla Medicamento
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Medicamento')
BEGIN
    CREATE TABLE Medicamento (
        IdMedicamento  INT IDENTITY(1,1) PRIMARY KEY,
        IdLaboratorio  INT NOT NULL,
        IdCategoria    INT NOT NULL,
        Nombre         VARCHAR(150) NOT NULL,
        Descripcion    VARCHAR(300) NULL,
        PrecioVenta    DECIMAL(10,2) NOT NULL DEFAULT 0,
        PrecioCompra   DECIMAL(10,2) NOT NULL DEFAULT 0,
        StockActual    INT NOT NULL DEFAULT 0,
        StockMinimo    INT NOT NULL DEFAULT 0,
        RequiereReceta BIT DEFAULT 0 NOT NULL,
        Activo         BIT DEFAULT 1 NOT NULL,

        CONSTRAINT FK_Medicamento_Laboratorio FOREIGN KEY (IdLaboratorio) REFERENCES Laboratorio(IdLaboratorio),
        CONSTRAINT FK_Medicamento_Categoria   FOREIGN KEY (IdCategoria)   REFERENCES Categoria(IdCategoria)
    );
END;

-- Tabla Compra
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Compra')
BEGIN
    CREATE TABLE Compra (
        IdCompra      INT IDENTITY(1,1) PRIMARY KEY,
        Fecha         DATETIME NOT NULL DEFAULT GETDATE(),
        IdLaboratorio INT NOT NULL,
        IdUsuario     INT NOT NULL,
        Total         DECIMAL(10,2) NOT NULL DEFAULT 0,

        CONSTRAINT FK_Compra_Laboratorio FOREIGN KEY (IdLaboratorio) REFERENCES Laboratorio(IdLaboratorio),
        CONSTRAINT FK_Compra_Usuario     FOREIGN KEY (IdUsuario)     REFERENCES Usuario(IdUsuario)
    );
END;

-- Tabla DetalleCompra
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'DetalleCompra')
BEGIN
    CREATE TABLE DetalleCompra (
        IdDetalleCompra INT IDENTITY(1,1) PRIMARY KEY,
        IdCompra        INT NOT NULL,
        IdMedicamento   INT NOT NULL,
        Cantidad        INT NOT NULL,
        PrecioUnitario  DECIMAL(10,2) NOT NULL,

        CONSTRAINT FK_DetalleCompra_Compra      FOREIGN KEY (IdCompra)      REFERENCES Compra(IdCompra),
        CONSTRAINT FK_DetalleCompra_Medicamento FOREIGN KEY (IdMedicamento) REFERENCES Medicamento(IdMedicamento)
    );
END;
GO

-- ---------------------------------------------
-- SEED DATA
-- ---------------------------------------------

-- categorias iniciales
IF NOT EXISTS (SELECT 1 FROM Categoria WHERE IdCategoria = 1)
BEGIN
    SET IDENTITY_INSERT Categoria ON;
    INSERT INTO Categoria (IdCategoria, Nombre, Descripcion) VALUES
    (1, 'Analgesicos',      'Medicamentos para el dolor'),
    (2, 'Antibioticos',     'Medicamentos para infecciones bacterianas'),
    (3, 'Antiinflamatorios','Medicamentos para reducir la inflamacion'),
    (4, 'Antialergicos',    'Medicamentos para alergias'),
    (5, 'Cardiovasculares', 'Medicamentos para el sistema cardiovascular');
    SET IDENTITY_INSERT Categoria OFF;
END;

-- laboratorios iniciales
IF NOT EXISTS (SELECT 1 FROM Laboratorio WHERE IdLaboratorio = 1)
BEGIN
    SET IDENTITY_INSERT Laboratorio ON;
    INSERT INTO Laboratorio (IdLaboratorio, RazonSocial, Cuit, Telefono, Email, Direccion, Activo) VALUES
    (1, 'Bayer S.A.',  '30-12345678-9', '11-4444-5555', 'ventas@bayer.com',        'Calle Falsa 123',   1),
    (2, 'Roemmers',    '30-87654321-0', '11-2222-3333', 'contacto@roemmers.com',   'Av. Santa Fe 456',  1),
    (3, 'Bago',        '30-55556666-1', '11-9999-8888', 'info@bago.com.ar',        'Rivadavia 789',     1);
    SET IDENTITY_INSERT Laboratorio OFF;
END;

-- medicamentos iniciales
IF NOT EXISTS (SELECT 1 FROM Medicamento WHERE IdMedicamento = 1)
BEGIN
    SET IDENTITY_INSERT Medicamento ON;
    INSERT INTO Medicamento (IdMedicamento, IdLaboratorio, IdCategoria, Nombre, Descripcion, PrecioVenta, PrecioCompra, StockActual, StockMinimo, RequiereReceta, Activo) VALUES
    (1, 1, 1, 'Paracetamol', '500mg - Analgesico',        500.00, 300.00,  50,  10, 0, 1),
    (2, 1, 3, 'Ibuprofeno',  '600mg - Antiinflamatorio',  800.00, 450.00,   5,  15, 0, 1),
    (3, 2, 2, 'Amoxicilina', '1g - Antibiotico',         1200.00, 700.00,  20,   5, 1, 1),
    (4, 1, 4, 'Loratadina',  '10mg - Antialergico',       450.00, 200.00,   3,  10, 0, 1),
    (5, 1, 1, 'Aspirina',    '100mg - Preventivo',        300.00, 150.00, 100,  20, 0, 1);
    SET IDENTITY_INSERT Medicamento OFF;
END;
GO

-- ---------------------------------------------
-- STORED PROCEDURES - CATEGORIA
-- ---------------------------------------------

CREATE OR ALTER PROCEDURE SP_RecuperarCategorias
AS
BEGIN
    SET NOCOUNT ON;
    SELECT IdCategoria, Nombre, Descripcion
    FROM Categoria
    ORDER BY Nombre;
END;
GO

-- ---------------------------------------------
-- STORED PROCEDURES - LABORATORIO
-- ---------------------------------------------

CREATE OR ALTER PROCEDURE SP_RecuperarLaboratorios
AS
BEGIN
    SET NOCOUNT ON;
    SELECT IdLaboratorio, RazonSocial, Cuit, Telefono, Email, Direccion, Activo
    FROM Laboratorio
    ORDER BY RazonSocial;
END;
GO

CREATE OR ALTER PROCEDURE SP_RecuperarLaboratoriosActivos
AS
BEGIN
    SET NOCOUNT ON;
    SELECT IdLaboratorio, RazonSocial, Cuit, Telefono, Email, Direccion, Activo
    FROM Laboratorio
    WHERE Activo = 1
    ORDER BY RazonSocial;
END;
GO

CREATE OR ALTER PROCEDURE SP_AgregarLaboratorio
    @RazonSocial VARCHAR(200),
    @Cuit        VARCHAR(15),
    @Telefono    VARCHAR(20),
    @Email       VARCHAR(100),
    @Direccion   VARCHAR(200)
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO Laboratorio (RazonSocial, Cuit, Telefono, Email, Direccion, Activo)
    VALUES (@RazonSocial, @Cuit, @Telefono, @Email, @Direccion, 1);

    -- retorna el id generado
    SELECT SCOPE_IDENTITY() AS IdLaboratorio;
END;
GO

CREATE OR ALTER PROCEDURE SP_ModificarLaboratorio
    @IdLaboratorio INT,
    @RazonSocial   VARCHAR(200),
    @Cuit          VARCHAR(15),
    @Telefono      VARCHAR(20),
    @Email         VARCHAR(100),
    @Direccion     VARCHAR(200)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Laboratorio
    SET RazonSocial = @RazonSocial,
        Cuit        = @Cuit,
        Telefono    = @Telefono,
        Email       = @Email,
        Direccion   = @Direccion
    WHERE IdLaboratorio = @IdLaboratorio;
END;
GO

CREATE OR ALTER PROCEDURE SP_DarDeBajaLaboratorio
    @IdLaboratorio INT
AS
BEGIN
    SET NOCOUNT ON;

    -- no se puede dar de baja si tiene medicamentos activos asociados
    IF EXISTS (SELECT 1 FROM Medicamento WHERE IdLaboratorio = @IdLaboratorio AND Activo = 1)
    BEGIN
        SELECT 0 AS Resultado, 'El laboratorio tiene medicamentos activos asociados' AS Mensaje;
        RETURN;
    END;

    UPDATE Laboratorio SET Activo = 0 WHERE IdLaboratorio = @IdLaboratorio;
    SELECT 1 AS Resultado, 'Laboratorio dado de baja correctamente' AS Mensaje;
END;
GO

-- ---------------------------------------------
-- STORED PROCEDURES - MEDICAMENTO
-- ---------------------------------------------

CREATE OR ALTER PROCEDURE SP_RecuperarMedicamentos
AS
BEGIN
    SET NOCOUNT ON;
    SELECT m.IdMedicamento, m.IdLaboratorio, m.IdCategoria, m.Nombre, m.Descripcion,
           m.PrecioVenta, m.PrecioCompra, m.StockActual, m.StockMinimo,
           m.RequiereReceta, m.Activo,
           l.RazonSocial AS NombreLaboratorio,
           c.Nombre AS NombreCategoria
    FROM Medicamento m
    INNER JOIN Laboratorio l ON m.IdLaboratorio = l.IdLaboratorio
    INNER JOIN Categoria   c ON m.IdCategoria   = c.IdCategoria
    ORDER BY m.Nombre;
END;
GO

CREATE OR ALTER PROCEDURE SP_RecuperarMedicamentosCriticos
AS
BEGIN
    SET NOCOUNT ON;
    SELECT m.IdMedicamento, m.IdLaboratorio, m.IdCategoria, m.Nombre, m.Descripcion,
           m.PrecioVenta, m.PrecioCompra, m.StockActual, m.StockMinimo,
           m.RequiereReceta, m.Activo,
           l.RazonSocial AS NombreLaboratorio,
           c.Nombre AS NombreCategoria
    FROM Medicamento m
    INNER JOIN Laboratorio l ON m.IdLaboratorio = l.IdLaboratorio
    INNER JOIN Categoria   c ON m.IdCategoria   = c.IdCategoria
    -- stock critico: actual <= minimo
    WHERE m.StockActual <= m.StockMinimo AND m.Activo = 1
    ORDER BY m.Nombre;
END;
GO

CREATE OR ALTER PROCEDURE SP_AgregarMedicamento
    @IdLaboratorio  INT,
    @IdCategoria    INT,
    @Nombre         VARCHAR(150),
    @Descripcion    VARCHAR(300),
    @PrecioVenta    DECIMAL(10,2),
    @PrecioCompra   DECIMAL(10,2),
    @StockActual    INT,
    @StockMinimo    INT,
    @RequiereReceta BIT
AS
BEGIN
    SET NOCOUNT ON;

    -- validar nombre duplicado
    IF EXISTS (SELECT 1 FROM Medicamento WHERE Nombre = @Nombre AND Activo = 1)
    BEGIN
        SELECT 0 AS Resultado, 'Ya existe un medicamento activo con ese nombre' AS Mensaje;
        RETURN;
    END;

    INSERT INTO Medicamento (IdLaboratorio, IdCategoria, Nombre, Descripcion, PrecioVenta, PrecioCompra, StockActual, StockMinimo, RequiereReceta, Activo)
    VALUES (@IdLaboratorio, @IdCategoria, @Nombre, @Descripcion, @PrecioVenta, @PrecioCompra, @StockActual, @StockMinimo, @RequiereReceta, 1);

    SELECT 1 AS Resultado, CAST(SCOPE_IDENTITY() AS VARCHAR) AS Mensaje;
END;
GO

CREATE OR ALTER PROCEDURE SP_ModificarMedicamento
    @IdMedicamento  INT,
    @IdLaboratorio  INT,
    @IdCategoria    INT,
    @Nombre         VARCHAR(150),
    @Descripcion    VARCHAR(300),
    @PrecioVenta    DECIMAL(10,2),
    @StockMinimo    INT,
    @RequiereReceta BIT
AS
BEGIN
    SET NOCOUNT ON;

    -- validar nombre duplicado en otro medicamento
    IF EXISTS (SELECT 1 FROM Medicamento WHERE Nombre = @Nombre AND IdMedicamento <> @IdMedicamento AND Activo = 1)
    BEGIN
        SELECT 0 AS Resultado, 'Ya existe otro medicamento activo con ese nombre' AS Mensaje;
        RETURN;
    END;

    -- precio de compra es historico: no se modifica, queda el valor original del alta
    UPDATE Medicamento
    SET IdLaboratorio  = @IdLaboratorio,
        IdCategoria    = @IdCategoria,
        Nombre         = @Nombre,
        Descripcion    = @Descripcion,
        PrecioVenta    = @PrecioVenta,
        StockMinimo    = @StockMinimo,
        RequiereReceta = @RequiereReceta
    WHERE IdMedicamento = @IdMedicamento;

    SELECT 1 AS Resultado, 'Medicamento modificado correctamente' AS Mensaje;
END;
GO

CREATE OR ALTER PROCEDURE SP_DarDeBajaMedicamento
    @IdMedicamento INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Medicamento SET Activo = 0 WHERE IdMedicamento = @IdMedicamento;
    SELECT 1 AS Resultado, 'Medicamento dado de baja correctamente' AS Mensaje;
END;
GO

-- ---------------------------------------------
-- STORED PROCEDURES - COMPRA
-- ---------------------------------------------

CREATE OR ALTER PROCEDURE SP_RegistrarCompra
    @IdLaboratorio INT,
    @IdUsuario     INT,
    @Total         DECIMAL(10,2)
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO Compra (Fecha, IdLaboratorio, IdUsuario, Total)
    VALUES (GETDATE(), @IdLaboratorio, @IdUsuario, @Total);

    SELECT SCOPE_IDENTITY() AS IdCompra;
END;
GO

CREATE OR ALTER PROCEDURE SP_AgregarDetalleCompra
    @IdCompra       INT,
    @IdMedicamento  INT,
    @Cantidad       INT,
    @PrecioUnitario DECIMAL(10,2)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO DetalleCompra (IdCompra, IdMedicamento, Cantidad, PrecioUnitario)
    VALUES (@IdCompra, @IdMedicamento, @Cantidad, @PrecioUnitario);

    -- precio promedio ponderado: (stock_actual * precio_actual + cantidad_nueva * precio_nuevo) / stock_total
    -- evita division por cero si el stock actual es 0
    DECLARE @StockActual    INT;
    DECLARE @PrecioActual   DECIMAL(10,2);
    DECLARE @NuevoPrecio    DECIMAL(10,2);
    DECLARE @NuevoStock     INT;

    SELECT @StockActual  = StockActual,
           @PrecioActual = PrecioCompra
    FROM Medicamento
    WHERE IdMedicamento = @IdMedicamento;

    SET @NuevoStock = @StockActual + @Cantidad;

    IF @StockActual = 0
        SET @NuevoPrecio = @PrecioUnitario;
    ELSE
        SET @NuevoPrecio = ROUND(
            ((@StockActual * @PrecioActual) + (@Cantidad * @PrecioUnitario)) / @NuevoStock,
            2
        );

    UPDATE Medicamento
    SET StockActual  = @NuevoStock,
        PrecioCompra = @NuevoPrecio
    WHERE IdMedicamento = @IdMedicamento;
END;
GO
