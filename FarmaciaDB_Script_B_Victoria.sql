USE FarmaciaDB;
GO

CREATE TABLE Venta (
    IdVenta INT IDENTITY(1,1) PRIMARY KEY,
    Fecha DATETIME NOT NULL,
    IdCliente INT NOT NULL,              
    IdUsuarioVendedor INT NOT NULL,     
    Total DECIMAL(18,2) NOT NULL,
    Activo BIT DEFAULT 1         
);
GO


CREATE TABLE DetalleVenta (
    IdDetalleVenta INT IDENTITY(1,1) PRIMARY KEY,
    IdVenta INT NOT NULL,  
    IdMedicamento INT NOT NULL,
    Cantidad INT NOT NULL,
    PrecioUnitario DECIMAL(18,2) NOT NULL,
    CONSTRAINT FK_DetalleVenta_Venta FOREIGN KEY (IdVenta) REFERENCES Venta(IdVenta)
);
GO

USE FarmaciaDB;
GO

CREATE TABLE Cliente (
    IdCliente INT IDENTITY(1,1) PRIMARY KEY,
    Nombre VARCHAR(100) NOT NULL,
    Apellido VARCHAR(100) NOT NULL,
    Dni VARCHAR(20) NOT NULL UNIQUE,    
    Telefono VARCHAR(50) NOT NULL,
    Email VARCHAR(150) NOT NULL,
    ObraSocial VARCHAR(100) NOT NULL,
    Activo BIT DEFAULT 1 
);
GO


CREATE PROCEDURE SP_BuscarClientePorDni
    @Dni VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT IdCliente, Nombre, Apellido, Dni, Telefono, Email, ObraSocial, Activo
    FROM Cliente
    WHERE TRIM(Dni) = TRIM(@Dni); 
    
END
GO


CREATE PROCEDURE SP_CrearCliente
    @Nombre VARCHAR(100),
    @Apellido VARCHAR(100),
    @Dni VARCHAR(20),
    @Telefono VARCHAR(50),
    @Email VARCHAR(150),
    @ObraSocial VARCHAR(100),
    @Activo BIT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Cliente (Nombre, Apellido, Dni, Telefono, Email, ObraSocial, Activo)
    VALUES (@Nombre, @Apellido, @Dni, @Telefono, @Email, @ObraSocial, @Activo);

    
    SELECT SCOPE_IDENTITY() AS IdCliente;
END
GO


CREATE PROCEDURE SP_ModificarCliente
    @IdCliente INT,
    @Nombre VARCHAR(100),
    @Apellido VARCHAR(100),
    @Dni VARCHAR(20),
    @Telefono VARCHAR(50),
    @Email VARCHAR(150),
    @ObraSocial VARCHAR(100),
    @Activo BIT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Cliente
    SET Nombre = @Nombre,
        Apellido = @Apellido,
        Dni = @Dni,
        Telefono = @Telefono,
        Email = @Email,
        ObraSocial = @ObraSocial,
        Activo = @Activo 
    WHERE IdCliente = @IdCliente;

    SELECT @@ROWCOUNT AS FilasAfectadas;
END
GO