USE FarmaciaDB;
GO


IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Cliente')
BEGIN
CREATE TABLE Cliente (
    IdCliente INT IDENTITY(1,1) PRIMARY KEY,
    ObraSocial VARCHAR(45) NOT NULL,
    Activo BIT DEFAULT 1,
    EsEmpleado BIT DEFAULT 0, 
    IdPersona INT NOT NULL,
    CONSTRAINT FK_Cliente_Persona FOREIGN KEY (IdPersona) REFERENCES Persona(idPersona)
);
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Venta')
BEGIN
CREATE TABLE Venta (
    IdVenta INT IDENTITY(1,1) PRIMARY KEY,
    Fecha DATETIME NOT NULL,
    IdCliente INT NOT NULL,
    IdUsuarioVendedor INT NOT NULL,
    Total DECIMAL(18,2) NOT NULL,
    Activo BIT DEFAULT 1
);
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'DetalleVenta')
BEGIN
CREATE TABLE DetalleVenta (
    IdDetalleVenta INT IDENTITY(1,1) PRIMARY KEY,
    IdVenta INT NOT NULL,
    IdMedicamento INT NOT NULL,
    Cantidad INT NOT NULL,
    PrecioUnitario DECIMAL(18,2) NOT NULL,
    CONSTRAINT FK_DetalleVenta_Venta FOREIGN KEY (IdVenta) REFERENCES Venta(IdVenta)
);
END
GO

IF NOT EXISTS (SELECT * FROM sys.types WHERE name = 'TipoDetalleVenta' AND is_table_type = 1)
BEGIN
CREATE TYPE TipoDetalleVenta AS TABLE (
    IdMedicamento INT,
    Cantidad INT,
    PrecioUnitario DECIMAL(18,2)
);
END
GO


IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SP_BuscarClientePorDni]') AND type in (N'P', N'PC'))
BEGIN
    EXEC sys.sp_executesql N'
    CREATE PROCEDURE SP_BuscarClientePorDni
        @Dni VARCHAR(20)
    AS
    BEGIN
        SET NOCOUNT ON;
        SELECT 
            c.IdCliente, 
            p.Nombre, 
            p.Apellido, 
            p.DNI AS Dni, 
            p.Telefono, 
            p.Email, 
            c.ObraSocial, 
            c.Activo, 
            c.EsEmpleado
        FROM [dbo].[Cliente] c
        INNER JOIN [dbo].[Persona] p ON c.IdPersona = p.IdPersona
        WHERE TRIM(p.DNI) = TRIM(@Dni);
    END'
END;
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SP_CrearCliente]') AND type in (N'P', N'PC'))
BEGIN
EXEC sys.sp_executesql N'
    CREATE PROCEDURE SP_CrearCliente
        @Nombre VARCHAR(45),
        @Apellido VARCHAR(45),
        @Dni VARCHAR(20),
        @Telefono VARCHAR(30),
        @Email VARCHAR(45),
        @ObraSocial VARCHAR(45),
        @Activo BIT
        AS
    BEGIN
        SET NOCOUNT ON;
        BEGIN TRANSACTION;
        BEGIN TRY
            INSERT INTO Persona (DNI, Nombre, Apellido, Telefono, Email)
            VALUES (@Dni, @Nombre, @Apellido, @Telefono, @Email);

            DECLARE @NuevoIdPersona INT = SCOPE_IDENTITY();

            INSERT INTO Cliente (ObraSocial, Activo, EsEmpleado, IdPersona)
            VALUES (@ObraSocial, @Activo, 0, @NuevoIdPersona);

            DECLARE @NuevoIdCliente INT = SCOPE_IDENTITY();

            COMMIT TRANSACTION;
            SELECT @NuevoIdCliente AS IdCliente;
        END TRY
        BEGIN CATCH
            ROLLBACK TRANSACTION;
            THROW;
        END CATCH
    END'
    END
GO


IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SP_ModificarCliente]') AND type in (N'P', N'PC'))
BEGIN
    EXEC sys.sp_executesql N'
    CREATE PROCEDURE SP_ModificarCliente
        @IdCliente INT,
        @Nombre VARCHAR(45),
        @Apellido VARCHAR(45),
        @Dni VARCHAR(20),
        @Telefono VARCHAR(30),
        @Email VARCHAR(45),
        @ObraSocial VARCHAR(45),
        @Activo BIT,
        @EsEmpleado BIT
    AS
    BEGIN
        SET NOCOUNT ON;
        BEGIN TRANSACTION;
        BEGIN TRY
            DECLARE @IdPersonaAsociada INT;
            SELECT @IdPersonaAsociada = IdPersona FROM Cliente WHERE IdCliente = @IdCliente;

            UPDATE Persona
            SET DNI = @Dni, Nombre = @Nombre, Apellido = @Apellido, Telefono = @Telefono, Email = @Email
            WHERE idPersona = @IdPersonaAsociada;

            UPDATE Cliente
            SET ObraSocial = @ObraSocial, Activo = @Activo, EsEmpleado = @EsEmpleado
            WHERE IdCliente = @IdCliente;

            COMMIT TRANSACTION;
            SELECT 1 AS FilasAfectadas;
        END TRY
        BEGIN CATCH
            ROLLBACK TRANSACTION;
            THROW;
        END CATCH
END'
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SP_RegistrarVenta]') AND type in (N'P', N'PC'))
BEGIN
    EXEC sys.sp_executesql N'
    CREATE PROCEDURE SP_RegistrarVenta
        @IdCliente INT,
        @IdUsuarioVendedor INT,
        @Total DECIMAL(18,2),
        @Detalle TipoDetalleVenta READONLY
    AS
    BEGIN
        SET NOCOUNT ON;
        BEGIN TRANSACTION;
        BEGIN TRY
            INSERT INTO Venta (Fecha, IdCliente, IdUsuarioVendedor, Total, Activo)
            VALUES (GETDATE(), @IdCliente, @IdUsuarioVendedor, @Total, 1);

            DECLARE @NuevoIdVenta INT = SCOPE_IDENTITY();

            INSERT INTO DetalleVenta (IdVenta, IdMedicamento, Cantidad, PrecioUnitario)
            SELECT @NuevoIdVenta, IdMedicamento, Cantidad, PrecioUnitario
            FROM @Detalle;

            UPDATE M_SET
            SET M_SET.StockActual = M_SET.StockActual - D.Cantidad
            FROM Medicamento M_SET
            INNER JOIN @Detalle D ON M_SET.IdMedicamento = D.IdMedicamento;

            IF EXISTS (
                SELECT 1 FROM Medicamento M 
                INNER JOIN @Detalle D ON M.IdMedicamento = D.IdMedicamento
                WHERE M.StockActual < 0
            )
            BEGIN
                RAISERROR(''Stock insuficiente para completar la transacción.'', 16, 1);
            END

            COMMIT TRANSACTION;
            SELECT @NuevoIdVenta AS IdVenta;
        END TRY
        BEGIN CATCH
            ROLLBACK TRANSACTION;
            THROW;
        END CATCH
END'
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SP_ObtenerVentasPorPeriodo]') AND type in (N'P', N'PC'))
BEGIN
    EXEC sys.sp_executesql N'
    CREATE PROCEDURE SP_ObtenerVentasPorPeriodo
        @FechaDesde DATETIME,
        @FechaHasta DATETIME
    AS
    BEGIN
        SELECT IdVenta, Fecha, IdCliente, IdUsuarioVendedor, Total, Activo
        FROM Venta
        WHERE Fecha >= @FechaDesde AND Fecha <= @FechaHasta
        AND Activo = 1; 
    END'
    END
    GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SP_ObtenerVentasPorVendedor]') AND type in (N'P', N'PC'))
BEGIN
    EXEC sys.sp_executesql N'
    CREATE PROCEDURE SP_ObtenerVentasPorVendedor
        @IdUsuarioVendedor INT
    AS
    BEGIN
        SELECT IdVenta, Fecha, IdCliente, IdUsuarioVendedor, Total, Activo
        FROM Venta
        WHERE IdUsuarioVendedor = @IdUsuarioVendedor
        AND Activo = 1;
        END'
    END
    GO