USE [GamoraIndumentariaDB]
GO

-- Ventas de prueba para validar Estadísticas de Ventas
-- IMPORTANTE: Asegurate de que existan productos en la tabla Inventario
-- y ajusta los IDs de ProductoId si es necesario.

-- Limpia solo si quieres empezar desde cero en pruebas (opcional)
-- DELETE FROM DetalleVentas;
-- DELETE FROM Ventas;

SET NOCOUNT ON;

DECLARE @i INT = 1;
DECLARE @fechaBase DATE = DATEADD(DAY, -30, CAST(GETDATE() AS DATE));
DECLARE @ventaId INT;
DECLARE @monto DECIMAL(10,2);
DECLARE @metodo NVARCHAR(50);
DECLARE @cantidadProductos INT;

-- Obtener lista de productos existentes para respetar la FK con Inventario
DECLARE @Productos TABLE (
    RowNum INT IDENTITY(1,1) PRIMARY KEY,
    Id INT NOT NULL
);

INSERT INTO @Productos (Id)
SELECT Id FROM Inventario ORDER BY Id;

SELECT @cantidadProductos = COUNT(*) FROM @Productos;

IF (@cantidadProductos = 0)
BEGIN
    RAISERROR('No hay productos en la tabla Inventario. Crea productos antes de insertar ventas de prueba.', 16, 1);
    RETURN;
END;

WHILE @i <= 50
BEGIN
    -- Fecha entre hace 30 días y hoy, distribuida, con hora entre 10:00 y 19:00
    DECLARE @fecha DATETIME = DATEADD(HOUR, (@i % 10) + 10,
                                      DATEADD(DAY, @i % 30, CAST(@fechaBase AS DATETIME)));

    -- Monto total entre 3.000 y 25.000 aprox
    SET @monto = 3000 + (@i * 250) % 22000;

        -- Método de pago alternado (incluye casos mixtos)
        SET @metodo = CASE (@i % 5)
                                        WHEN 0 THEN N'Efectivo'
                                        WHEN 1 THEN N'Tarjeta Débito'
                                        WHEN 2 THEN N'Tarjeta Crédito'
                                        WHEN 3 THEN N'Efectivo + Transferencia'
                                        ELSE N'Efectivo + Tarjeta Débito'
                                    END;

    INSERT INTO Ventas (FechaVenta, Total, MetodoPago)
    VALUES (@fecha, @monto, @metodo);

    SET @ventaId = SCOPE_IDENTITY();

    -- 2 o 3 renglones por venta
    DECLARE @lineas INT = CASE WHEN (@i % 2) = 0 THEN 2 ELSE 3 END;
    DECLARE @l INT = 1;

    WHILE @l <= @lineas
    BEGIN
        DECLARE @productoIndex INT = ((@i + @l - 1) % @cantidadProductos) + 1;
        DECLARE @productoId INT;
        SELECT @productoId = Id FROM @Productos WHERE RowNum = @productoIndex;
        DECLARE @cantidad INT = 1 + ((@i + @l) % 4);    -- 1..4 unidades
        DECLARE @precioUnit DECIMAL(10,2) = 1500 + ((@i * @l) % 3500); -- 1500..4999
        DECLARE @subtotal DECIMAL(10,2) = @cantidad * @precioUnit;

        INSERT INTO DetalleVentas (VentaId, ProductoId, Cantidad, PrecioUnitario, Subtotal, Descuento)
        VALUES (@ventaId, @productoId, @cantidad, @precioUnit, @subtotal, 0);

        SET @l = @l + 1;
    END

    SET @i = @i + 1;
END

SET NOCOUNT OFF;

PRINT 'Se insertaron 50 ventas de prueba (con detalle).';
GO
