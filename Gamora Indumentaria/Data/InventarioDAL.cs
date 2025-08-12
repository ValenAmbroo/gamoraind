using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

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
                    categorias.Add(new Categoria
                    {
                        Id = Convert.ToInt32(row["Id"]),
                        Nombre = row["Nombre"].ToString()
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
                INSERT INTO Inventario (CategoriaId, Nombre, Descripcion, CodigoBarras, Stock, Precio, FechaCreacion)
                VALUES (@CategoriaId, @Nombre, @Descripcion, @CodigoBarras, @Stock, @Precio, GETDATE());
                SELECT SCOPE_IDENTITY();";

            object result = DatabaseManager.ExecuteScalar(query,
                new SqlParameter("@CategoriaId", producto.CategoriaId),
                new SqlParameter("@Nombre", producto.Nombre),
                new SqlParameter("@Descripcion", (object)producto.Descripcion ?? DBNull.Value),
                new SqlParameter("@CodigoBarras", (object)producto.CodigoBarra ?? DBNull.Value),
                new SqlParameter("@Stock", producto.Cantidad),
                new SqlParameter("@Precio", (object)producto.PrecioVenta ?? DBNull.Value));

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
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }
        public bool Activo { get; set; }
    }

    #endregion
}
