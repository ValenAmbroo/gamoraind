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
        private readonly string connectionString;

        public InventarioDAL()
        {
            connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\VentasDB.mdf;Integrated Security=True;";
        }

        #region Categorías

        /// <summary>
        /// Obtiene todas las categorías disponibles
        /// </summary>
        public List<Categoria> ObtenerCategorias()
        {
            List<Categoria> categorias = new List<Categoria>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT Id, Nombre, TieneTalle, TipoTalle FROM Categorias ORDER BY Nombre";
                SqlCommand cmd = new SqlCommand(query, con);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    categorias.Add(new Categoria
                    {
                        Id = (int)reader["Id"],
                        Nombre = reader["Nombre"].ToString(),
                        TieneTalle = (bool)reader["TieneTalle"],
                        TipoTalle = reader["TipoTalle"].ToString()
                    });
                }
            }

            return categorias;
        }

        /// <summary>
        /// Obtiene los talles disponibles para una categoría específica
        /// </summary>
        public List<TallePorCategoria> ObtenerTallesPorCategoria(int categoriaId)
        {
            List<TallePorCategoria> talles = new List<TallePorCategoria>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"SELECT Id, TalleValor, Orden 
                               FROM TiposTalle 
                               WHERE CategoriaId = @CategoriaId 
                               ORDER BY Orden";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@CategoriaId", categoriaId);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    talles.Add(new TallePorCategoria
                    {
                        Id = (int)reader["Id"],
                        TalleValor = reader["TalleValor"].ToString(),
                        Orden = (int)reader["Orden"]
                    });
                }
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
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM vw_InventarioCompleto ORDER BY Categoria, Producto, Talle";
                SqlDataAdapter da = new SqlDataAdapter(query, con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        /// <summary>
        /// Obtiene productos por categoría
        /// </summary>
        public DataTable ObtenerInventarioPorCategoria(string categoria)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"SELECT * FROM vw_InventarioCompleto 
                               WHERE Categoria = @Categoria 
                               ORDER BY Producto, Talle";

                SqlDataAdapter da = new SqlDataAdapter(query, con);
                da.SelectCommand.Parameters.AddWithValue("@Categoria", categoria);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        /// <summary>
        /// Agrega un nuevo producto al inventario
        /// </summary>
        public int AgregarProducto(ProductoInventario producto)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("sp_AgregarProducto", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@CategoriaId", producto.CategoriaId);
                cmd.Parameters.AddWithValue("@Nombre", producto.Nombre);
                cmd.Parameters.AddWithValue("@Descripcion", (object)producto.Descripcion ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@CodigoBarra", (object)producto.CodigoBarra ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@TalleId", (object)producto.TalleId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Sabor", (object)producto.Sabor ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Cantidad", producto.Cantidad);
                cmd.Parameters.AddWithValue("@PrecioVenta", (object)producto.PrecioVenta ?? DBNull.Value);

                object result = cmd.ExecuteScalar();
                return Convert.ToInt32(result);
            }
        }

        /// <summary>
        /// Actualiza el stock de un producto
        /// </summary>
        public void ActualizarStock(int inventarioId, int cantidadNueva)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("sp_ActualizarStock", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@InventarioId", inventarioId);
                cmd.Parameters.AddWithValue("@CantidadNueva", cantidadNueva);

                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Obtiene productos con stock bajo
        /// </summary>
        public DataTable ObtenerStockBajo()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM vw_StockBajo";
                SqlDataAdapter da = new SqlDataAdapter(query, con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        /// <summary>
        /// Elimina un producto del inventario (eliminación lógica)
        /// </summary>
        public void EliminarProducto(int inventarioId)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string query = @"UPDATE Inventario 
                               SET Activo = 0, FechaModificacion = GETDATE() 
                               WHERE Id = @Id";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Id", inventarioId);
                cmd.ExecuteNonQuery();
            }
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
