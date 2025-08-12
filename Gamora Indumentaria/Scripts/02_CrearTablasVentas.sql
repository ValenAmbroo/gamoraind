USE [GamoraIndumentariaDB]
GO

-- Tabla: Ventas
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Ventas]') AND type in (N'U'))
BEGIN
    CREATE TABLE Ventas (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        FechaVenta DATETIME NOT NULL DEFAULT GETDATE(),
        Total DECIMAL(10,2) NOT NULL,
        MetodoPago NVARCHAR(50) NULL
    );
END
GO

-- Tabla: DetalleVentas
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DetalleVentas]') AND type in (N'U'))
BEGIN
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
END
GO
