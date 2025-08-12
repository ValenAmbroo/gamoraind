-- ====================================================================
-- SCRIPT DE CREACION COMPLETA DE BASE DE DATOS
-- Sistema: Gamora Indumentaria
-- Fecha: 09/08/2025
-- Descripcion: Script completo para crear toda la base de datos desde cero
-- ====================================================================

-- ====================================================================
-- 1. CREAR BASE DE DATOS
-- ====================================================================
USE master;
GO

-- Cerrar conexiones existentes a la base de datos si existe
IF EXISTS (SELECT name FROM sys.databases WHERE name = 'GamoraIndumentariaDB')
BEGIN
    ALTER DATABASE GamoraIndumentariaDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE GamoraIndumentariaDB;
END
GO

-- Crear la base de datos
CREATE DATABASE GamoraIndumentariaDB;
GO

-- Usar la nueva base de datos
USE GamoraIndumentariaDB;
GO

-- ====================================================================
-- 2. CREAR TABLAS
-- ====================================================================

-- Tabla: Categorias
CREATE TABLE Categorias (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(100) NOT NULL,
    Descripcion NVARCHAR(500) NULL,
    Activo BIT DEFAULT 1,
    FechaCreacion DATETIME DEFAULT GETDATE()
);
GO

-- Tabla: Inventario
CREATE TABLE Inventario (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(200) NOT NULL,
    Descripcion NVARCHAR(500) NULL,
    Precio DECIMAL(10,2) NOT NULL,
    Stock INT NOT NULL DEFAULT 0,
    CategoriaId INT NOT NULL,
    CodigoBarras NVARCHAR(50) NULL,
    FechaCreacion DATETIME DEFAULT GETDATE(),
    FechaModificacion DATETIME DEFAULT GETDATE(),
    Activo BIT DEFAULT 1,
    CONSTRAINT FK_Inventario_Categoria FOREIGN KEY (CategoriaId) REFERENCES Categorias(Id)
);
GO

-- Tabla: Ventas
CREATE TABLE Ventas (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    FechaVenta DATETIME NOT NULL DEFAULT GETDATE(),
    Total DECIMAL(10,2) NOT NULL,
    MetodoPago NVARCHAR(50) NULL
);
GO

-- Tabla: DetalleVentas
CREATE TABLE DetalleVentas (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    VentaId INT NOT NULL,
    ProductoId INT NOT NULL,
    Cantidad INT NOT NULL,
    PrecioUnitario DECIMAL(10,2) NOT NULL,
    Subtotal DECIMAL(10,2) NOT NULL,
    Descuento DECIMAL(5,2) DEFAULT 0,
    CONSTRAINT FK_DetalleVentas_Venta FOREIGN KEY (VentaId) REFERENCES Ventas(Id),
    CONSTRAINT FK_DetalleVentas_Producto FOREIGN KEY (ProductoId) REFERENCES Inventario(Id)
);
GO

-- Tabla: Proveedores
CREATE TABLE Proveedores (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(200) NOT NULL,
    ContactoPrincipal NVARCHAR(200) NULL,
    Telefono NVARCHAR(20) NULL,
    Email NVARCHAR(100) NULL,
    Direccion NVARCHAR(300) NULL,
    CUIT NVARCHAR(15) NULL,
    Activo BIT DEFAULT 1,
    FechaCreacion DATETIME DEFAULT GETDATE()
);
GO

-- Tabla: Compras
CREATE TABLE Compras (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ProveedorId INT NOT NULL,
    FechaCompra DATETIME DEFAULT GETDATE(),
    NumeroFactura NVARCHAR(50) NULL,
    Total DECIMAL(10,2) NOT NULL,
    Estado NVARCHAR(20) DEFAULT 'Pendiente',
    Observaciones NVARCHAR(500) NULL,
    CONSTRAINT FK_Compras_Proveedor FOREIGN KEY (ProveedorId) REFERENCES Proveedores(Id)
);
GO

-- Tabla: DetalleCompras
CREATE TABLE DetalleCompras (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    CompraId INT NOT NULL,
    ProductoId INT NOT NULL,
    Cantidad INT NOT NULL,
    PrecioCosto DECIMAL(10,2) NOT NULL,
    Subtotal DECIMAL(10,2) NOT NULL,
    CONSTRAINT FK_DetalleCompras_Compra FOREIGN KEY (CompraId) REFERENCES Compras(Id),
    CONSTRAINT FK_DetalleCompras_Producto FOREIGN KEY (ProductoId) REFERENCES Inventario(Id)
);
GO

-- ====================================================================
-- 3. CREAR INDICES
-- ====================================================================
CREATE NONCLUSTERED INDEX IX_Inventario_CodigoBarras ON Inventario(CodigoBarras);
CREATE NONCLUSTERED INDEX IX_Inventario_Categoria ON Inventario(CategoriaId);
CREATE NONCLUSTERED INDEX IX_Inventario_Nombre ON Inventario(Nombre);
CREATE NONCLUSTERED INDEX IX_Ventas_Fecha ON Ventas(FechaVenta);

CREATE NONCLUSTERED INDEX IX_DetalleVentas_Venta ON DetalleVentas(VentaId);
CREATE NONCLUSTERED INDEX IX_DetalleVentas_Producto ON DetalleVentas(ProductoId);
GO

-- ====================================================================
-- 4. CREAR VISTAS
-- ====================================================================

-- Vista: vw_InventarioCompleto
CREATE VIEW vw_InventarioCompleto AS
SELECT 
    i.Id,
    i.Nombre AS Producto,
    ISNULL(i.Descripcion, '') AS Descripcion,
    i.Precio AS PrecioVenta,
    i.Stock AS Cantidad,
    c.Nombre AS Categoria,
    ISNULL(i.CodigoBarras, '') AS CodigoBarras,
    i.FechaCreacion,
    i.CategoriaId,
    NULL AS TalleId,
    '' AS Talle,
    '' AS Sabor,
    i.FechaModificacion,
    i.Activo
FROM Inventario i
INNER JOIN Categorias c ON i.CategoriaId = c.Id
WHERE i.Activo = 1;
GO

-- Vista: vw_StockBajo
CREATE VIEW vw_StockBajo AS
SELECT 
    i.Id,
    i.Nombre AS Producto,
    c.Nombre AS Categoria,
    i.Stock AS Cantidad,
    i.Precio AS PrecioVenta,
    'Stock Bajo' AS Estado
FROM Inventario i
INNER JOIN Categorias c ON i.CategoriaId = c.Id
WHERE i.Stock <= 10 AND i.Activo = 1;
GO

-- Vista: vw_VentasCompletas
CREATE VIEW vw_VentasCompletas AS
SELECT 
    v.Id AS VentaId,
    v.FechaVenta,
    v.Total,
    v.MetodoPago,
    dv.ProductoId,
    i.Nombre AS Producto,
    dv.Cantidad,
    dv.PrecioUnitario,
    dv.Subtotal
FROM Ventas v

INNER JOIN DetalleVentas dv ON v.Id = dv.VentaId
INNER JOIN Inventario i ON dv.ProductoId = i.Id;
GO

-- ====================================================================
-- 5. INSERTAR DATOS INICIALES
-- ====================================================================

-- Insertar categorias
INSERT INTO Categorias (Nombre, Descripcion) VALUES 
('Remeras', 'Remeras y camisetas de diversos estilos'),
('Pantalones', 'Pantalones y jeans'),
('Vestidos', 'Vestidos para diferentes ocasiones'),
('Accesorios', 'Collares, pulseras y accesorios'),
('Calzado', 'Zapatos, zapatillas y botas');
GO



-- Insertar productos
INSERT INTO Inventario (Nombre, Descripcion, Precio, Stock, CategoriaId, CodigoBarras) VALUES 
('Remera Basica Blanca', 'Remera de algodon 100% color blanco', 2500.00, 50, 1, '7891234567890'),
('Remera Estampada Negra', 'Remera con diseño exclusivo color negro', 3000.00, 40, 1, '7891234567891'),
('Jean Clasico Azul', 'Pantalon jean corte recto color azul', 4500.00, 30, 2, '7891234567892'),
('Pantalon Cargo Beige', 'Pantalon cargo con bolsillos laterales', 5500.00, 20, 2, '7891234567893'),
('Vestido Casual Negro', 'Vestido para uso diario color negro', 3500.00, 20, 3, '7891234567894'),
('Vestido de Noche Rojo', 'Vestido elegante para ocasiones especiales', 8500.00, 10, 3, '7891234567895'),
('Collar Dorado', 'Collar de acero inoxidable con baño de oro', 1500.00, 15, 4, '7891234567896'),
('Pulsera de Plata', 'Pulsera artesanal de plata 925', 2200.00, 12, 4, '7891234567897'),
('Zapatillas Deportivas', 'Zapatillas para running y deporte', 8500.00, 25, 5, '7891234567898'),
('Botas de Cuero', 'Botas de cuero genuino para invierno', 12000.00, 15, 5, '7891234567899');
GO

-- Insertar proveedores
INSERT INTO Proveedores (Nombre, ContactoPrincipal, Telefono, Email, Direccion, CUIT) VALUES 
('TextilSur S.A.', 'Maria Gonzalez', '011-4567-8901', 'ventas@textilsur.com', 'Av. Corrientes 1234, CABA', '20-12345678-9'),
('Moda & Diseño S.R.L.', 'Carlos Rodriguez', '011-2345-6789', 'info@modaydiseño.com', 'Av. Santa Fe 5678, CABA', '20-87654321-0'),
('Accesorios Premium', 'Ana Martinez', '011-8765-4321', 'contacto@accesorios.com', 'Av. Rivadavia 9012, CABA', '20-11111111-1');
GO

-- Insertar ventas de ejemplo
DECLARE @fechaBase DATETIME = GETDATE();

INSERT INTO Ventas (FechaVenta, Total, MetodoPago) VALUES 
(DATEADD(HOUR, -2, @fechaBase), 6500.00, 'Efectivo'),
(DATEADD(HOUR, -4, @fechaBase), 4500.00, 'Tarjeta de Debito'),
(DATEADD(DAY, -1, DATEADD(HOUR, -3, @fechaBase)), 11500.00, 'Tarjeta de Credito'),
(DATEADD(DAY, -1, DATEADD(HOUR, -5, @fechaBase)), 8500.00, 'Transferencia'),
(DATEADD(DAY, -1, DATEADD(HOUR, -7, @fechaBase)), 15500.00, 'Mercado Pago');
GO

-- Insertar detalles de ventas
INSERT INTO DetalleVentas (VentaId, ProductoId, Cantidad, PrecioUnitario, Subtotal) VALUES 
(1, 1, 2, 2500.00, 5000.00), (1, 7, 1, 1500.00, 1500.00),
(2, 3, 1, 4500.00, 4500.00),
(3, 9, 1, 8500.00, 8500.00), (3, 2, 1, 3000.00, 3000.00),
(4, 5, 2, 3500.00, 7000.00), (4, 7, 1, 1500.00, 1500.00),
(5, 10, 1, 12000.00, 12000.00), (5, 2, 1, 3000.00, 3000.00);
GO

-- ====================================================================
-- 6. TIPOS DE DATOS Y PROCEDIMIENTOS ALMACENADOS
-- ====================================================================

-- Tipo de tabla para detalles de venta
CREATE TYPE DetalleVentaType AS TABLE
(
    ProductoId INT,
    Cantidad INT,
    PrecioUnitario DECIMAL(10,2)
);
GO

-- Procedimiento: sp_ProcesarVenta
CREATE PROCEDURE sp_ProcesarVenta
    @Total DECIMAL(10,2),
    @MetodoPago NVARCHAR(50) = NULL,
    @Detalles DetalleVentaType READONLY
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    
    BEGIN TRY
        DECLARE @VentaId INT;
        
        -- Insertar la venta principal
        INSERT INTO Ventas (Total, MetodoPago)
        VALUES (@Total, @MetodoPago);
        
        SET @VentaId = SCOPE_IDENTITY();

        -- Debug: Imprimir el ID de la venta
        PRINT 'VentaId generado: ' + CAST(@VentaId AS VARCHAR(10));
        
        -- Insertar los detalles de la venta
        INSERT INTO DetalleVentas (VentaId, ProductoId, Cantidad, PrecioUnitario, Subtotal)
        SELECT 
            @VentaId,
            ProductoId,
            Cantidad,
            PrecioUnitario,
            (Cantidad * PrecioUnitario) AS Subtotal
        FROM @Detalles;

        -- Actualizar el stock
        UPDATE i
        SET i.Stock = i.Stock - d.Cantidad
        FROM Inventario i
        INNER JOIN @Detalles d ON i.Id = d.ProductoId;
        
        COMMIT TRANSACTION;
        SELECT @VentaId AS VentaId;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        
        -- Capturar detalles del error
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        
        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END;
GO

-- ====================================================================
-- 7. TRIGGERS
-- ====================================================================

-- Trigger: Actualizar FechaModificacion
CREATE TRIGGER tr_Inventario_UpdateFecha
ON Inventario
AFTER UPDATE
AS
BEGIN
    UPDATE Inventario 
    SET FechaModificacion = GETDATE()
    WHERE Id IN (SELECT Id FROM inserted);
END;
GO

-- ====================================================================
-- 8. CONFIGURACION FINAL
-- ====================================================================
ALTER DATABASE GamoraIndumentariaDB SET RECOVERY SIMPLE;
GO

-- ====================================================================
-- VERIFICACIONES Y RESUMEN
-- ====================================================================
PRINT '=================================================================';
PRINT 'BASE DE DATOS GAMORA INDUMENTARIA CREADA EXITOSAMENTE';
PRINT '=================================================================';

-- Mostrar resumen de datos
SELECT 'Categorias' AS Tabla, COUNT(*) AS Registros FROM Categorias
UNION ALL
SELECT 'Inventario', COUNT(*) FROM Inventario
UNION ALL

SELECT 'Ventas', COUNT(*) FROM Ventas
UNION ALL
SELECT 'DetalleVentas', COUNT(*) FROM DetalleVentas
UNION ALL
SELECT 'Proveedores', COUNT(*) FROM Proveedores;

-- Verificar productos con stock
SELECT 
    i.Nombre AS Producto,
    c.Nombre AS Categoria,
    i.Stock,
    i.Precio,
    CASE WHEN i.Stock <= 10 THEN 'STOCK BAJO' ELSE 'OK' END AS Estado
FROM Inventario i
INNER JOIN Categorias c ON i.CategoriaId = c.Id
ORDER BY c.Nombre, i.Nombre;

PRINT '';

PRINT 'Base de datos lista para usar!';
