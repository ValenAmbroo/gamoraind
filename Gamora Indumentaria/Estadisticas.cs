using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using Gamora_Indumentaria.Data;

namespace Gamora_Indumentaria
{
    public partial class Estadisticas : Form
    {
        public Estadisticas()
        {
            try
            {
                InitializeComponent();
                DatabaseManager.InitializeDatabase(); // Usar el manager centralizado
                // No cargar estadísticas automáticamente para evitar errores
                // CargarEstadisticas();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al inicializar estadísticas: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarEstadisticas()
        {
            try
            {
                CargarEstadisticasGenerales();
                CargarTopCategorias();
                CargarStockBajo();
                CargarProductosSinStock();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar estadísticas: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarEstadisticasGenerales()
        {
            try
            {
                // Total de productos
                object totalProductos = DatabaseManager.ExecuteScalar("SELECT COUNT(*) FROM Inventario");
                if (lblTotalProductos != null)
                    lblTotalProductos.Text = totalProductos.ToString();

                // Total de unidades en stock
                object totalUnidades = DatabaseManager.ExecuteScalar("SELECT ISNULL(SUM(Stock), 0) FROM Inventario");
                if (lblTotalUnidades != null)
                    lblTotalUnidades.Text = totalUnidades.ToString();

                // Total de categorías
                object totalCategorias = DatabaseManager.ExecuteScalar("SELECT COUNT(*) FROM Categorias");
                if (lblTotalCategorias != null)
                    lblTotalCategorias.Text = totalCategorias.ToString();

                // Productos con stock bajo
                object stockBajo = DatabaseManager.ExecuteScalar("SELECT COUNT(*) FROM Inventario WHERE Stock <= 5");
                if (lblStockBajo != null)
                    lblStockBajo.Text = stockBajo.ToString();

                // Valor total del inventario (adaptar a columna existente)
                string precioCol = DatabaseManager.ColumnExists("Inventario", "PrecioVenta") ? "PrecioVenta" :
                                   (DatabaseManager.ColumnExists("Inventario", "Precio") ? "Precio" : null);
                object valorTotal = 0m;
                if (precioCol != null)
                {
                    string sqlValor = $"SELECT ISNULL(SUM(Stock * {precioCol}), 0) FROM Inventario";
                    valorTotal = DatabaseManager.ExecuteScalar(sqlValor);
                }
                if (lblValorTotal != null)
                    lblValorTotal.Text = "$" + Convert.ToDecimal(valorTotal).ToString("N2");

                // Productos sin stock
                object sinStock = DatabaseManager.ExecuteScalar("SELECT COUNT(*) FROM Inventario WHERE Stock = 0");
                if (lblSinStock != null)
                    lblSinStock.Text = sinStock.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar estadísticas generales: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarTopCategorias()
        {
            try
            {
                string query = @"
                    SELECT 
                        c.Nombre AS Categoria,
                        COUNT(i.Id) AS CantidadProductos,
                        SUM(i.Stock) AS TotalUnidades,
                        AVG(CAST(i.Stock AS FLOAT)) AS PromedioStock
                    FROM Categorias c
                    LEFT JOIN Inventario i ON c.Id = i.CategoriaId
                    GROUP BY c.Nombre
                    ORDER BY TotalUnidades DESC";

                DataTable dt = DatabaseManager.ExecuteQuery(query);

                // Formatear la columna de promedio
                foreach (DataRow row in dt.Rows)
                {
                    if (row["PromedioStock"] != DBNull.Value)
                    {
                        row["PromedioStock"] = Math.Round(Convert.ToDouble(row["PromedioStock"]), 2);
                    }
                }

                if (dgvTopCategorias != null)
                {
                    dgvTopCategorias.DataSource = dt;

                    // Configurar columnas
                    if (dgvTopCategorias.Columns.Count > 0)
                    {
                        dgvTopCategorias.Columns["Categoria"].HeaderText = "Categoría";
                        dgvTopCategorias.Columns["CantidadProductos"].HeaderText = "Productos";
                        dgvTopCategorias.Columns["TotalUnidades"].HeaderText = "Unidades";
                        dgvTopCategorias.Columns["PromedioStock"].HeaderText = "Promedio Stock";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar top categorías: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarStockBajo()
        {
            try
            {
                bool tienePrecioVenta = DatabaseManager.ColumnExists("Inventario", "PrecioVenta");
                bool tienePrecioLegacy = !tienePrecioVenta && DatabaseManager.ColumnExists("Inventario", "Precio");
                string exprPrecio = tienePrecioVenta ? "i.PrecioVenta" : (tienePrecioLegacy ? "i.Precio" : "0");
                string query = $@"
                    SELECT 
                        c.Nombre AS Categoria,
                        i.Nombre AS Producto,
                        i.Stock,
                        {exprPrecio} AS Precio
                    FROM Inventario i
                    INNER JOIN Categorias c ON i.CategoriaId = c.Id
                    WHERE i.Stock <= 5 AND i.Stock > 0
                    ORDER BY i.Stock ASC";

                DataTable dt = DatabaseManager.ExecuteQuery(query);

                if (dgvStockBajo != null)
                {
                    dgvStockBajo.DataSource = dt;

                    // Configurar columnas
                    if (dgvStockBajo.Columns.Count > 0)
                    {
                        dgvStockBajo.Columns["Categoria"].HeaderText = "Categoría";
                        dgvStockBajo.Columns["Producto"].HeaderText = "Producto";
                        dgvStockBajo.Columns["Stock"].HeaderText = "Stock";
                        if (dgvStockBajo.Columns.Contains("Precio"))
                        {
                            dgvStockBajo.Columns["Precio"].HeaderText = "Precio";
                            dgvStockBajo.Columns["Precio"].DefaultCellStyle.Format = "C2";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar productos con stock bajo: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarProductosSinStock()
        {
            try
            {
                string query = @"
                    SELECT 
                        c.Nombre AS Categoria,
                        i.Nombre AS Producto,
                        i.FechaCreacion AS FechaCreacion
                    FROM Inventario i
                    INNER JOIN Categorias c ON i.CategoriaId = c.Id
                    WHERE i.Stock = 0
                    ORDER BY i.FechaCreacion DESC";

                DataTable dt = DatabaseManager.ExecuteQuery(query);

                if (dgvSinStock != null)
                {
                    dgvSinStock.DataSource = dt;

                    // Configurar columnas
                    if (dgvSinStock.Columns.Count > 0)
                    {
                        dgvSinStock.Columns["Categoria"].HeaderText = "Categoría";
                        dgvSinStock.Columns["Producto"].HeaderText = "Producto";
                        dgvSinStock.Columns["FechaCreacion"].HeaderText = "Fecha Creación";
                        dgvSinStock.Columns["FechaCreacion"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar productos sin stock: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            CargarEstadisticas();
            MessageBox.Show("Estadísticas actualizadas correctamente", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnExportar_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.Filter = "Archivo CSV|*.csv";
                saveDialog.Title = "Exportar Estadísticas";
                saveDialog.FileName = "Estadisticas_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    ExportarACSV(saveDialog.FileName);
                    MessageBox.Show("Estadísticas exportadas exitosamente", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al exportar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExportarACSV(string fileName)
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(fileName))
            {
                file.WriteLine("ESTADÍSTICAS GENERALES - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
                file.WriteLine("========================================");
                file.WriteLine("Total Productos," + lblTotalProductos.Text);
                file.WriteLine("Total Unidades," + lblTotalUnidades.Text);
                file.WriteLine("Total Categorías," + lblTotalCategorias.Text);
                file.WriteLine("Stock Bajo," + lblStockBajo.Text);
                file.WriteLine("Sin Stock," + lblSinStock.Text);
                file.WriteLine("Valor Total," + lblValorTotal.Text);
                file.WriteLine("");

                file.WriteLine("TOP CATEGORÍAS");
                file.WriteLine("========================================");
                file.WriteLine("Categoría,Productos,Unidades,Promedio Stock");
                foreach (DataGridViewRow row in dgvTopCategorias.Rows)
                {
                    if (row.Cells[0].Value != null)
                    {
                        file.WriteLine(string.Format("{0},{1},{2},{3}",
                            row.Cells[0].Value,
                            row.Cells[1].Value,
                            row.Cells[2].Value,
                            row.Cells[3].Value));
                    }
                }

                file.WriteLine("");
                file.WriteLine("PRODUCTOS CON STOCK BAJO");
                file.WriteLine("========================================");
                file.WriteLine("Categoría,Producto,Talle,Stock,Precio");
                foreach (DataGridViewRow row in dgvStockBajo.Rows)
                {
                    if (row.Cells[0].Value != null)
                    {
                        file.WriteLine(string.Format("{0},{1},{2},{3},{4}",
                            row.Cells[0].Value,
                            row.Cells[1].Value,
                            row.Cells[2].Value,
                            row.Cells[3].Value,
                            row.Cells[4].Value));
                    }
                }
            }
        }

        private void Estadisticas_Load(object sender, EventArgs e)
        {
            // Configurar el formulario
            this.WindowState = FormWindowState.Normal; // Cambiar a Normal en lugar de Maximized

            // Cargar estadísticas cuando el formulario esté completamente cargado
            try
            {
                CargarEstadisticas();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar estadísticas iniciales: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
