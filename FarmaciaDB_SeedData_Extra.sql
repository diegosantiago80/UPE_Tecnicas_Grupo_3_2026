-- script de datos de prueba adicionales
-- ejecutar sobre FarmaciaDB despues de FarmaciaDB_Script_C.sql
-- agrega 10 laboratorios y 10 medicamentos realistas de farmacia

USE FarmaciaDB;
GO

-- ---------------------------------------------
-- 10 LABORATORIOS ADICIONALES
-- ---------------------------------------------

IF NOT EXISTS (SELECT 1 FROM Laboratorio WHERE Cuit = '30-11111111-1')
BEGIN
    INSERT INTO Laboratorio (RazonSocial, Cuit, Telefono, Email, Direccion, Activo) VALUES
    ('Pfizer Argentina S.A.',        '30-11111111-1', '11-5555-0001', 'ventas@pfizer.com.ar',      'Av. del Libertador 2222, CABA',     1),
    ('Novartis Argentina S.A.',      '30-22222222-2', '11-5555-0002', 'contacto@novartis.com.ar',  'Av. Corrientes 1234, CABA',         1),
    ('Gador S.A.',                   '30-33333333-3', '11-5555-0003', 'info@gador.com.ar',         'Thames 1234, CABA',                 1),
    ('Elea Phoenix S.A.',            '30-44444444-4', '11-5555-0004', 'ventas@elea.com.ar',        'Av. Warnes 2503, CABA',             1),
    ('Laboratorio Pablo Cassara',    '30-55555555-5', '11-5555-0005', 'info@cassara.com.ar',       'Av. San Martin 3000, GBA',          1),
    ('Richmond Laboratorios',        '30-66666666-6', '11-5555-0006', 'comercial@richmond.com.ar', 'Av. Caseros 3039, CABA',            1),
    ('Laboratorio Roche S.A.',       '30-77777777-7', '11-5555-0007', 'roche@roche.com.ar',        'Av. del Libertador 5930, CABA',     1),
    ('Abbvie Argentina S.R.L.',      '30-88888888-8', '11-5555-0008', 'ventas@abbvie.com.ar',      'Olga Cossettini 240, CABA',         1),
    ('GlaxoSmithKline S.A.',         '30-99999999-9', '11-5555-0009', 'gsk@gsk.com.ar',            'Av. Juan B. Justo 1111, CABA',      1),
    ('Sanofi Aventis Argentina',     '30-10101010-1', '11-5555-0010', 'sanofi@sanofi.com.ar',      'Av. Alicia Moreau de Justo 50, CABA', 1);
END;
GO

-- ---------------------------------------------
-- 10 MEDICAMENTOS ADICIONALES
-- ---------------------------------------------

IF NOT EXISTS (SELECT 1 FROM Medicamento WHERE Nombre = 'Omeprazol')
BEGIN
    INSERT INTO Medicamento (IdLaboratorio, IdCategoria, Nombre, Descripcion, PrecioVenta, PrecioCompra, StockActual, StockMinimo, RequiereReceta, Activo)
    SELECT l.IdLaboratorio, c.IdCategoria, m.Nombre, m.Descripcion, m.PrecioVenta, m.PrecioCompra, m.StockActual, m.StockMinimo, m.RequiereReceta, 1
    FROM (VALUES
        ('Gador S.A.',              'Analgesicos',       'Omeprazol',         '20mg - Inhibidor de la bomba de protones',  650.00,  380.00,  80, 15, 0),
        ('Pfizer Argentina S.A.',   'Antibioticos',      'Azitromicina',      '500mg - Antibiotico macrolido',            1500.00,  850.00,  30, 10, 1),
        ('Novartis Argentina S.A.', 'Cardiovasculares',  'Enalapril',         '10mg - Inhibidor de la ECA',               420.00,  220.00,  60, 20, 1),
        ('Elea Phoenix S.A.',       'Analgesicos',       'Diclofenac',        '50mg - Antiinflamatorio no esteroideo',    380.00,  180.00,  45, 10, 0),
        ('Sanofi Aventis Argentina','Cardiovasculares',  'Metformina',        '850mg - Antidiabetico oral',               550.00,  290.00,  40, 15, 1),
        ('Richmond Laboratorios',   'Antialergicos',     'Cetirizina',        '10mg - Antihistaminico de segunda generacion', 350.00, 160.00, 70, 20, 0),
        ('Laboratorio Roche S.A.',  'Antibioticos',      'Claritromicina',    '500mg - Antibiotico macrolido de amplio espectro', 1800.00, 1000.00, 20, 8, 1),
        ('GlaxoSmithKline S.A.',    'Antialergicos',     'Salbutamol',        '100mcg - Broncodilatador beta2 agonista',  750.00,  400.00,  35, 10, 1),
        ('Abbvie Argentina S.R.L.', 'Cardiovasculares',  'Atorvastatina',     '20mg - Reductor del colesterol',           890.00,  480.00,  50, 15, 1),
        ('Laboratorio Pablo Cassara','Analgesicos',      'Naproxeno',         '500mg - Antiinflamatorio y analgesico',    460.00,  240.00,  55, 12, 0)
    ) AS m(NombreLab, NombreCat, Nombre, Descripcion, PrecioVenta, PrecioCompra, StockActual, StockMinimo, RequiereReceta)
    INNER JOIN Laboratorio l ON l.RazonSocial = m.NombreLab AND l.Activo = 1
    INNER JOIN Categoria   c ON c.Nombre      = m.NombreCat;
END;
GO

PRINT 'Seed data extra cargado correctamente.';
GO
