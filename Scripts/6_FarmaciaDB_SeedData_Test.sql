-- =============================================================
-- Script de datos de prueba - Farmacia UPE 2026
-- Inserta clientes, ventas y detalles para testear modulos de
-- ventas y reportes del gerente.
-- Requisito previo: haber ejecutado FarmaciaDB.sql,
-- FarmaciaDB_Script_B_Victoria.sql y FarmaciaDB_Script_C.sql
-- =============================================================

USE FarmaciaDB;
GO

-- ---------------------------------------------
-- CLIENTES (10)
-- Cada cliente requiere primero una Persona
-- ---------------------------------------------

IF NOT EXISTS (SELECT 1 FROM Persona WHERE DNI = '30111001')
BEGIN
    INSERT INTO Persona (DNI, Nombre, Apellido, Telefono, Email) VALUES
    ('30111001', 'Lucas',     'Fernandez',  '1122334401', 'lucas.fernandez@mail.com'),
    ('30111002', 'Maria',     'Gonzalez',   '1122334402', 'maria.gonzalez@mail.com'),
    ('30111003', 'Juan',      'Perez',      '1122334403', 'juan.perez@mail.com'),
    ('30111004', 'Sofia',     'Ramirez',    '1122334404', 'sofia.ramirez@mail.com'),
    ('30111005', 'Carlos',    'Torres',     '1122334405', 'carlos.torres@mail.com'),
    ('30111006', 'Valentina', 'Sosa',       '1122334406', 'valentina.sosa@mail.com'),
    ('30111007', 'Matias',    'Lopez',      '1122334407', 'matias.lopez@mail.com'),
    ('30111008', 'Camila',    'Diaz',       '1122334408', 'camila.diaz@mail.com'),
    ('30111009', 'Rodrigo',   'Martinez',   '1122334409', 'rodrigo.martinez@mail.com'),
    ('30111010', 'Florencia', 'Sanchez',    '1122334410', 'florencia.sanchez@mail.com');
END;

IF NOT EXISTS (SELECT 1 FROM Cliente WHERE IdPersona = (SELECT IdPersona FROM Persona WHERE DNI = '30111001'))
BEGIN
    INSERT INTO Cliente (ObraSocial, Activo, EsEmpleado, IdPersona) VALUES
    ('OSDE',         1, 0, (SELECT IdPersona FROM Persona WHERE DNI = '30111001')),
    ('PAMI',         1, 0, (SELECT IdPersona FROM Persona WHERE DNI = '30111002')),
    ('Swiss Medical', 1, 0, (SELECT IdPersona FROM Persona WHERE DNI = '30111003')),
    ('Particular',   1, 0, (SELECT IdPersona FROM Persona WHERE DNI = '30111004')),
    ('IOMA',         1, 0, (SELECT IdPersona FROM Persona WHERE DNI = '30111005')),
    ('OSDE',         1, 0, (SELECT IdPersona FROM Persona WHERE DNI = '30111006')),
    ('Particular',   1, 0, (SELECT IdPersona FROM Persona WHERE DNI = '30111007')),
    ('PAMI',         1, 0, (SELECT IdPersona FROM Persona WHERE DNI = '30111008')),
    ('IOMA',         1, 0, (SELECT IdPersona FROM Persona WHERE DNI = '30111009')),
    ('Swiss Medical', 1, 0, (SELECT IdPersona FROM Persona WHERE DNI = '30111010'));
END;
GO

-- ---------------------------------------------
-- VENTAS Y DETALLE (10 ventas distribuidas en
-- distintas fechas para que los reportes por
-- periodo devuelvan resultados variados)
-- vendedor = usuario con NombreUsuario 'vendedor'
-- ---------------------------------------------

DECLARE @IdVendedor INT = (SELECT IdUsuario FROM Usuario WHERE NombreUsuario = 'vendedor');

-- cliente ids por DNI (mas legible que hardcodear)
DECLARE @C1  INT = (SELECT c.IdCliente FROM Cliente c INNER JOIN Persona p ON c.IdPersona = p.IdPersona WHERE p.DNI = '30111001');
DECLARE @C2  INT = (SELECT c.IdCliente FROM Cliente c INNER JOIN Persona p ON c.IdPersona = p.IdPersona WHERE p.DNI = '30111002');
DECLARE @C3  INT = (SELECT c.IdCliente FROM Cliente c INNER JOIN Persona p ON c.IdPersona = p.IdPersona WHERE p.DNI = '30111003');
DECLARE @C4  INT = (SELECT c.IdCliente FROM Cliente c INNER JOIN Persona p ON c.IdPersona = p.IdPersona WHERE p.DNI = '30111004');
DECLARE @C5  INT = (SELECT c.IdCliente FROM Cliente c INNER JOIN Persona p ON c.IdPersona = p.IdPersona WHERE p.DNI = '30111005');
DECLARE @C6  INT = (SELECT c.IdCliente FROM Cliente c INNER JOIN Persona p ON c.IdPersona = p.IdPersona WHERE p.DNI = '30111006');
DECLARE @C7  INT = (SELECT c.IdCliente FROM Cliente c INNER JOIN Persona p ON c.IdPersona = p.IdPersona WHERE p.DNI = '30111007');
DECLARE @C8  INT = (SELECT c.IdCliente FROM Cliente c INNER JOIN Persona p ON c.IdPersona = p.IdPersona WHERE p.DNI = '30111008');
DECLARE @C9  INT = (SELECT c.IdCliente FROM Cliente c INNER JOIN Persona p ON c.IdPersona = p.IdPersona WHERE p.DNI = '30111009');
DECLARE @C10 INT = (SELECT c.IdCliente FROM Cliente c INNER JOIN Persona p ON c.IdPersona = p.IdPersona WHERE p.DNI = '30111010');

-- solo insertar si no hay ventas de prueba ya cargadas
IF NOT EXISTS (SELECT 1 FROM Venta WHERE IdCliente = @C1)
BEGIN

    -- Venta 1 - hace 2 dias (aparece en reporte Diario si se consulta esa fecha)
    INSERT INTO Venta (Fecha, IdCliente, IdUsuarioVendedor, Total, Activo)
    VALUES (DATEADD(day, -2, GETDATE()), @C1, @IdVendedor, 1300.00, 1);
    DECLARE @V1 INT = SCOPE_IDENTITY();
    INSERT INTO DetalleVenta (IdVenta, IdMedicamento, Cantidad, PrecioUnitario) VALUES
    (@V1, 1, 2, 500.00),  -- 2x Paracetamol
    (@V1, 5, 1, 300.00);  -- 1x Aspirina

    -- Venta 2 - hace 3 dias
    INSERT INTO Venta (Fecha, IdCliente, IdUsuarioVendedor, Total, Activo)
    VALUES (DATEADD(day, -3, GETDATE()), @C2, @IdVendedor, 800.00, 1);
    DECLARE @V2 INT = SCOPE_IDENTITY();
    INSERT INTO DetalleVenta (IdVenta, IdMedicamento, Cantidad, PrecioUnitario) VALUES
    (@V2, 2, 1, 800.00);  -- 1x Ibuprofeno

    -- Venta 3 - hace 4 dias
    INSERT INTO Venta (Fecha, IdCliente, IdUsuarioVendedor, Total, Activo)
    VALUES (DATEADD(day, -4, GETDATE()), @C3, @IdVendedor, 2400.00, 1);
    DECLARE @V3 INT = SCOPE_IDENTITY();
    INSERT INTO DetalleVenta (IdVenta, IdMedicamento, Cantidad, PrecioUnitario) VALUES
    (@V3, 3, 2, 1200.00); -- 2x Amoxicilina

    -- Venta 4 - hace 5 dias (dentro del rango semanal)
    INSERT INTO Venta (Fecha, IdCliente, IdUsuarioVendedor, Total, Activo)
    VALUES (DATEADD(day, -5, GETDATE()), @C4, @IdVendedor, 950.00, 1);
    DECLARE @V4 INT = SCOPE_IDENTITY();
    INSERT INTO DetalleVenta (IdVenta, IdMedicamento, Cantidad, PrecioUnitario) VALUES
    (@V4, 1, 1, 500.00),
    (@V4, 5, 1, 300.00),
    (@V4, 4, 1, 150.00); -- 1x Loratadina con descuento hipotetico

    -- Venta 5 - hace 6 dias (ultimo dia del rango semanal)
    INSERT INTO Venta (Fecha, IdCliente, IdUsuarioVendedor, Total, Activo)
    VALUES (DATEADD(day, -6, GETDATE()), @C5, @IdVendedor, 1600.00, 1);
    DECLARE @V5 INT = SCOPE_IDENTITY();
    INSERT INTO DetalleVenta (IdVenta, IdMedicamento, Cantidad, PrecioUnitario) VALUES
    (@V5, 2, 2, 800.00); -- 2x Ibuprofeno

    -- Venta 6 - hace 10 dias (fuera del semanal, dentro del mensual)
    INSERT INTO Venta (Fecha, IdCliente, IdUsuarioVendedor, Total, Activo)
    VALUES (DATEADD(day, -10, GETDATE()), @C6, @IdVendedor, 450.00, 1);
    DECLARE @V6 INT = SCOPE_IDENTITY();
    INSERT INTO DetalleVenta (IdVenta, IdMedicamento, Cantidad, PrecioUnitario) VALUES
    (@V6, 4, 1, 450.00); -- 1x Loratadina

    -- Venta 7 - hace 15 dias (dentro del mensual)
    INSERT INTO Venta (Fecha, IdCliente, IdUsuarioVendedor, Total, Activo)
    VALUES (DATEADD(day, -15, GETDATE()), @C7, @IdVendedor, 3000.00, 1);
    DECLARE @V7 INT = SCOPE_IDENTITY();
    INSERT INTO DetalleVenta (IdVenta, IdMedicamento, Cantidad, PrecioUnitario) VALUES
    (@V7, 3, 1, 1200.00),
    (@V7, 1, 2, 500.00),
    (@V7, 5, 2, 300.00); -- venta mixta

    -- Venta 8 - hace 20 dias (dentro del mensual si el mes tiene 30 dias)
    INSERT INTO Venta (Fecha, IdCliente, IdUsuarioVendedor, Total, Activo)
    VALUES (DATEADD(day, -20, GETDATE()), @C8, @IdVendedor, 900.00, 1);
    DECLARE @V8 INT = SCOPE_IDENTITY();
    INSERT INTO DetalleVenta (IdVenta, IdMedicamento, Cantidad, PrecioUnitario) VALUES
    (@V8, 5, 3, 300.00); -- 3x Aspirina

    -- Venta 9 - hace 25 dias
    INSERT INTO Venta (Fecha, IdCliente, IdUsuarioVendedor, Total, Activo)
    VALUES (DATEADD(day, -25, GETDATE()), @C9, @IdVendedor, 2000.00, 1);
    DECLARE @V9 INT = SCOPE_IDENTITY();
    INSERT INTO DetalleVenta (IdVenta, IdMedicamento, Cantidad, PrecioUnitario) VALUES
    (@V9, 2, 1, 800.00),
    (@V9, 3, 1, 1200.00);

    -- Venta 10 - hace 35 dias (mes anterior, util para estadisticas de variacion)
    INSERT INTO Venta (Fecha, IdCliente, IdUsuarioVendedor, Total, Activo)
    VALUES (DATEADD(day, -35, GETDATE()), @C10, @IdVendedor, 1500.00, 1);
    DECLARE @V10 INT = SCOPE_IDENTITY();
    INSERT INTO DetalleVenta (IdVenta, IdMedicamento, Cantidad, PrecioUnitario) VALUES
    (@V10, 1, 3, 500.00); -- 3x Paracetamol el mes anterior

END;
GO

-- ---------------------------------------------
-- VERIFICACION RAPIDA
-- Ejecutar estos SELECT para confirmar que
-- los datos quedaron cargados correctamente
-- ---------------------------------------------

SELECT 'Clientes' AS Tabla, COUNT(*) AS Total FROM Cliente
UNION ALL
SELECT 'Ventas',            COUNT(*)            FROM Venta
UNION ALL
SELECT 'DetalleVenta',      COUNT(*)            FROM DetalleVenta;

-- vista previa de ventas con nombre del vendedor y cliente
SELECT
    v.IdVenta,
    CAST(v.Fecha AS DATE) AS Fecha,
    u.NombreUsuario AS Vendedor,
    p.Nombre + ' ' + p.Apellido AS Cliente,
    v.Total
FROM Venta v
INNER JOIN Usuario u ON v.IdUsuarioVendedor = u.IdUsuario
INNER JOIN Cliente c ON v.IdCliente = c.IdCliente
INNER JOIN Persona p ON c.IdPersona = p.IdPersona
ORDER BY v.Fecha DESC;
GO
