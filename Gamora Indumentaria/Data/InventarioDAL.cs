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

                // Obtener ids de categorías que tengan talles definidos en TiposTalle
                var categoriasConTalleIds = new HashSet<int>();
                try
                {
                    DataTable dtT = DatabaseManager.ExecuteQuery("SELECT DISTINCT CategoriaId FROM TiposTalle WHERE CategoriaId IS NOT NULL");
                    foreach (DataRow r in dtT.Rows)
                    {
                        if (r["CategoriaId"] != DBNull.Value)
                        {
                            categoriasConTalleIds.Add(Convert.ToInt32(r["CategoriaId"]));
                        }
                    }
                }
                catch { /* si falla la comprobación, caer de nuevo a heurística */ }

                foreach (DataRow row in dt.Rows)
                {
                    string nombre = row["Nombre"].ToString();
                    int id = Convert.ToInt32(row["Id"]);
                    // Determinar si la categoría posee talles en la BD; si no, usar heurística
                    bool tieneTalle = categoriasConTalleIds.Contains(id) || EsCategoriaConTalle(nombre);
                    string tipoTalle = InferirTipoTalle(nombre);
                    categorias.Add(new Categoria
                    {
                        Id = id,
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

        /// <summary>
        /// Actualiza el nombre de una categoría
        /// </summary>
        public void ActualizarCategoria(int id, string nuevoNombre)
        {
            if (id <= 0) throw new ArgumentException("Id inválido", nameof(id));
            if (string.IsNullOrWhiteSpace(nuevoNombre)) throw new ArgumentException("Nombre requerido", nameof(nuevoNombre));
            try
            {
                string query = "UPDATE Categorias SET Nombre = @Nombre WHERE Id = @Id";
                DatabaseManager.ExecuteNonQuery(query,
                    new SqlParameter("@Nombre", nuevoNombre.Trim()),
                    new SqlParameter("@Id", id));
            }
            catch (Exception ex)
            {
                throw new Exception("Error al actualizar categoría: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Indica si una categoría tiene productos asociados en Inventario
        /// </summary>
        public bool CategoriaTieneProductos(int categoriaId)
        {
            string query = "SELECT TOP 1 1 FROM Inventario WHERE CategoriaId = @Id";
            var r = DatabaseManager.ExecuteScalar(query, new SqlParameter("@Id", categoriaId));
            return r != null;
        }

        /// <summary>
        /// Devuelve la cantidad de productos asociados a una categoría.
        /// </summary>
        public int ContarProductosPorCategoria(int categoriaId)
        {
            string query = "SELECT COUNT(1) FROM Inventario WHERE CategoriaId = @Id";
            var r = DatabaseManager.ExecuteScalar(query, new SqlParameter("@Id", categoriaId));
            return r == null ? 0 : Convert.ToInt32(r);
        }

        /// <summary>
        /// Elimina una categoría. Lógica actualizada:
        /// - Productos sin ventas asociadas: borrado físico.
        /// - Productos con ventas asociadas: se desasocian (CategoriaId = NULL) y se marca Activo = 0 (baja lógica) manteniendo su stock histórico.
        /// - Luego se eliminan los talles y la categoría.
        /// Esto evita errores por FKs en DetalleVentas y cumple con el requerimiento de eliminar la categoría sin mensaje de bloqueo.
        /// </summary>
        public void EliminarCategoria(int id)
        {
            if (id <= 0) throw new ArgumentException("Id inválido", nameof(id));
            try
            {
                using (var conn = DatabaseManager.GetConnection())
                {
                    conn.Open();
                    using (var tx = conn.BeginTransaction())
                    {
                        // 1. Eliminar físicamente productos SIN ventas
                        string deleteProductosSinVentas = @"
                            DELETE i FROM Inventario i
                            LEFT JOIN DetalleVentas d ON d.ProductoId = i.Id
                            WHERE i.CategoriaId = @CatId AND d.ProductoId IS NULL";
                        using (var cmdDel = new SqlCommand(deleteProductosSinVentas, conn, tx))
                        {
                            cmdDel.Parameters.AddWithValue("@CatId", id);
                            cmdDel.ExecuteNonQuery();
                        }

                        // 2. Desasociar productos CON ventas (mantener registro para integridad)
                        string updateProductosConVentas = @"
                            UPDATE Inventario SET CategoriaId = NULL, Activo = 0
                            WHERE CategoriaId = @CatId AND Id IN (
                                SELECT DISTINCT ProductoId FROM DetalleVentas WHERE ProductoId IN (
                                    SELECT Id FROM Inventario WHERE CategoriaId = @CatId
                                )
                            )";
                        using (var cmdUpd = new SqlCommand(updateProductosConVentas, conn, tx))
                        {
                            cmdUpd.Parameters.AddWithValue("@CatId", id);
                            cmdUpd.ExecuteNonQuery();
                        }

                        // 3. Borrar talles asociados
                        using (var cmdT = new SqlCommand("DELETE FROM TiposTalle WHERE CategoriaId = @Id", conn, tx))
                        {
                            cmdT.Parameters.AddWithValue("@Id", id);
                            cmdT.ExecuteNonQuery();
                        }

                        // 4. Borrar categoría
                        using (var cmdC = new SqlCommand("DELETE FROM Categorias WHERE Id = @Id", conn, tx))
                        {
                            cmdC.Parameters.AddWithValue("@Id", id);
                            int rows = cmdC.ExecuteNonQuery();
                            if (rows == 0) throw new Exception("Categoría no encontrada");
                        }

                        tx.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al eliminar categoría: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Reemplaza completamente los talles de una categoría por una nueva lista
        /// </summary>
        public void ReemplazarTalles(int categoriaId, List<string> talles)
        {
            try
            {
                using (var conn = DatabaseManager.GetConnection())
                {
                    conn.Open();
                    using (var tx = conn.BeginTransaction())
                    {
                        using (var del = new SqlCommand("DELETE FROM TiposTalle WHERE CategoriaId = @Id", conn, tx))
                        {
                            del.Parameters.AddWithValue("@Id", categoriaId);
                            del.ExecuteNonQuery();
                        }
                        if (talles != null && talles.Count > 0)
                        {
                            int orden = 1;
                            foreach (var t in talles)
                            {
                                var tv = (t ?? string.Empty).Trim();
                                if (string.IsNullOrEmpty(tv)) { orden++; continue; }
                                using (var ins = new SqlCommand("INSERT INTO TiposTalle (CategoriaId, TalleValor, Orden) VALUES (@c,@v,@o)", conn, tx))
                                {
                                    ins.Parameters.AddWithValue("@c", categoriaId);
                                    ins.Parameters.AddWithValue("@v", tv);
                                    ins.Parameters.AddWithValue("@o", orden);
                                    ins.ExecuteNonQuery();
                                }
                                orden++;
                            }
                        }
                        tx.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al reemplazar talles: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Agrega una lista de talles a una categoría (si no existen ya)
        /// </summary>
        public void AgregarTalles(int categoriaId, List<string> talles)
        {
            if (talles == null || talles.Count == 0) return;
            try
            {
                using (var conn = DatabaseManager.GetConnection())
                {
                    conn.Open();
                    int orden = 1;
                    foreach (var t in talles)
                    {
                        var tv = (t ?? string.Empty).Trim();
                        if (string.IsNullOrEmpty(tv)) { orden++; continue; }
                        // comprobar si ya existe
                        string check = "SELECT 1 FROM TiposTalle WHERE CategoriaId = @c AND TalleValor = @tv";
                        using (var cmdCheck = new SqlCommand(check, conn))
                        {
                            cmdCheck.Parameters.AddWithValue("@c", categoriaId);
                            cmdCheck.Parameters.AddWithValue("@tv", tv);
                            var exists = cmdCheck.ExecuteScalar();
                            if (exists == null)
                            {
                                string insert = "INSERT INTO TiposTalle (CategoriaId, TalleValor, Orden) VALUES (@c,@tv,@ord)";
                                using (var cmdIns = new SqlCommand(insert, conn))
                                {
                                    cmdIns.Parameters.AddWithValue("@c", categoriaId);
                                    cmdIns.Parameters.AddWithValue("@tv", tv);
                                    cmdIns.Parameters.AddWithValue("@ord", orden);
                                    cmdIns.ExecuteNonQuery();
                                }
                            }
                        }
                        orden++;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error al agregar talles: {0}", ex.Message), ex);
            }
        }

        // Determina si una categoría maneja talles (heurística basada en nombre)
        private bool EsCategoriaConTalle(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre)) return false;
            nombre = nombre.Trim().ToUpperInvariant();
            // Ajusta aquí según tus categorías reales
            string[] categoriasConTalle = { "REMERAS", "PANTALONES", "VESTIDOS", "CALZADO", "BUZOS", "CAMPERAS", "CHALECOS", "JOGGING", "JEANS HOMBRE", "JEANS DAMA", "BOXER" };
            return categoriasConTalle.Contains(nombre);
        }

        // Inferir tipo de talle (alfanumérico vs numérico) para uso futuro
        private string InferirTipoTalle(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre)) return null;
            nombre = nombre.Trim().ToUpperInvariant();
            // Ejemplo simple: Calzado suele ser numérico, Jeans suelen ser numéricos, resto alfanumérico
            if (nombre == "CALZADO") return "NUM";
            if (nombre.Contains("JEANS")) return "NUM";
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
        /// Elimina un producto del inventario.
        /// Retorna true si el borrado fue físico (sin ventas asociadas).
        /// Retorna false si se realizó baja lógica (con ventas asociadas: Stock=0).
        /// </summary>
        public bool EliminarProducto(int inventarioId)
        {
            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();
                // Verificar referencias en DetalleVentas
                using (var cmdCheck = new SqlCommand("SELECT TOP 1 1 FROM DetalleVentas WHERE ProductoId = @Id", conn))
                {
                    cmdCheck.Parameters.AddWithValue("@Id", inventarioId);
                    var exists = cmdCheck.ExecuteScalar();
                    if (exists == null)
                    {
                        // Borrado físico
                        using (var del = new SqlCommand("DELETE FROM Inventario WHERE Id=@Id", conn))
                        {
                            del.Parameters.AddWithValue("@Id", inventarioId);
                            del.ExecuteNonQuery();
                        }
                        return true;
                    }
                    else
                    {
                        // Baja lógica: marcar Activo = 0 (mantener stock como estaba para referencia histórica) opcionalmente podría ponerse a 0
                        using (var upd = new SqlCommand("UPDATE Inventario SET Activo = 0 WHERE Id=@Id", conn))
                        {
                            upd.Parameters.AddWithValue("@Id", inventarioId);
                            upd.ExecuteNonQuery();
                        }
                        return false;
                    }
                }
            }
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
            // Incluir FechaModificacion solo si la columna existe en la tabla Inventario
            if (DatabaseManager.ColumnExists("Inventario", "FechaModificacion"))
            {
                setParts.Add("FechaModificacion = GETDATE()");
            }

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
