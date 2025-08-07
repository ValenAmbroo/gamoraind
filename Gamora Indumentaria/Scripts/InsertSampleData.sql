-- Script para insertar algunos productos de ejemplo en el inventario
USE VentasDB;

-- Obtener IDs de categorías
DECLARE @BuzosId INT, @CamperasId INT, @RemerasId INT, @JeansHombreId INT, @GorrasId INT, @VaperId INT;
DECLARE @TalleM INT, @TalleL INT, @TalleXL INT, @Talle38 INT, @Talle40 INT;

SELECT @BuzosId = Id FROM Categorias WHERE Nombre = 'BUZOS';
SELECT @CamperasId = Id FROM Categorias WHERE Nombre = 'CAMPERAS';
SELECT @RemerasId = Id FROM Categorias WHERE Nombre = 'REMERAS';
SELECT @JeansHombreId = Id FROM Categorias WHERE Nombre = 'JEANS HOMBRE';
SELECT @GorrasId = Id FROM Categorias WHERE Nombre = 'GORRAS';
SELECT @VaperId = Id FROM Categorias WHERE Nombre = 'VAPER';

-- Obtener IDs de talles
SELECT @TalleM = Id FROM TiposTalle WHERE CategoriaId = @BuzosId AND TalleValor = 'M';
SELECT @TalleL = Id FROM TiposTalle WHERE CategoriaId = @BuzosId AND TalleValor = 'L';
SELECT @TalleXL = Id FROM TiposTalle WHERE CategoriaId = @BuzosId AND TalleValor = 'XL';
SELECT @Talle38 = Id FROM TiposTalle WHERE CategoriaId = @JeansHombreId AND TalleValor = '38';
SELECT @Talle40 = Id FROM TiposTalle WHERE CategoriaId = @JeansHombreId AND TalleValor = '40';

-- Insertar productos de ejemplo
INSERT INTO Inventario (CategoriaId, Nombre, Descripcion, CodigoBarra, TalleId, Cantidad, PrecioVenta) VALUES
(@BuzosId, 'Buzo Nike Negro', 'Buzo deportivo color negro con capucha', '7890123456001', @TalleM, 15, 25000.00),
(@BuzosId, 'Buzo Adidas Azul', 'Buzo deportivo azul marino', '7890123456002', @TalleL, 12, 28000.00),
(@BuzosId, 'Buzo Puma Gris', 'Buzo casual color gris', '7890123456003', @TalleXL, 8, 23000.00),
(@CamperasId, 'Campera de Cuero', 'Campera de cuero sintético negro', '7890123456004', @TalleL, 5, 45000.00),
(@CamperasId, 'Campera Deportiva', 'Campera deportiva impermeable', '7890123456005', @TalleM, 10, 35000.00),
(@RemerasId, 'Remera Básica Blanca', 'Remera lisa de algodón blanco', '7890123456006', @TalleM, 25, 8000.00),
(@RemerasId, 'Remera Estampada', 'Remera con estampado frontal', '7890123456007', @TalleL, 18, 12000.00),
(@JeansHombreId, 'Jean Clásico Azul', 'Jean de mezclilla azul clásico', '7890123456008', @Talle38, 20, 15000.00),
(@JeansHombreId, 'Jean Negro Slim', 'Jean negro corte slim fit', '7890123456009', @Talle40, 15, 18000.00),
(@GorrasId, 'Gorra New Era', 'Gorra deportiva ajustable', '7890123456010', NULL, 30, 6000.00),
(@GorrasId, 'Gorra Trucker', 'Gorra estilo trucker', '7890123456011', NULL, 25, 5500.00);

-- Insertar productos con sabor (vapers) por separado
INSERT INTO Inventario (CategoriaId, Nombre, Descripcion, CodigoBarra, TalleId, Sabor, Cantidad, PrecioVenta) VALUES
(@VaperId, 'Vaper Elfbar', 'Vaper desechable sabor menta', '7890123456012', NULL, 'Menta', 50, 3000.00),
(@VaperId, 'Vaper Puff', 'Vaper desechable sabor frutas', '7890123456013', NULL, 'Frutas Tropicales', 45, 3200.00);

PRINT 'Productos de ejemplo insertados exitosamente.';

-- Mostrar resumen de productos insertados
SELECT 
    c.Nombre AS Categoria,
    COUNT(*) AS CantidadProductos,
    SUM(i.Cantidad) AS StockTotal
FROM Inventario i
INNER JOIN Categorias c ON i.CategoriaId = c.Id
WHERE i.Activo = 1
GROUP BY c.Nombre
ORDER BY c.Nombre;
