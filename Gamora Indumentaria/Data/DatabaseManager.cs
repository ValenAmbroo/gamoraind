using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Linq;
using Gamora_Indumentaria.Models;

namespace Gamora_Indumentaria.Data
{
    /// <summary>
    /// Clase centralizada para manejar la conexión y configuración de la base de datos
    /// </summary>
    public static class DatabaseManager
    {
        // Flag para controlar si se cargan datos de ejemplo (desactivado para usar solo datos reales)
        private const bool CARGAR_DATOS_EJEMPLO = false;
        // Cadena de conexión principal para la aplicación
        public static readonly string ConnectionString = @"Data Source=DESKTOP-8860VEA;Initial Catalog=GamoraIndumentariaDB;Integrated Security=True;TrustServerCertificate=True;Connect Timeout=30;";

        // Cadena de conexión para operaciones en master (crear BD)
        private static readonly string MasterConnectionString = @"Data Source=DESKTOP-8860VEA;Initial Catalog=master;Integrated Security=True;TrustServerCertificate=True;Connect Timeout=30;";

        /// <summary>
        /// Obtiene una nueva conexión a la base de datos
        /// </summary>
        /// <returns>SqlConnection configurada</returns>
        public static SqlConnection GetConnection()
        {
            return new SqlConnection(ConnectionString);
        }

        /// <summary>
        /// Verifica si una columna existe en una tabla dada.
        /// </summary>
        public static bool ColumnExists(string tableName, string columnName)
        {
            try
            {
                string q = @"SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @t AND COLUMN_NAME = @c";
                var dt = ExecuteQuery(q, new SqlParameter("@t", tableName), new SqlParameter("@c", columnName));
                return dt.Rows.Count > 0;
            }
            catch { return false; }
        }

        /// <summary>
        /// Inicializa la base de datos completa (crea BD, tablas y datos de ejemplo)
        /// </summary>
        public static void InitializeDatabase()
        {
            try
            {
                VerificarYCrearBaseDeDatos();
                VerificarEstructuraTablas();
                VerificarYCrearTablas();
                CrearVistas();
                if (CARGAR_DATOS_EJEMPLO)
                {
                    InsertarDatosEjemplo();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error al inicializar la base de datos: {0}", ex.Message), ex);
            }
        }

        /// <summary>
        /// Verifica la estructura actual de las tablas
        /// </summary>
        public static void VerificarEstructuraTablas()
        {
            try
            {
                using (SqlConnection connection = GetConnection())
                {
                    connection.Open();

                    // Verificar columnas de la tabla Inventario
                    string queryInventario = @"
                        SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE 
                        FROM INFORMATION_SCHEMA.COLUMNS 
                        WHERE TABLE_NAME = 'Inventario'
                        ORDER BY ORDINAL_POSITION";

                    SqlCommand cmd = new SqlCommand(queryInventario, connection);
                    SqlDataReader reader = cmd.ExecuteReader();

                    System.Diagnostics.Debug.WriteLine("=== Estructura tabla Inventario ===");
                    while (reader.Read())
                    {
                        System.Diagnostics.Debug.WriteLine(string.Format("Columna: {0}, Tipo: {1}, Nullable: {2}", reader["COLUMN_NAME"], reader["DATA_TYPE"], reader["IS_NULLABLE"]));
                    }
                    reader.Close();

                    // Verificar columnas de la tabla DetalleVentas
                    string queryDetalles = @"
                        SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE 
                        FROM INFORMATION_SCHEMA.COLUMNS 
                        WHERE TABLE_NAME = 'DetalleVentas'
                        ORDER BY ORDINAL_POSITION";

                    cmd = new SqlCommand(queryDetalles, connection);
                    reader = cmd.ExecuteReader();

                    System.Diagnostics.Debug.WriteLine("=== Estructura tabla DetalleVentas ===");
                    while (reader.Read())
                    {
                        System.Diagnostics.Debug.WriteLine(string.Format("Columna: {0}, Tipo: {1}, Nullable: {2}", reader["COLUMN_NAME"], reader["DATA_TYPE"], reader["IS_NULLABLE"]));
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("Error al verificar estructura: {0}", ex.Message));
            }
        }

        /// <summary>
        /// Verifica si la base de datos existe, si no la crea
        /// </summary>
        public static void VerificarYCrearBaseDeDatos()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(MasterConnectionString))
                {
                    connection.Open();

                    string checkDbQuery = @"
                        IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'GamoraIndumentariaDB')
                        BEGIN
                            CREATE DATABASE GamoraIndumentariaDB;
                        END";

                    SqlCommand cmd = new SqlCommand(checkDbQuery, connection);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error al crear base de datos: {0}", ex.Message), ex);
            }
        }

        /// <summary>
        /// Verifica y crea todas las tablas necesarias para la aplicación
        /// </summary>
        public static void VerificarYCrearTablas()
        {
            try
            {
                using (SqlConnection connection = GetConnection())
                {
                    connection.Open();

                    // Verificar que la conexión sea exitosa
                    if (connection.State != ConnectionState.Open)
                    {
                        throw new Exception("No se pudo establecer conexión con la base de datos");
                    }

                    // Script para crear todas las tablas
                    string createTables = @"
                        -- Tabla Categorias
                        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Categorias')
                        BEGIN
                            CREATE TABLE Categorias (
                                Id INT IDENTITY(1,1) PRIMARY KEY,
                                Nombre NVARCHAR(100) NOT NULL
                            );
                            
                            INSERT INTO Categorias (Nombre) VALUES 
                            ('Remeras'), ('Pantalones'), ('Vestidos'), ('Accesorios'), ('Calzado');
                        END

                        -- Asegurar categoría VAPER (y opcional VAPERS) exista aunque la tabla ya exista
                        IF NOT EXISTS (SELECT 1 FROM Categorias WHERE Nombre = 'VAPER')
                        BEGIN
                            INSERT INTO Categorias (Nombre) VALUES ('VAPER');
                        END
                        IF NOT EXISTS (SELECT 1 FROM Categorias WHERE Nombre = 'VAPERS')
                        BEGIN
                            INSERT INTO Categorias (Nombre) VALUES ('VAPERS');
                        END

                        -- Tabla Inventario
                        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Inventario')
                        BEGIN
                            CREATE TABLE Inventario (
                                Id INT IDENTITY(1,1) PRIMARY KEY,
                                Nombre NVARCHAR(200) NOT NULL,
                                Descripcion NVARCHAR(500),
                                PrecioVenta DECIMAL(10,2) NOT NULL, -- Precio de venta
                                PrecioCompra DECIMAL(10,2) NULL, -- Precio de compra (puede ser NULL al inicio)
                                Stock INT NOT NULL DEFAULT 0,
                                CategoriaId INT,
                                TalleId INT NULL,
                                Sabor NVARCHAR(100) NULL,
                                CodigoBarras NVARCHAR(50) NULL,
                                FechaCreacion DATETIME DEFAULT GETDATE(),
                                FOREIGN KEY (CategoriaId) REFERENCES Categorias(Id)
                            );
                        END

                        -- Compatibilidad: renombrar columna antigua 'Precio' a 'PrecioVenta' si existe
                        IF COL_LENGTH('Inventario','PrecioVenta') IS NULL AND COL_LENGTH('Inventario','Precio') IS NOT NULL
                        BEGIN
                            EXEC sp_rename 'Inventario.Precio','PrecioVenta','COLUMN';
                        END

                        -- Compatibilidad: renombrar columna antigua 'PrecioCosto' a 'PrecioCompra'
                        IF COL_LENGTH('Inventario','PrecioCompra') IS NULL AND COL_LENGTH('Inventario','PrecioCosto') IS NOT NULL
                        BEGIN
                            EXEC sp_rename 'Inventario.PrecioCosto','PrecioCompra','COLUMN';
                        END

                        -- Agregar columna PrecioCompra si no existe (nueva estructura)
                        IF COL_LENGTH('Inventario','PrecioCompra') IS NULL
                        BEGIN
                            ALTER TABLE Inventario ADD PrecioCompra DECIMAL(10,2) NULL;
                        END

                        -- Agregar columna CodigoBarras si no existe
                        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
                                     WHERE TABLE_NAME = 'Inventario' AND COLUMN_NAME = 'CodigoBarras')
                        BEGIN
                            ALTER TABLE Inventario ADD CodigoBarras NVARCHAR(50) NULL;
                        END
                        -- Agregar columna TalleId si no existe
                        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
                                     WHERE TABLE_NAME = 'Inventario' AND COLUMN_NAME = 'TalleId')
                        BEGIN
                            ALTER TABLE Inventario ADD TalleId INT NULL;
                        END
                        -- Agregar columna Sabor si no existe
                        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
                                     WHERE TABLE_NAME = 'Inventario' AND COLUMN_NAME = 'Sabor')
                        BEGIN
                            ALTER TABLE Inventario ADD Sabor NVARCHAR(100) NULL;
                        END

                        -- Tabla Ventas
                        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Ventas')
                        BEGIN
                            CREATE TABLE Ventas (
                                Id INT IDENTITY(1,1) PRIMARY KEY,
                                FechaVenta DATETIME NOT NULL DEFAULT GETDATE(),
                                Total DECIMAL(10,2) NOT NULL,
                                MetodoPago NVARCHAR(50) NOT NULL,
                                Cliente NVARCHAR(200) NULL
                            );
                        END

                        -- Tabla TiposTalle (relaciona categorías con valores de talle)
                        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'TiposTalle')
                        BEGIN
                            CREATE TABLE TiposTalle (
                                Id INT IDENTITY(1,1) PRIMARY KEY,
                                CategoriaId INT NOT NULL,
                                TalleValor NVARCHAR(20) NOT NULL,
                                Orden INT NOT NULL,
                                FOREIGN KEY (CategoriaId) REFERENCES Categorias(Id)
                            );

                            -- Poblar talles básicos si la tabla estaba vacía
                            INSERT INTO TiposTalle (CategoriaId, TalleValor, Orden)
                            SELECT c.Id, v.TalleValor, v.Orden FROM Categorias c
                            CROSS APPLY (VALUES
                                ('Remeras','XS',1),('Remeras','S',2),('Remeras','M',3),('Remeras','L',4),('Remeras','XL',5),
                                ('Pantalones','36',1),('Pantalones','38',2),('Pantalones','40',3),('Pantalones','42',4),('Pantalones','44',5),
                                ('Vestidos','S',1),('Vestidos','M',2),('Vestidos','L',3),
                                ('Calzado','35',1),('Calzado','36',2),('Calzado','37',3),('Calzado','38',4),('Calzado','39',5),('Calzado','40',6)
                            ) v(CategoriaNombre,TalleValor,Orden)
                            WHERE c.Nombre = v.CategoriaNombre;
                        END
                        ELSE
                        BEGIN
                            -- Verificar que la columna Id sea IDENTITY
                            IF NOT EXISTS (
                                SELECT 1 FROM sys.identity_columns 
                                WHERE object_id = OBJECT_ID('Ventas') 
                                AND name = 'Id'
                            )
                            BEGIN
                                -- Eliminar tabla y recrear con IDENTITY
                                DROP TABLE Ventas;
                                CREATE TABLE Ventas (
                                    Id INT IDENTITY(1,1) PRIMARY KEY,
                                    FechaVenta DATETIME NOT NULL DEFAULT GETDATE(),
                                    Total DECIMAL(10,2) NOT NULL,
                                    MetodoPago NVARCHAR(50) NOT NULL,
                                    Cliente NVARCHAR(200) NULL
                                );
                            END
                        END

                        -- Agregar columna MetodoPago si no existe
                        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
                                     WHERE TABLE_NAME = 'Ventas' AND COLUMN_NAME = 'MetodoPago')
                        BEGIN
                            ALTER TABLE Ventas ADD MetodoPago NVARCHAR(50);
                        END

                        -- Agregar columna Cliente si no existe (para compatibilidad con bases antiguas)
                        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
                                     WHERE TABLE_NAME = 'Ventas' AND COLUMN_NAME = 'Cliente')
                        BEGIN
                            ALTER TABLE Ventas ADD Cliente NVARCHAR(200) NULL;
                        END

            -- Tabla DetalleVentas
                        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'DetalleVentas')
                        BEGIN
                            CREATE TABLE DetalleVentas (
                                Id INT IDENTITY(1,1) PRIMARY KEY,
                                VentaId INT NOT NULL,
                                ProductoId INT NOT NULL,
                                Cantidad INT NOT NULL,
                                PrecioUnitario DECIMAL(10,2) NOT NULL,
                                Subtotal DECIMAL(10,2) NOT NULL,
                Descuento DECIMAL(10,2) NULL DEFAULT 0,
                                FOREIGN KEY (VentaId) REFERENCES Ventas(Id),
                                FOREIGN KEY (ProductoId) REFERENCES Inventario(Id)
                            );
                        END

                        -- Agregar columna Subtotal si no existe
                        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
                                     WHERE TABLE_NAME = 'DetalleVentas' AND COLUMN_NAME = 'Subtotal')
                        BEGIN
                            ALTER TABLE DetalleVentas ADD Subtotal DECIMAL(10,2);
                            -- Calcular subtotal para registros existentes
                            UPDATE DetalleVentas SET Subtotal = Cantidad * PrecioUnitario WHERE Subtotal IS NULL;
                        END

                        -- Agregar columna Descuento si no existe
                        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
                                     WHERE TABLE_NAME = 'DetalleVentas' AND COLUMN_NAME = 'Descuento')
                        BEGIN
                            ALTER TABLE DetalleVentas ADD Descuento DECIMAL(10,2) NULL DEFAULT 0;
                            UPDATE DetalleVentas SET Descuento = 0 WHERE Descuento IS NULL;
                        END

                        -- Agregar columna CostoUnitario si no existe (para cálculo de ganancias históricas)
                        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
                                     WHERE TABLE_NAME = 'DetalleVentas' AND COLUMN_NAME = 'CostoUnitario')
                        BEGIN
                            ALTER TABLE DetalleVentas ADD CostoUnitario DECIMAL(10,2) NULL;
                        END

                        -- Tabla CierresCaja para almacenar resumen diario
                        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'CierresCaja')
                        BEGIN
                            CREATE TABLE CierresCaja (
                                Id INT IDENTITY(1,1) PRIMARY KEY,
                                Fecha DATE NOT NULL UNIQUE,
                                TotalVentas DECIMAL(18,2) NOT NULL,
                                CostoMercaderia DECIMAL(18,2) NOT NULL,
                                Ganancia DECIMAL(18,2) NOT NULL,
                                CantidadVentas INT NOT NULL,
                                CantidadItems INT NOT NULL,
                                HoraCierre DATETIME NOT NULL DEFAULT GETDATE()
                            );
                        END";

                    SqlCommand cmd = new SqlCommand(createTables, connection);
                    cmd.ExecuteNonQuery();

                    string createIndexes = @"
                        IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Ventas_FechaVenta')
                            CREATE INDEX IX_Ventas_FechaVenta ON Ventas(FechaVenta);
                        IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Ventas_MetodoPago')
                            CREATE INDEX IX_Ventas_MetodoPago ON Ventas(MetodoPago);
                        IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_DetalleVentas_VentaId')
                            CREATE INDEX IX_DetalleVentas_VentaId ON DetalleVentas(VentaId);
                        IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_DetalleVentas_ProductoId')
                            CREATE INDEX IX_DetalleVentas_ProductoId ON DetalleVentas(ProductoId);";
                    using (SqlCommand idxCmd = new SqlCommand(createIndexes, connection))
                    {
                        idxCmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error al crear tablas: {0}", ex.Message), ex);
            }
        }

        /// <summary>
        /// Crea las vistas necesarias para la aplicación solo si no existen
        /// </summary>
        public static void CrearVistas()
        {
            try
            {
                using (SqlConnection connection = GetConnection())
                {
                    connection.Open();
                    // Siempre recrear vistas para asegurar estructura actualizada (incluye columna Talle)
                    string dropViews = @"
                        IF EXISTS (SELECT * FROM sys.views WHERE name = 'vw_InventarioCompleto')
                            DROP VIEW vw_InventarioCompleto;
                        IF EXISTS (SELECT * FROM sys.views WHERE name = 'vw_StockBajo')
                            DROP VIEW vw_StockBajo;";
                    using (SqlCommand dropCmd = new SqlCommand(dropViews, connection))
                    {
                        dropCmd.ExecuteNonQuery();
                    }

                    string createView1 = @"
                        CREATE VIEW vw_InventarioCompleto AS
                        SELECT 
                            i.Id,
                            i.Nombre AS Producto,
                            ISNULL(i.Descripcion, '') AS Descripcion,
                            ISNULL(i.PrecioVenta, 0) AS PrecioVenta,
                            ISNULL(i.PrecioCompra, 0) AS PrecioCompra,
                            ISNULL(i.Stock, 0) AS Cantidad,
                            c.Nombre AS Categoria,
                            ISNULL(i.CodigoBarras, '') AS CodigoBarras,
                            ISNULL(i.FechaCreacion, GETDATE()) AS FechaCreacion,
                            i.CategoriaId,
                            i.TalleId,
                            ISNULL(tt.TalleValor,'') AS Talle,
                            ISNULL(i.Sabor,'') AS Sabor,
                            ISNULL(i.FechaCreacion, GETDATE()) AS FechaModificacion,
                            1 AS Activo
                        FROM Inventario i
                        INNER JOIN Categorias c ON i.CategoriaId = c.Id
                        LEFT JOIN TiposTalle tt ON i.TalleId = tt.Id";
                    using (SqlCommand cmd1 = new SqlCommand(createView1, connection))
                    {
                        cmd1.ExecuteNonQuery();
                    }

                    string createView2 = @"
                        CREATE VIEW vw_StockBajo AS
                        SELECT 
                            i.Id,
                            i.Nombre AS Producto,
                            c.Nombre AS Categoria,
                            ISNULL(i.Stock, 0) AS Cantidad,
                            ISNULL(i.PrecioVenta, 0) AS PrecioVenta,
                            ISNULL(i.PrecioCompra, 0) AS PrecioCompra,
                            'Stock Bajo' AS Estado
                        FROM Inventario i
                        INNER JOIN Categorias c ON i.CategoriaId = c.Id
                        WHERE ISNULL(i.Stock, 0) <= 10";
                    using (SqlCommand cmd2 = new SqlCommand(createView2, connection))
                    {
                        cmd2.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error al crear vistas: {0}", ex.Message), ex);
            }
        }

        /// <summary>
        /// Inserta datos de ejemplo si las tablas están vacías
        /// </summary>
        public static void InsertarDatosEjemplo()
        {
            try
            {
                using (SqlConnection connection = GetConnection())
                {
                    connection.Open();

                    // Verificar si ya hay productos en inventario
                    SqlCommand checkInventario = new SqlCommand("SELECT COUNT(*) FROM Inventario", connection);
                    int inventarioCount = (int)checkInventario.ExecuteScalar();

                    if (inventarioCount == 0)
                    {
                        string insertInventario = @"
                            INSERT INTO Inventario (Nombre, Descripcion, PrecioVenta, PrecioCompra, Stock, CategoriaId, CodigoBarras) VALUES 
                            ('Remera Básica Blanca', 'Remera de algodón 100%', 2500.00, 1500.00, 50, 1, '7891234567890'),
                            ('Jean Clásico Azul', 'Pantalón jean corte recto', 4500.00, 3000.00, 30, 2, '7891234567891'),
                            ('Vestido Casual Negro', 'Vestido para uso diario', 3500.00, 2100.00, 20, 3, '7891234567892'),
                            ('Collar Dorado', 'Collar de acero dorado', 1500.00, 800.00, 15, 4, '7891234567893'),
                            ('Zapatillas Deportivas', 'Zapatillas para correr', 8500.00, 6000.00, 25, 5, '7891234567894'),
                            ('Remera Estampada', 'Remera con diseño exclusivo', 3000.00, 1800.00, 40, 1, '7891234567895'),
                            ('Pantalón Cargo', 'Pantalón con bolsillos', 5500.00, 3500.00, 20, 2, '7891234567896'),
                            ('Vestido de Noche', 'Vestido elegante', 8500.00, 5000.00, 10, 3, '7891234567897'),
                            ('Pulsera de Plata', 'Pulsera artesanal', 2200.00, 1200.00, 12, 4, '7891234567898'),
                            ('Botas de Cuero', 'Botas para invierno', 12000.00, 8000.00, 15, 5, '7891234567899');";

                        SqlCommand inventarioCmd = new SqlCommand(insertInventario, connection);
                        inventarioCmd.ExecuteNonQuery();

                        // Actualizar códigos de barras para productos existentes sin código
                        string updateCodigos = @"
                            UPDATE Inventario 
                            SET CodigoBarras = '7891234' + RIGHT('000000' + CAST(Id AS VARCHAR), 6)
                            WHERE CodigoBarras IS NULL OR CodigoBarras = ''";

                        SqlCommand updateCmd = new SqlCommand(updateCodigos, connection);
                        updateCmd.ExecuteNonQuery();
                    }

                    // Verificar si ya hay ventas
                    SqlCommand checkVentas = new SqlCommand("SELECT COUNT(*) FROM Ventas", connection);
                    int ventasCount = (int)checkVentas.ExecuteScalar();

                    if (ventasCount == 0)
                    {
                        // Insertar ventas de ejemplo
                        string insertVentas = @"
                            DECLARE @fechaBase DATETIME = GETDATE();
                            
                            -- Ventas de hoy
                            INSERT INTO Ventas (FechaVenta, Total, MetodoPago, Cliente) VALUES 
                            (DATEADD(HOUR, -2, @fechaBase), 6500.00, 'Efectivo', 'María García'),
                            (DATEADD(HOUR, -4, @fechaBase), 4500.00, 'Tarjeta de Débito', 'Juan Pérez'),
                            
                            -- Ventas de ayer  
                            (DATEADD(DAY, -1, DATEADD(HOUR, -3, @fechaBase)), 11500.00, 'Tarjeta de Crédito', 'Ana López'),
                            (DATEADD(DAY, -1, DATEADD(HOUR, -5, @fechaBase)), 8500.00, 'Transferencia', 'Carlos Ruiz'),
                            (DATEADD(DAY, -1, DATEADD(HOUR, -7, @fechaBase)), 15500.00, 'Mercado Pago', 'Laura Martín'),
                            (DATEADD(DAY, -1, DATEADD(HOUR, -9, @fechaBase)), 6200.00, 'Efectivo', 'Pedro Silva'),
                            
                            -- Ventas de la semana pasada
                            (DATEADD(DAY, -3, @fechaBase), 9800.00, 'Tarjeta de Débito', 'Sofia Herrera'),
                            (DATEADD(DAY, -4, @fechaBase), 11200.00, 'Efectivo', 'Diego Torres'),
                            (DATEADD(DAY, -5, @fechaBase), 7800.00, 'Tarjeta de Crédito', 'Carmen Vega'),
                            (DATEADD(DAY, -6, @fechaBase), 13400.00, 'Transferencia', 'Roberto Díaz'),
                            (DATEADD(DAY, -7, @fechaBase), 5900.00, 'Mercado Pago', 'Lucía Morales'),
                            
                            -- Ventas del mes pasado
                            (DATEADD(DAY, -15, @fechaBase), 16800.00, 'Tarjeta de Crédito', 'Fernando Castro'),
                            (DATEADD(DAY, -20, @fechaBase), 8900.00, 'Efectivo', 'Elena Jiménez'),
                            (DATEADD(DAY, -25, @fechaBase), 12300.00, 'Tarjeta de Débito', 'Andrés Guerrero'),
                            (DATEADD(DAY, -30, @fechaBase), 14700.00, 'Mercado Pago', 'Patricia Ramos');";

                        SqlCommand ventasCmd = new SqlCommand(insertVentas, connection);
                        ventasCmd.ExecuteNonQuery();

                        // Insertar detalles de ventas
                        string insertDetalles = @"
                            INSERT INTO DetalleVentas (VentaId, ProductoId, Cantidad, PrecioUnitario, Subtotal) VALUES 
                            -- Venta 1 (María García)
                            (1, 1, 2, 2500.00, 5000.00), (1, 4, 1, 1500.00, 1500.00),
                            -- Venta 2 (Juan Pérez)  
                            (2, 2, 1, 4500.00, 4500.00),
                            -- Venta 3 (Ana López)
                            (3, 5, 1, 8500.00, 8500.00), (3, 6, 1, 3000.00, 3000.00),
                            -- Venta 4 (Carlos Ruiz)
                            (4, 3, 2, 3500.00, 7000.00), (4, 4, 1, 1500.00, 1500.00),
                            -- Venta 5 (Laura Martín)
                            (5, 10, 1, 12000.00, 12000.00), (5, 6, 1, 3000.00, 3000.00),
                            -- Venta 6 (Pedro Silva)
                            (6, 1, 1, 2500.00, 2500.00), (6, 6, 1, 3000.00, 3000.00),
                            -- Venta 7 (Sofia Herrera)
                            (7, 7, 1, 5500.00, 5500.00), (7, 2, 1, 4500.00, 4500.00),
                            -- Venta 8 (Diego Torres)
                            (8, 8, 1, 8500.00, 8500.00), (8, 1, 1, 2500.00, 2500.00),
                            -- Venta 9 (Carmen Vega)
                            (9, 6, 2, 3000.00, 6000.00), (9, 9, 1, 2200.00, 2200.00),
                            -- Venta 10 (Roberto Díaz)
                            (10, 10, 1, 12000.00, 12000.00), (10, 4, 1, 1500.00, 1500.00),
                            -- Venta 11 (Lucía Morales)
                            (11, 1, 2, 2500.00, 5000.00), (11, 9, 1, 2200.00, 2200.00),
                            -- Venta 12 (Fernando Castro)
                            (12, 5, 2, 8500.00, 17000.00),
                            -- Venta 13 (Elena Jiménez)
                            (13, 7, 1, 5500.00, 5500.00), (13, 6, 1, 3000.00, 3000.00),
                            -- Venta 14 (Andrés Guerrero)
                            (14, 8, 1, 8500.00, 8500.00), (14, 3, 1, 3500.00, 3500.00),
                            -- Venta 15 (Patricia Ramos)
                            (15, 10, 1, 12000.00, 12000.00), (15, 1, 1, 2500.00, 2500.00);";

                        SqlCommand detallesCmd = new SqlCommand(insertDetalles, connection);
                        detallesCmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                // Log pero no lanzar excepción para no interrumpir la aplicación
                System.Diagnostics.Debug.WriteLine("Error al insertar datos de ejemplo: " + ex.Message);
            }
        }

        /// <summary>
        /// Ejecuta una consulta que devuelve datos (SELECT)
        /// </summary>
        /// <param name="query">Consulta SQL</param>
        /// <param name="parameters">Parámetros opcionales</param>
        /// <returns>DataTable con los resultados</returns>
        public static DataTable ExecuteQuery(string query, params SqlParameter[] parameters)
        {
            try
            {
                using (SqlConnection connection = GetConnection())
                {
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        if (parameters != null)
                        {
                            cmd.Parameters.AddRange(parameters);
                        }

                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        return dt;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error al ejecutar consulta: {0}", ex.Message), ex);
            }
        }

        /// <summary>
        /// Ejecuta una consulta que no devuelve datos (INSERT, UPDATE, DELETE)
        /// </summary>
        /// <param name="query">Consulta SQL</param>
        /// <param name="parameters">Parámetros opcionales</param>
        /// <returns>Número de filas afectadas</returns>
        public static int ExecuteNonQuery(string query, params SqlParameter[] parameters)
        {
            try
            {
                using (SqlConnection connection = GetConnection())
                {
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        if (parameters != null)
                        {
                            cmd.Parameters.AddRange(parameters);
                        }

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error al ejecutar comando: {0}", ex.Message), ex);
            }
        }

        /// <summary>
        /// Ejecuta una consulta que devuelve un valor único (COUNT, SUM, etc.)
        /// </summary>
        /// <param name="query">Consulta SQL</param>
        /// <param name="parameters">Parámetros opcionales</param>
        /// <returns>Valor escalar</returns>
        public static object ExecuteScalar(string query, params SqlParameter[] parameters)
        {
            try
            {
                using (SqlConnection connection = GetConnection())
                {
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        if (parameters != null)
                        {
                            cmd.Parameters.AddRange(parameters);
                        }

                        return cmd.ExecuteScalar();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error al ejecutar consulta escalar: {0}", ex.Message), ex);
            }
        }

        /// <summary>
        /// Verifica si la base de datos está disponible
        /// </summary>
        /// <returns>True si la conexión es exitosa</returns>
        public static bool TestConnection()
        {
            try
            {
                using (SqlConnection connection = GetConnection())
                {
                    connection.Open();
                    return connection.State == ConnectionState.Open;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Procesa una venta completa con transacción
        /// </summary>
        /// <param name="carrito">Lista de items del carrito</param>
        /// <param name="metodoPago">Método de pago</param>
        /// <param name="total">Total de la venta</param>
        /// <param name="metodoPago">Método de pago</param>
        /// <param name="detalles">Lista de detalles de la venta</param>
        /// <returns>ID de la venta creada</returns>
        public static int ProcesarVenta(decimal total, string metodoPago, List<ItemVenta> detalles)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // 1. Insertar la venta principal
                        string insertVenta = @"
                            INSERT INTO Ventas (Total, MetodoPago)
                            VALUES (@Total, @MetodoPago);
                            SELECT SCOPE_IDENTITY();";

                        SqlCommand cmd = new SqlCommand(insertVenta, conn, transaction);
                        cmd.Parameters.AddWithValue("@Total", total);
                        cmd.Parameters.AddWithValue("@MetodoPago", metodoPago);

                        int ventaId = Convert.ToInt32(cmd.ExecuteScalar());

                        // 2. Insertar los detalles y actualizar stock con validación de stock
                        bool existeCostoUnitario = false;
                        using (SqlCommand checkCosto = new SqlCommand("SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='DetalleVentas' AND COLUMN_NAME='CostoUnitario'", conn, transaction))
                        {
                            existeCostoUnitario = Convert.ToInt32(checkCosto.ExecuteScalar()) > 0;
                        }
                        string insertDetalle = existeCostoUnitario ? @"
                            INSERT INTO DetalleVentas (VentaId, ProductoId, Cantidad, PrecioUnitario, Subtotal, CostoUnitario, Descuento)
                            VALUES (@VentaId, @ProductoId, @Cantidad, @PrecioUnitario, @Subtotal, @CostoUnitario, @Descuento);
                            UPDATE Inventario SET Stock = Stock - @Cantidad WHERE Id = @ProductoId;" : @"
                            INSERT INTO DetalleVentas (VentaId, ProductoId, Cantidad, PrecioUnitario, Subtotal, Descuento)
                            VALUES (@VentaId, @ProductoId, @Cantidad, @PrecioUnitario, @Subtotal, @Descuento);
                            UPDATE Inventario SET Stock = Stock - @Cantidad WHERE Id = @ProductoId;";

                        foreach (var detalle in detalles)
                        {
                            // Validar stock actual antes de descontar
                            using (SqlCommand checkStock = new SqlCommand("SELECT Stock FROM Inventario WHERE Id = @Id", conn, transaction))
                            {
                                checkStock.Parameters.AddWithValue("@Id", detalle.ProductoId);
                                object stockVal = checkStock.ExecuteScalar();
                                if (stockVal == null)
                                    throw new Exception("Producto inexistente (ID: " + detalle.ProductoId + ")");
                                int stockActual = Convert.ToInt32(stockVal);
                                if (stockActual < detalle.Cantidad)
                                    throw new Exception(string.Format("Stock insuficiente para el producto ID {0}. Disponible: {1}, Requerido: {2}", detalle.ProductoId, stockActual, detalle.Cantidad));
                            }

                            using (SqlCommand cmdDetalle = new SqlCommand(insertDetalle, conn, transaction))
                            {
                                cmdDetalle.Parameters.AddWithValue("@VentaId", ventaId);
                                cmdDetalle.Parameters.AddWithValue("@ProductoId", detalle.ProductoId);
                                cmdDetalle.Parameters.AddWithValue("@Cantidad", detalle.Cantidad);
                                cmdDetalle.Parameters.AddWithValue("@PrecioUnitario", detalle.PrecioUnitario);
                                cmdDetalle.Parameters.AddWithValue("@Subtotal", detalle.Subtotal);
                                if (insertDetalle.Contains("@CostoUnitario"))
                                {
                                    // Obtener costo (PrecioCompra) actual del producto
                                    using (SqlCommand costoCmd = new SqlCommand("SELECT ISNULL(PrecioCompra,0) FROM Inventario WHERE Id=@Id", conn, transaction))
                                    {
                                        costoCmd.Parameters.AddWithValue("@Id", detalle.ProductoId);
                                        var costo = Convert.ToDecimal(costoCmd.ExecuteScalar());
                                        cmdDetalle.Parameters.AddWithValue("@CostoUnitario", costo);
                                    }
                                }
                                cmdDetalle.Parameters.AddWithValue("@Descuento", detalle.Descuento);
                                cmdDetalle.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                        return ventaId;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception(string.Format("Error al procesar la venta: {0}", ex.Message), ex);
                    }
                }
            }
        }

        /// <summary>
        /// Procesa una venta desde el carrito de compras
        /// </summary>
        public static int ProcesarVenta(List<ItemCarrito> carrito, string metodoPago, decimal totalVenta)
        {
            var detalles = carrito.Select(item => new ItemVenta
            {
                ProductoId = item.ProductoId,
                Cantidad = item.Cantidad,
                PrecioUnitario = item.PrecioUnitario,
                // Subtotal ya con el descuento aplicado
                Subtotal = item.Subtotal * (1 - (item.Descuento / 100m)),
                Descuento = item.Descuento
            }).ToList();
            return ProcesarVenta(totalVenta, metodoPago, detalles);
        }

        /// <summary>
        /// Realiza el cierre de caja del día (fecha local) calculando ventas, costo y ganancia.
        /// </summary>
        public static void CerrarCaja(DateTime fecha)
        {
            DateTime dia = fecha.Date;
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (SqlTransaction tr = conn.BeginTransaction())
                {
                    try
                    {
                        // Evitar duplicar cierre
                        using (SqlCommand existe = new SqlCommand("SELECT COUNT(*) FROM CierresCaja WHERE Fecha=@F", conn, tr))
                        {
                            existe.Parameters.AddWithValue("@F", dia);
                            if (Convert.ToInt32(existe.ExecuteScalar()) > 0)
                                throw new Exception("Ya existe un cierre para esta fecha");
                        }

                        string sql = @"
                            SELECT v.Id, v.Total, v.FechaVenta, d.Cantidad, d.PrecioUnitario, ISNULL(d.CostoUnitario,0) AS CostoUnitario
                            FROM Ventas v
                            INNER JOIN DetalleVentas d ON v.Id = d.VentaId
                            WHERE CONVERT(date, v.FechaVenta) = @Fecha";
                        decimal totalVentas = 0m, costoTotal = 0m; int cantVentas = 0, cantItems = 0; int lastVenta = -1;
                        using (SqlCommand cmd = new SqlCommand(sql, conn, tr))
                        {
                            cmd.Parameters.AddWithValue("@Fecha", dia);
                            using (SqlDataReader rd = cmd.ExecuteReader())
                            {
                                while (rd.Read())
                                {
                                    int ventaId = rd.GetInt32(0);
                                    decimal total = rd.GetDecimal(1);
                                    if (ventaId != lastVenta)
                                    {
                                        totalVentas += total;
                                        cantVentas++;
                                        lastVenta = ventaId;
                                    }
                                    int cantidad = rd.GetInt32(3);
                                    decimal costoUnit = rd.GetDecimal(5);
                                    decimal precioUnit = rd.GetDecimal(4);
                                    cantItems += cantidad;
                                    costoTotal += costoUnit * cantidad;
                                }
                            }
                        }
                        decimal ganancia = totalVentas - costoTotal;
                        using (SqlCommand insert = new SqlCommand(@"INSERT INTO CierresCaja (Fecha, TotalVentas, CostoMercaderia, Ganancia, CantidadVentas, CantidadItems) VALUES (@Fecha,@Total,@Costo,@Gan,@CV,@CI)", conn, tr))
                        {
                            insert.Parameters.AddWithValue("@Fecha", dia);
                            insert.Parameters.AddWithValue("@Total", totalVentas);
                            insert.Parameters.AddWithValue("@Costo", costoTotal);
                            insert.Parameters.AddWithValue("@Gan", ganancia);
                            insert.Parameters.AddWithValue("@CV", cantVentas);
                            insert.Parameters.AddWithValue("@CI", cantItems);
                            insert.ExecuteNonQuery();
                        }
                        tr.Commit();
                    }
                    catch (Exception ex)
                    {
                        tr.Rollback();
                        throw new Exception("Error al cerrar caja: " + ex.Message, ex);
                    }
                }
            }
        }
    }
}
