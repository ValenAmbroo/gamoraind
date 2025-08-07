-- Paso 2: Crear tablas, datos, vistas y procedimientos en VentasDB
USE VentasDB;

-- =====================================================
-- Tabla principal de Categorías de Productos
-- =====================================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Categorias' AND xtype='U')
BEGIN
    CREATE TABLE Categorias (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Nombre NVARCHAR(50) NOT NULL UNIQUE,
        TieneTalle BIT NOT NULL DEFAULT 1, -- Indica si la categoría maneja talles
        TipoTalle NVARCHAR(20) NULL, -- 'LETRAS' (S,M,L), 'NUMEROS' (34,36,38), 'ESPECIAL' (36/38/40 para jeans), 'NINGUNO'
        FechaCreacion DATETIME DEFAULT GETDATE()
    );
    PRINT 'Tabla Categorias creada.';
END

-- =====================================================
-- Tabla de Tipos de Talle (para definir los talles disponibles por categoría)
-- =====================================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='TiposTalle' AND xtype='U')
BEGIN
    CREATE TABLE TiposTalle (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        CategoriaId INT NOT NULL,
        TalleValor NVARCHAR(10) NOT NULL, -- S, M, L, XL, XXL, 34, 36, 38, etc.
        Orden INT NOT NULL, -- Para ordenar los talles correctamente
        FOREIGN KEY (CategoriaId) REFERENCES Categorias(Id)
    );
    PRINT 'Tabla TiposTalle creada.';
END

-- =====================================================
-- Tabla principal de Productos/Inventario
-- =====================================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Inventario' AND xtype='U')
BEGIN
    CREATE TABLE Inventario (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        CategoriaId INT NOT NULL,
        Nombre NVARCHAR(100) NOT NULL,
        Descripcion NVARCHAR(255) NULL,
        CodigoBarra NVARCHAR(50) NULL,
        TalleId INT NULL, -- Referencia a TiposTalle, NULL para productos sin talle
        Sabor NVARCHAR(50) NULL, -- Para vapers
        Cantidad INT NOT NULL DEFAULT 0,
        PrecioVenta DECIMAL(10,2) NULL,
        FechaCreacion DATETIME DEFAULT GETDATE(),
        FechaModificacion DATETIME DEFAULT GETDATE(),
        Activo BIT DEFAULT 1,
        FOREIGN KEY (CategoriaId) REFERENCES Categorias(Id),
        FOREIGN KEY (TalleId) REFERENCES TiposTalle(Id)
    );
    PRINT 'Tabla Inventario creada.';
END

-- =====================================================
-- Insertar las categorías de productos (solo si no existen)
-- =====================================================
IF NOT EXISTS (SELECT 1 FROM Categorias)
BEGIN
    INSERT INTO Categorias (Nombre, TieneTalle, TipoTalle) VALUES
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
    
    PRINT 'Categorías insertadas.';
END

-- =====================================================
-- Insertar los talles para cada categoría (solo si no existen)
-- =====================================================
IF NOT EXISTS (SELECT 1 FROM TiposTalle)
BEGIN
    -- Talles para ropa con letras (BUZOS, CAMPERAS, REMERAS, CHALECOS, JOGGING, BOXER)
    DECLARE @CategoriaId INT;

    -- BUZOS
    SELECT @CategoriaId = Id FROM Categorias WHERE Nombre = 'BUZOS';
    INSERT INTO TiposTalle (CategoriaId, TalleValor, Orden) VALUES
    (@CategoriaId, 'S', 1),
    (@CategoriaId, 'M', 2),
    (@CategoriaId, 'L', 3),
    (@CategoriaId, 'XL', 4),
    (@CategoriaId, 'XXL', 5);

    -- CAMPERAS
    SELECT @CategoriaId = Id FROM Categorias WHERE Nombre = 'CAMPERAS';
    INSERT INTO TiposTalle (CategoriaId, TalleValor, Orden) VALUES
    (@CategoriaId, 'S', 1),
    (@CategoriaId, 'M', 2),
    (@CategoriaId, 'L', 3),
    (@CategoriaId, 'XL', 4),
    (@CategoriaId, 'XXL', 5);

    -- REMERAS
    SELECT @CategoriaId = Id FROM Categorias WHERE Nombre = 'REMERAS';
    INSERT INTO TiposTalle (CategoriaId, TalleValor, Orden) VALUES
    (@CategoriaId, 'S', 1),
    (@CategoriaId, 'M', 2),
    (@CategoriaId, 'L', 3),
    (@CategoriaId, 'XL', 4),
    (@CategoriaId, 'XXL', 5);

    -- CHALECOS
    SELECT @CategoriaId = Id FROM Categorias WHERE Nombre = 'CHALECOS';
    INSERT INTO TiposTalle (CategoriaId, TalleValor, Orden) VALUES
    (@CategoriaId, 'S', 1),
    (@CategoriaId, 'M', 2),
    (@CategoriaId, 'L', 3),
    (@CategoriaId, 'XL', 4),
    (@CategoriaId, 'XXL', 5);

    -- JOGGING
    SELECT @CategoriaId = Id FROM Categorias WHERE Nombre = 'JOGGING';
    INSERT INTO TiposTalle (CategoriaId, TalleValor, Orden) VALUES
    (@CategoriaId, 'S', 1),
    (@CategoriaId, 'M', 2),
    (@CategoriaId, 'L', 3),
    (@CategoriaId, 'XL', 4),
    (@CategoriaId, 'XXL', 5);

    -- BOXER
    SELECT @CategoriaId = Id FROM Categorias WHERE Nombre = 'BOXER';
    INSERT INTO TiposTalle (CategoriaId, TalleValor, Orden) VALUES
    (@CategoriaId, 'S', 1),
    (@CategoriaId, 'M', 2),
    (@CategoriaId, 'L', 3),
    (@CategoriaId, 'XL', 4),
    (@CategoriaId, 'XXL', 5);

    -- JEANS HOMBRE
    SELECT @CategoriaId = Id FROM Categorias WHERE Nombre = 'JEANS HOMBRE';
    INSERT INTO TiposTalle (CategoriaId, TalleValor, Orden) VALUES
    (@CategoriaId, '36', 1),
    (@CategoriaId, '38', 2),
    (@CategoriaId, '40', 3),
    (@CategoriaId, '42', 4),
    (@CategoriaId, '44', 5),
    (@CategoriaId, '46', 6),
    (@CategoriaId, '48', 7);

    -- JEANS DAMA
    SELECT @CategoriaId = Id FROM Categorias WHERE Nombre = 'JEANS DAMA';
    INSERT INTO TiposTalle (CategoriaId, TalleValor, Orden) VALUES
    (@CategoriaId, '34', 1),
    (@CategoriaId, '36', 2),
    (@CategoriaId, '38', 3),
    (@CategoriaId, '40', 4),
    (@CategoriaId, '42', 5),
    (@CategoriaId, '44', 6),
    (@CategoriaId, '46', 7),
    (@CategoriaId, '48', 8);

    -- ZAPATILLAS
    SELECT @CategoriaId = Id FROM Categorias WHERE Nombre = 'ZAPATILLAS';
    INSERT INTO TiposTalle (CategoriaId, TalleValor, Orden) VALUES
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
    
    PRINT 'Talles insertados.';
END

-- =====================================================
-- Crear índices para mejorar el rendimiento
-- =====================================================
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Inventario_CategoriaId')
    CREATE INDEX IX_Inventario_CategoriaId ON Inventario(CategoriaId);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Inventario_TalleId')
    CREATE INDEX IX_Inventario_TalleId ON Inventario(TalleId);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Inventario_Activo')
    CREATE INDEX IX_Inventario_Activo ON Inventario(Activo);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_TiposTalle_CategoriaId')
    CREATE INDEX IX_TiposTalle_CategoriaId ON TiposTalle(CategoriaId);

-- =====================================================
-- Crear vistas útiles para consultas frecuentes
-- =====================================================

-- Vista para mostrar el inventario completo con información legible
IF NOT EXISTS (SELECT * FROM sys.views WHERE name = 'vw_InventarioCompleto')
BEGIN
    EXEC('CREATE VIEW vw_InventarioCompleto AS
    SELECT 
        i.Id,
        c.Nombre AS Categoria,
        i.Nombre AS Producto,
        i.Descripcion,
        i.CodigoBarra,
        CASE 
            WHEN t.TalleValor IS NOT NULL THEN t.TalleValor 
            ELSE ''N/A'' 
        END AS Talle,
        i.Sabor,
        i.Cantidad,
        i.PrecioVenta,
        i.FechaCreacion,
        i.FechaModificacion,
        i.Activo
    FROM Inventario i
    INNER JOIN Categorias c ON i.CategoriaId = c.Id
    LEFT JOIN TiposTalle t ON i.TalleId = t.Id
    WHERE i.Activo = 1');
    
    PRINT 'Vista vw_InventarioCompleto creada.';
END

-- Vista para mostrar stock bajo (menos de 5 unidades)
IF NOT EXISTS (SELECT * FROM sys.views WHERE name = 'vw_StockBajo')
BEGIN
    EXEC('CREATE VIEW vw_StockBajo AS
    SELECT 
        c.Nombre AS Categoria,
        i.Nombre AS Producto,
        CASE 
            WHEN t.TalleValor IS NOT NULL THEN t.TalleValor 
            ELSE ''N/A'' 
        END AS Talle,
        i.Cantidad
    FROM Inventario i
    INNER JOIN Categorias c ON i.CategoriaId = c.Id
    LEFT JOIN TiposTalle t ON i.TalleId = t.Id
    WHERE i.Activo = 1 AND i.Cantidad <= 5');
    
    PRINT 'Vista vw_StockBajo creada.';
END

-- =====================================================
-- Crear procedimientos almacenados útiles
-- =====================================================

-- Procedimiento para obtener talles por categoría
IF NOT EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_ObtenerTallesPorCategoria')
BEGIN
    EXEC('CREATE PROCEDURE sp_ObtenerTallesPorCategoria
        @CategoriaId INT
    AS
    BEGIN
        SELECT 
            t.Id,
            t.TalleValor,
            t.Orden
        FROM TiposTalle t
        WHERE t.CategoriaId = @CategoriaId
        ORDER BY t.Orden;
    END');
    
    PRINT 'Procedimiento sp_ObtenerTallesPorCategoria creado.';
END

-- Procedimiento para agregar un producto al inventario
IF NOT EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_AgregarProducto')
BEGIN
    EXEC('CREATE PROCEDURE sp_AgregarProducto
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
        INSERT INTO Inventario (CategoriaId, Nombre, Descripcion, CodigoBarra, TalleId, Sabor, Cantidad, PrecioVenta)
        VALUES (@CategoriaId, @Nombre, @Descripcion, @CodigoBarra, @TalleId, @Sabor, @Cantidad, @PrecioVenta);
        
        SELECT SCOPE_IDENTITY() AS NuevoId;
    END');
    
    PRINT 'Procedimiento sp_AgregarProducto creado.';
END

-- Procedimiento para actualizar stock
IF NOT EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_ActualizarStock')
BEGIN
    EXEC('CREATE PROCEDURE sp_ActualizarStock
        @InventarioId INT,
        @CantidadNueva INT
    AS
    BEGIN
        UPDATE Inventario 
        SET Cantidad = @CantidadNueva,
            FechaModificacion = GETDATE()
        WHERE Id = @InventarioId;
    END');
    
    PRINT 'Procedimiento sp_ActualizarStock creado.';
END

PRINT 'Setup completo de tablas, datos, vistas y procedimientos finalizado exitosamente.';
