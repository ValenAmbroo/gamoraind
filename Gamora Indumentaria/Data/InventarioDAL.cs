using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Linq;

namespace Gamora_Indumentaria.Data
{
    /// <summary>
    /// Clase para manejar las operaciones de base de datos del inventario
    /// </summary>
    public class InventarioDAL
    {
        public InventarioDAL()
        {
            // Inicializar la base de datos si es necesario
            DatabaseManager.InitializeDatabase();
        }

        #region Categorías

        /// <summary>
        /// Obtiene todas las categorías disponibles
        /// </summary>
        public List<Categoria> ObtenerCategorias()
        {
            List<Categoria> categorias = new List<Categoria>();

            try
            {
                string query = "SELECT Id, Nombre FROM Categorias ORDER BY Nombre";
                DataTable dt = DatabaseManager.ExecuteQuery(query);

                foreach (DataRow row in dt.Rows)
                {
                    string nombre = row["Nombre"].ToString();
                    // Heurística: categorías de indumentaria que manejan talles
                    bool tieneTalle = EsCategoriaConTalle(nombre);
                    string tipoTalle = InferirTipoTalle(nombre);
                    categorias.Add(new Categoria
                    {
                        Id = Convert.ToInt32(row["Id"]),
                        Nombre = nombre,
                        TieneTalle = tieneTalle,
                        TipoTalle = tipoTalle
                    });
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error al obtener categorías: {0}", ex.Message), ex);
            }

            return categorias;
        }

        /// <summary>
        /// Agrega una nueva categoría y devuelve su Id (SCOPE_IDENTITY)
        /// </summary>
        public int AgregarCategoria(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre)) throw new ArgumentException("El nombre de la categoría no puede estar vacío.", nameof(nombre));

            try
            {
                string query = @"INSERT INTO Categorias (Nombre) VALUES (@Nombre); SELECT SCOPE_IDENTITY();";
                object result = DatabaseManager.ExecuteScalar(query, new SqlParameter("@Nombre", nombre.Trim()));
                return Convert.ToInt32(result);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error al agregar categoría: {0}", ex.Message), ex);
            }
        }

        // Determina si una categoría maneja talles (heurística basada en nombre)
        private bool EsCategoriaConTalle(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre)) return false;
            nombre = nombre.Trim().ToUpperInvariant();
            // Ajusta aquí según tus categorías reales
            string[] categoriasConTalle = { "REMERAS", "PANTALONES", "VESTIDOS", "CALZADO", "BUZOS", "CAMPERAS" };
            return categoriasConTalle.Contains(nombre);
        }

        // Inferir tipo de talle (alfanumérico vs numérico) para uso futuro
        private string InferirTipoTalle(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre)) return null;
            nombre = nombre.Trim().ToUpperInvariant();
            // Ejemplo simple: Calzado suele ser numérico, resto alfanumérico
            if (nombre == "CALZADO") return "NUM";
            if (EsCategoriaConTalle(nombre)) return "ALFA";
            return null;
        }

        /// <summary>
        /// Obtiene los talles disponibles para una categoría específica
        /// </summary>
        public List<TallePorCategoria> ObtenerTallesPorCategoria(int categoriaId)
        {
            List<TallePorCategoria> talles = new List<TallePorCategoria>();

            try
            {
                string query = @"SELECT Id, TalleValor, Orden 
                               FROM TiposTalle 
                               WHERE CategoriaId = @CategoriaId 
                               ORDER BY Orden";

                SqlParameter[] parameters = {
                    new SqlParameter("@CategoriaId", categoriaId)
                };

                DataTable dt = DatabaseManager.ExecuteQuery(query, parameters);

                foreach (DataRow row in dt.Rows)
                {
                    talles.Add(new TallePorCategoria
                    {
                        Id = Convert.ToInt32(row["Id"]),
                        TalleValor = row["TalleValor"].ToString(),
                        Orden = Convert.ToInt32(row["Orden"])
                    });
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error al obtener talles: {0}", ex.Message), ex);
            }

            return talles;
        }

        #endregion

        #region Inventario

        /// <summary>
        /// Obtiene todos los productos del inventario
        /// </summary>
        public DataTable ObtenerInventarioCompleto()
        {
            string query = "SELECT * FROM vw_InventarioCompleto ORDER BY Categoria, Producto, Talle";
            return DatabaseManager.ExecuteQuery(query);
        }

        /// <summary>
        /// Obtiene productos por categoría
        /// </summary>
        public DataTable ObtenerInventarioPorCategoria(string categoria)
        {
            string query = @"SELECT * FROM vw_InventarioCompleto 
                           WHERE Categoria = @Categoria 
                           ORDER BY Producto, Talle";

            return DatabaseManager.ExecuteQuery(query, new SqlParameter("@Categoria", categoria));
        }

        /// <summary>
        /// Agrega un nuevo producto al inventario
        /// </summary>
        public int AgregarProducto(ProductoInventario producto)
        {
            string query = @"
                INSERT INTO Inventario (CategoriaId, Nombre, Descripcion, CodigoBarras, TalleId, Sabor, Stock, PrecioVenta, PrecioCompra, FechaCreacion)
                VALUES (@CategoriaId, @Nombre, @Descripcion, @CodigoBarras, @TalleId, @Sabor, @Stock, @PrecioVenta, @PrecioCompra, GETDATE());
                SELECT SCOPE_IDENTITY();";

            object result = DatabaseManager.ExecuteScalar(query,
                new SqlParameter("@CategoriaId", producto.CategoriaId),
                new SqlParameter("@Nombre", producto.Nombre),
                new SqlParameter("@Descripcion", (object)producto.Descripcion ?? DBNull.Value),
                new SqlParameter("@CodigoBarras", (object)producto.CodigoBarra ?? DBNull.Value),
                new SqlParameter("@TalleId", (object)producto.TalleId ?? DBNull.Value),
                new SqlParameter("@Sabor", (object)producto.Sabor ?? DBNull.Value),
                new SqlParameter("@Stock", producto.Cantidad),
                new SqlParameter("@PrecioVenta", (object)producto.PrecioVenta ?? DBNull.Value),
                new SqlParameter("@PrecioCompra", (object)producto.PrecioCosto ?? DBNull.Value));

            return Convert.ToInt32(result);
        }

        /// <summary>
        /// Actualiza el stock de un producto
        /// </summary>
        public void ActualizarStock(int inventarioId, int cantidadNueva)
        {
            string query = @"
                UPDATE Inventario 
                SET Stock = @CantidadNueva
                WHERE Id = @InventarioId";

            DatabaseManager.ExecuteNonQuery(query,
                new SqlParameter("@InventarioId", inventarioId),
                new SqlParameter("@CantidadNueva", cantidadNueva));
        }

        /// <summary>
        /// Obtiene productos con stock bajo
        /// </summary>
        public DataTable ObtenerStockBajo()
        {
            string query = "SELECT * FROM vw_StockBajo";
            return DatabaseManager.ExecuteQuery(query);
        }

        /// <summary>
        /// Elimina un producto del inventario (eliminación lógica)
        /// </summary>
        public void EliminarProducto(int inventarioId)
        {
            string query = @"UPDATE Inventario 
                           SET Stock = 0
                           WHERE Id = @Id";

            DatabaseManager.ExecuteNonQuery(query, new SqlParameter("@Id", inventarioId));
        }

        /// <summary>
        /// Obtiene un producto por su Id
        /// </summary>
        public ProductoInventario ObtenerProductoPorId(int id)
        {
            string query = @"SELECT TOP 1 * FROM Inventario WHERE Id = @Id";
            DataTable dt = DatabaseManager.ExecuteQuery(query, new SqlParameter("@Id", id));
            if (dt.Rows.Count == 0) return null;
            DataRow r = dt.Rows[0];
            return new ProductoInventario
            {
                Id = id,
                CategoriaId = Convert.ToInt32(r["CategoriaId"]),
                Nombre = r["Nombre"].ToString(),
                Descripcion = r["Descripcion"] == DBNull.Value ? null : r["Descripcion"].ToString(),
                CodigoBarra = r.Table.Columns.Contains("CodigoBarras") && r["CodigoBarras"] != DBNull.Value ? r["CodigoBarras"].ToString() : null,
                TalleId = r.Table.Columns.Contains("TalleId") && r["TalleId"] != DBNull.Value ? (int?)Convert.ToInt32(r["TalleId"]) : null,
                Sabor = r.Table.Columns.Contains("Sabor") && r["Sabor"] != DBNull.Value ? r["Sabor"].ToString() : null,
                Cantidad = r.Table.Columns.Contains("Stock") ? Convert.ToInt32(r["Stock"]) : 0,
                PrecioVenta = r.Table.Columns.Contains("PrecioVenta") ? (r["PrecioVenta"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(r["PrecioVenta"]))
                              : (r.Table.Columns.Contains("Precio") ? (decimal?)Convert.ToDecimal(r["Precio"]) : null),
                PrecioCosto = r.Table.Columns.Contains("PrecioCompra") && r["PrecioCompra"] != DBNull.Value ? (decimal?)Convert.ToDecimal(r["PrecioCompra"]) : null,
                FechaCreacion = r.Table.Columns.Contains("FechaCreacion") && r["FechaCreacion"] != DBNull.Value ? Convert.ToDateTime(r["FechaCreacion"]) : DateTime.MinValue,
                FechaModificacion = r.Table.Columns.Contains("FechaModificacion") && r["FechaModificacion"] != DBNull.Value ? Convert.ToDateTime(r["FechaModificacion"]) : DateTime.MinValue,
                Activo = r.Table.Columns.Contains("Activo") && r["Activo"] != DBNull.Value ? Convert.ToBoolean(r["Activo"]) : true
            };
        }

        /// <summary>
        /// Actualiza un producto (sin afectar stock si no se pasa cambio explícito)
        /// </summary>
        public void ActualizarProducto(ProductoInventario producto, bool actualizarStock = true)
        {
            // Construimos query adaptando a columnas existentes: algunos esquemas usan PrecioVenta/PrecioCompra, otros Precio
            bool tienePrecioVenta = DatabaseManager.ColumnExists("Inventario", "PrecioVenta");
            bool tienePrecioCompra = DatabaseManager.ColumnExists("Inventario", "PrecioCompra");
            bool tienePrecioSimple = DatabaseManager.ColumnExists("Inventario", "Precio");
            bool tieneSabor = DatabaseManager.ColumnExists("Inventario", "Sabor");
            bool tieneTalle = DatabaseManager.ColumnExists("Inventario", "TalleId");

            var setParts = new List<string> {
                "CategoriaId = @CategoriaId",
                "Nombre = @Nombre",
                "Descripcion = @Descripcion",
                "CodigoBarras = @CodigoBarras"
            };
            if (tieneTalle) setParts.Add("TalleId = @TalleId");
            if (tieneSabor) setParts.Add("Sabor = @Sabor");
            if (actualizarStock) setParts.Add("Stock = @Stock");
            if (tienePrecioVenta) setParts.Add("PrecioVenta = @PrecioVenta");
            if (tienePrecioCompra) setParts.Add("PrecioCompra = @PrecioCompra");
            if (tienePrecioSimple) setParts.Add("Precio = @PrecioSimple");
            setParts.Add("FechaModificacion = GETDATE()");

            string query = "UPDATE Inventario SET " + string.Join(", ", setParts) + " WHERE Id = @Id";

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@Id", producto.Id),
                new SqlParameter("@CategoriaId", producto.CategoriaId),
                new SqlParameter("@Nombre", producto.Nombre),
                new SqlParameter("@Descripcion", (object)producto.Descripcion ?? DBNull.Value),
                new SqlParameter("@CodigoBarras", (object)producto.CodigoBarra ?? DBNull.Value)
            };
            if (tieneTalle) parametros.Add(new SqlParameter("@TalleId", (object)producto.TalleId ?? DBNull.Value));
            if (tieneSabor) parametros.Add(new SqlParameter("@Sabor", (object)producto.Sabor ?? DBNull.Value));
            if (actualizarStock) parametros.Add(new SqlParameter("@Stock", producto.Cantidad));
            if (tienePrecioVenta) parametros.Add(new SqlParameter("@PrecioVenta", (object)producto.PrecioVenta ?? DBNull.Value));
            if (tienePrecioCompra) parametros.Add(new SqlParameter("@PrecioCompra", (object)producto.PrecioCosto ?? DBNull.Value));
            if (tienePrecioSimple)
            {
                decimal? precioSimple = producto.PrecioVenta ?? producto.PrecioCosto; // fallback
                parametros.Add(new SqlParameter("@PrecioSimple", (object)precioSimple ?? DBNull.Value));
            }

            DatabaseManager.ExecuteNonQuery(query, parametros.ToArray());
        }

        #endregion
    }

    #region Clases de Modelo

    /// <summary>
    /// Representa una categoría de productos
    /// </summary>
    public class Categoria
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public bool TieneTalle { get; set; }
        public string TipoTalle { get; set; }
    }

    /// <summary>
    /// Representa un talle disponible para una categoría
    /// </summary>
    public class TallePorCategoria
    {
        public int Id { get; set; }
        public string TalleValor { get; set; }
        public int Orden { get; set; }
    }

    /// <summary>
    /// Representa un producto en el inventario
    /// </summary>
    public class ProductoInventario
    {
        public int Id { get; set; }
        public int CategoriaId { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string CodigoBarra { get; set; }
        public int? TalleId { get; set; }
        public string Sabor { get; set; } // Para vapers
        public int Cantidad { get; set; }
        public decimal? PrecioVenta { get; set; }
        public decimal? PrecioCosto { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }
        public bool Activo { get; set; }
    }

    #endregion
}
