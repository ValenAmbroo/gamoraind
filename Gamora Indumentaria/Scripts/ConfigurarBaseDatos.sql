-- Script para ejecutar en la base de datos VentasDB.mdf
-- Este script debe ejecutarse en SQL Server Management Studio o Visual Studio

-- Verificar si las tablas ya existen y eliminarlas si es necesario
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Inventario]') AND type in (N'U'))
    DROP TABLE [dbo].[Inventario]

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TiposTalle]') AND type in (N'U'))
    DROP TABLE [dbo].[TiposTalle]

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Categorias]') AND type in (N'U'))
    DROP TABLE [dbo].[Categorias]

-- Eliminar vistas si existen
IF EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[vw_InventarioCompleto]'))
    DROP VIEW [dbo].[vw_InventarioCompleto]

IF EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[vw_StockBajo]'))
    DROP VIEW [dbo].[vw_StockBajo]

-- Eliminar procedimientos almacenados si existen
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_ObtenerTallesPorCategoria]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_ObtenerTallesPorCategoria]

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_AgregarProducto]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_AgregarProducto]

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_ActualizarStock]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_ActualizarStock]

-- =====================================================
-- Crear las tablas
-- =====================================================

-- Tabla de Categorías
CREATE TABLE [dbo].[Categorias] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [Nombre] NVARCHAR(50) NOT NULL UNIQUE,
    [TieneTalle] BIT NOT NULL DEFAULT 1,
    [TipoTalle] NVARCHAR(20) NULL,
    [FechaCreacion] DATETIME DEFAULT GETDATE()
);

-- Tabla de Tipos de Talle
CREATE TABLE [dbo].[TiposTalle] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [CategoriaId] INT NOT NULL,
    [TalleValor] NVARCHAR(10) NOT NULL,
    [Orden] INT NOT NULL,
    FOREIGN KEY ([CategoriaId]) REFERENCES [dbo].[Categorias]([Id])
);

-- Tabla de Inventario
CREATE TABLE [dbo].[Inventario] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [CategoriaId] INT NOT NULL,
    [Nombre] NVARCHAR(100) NOT NULL,
    [Descripcion] NVARCHAR(255) NULL,
    [CodigoBarra] NVARCHAR(50) NULL,
    [TalleId] INT NULL,
    [Sabor] NVARCHAR(50) NULL,
    [Cantidad] INT NOT NULL DEFAULT 0,
    [PrecioVenta] DECIMAL(10,2) NULL,
    [FechaCreacion] DATETIME DEFAULT GETDATE(),
    [FechaModificacion] DATETIME DEFAULT GETDATE(),
    [Activo] BIT DEFAULT 1,
    FOREIGN KEY ([CategoriaId]) REFERENCES [dbo].[Categorias]([Id]),
    FOREIGN KEY ([TalleId]) REFERENCES [dbo].[TiposTalle]([Id])
);

-- =====================================================
-- Insertar datos iniciales
-- =====================================================

-- Insertar categorías
INSERT INTO [dbo].[Categorias] ([Nombre], [TieneTalle], [TipoTalle]) VALUES
('BUZOS', 1, 'LETRAS'),
('CAMPERAS', 1, 'LETRAS'),
('REMERAS', 1, 'LETRAS'),
('CHALECOS', 1, 'LETRAS'),
('JOGGING', 1, 'LETRAS'),
('JEANS HOMBRE', 1, 'NUMEROS_HOMBRE'),
('JEANS DAMA', 1, 'NUMEROS_DAMA'),
('BOXER', 1, 'LETRAS'),
('ZAPATILLAS', 1, 'NUMEROS_CALZADO'),
('GORRAS', 0, 'NINGUNO'),
('CADENITAS', 0, 'NINGUNO'),
('VAPER', 0, 'NINGUNO'),
('RELOJ', 0, 'NINGUNO'),
('ANTEOJOS', 0, 'NINGUNO');

-- Insertar talles para cada categoría
DECLARE @CategoriaId INT;

-- BUZOS
SELECT @CategoriaId = Id FROM [dbo].[Categorias] WHERE Nombre = 'BUZOS';
INSERT INTO [dbo].[TiposTalle] ([CategoriaId], [TalleValor], [Orden]) VALUES
(@CategoriaId, 'S', 1),
(@CategoriaId, 'M', 2),
(@CategoriaId, 'L', 3),
(@CategoriaId, 'XL', 4),
(@CategoriaId, 'XXL', 5);

-- CAMPERAS
SELECT @CategoriaId = Id FROM [dbo].[Categorias] WHERE Nombre = 'CAMPERAS';
INSERT INTO [dbo].[TiposTalle] ([CategoriaId], [TalleValor], [Orden]) VALUES
(@CategoriaId, 'S', 1),
(@CategoriaId, 'M', 2),
(@CategoriaId, 'L', 3),
(@CategoriaId, 'XL', 4),
(@CategoriaId, 'XXL', 5);

-- REMERAS
SELECT @CategoriaId = Id FROM [dbo].[Categorias] WHERE Nombre = 'REMERAS';
INSERT INTO [dbo].[TiposTalle] ([CategoriaId], [TalleValor], [Orden]) VALUES
(@CategoriaId, 'S', 1),
(@CategoriaId, 'M', 2),
(@CategoriaId, 'L', 3),
(@CategoriaId, 'XL', 4),
(@CategoriaId, 'XXL', 5);

-- CHALECOS
SELECT @CategoriaId = Id FROM [dbo].[Categorias] WHERE Nombre = 'CHALECOS';
INSERT INTO [dbo].[TiposTalle] ([CategoriaId], [TalleValor], [Orden]) VALUES
(@CategoriaId, 'S', 1),
(@CategoriaId, 'M', 2),
(@CategoriaId, 'L', 3),
(@CategoriaId, 'XL', 4),
(@CategoriaId, 'XXL', 5);

-- JOGGING
SELECT @CategoriaId = Id FROM [dbo].[Categorias] WHERE Nombre = 'JOGGING';
INSERT INTO [dbo].[TiposTalle] ([CategoriaId], [TalleValor], [Orden]) VALUES
(@CategoriaId, 'S', 1),
(@CategoriaId, 'M', 2),
(@CategoriaId, 'L', 3),
(@CategoriaId, 'XL', 4),
(@CategoriaId, 'XXL', 5);

-- BOXER
SELECT @CategoriaId = Id FROM [dbo].[Categorias] WHERE Nombre = 'BOXER';
INSERT INTO [dbo].[TiposTalle] ([CategoriaId], [TalleValor], [Orden]) VALUES
(@CategoriaId, 'S', 1),
(@CategoriaId, 'M', 2),
(@CategoriaId, 'L', 3),
(@CategoriaId, 'XL', 4),
(@CategoriaId, 'XXL', 5);

-- JEANS HOMBRE
SELECT @CategoriaId = Id FROM [dbo].[Categorias] WHERE Nombre = 'JEANS HOMBRE';
INSERT INTO [dbo].[TiposTalle] ([CategoriaId], [TalleValor], [Orden]) VALUES
(@CategoriaId, '36', 1),
(@CategoriaId, '38', 2),
(@CategoriaId, '40', 3),
(@CategoriaId, '42', 4),
(@CategoriaId, '44', 5),
(@CategoriaId, '46', 6),
(@CategoriaId, '48', 7);

-- JEANS DAMA
SELECT @CategoriaId = Id FROM [dbo].[Categorias] WHERE Nombre = 'JEANS DAMA';
INSERT INTO [dbo].[TiposTalle] ([CategoriaId], [TalleValor], [Orden]) VALUES
(@CategoriaId, '34', 1),
(@CategoriaId, '36', 2),
(@CategoriaId, '38', 3),
(@CategoriaId, '40', 4),
(@CategoriaId, '42', 5),
(@CategoriaId, '44', 6),
(@CategoriaId, '46', 7),
(@CategoriaId, '48', 8);

-- ZAPATILLAS
SELECT @CategoriaId = Id FROM [dbo].[Categorias] WHERE Nombre = 'ZAPATILLAS';
INSERT INTO [dbo].[TiposTalle] ([CategoriaId], [TalleValor], [Orden]) VALUES
(@CategoriaId, '34', 1),
(@CategoriaId, '35', 2),
(@CategoriaId, '36', 3),
(@CategoriaId, '37', 4),
(@CategoriaId, '38', 5),
(@CategoriaId, '39', 6),
(@CategoriaId, '40', 7),
(@CategoriaId, '41', 8),
(@CategoriaId, '42', 9),
(@CategoriaId, '43', 10),
(@CategoriaId, '44', 11),
(@CategoriaId, '45', 12);

-- =====================================================
-- Crear índices
-- =====================================================
CREATE INDEX IX_Inventario_CategoriaId ON [dbo].[Inventario]([CategoriaId]);
CREATE INDEX IX_Inventario_TalleId ON [dbo].[Inventario]([TalleId]);
CREATE INDEX IX_Inventario_Activo ON [dbo].[Inventario]([Activo]);
CREATE INDEX IX_TiposTalle_CategoriaId ON [dbo].[TiposTalle]([CategoriaId]);

-- =====================================================
-- Crear vistas
-- =====================================================

-- Vista para mostrar el inventario completo
CREATE VIEW [dbo].[vw_InventarioCompleto] AS
SELECT 
    i.Id,
    c.Nombre AS Categoria,
    i.Nombre AS Producto,
    i.Descripcion,
    i.CodigoBarra,
    CASE 
        WHEN t.TalleValor IS NOT NULL THEN t.TalleValor 
        ELSE 'N/A' 
    END AS Talle,
    i.Sabor,
    i.Cantidad,
    i.PrecioVenta,
    i.FechaCreacion,
    i.FechaModificacion,
    i.Activo
FROM [dbo].[Inventario] i
INNER JOIN [dbo].[Categorias] c ON i.CategoriaId = c.Id
LEFT JOIN [dbo].[TiposTalle] t ON i.TalleId = t.Id
WHERE i.Activo = 1;

-- Vista para mostrar stock bajo
CREATE VIEW [dbo].[vw_StockBajo] AS
SELECT 
    c.Nombre AS Categoria,
    i.Nombre AS Producto,
    CASE 
        WHEN t.TalleValor IS NOT NULL THEN t.TalleValor 
        ELSE 'N/A' 
    END AS Talle,
    i.Cantidad
FROM [dbo].[Inventario] i
INNER JOIN [dbo].[Categorias] c ON i.CategoriaId = c.Id
LEFT JOIN [dbo].[TiposTalle] t ON i.TalleId = t.Id
WHERE i.Activo = 1 AND i.Cantidad <= 5;

-- =====================================================
-- Crear procedimientos almacenados
-- =====================================================

-- Procedimiento para obtener talles por categoría
CREATE PROCEDURE [dbo].[sp_ObtenerTallesPorCategoria]
    @CategoriaId INT
AS
BEGIN
    SELECT 
        t.Id,
        t.TalleValor,
        t.Orden
    FROM [dbo].[TiposTalle] t
    WHERE t.CategoriaId = @CategoriaId
    ORDER BY t.Orden;
END;

-- Procedimiento para agregar un producto
CREATE PROCEDURE [dbo].[sp_AgregarProducto]
    @CategoriaId INT,
    @Nombre NVARCHAR(100),
    @Descripcion NVARCHAR(255) = NULL,
    @CodigoBarra NVARCHAR(50) = NULL,
    @TalleId INT = NULL,
    @Sabor NVARCHAR(50) = NULL,
    @Cantidad INT,
    @PrecioVenta DECIMAL(10,2) = NULL
AS
BEGIN
    INSERT INTO [dbo].[Inventario] (CategoriaId, Nombre, Descripcion, CodigoBarra, TalleId, Sabor, Cantidad, PrecioVenta)
    VALUES (@CategoriaId, @Nombre, @Descripcion, @CodigoBarra, @TalleId, @Sabor, @Cantidad, @PrecioVenta);
    
    SELECT SCOPE_IDENTITY() AS NuevoId;
END;

-- Procedimiento para actualizar stock
CREATE PROCEDURE [dbo].[sp_ActualizarStock]
    @InventarioId INT,
    @CantidadNueva INT
AS
BEGIN
    UPDATE [dbo].[Inventario] 
    SET Cantidad = @CantidadNueva,
        FechaModificacion = GETDATE()
    WHERE Id = @InventarioId;
END;

-- =====================================================
-- Mensaje de finalización
-- =====================================================
PRINT 'Base de datos configurada exitosamente para Gamora Indumentaria.';
PRINT 'Tablas creadas: Categorias, TiposTalle, Inventario';
PRINT 'Vistas creadas: vw_InventarioCompleto, vw_StockBajo';
PRINT 'Procedimientos creados: sp_ObtenerTallesPorCategoria, sp_AgregarProducto, sp_ActualizarStock';
PRINT 'El sistema está listo para usar.';
