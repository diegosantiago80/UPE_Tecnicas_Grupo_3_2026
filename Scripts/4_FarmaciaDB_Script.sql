USE FarmaciaDB;
GO

CREATE OR ALTER PROCEDURE dbo.sp_ReporteVentas
    @NombreUsuario VARCHAR(45) = NULL, -- null o vacio = todos los vendedores
    @Periodo VARCHAR(20) = NULL,       -- 'Mensual', 'Semanal', 'Diaria'
    @Fecha DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;
    IF @Fecha IS NULL SET @Fecha = CAST(GETDATE() AS DATE);

    -- string vacio se trata igual que null (todos los vendedores)
    IF @NombreUsuario = '' SET @NombreUsuario = NULL;

    SELECT
        v.IdVenta,
        u.NombreUsuario AS NombreVendedor,
        p.Nombre + ' ' + p.Apellido AS NombreCompletoVendedor,
        CAST(v.Fecha AS DATE) AS FechaVenta,
        CAST(v.Fecha AS TIME) AS Hora,
        m.Nombre AS Medicamento,
        (dv.Cantidad * dv.PrecioUnitario) AS Monto
    FROM dbo.Venta v
    INNER JOIN dbo.Usuario u ON v.IdUsuarioVendedor = u.IdUsuario
    INNER JOIN dbo.Persona p ON u.IdPersona = p.idPersona
    INNER JOIN dbo.DetalleVenta dv ON v.IdVenta = dv.IdVenta
    INNER JOIN dbo.Medicamento m ON dv.IdMedicamento = m.IdMedicamento
    WHERE (@NombreUsuario IS NULL OR u.NombreUsuario = @NombreUsuario)
      AND (
          @Periodo IS NULL
          OR (@Periodo = 'Diaria'   AND CAST(v.Fecha AS DATE) = @Fecha)
          OR (@Periodo = 'Semanal'  AND CAST(v.Fecha AS DATE) BETWEEN DATEADD(day, -7, @Fecha) AND @Fecha)
          OR (@Periodo = 'Mensual'  AND YEAR(v.Fecha) = YEAR(@Fecha) AND MONTH(v.Fecha) = MONTH(@Fecha))
      )
    ORDER BY v.Fecha DESC;
END;
GO

-- SP que devuelve estadisticas de TODOS los medicamentos activos de una sola vez
-- calcula unidades vendidas en el mes actual vs mes anterior y trae el stock real
CREATE OR ALTER PROCEDURE dbo.sp_EstadisticasTodosMedicamentos
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @InicioMesActual   DATE = DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1);
    DECLARE @FinMesActual      DATE = CAST(GETDATE() AS DATE);
    DECLARE @InicioMesAnterior DATE = DATEADD(month, -1, @InicioMesActual);
    DECLARE @FinMesAnterior    DATE = DATEADD(day, -1, @InicioMesActual);

    SELECT
        m.Nombre AS NombreMedicamento,
        -- unidades vendidas mes actual
        ISNULL(SUM(CASE
            WHEN CAST(v.Fecha AS DATE) BETWEEN @InicioMesActual AND @FinMesActual
            THEN dv.Cantidad ELSE 0 END), 0) AS CantidadUnidadesVendidas,
        -- unidades vendidas mes anterior
        ISNULL(SUM(CASE
            WHEN CAST(v.Fecha AS DATE) BETWEEN @InicioMesAnterior AND @FinMesAnterior
            THEN dv.Cantidad ELSE 0 END), 0) AS UnidadesMesAnterior,
        m.StockActual AS UnidadesEnStock
    FROM dbo.Medicamento m
    LEFT JOIN dbo.DetalleVenta dv ON dv.IdMedicamento = m.IdMedicamento
    LEFT JOIN dbo.Venta v         ON dv.IdVenta = v.IdVenta AND v.Activo = 1
    WHERE m.Activo = 1
    GROUP BY m.IdMedicamento, m.Nombre, m.StockActual
    ORDER BY m.Nombre;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_EstadisticasMedicamento
    @NombreMedicamento VARCHAR(45),
    @FechaInicio       DATE,
    @FechaFin          DATE
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @FechaInicioAnterior DATE = DATEADD(month, -1, @FechaInicio);
    DECLARE @FechaFinAnterior    DATE = DATEADD(month, -1, @FechaFin);
    DECLARE @UnidadesActuales    INT          = 0;
    DECLARE @UnidadesAnteriores  INT          = 0;
    DECLARE @PorcentajeVariacion DECIMAL(10,2) = 0.00;

    -- unidades vendidas en el periodo actual
    SELECT @UnidadesActuales = ISNULL(SUM(dv.Cantidad), 0)
    FROM dbo.DetalleVenta dv
    INNER JOIN dbo.Venta      v ON dv.IdVenta       = v.IdVenta
    INNER JOIN dbo.Medicamento m ON dv.IdMedicamento = m.IdMedicamento
    WHERE m.Nombre = @NombreMedicamento
      AND CAST(v.Fecha AS DATE) BETWEEN @FechaInicio AND @FechaFin;

    -- unidades vendidas en el mismo periodo del mes anterior
    SELECT @UnidadesAnteriores = ISNULL(SUM(dv.Cantidad), 0)
    FROM dbo.DetalleVenta dv
    INNER JOIN dbo.Venta      v ON dv.IdVenta       = v.IdVenta
    INNER JOIN dbo.Medicamento m ON dv.IdMedicamento = m.IdMedicamento
    WHERE m.Nombre = @NombreMedicamento
      AND CAST(v.Fecha AS DATE) BETWEEN @FechaInicioAnterior AND @FechaFinAnterior;

    -- porcentaje de variacion (evita division por cero)
    IF @UnidadesAnteriores > 0
    BEGIN
        SET @PorcentajeVariacion = ((CAST(@UnidadesActuales AS DECIMAL(10,2)) - @UnidadesAnteriores) / @UnidadesAnteriores) * 100.00;
    END
    ELSE IF @UnidadesActuales > 0
    BEGIN
        SET @PorcentajeVariacion = 100.00;
    END

    SELECT
        m.Nombre       AS NombreMedicamento,
        @UnidadesActuales    AS CantidadUnidadesVendidas,
        @PorcentajeVariacion AS PorcentajeVsMesAnterior,
        m.StockActual        AS UnidadesEnStock
    FROM dbo.Medicamento m
    WHERE m.Nombre = @NombreMedicamento;
END;
