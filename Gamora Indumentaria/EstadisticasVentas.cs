using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Gamora_Indumentaria.Data;

namespace Gamora_Indumentaria
{
    public partial class EstadisticasVentas : Form
    {
        private DateTime fechaDesde;
        private DateTime fechaHasta;
        public EstadisticasVentas()
        {
            try
            {
                InitializeComponent();
                DatabaseManager.InitializeDatabase(); // Usar el manager centralizado
                fechaHasta = DateTime.Today.AddDays(1); // exclusivo
                fechaDesde = fechaHasta.AddDays(-30);
                ConfigurarFiltros();
                CargarEstadisticasVentas("Hoy");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al inicializar formulario de estadísticas de ventas: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConfigurarFiltros()
        {
            if (cmbFiltroTiempo != null)
            {
                cmbFiltroTiempo.Items.Clear();
                cmbFiltroTiempo.Items.Add("Hoy");
                cmbFiltroTiempo.Items.Add("Esta Semana");
                cmbFiltroTiempo.Items.Add("Este Mes");
                cmbFiltroTiempo.Items.Add("Últimos 7 días");
                cmbFiltroTiempo.Items.Add("Últimos 30 días");
                cmbFiltroTiempo.SelectedIndex = 0;
            }

            ConfigurarGraficos();
        }

        private void ConfigurarGraficos()
        {
            ConfigurarGraficoVentasTiempo();
            ConfigurarGraficoProductos();
            ConfigurarGraficoCategorias();
            ConfigurarGraficoTendencia();
        }

        private void ConfigurarGraficoVentasTiempo()
        {
            if (chartVentasTiempo == null) return;

            chartVentasTiempo.Series.Clear();
            chartVentasTiempo.ChartAreas.Clear();

            ChartArea area = new ChartArea("VentasArea");
            area.AxisX.Title = "Período";
            area.AxisY.Title = "Ventas ($)";
            area.BackColor = Color.White;
            chartVentasTiempo.ChartAreas.Add(area);

            Series serie = new Series("Ventas");
            serie.ChartType = SeriesChartType.Column;
            serie.Color = Color.FromArgb(52, 152, 219);
            chartVentasTiempo.Series.Add(serie);

            chartVentasTiempo.Titles.Clear();
            chartVentasTiempo.Titles.Add("📈 Ventas por Tiempo");
        }

        private void ConfigurarGraficoProductos()
        {
            if (chartProductosVendidos == null) return;

            chartProductosVendidos.Series.Clear();
            chartProductosVendidos.ChartAreas.Clear();

            ChartArea area = new ChartArea("ProductosArea");
            area.AxisX.Title = "Cantidad";
            area.AxisY.Title = "Productos";
            area.BackColor = Color.White;
            chartProductosVendidos.ChartAreas.Add(area);

            Series serie = new Series("Productos");
            serie.ChartType = SeriesChartType.Bar;
            serie.IsValueShownAsLabel = true;
            chartProductosVendidos.Series.Add(serie);

            chartProductosVendidos.Titles.Clear();
            chartProductosVendidos.Titles.Add("🏆 Productos Más Vendidos");
        }

        private void ConfigurarGraficoCategorias()
        {
            if (chartCategorias == null) return;

            chartCategorias.Series.Clear();
            chartCategorias.ChartAreas.Clear();

            ChartArea area = new ChartArea("CategoriasArea");
            area.BackColor = Color.White;
            chartCategorias.ChartAreas.Add(area);

            Series serie = new Series("Categorías");
            serie.ChartType = SeriesChartType.Pie;
            serie.IsValueShownAsLabel = true;
            serie.LabelFormat = "{P1}";
            chartCategorias.Series.Add(serie);

            chartCategorias.Titles.Clear();
            chartCategorias.Titles.Add("🎯 Ventas por Categoría");

            Legend legend = new Legend();
            legend.Docking = Docking.Right;
            chartCategorias.Legends.Add(legend);
        }

        private void ConfigurarGraficoTendencia()
        {
            if (chartTendenciaMensual == null) return;

            chartTendenciaMensual.Series.Clear();
            chartTendenciaMensual.ChartAreas.Clear();

            ChartArea area = new ChartArea("TendenciaArea");
            area.AxisX.Title = "Mes";
            area.AxisY.Title = "Ventas ($)";
            area.BackColor = Color.White;
            chartTendenciaMensual.ChartAreas.Add(area);

            Series serie = new Series("Tendencia");
            serie.ChartType = SeriesChartType.Line;
            serie.Color = Color.FromArgb(46, 204, 113);
            serie.BorderWidth = 3;
            serie.MarkerStyle = MarkerStyle.Circle;
            serie.MarkerSize = 8;
            chartTendenciaMensual.Series.Add(serie);

            chartTendenciaMensual.Titles.Clear();
            chartTendenciaMensual.Titles.Add("📈 Tendencia Mensual");
        }

        private void CargarEstadisticasVentas(string periodo)
        {
            try
            {
                CargarVentasPorTiempo(periodo);
                CargarProductosMasVendidos();
                CargarVentasPorCategoria();
                CargarTendenciaMensual();
                CargarResumenVentas();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar estadísticas: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarVentasPorTiempo(string periodo)
        {
            if (chartVentasTiempo == null) return;

            chartVentasTiempo.Series["Ventas"].Points.Clear();

            try
            {
                string query = "";
                string formatoFecha = "";

                switch (periodo.ToLower())
                {
                    case "hoy":
                        formatoFecha = "FORMAT(FechaVenta, 'HH:00') + 'h'";
                        query = string.Format(@"
                            SELECT {0} as Periodo, 
                                   SUM(Total) as TotalVentas
                            FROM Ventas 
                            WHERE CAST(FechaVenta AS DATE) = CAST(GETDATE() AS DATE)
                            GROUP BY {0}, DATEPART(HOUR, FechaVenta)
                            ORDER BY DATEPART(HOUR, FechaVenta)", formatoFecha);
                        break;

                    case "esta semana":
                        formatoFecha = "FORMAT(FechaVenta, 'ddd')";
                        query = string.Format(@"
                            SELECT {0} as Periodo, 
                                   SUM(Total) as TotalVentas
                            FROM Ventas 
                            WHERE FechaVenta >= DATEADD(WEEK, DATEDIFF(WEEK, 0, GETDATE()), 0) 
                              AND FechaVenta < DATEADD(WEEK, DATEDIFF(WEEK, 0, GETDATE()) + 1, 0)
                            GROUP BY {0}, DATEPART(WEEKDAY, FechaVenta)
                            ORDER BY DATEPART(WEEKDAY, FechaVenta)", formatoFecha);
                        break;

                    case "este mes":
                        formatoFecha = "FORMAT(FechaVenta, 'dd/MM')";
                        query = string.Format(@"
                            SELECT {0} as Periodo, 
                                   SUM(Total) as TotalVentas
                            FROM Ventas 
                            WHERE YEAR(FechaVenta) = YEAR(GETDATE()) 
                              AND MONTH(FechaVenta) = MONTH(GETDATE())
                            GROUP BY {0}, DAY(FechaVenta)
                            ORDER BY DAY(FechaVenta)", formatoFecha);
                        break;

                    case "últimos 7 días":
                        formatoFecha = "FORMAT(FechaVenta, 'dd/MM')";
                        query = string.Format(@"
                            SELECT {0} as Periodo, 
                                   SUM(Total) as TotalVentas
                            FROM Ventas 
                            WHERE FechaVenta >= DATEADD(DAY, -7, GETDATE())
                            GROUP BY {0}, CAST(FechaVenta AS DATE)
                            ORDER BY CAST(FechaVenta AS DATE)", formatoFecha);
                        break;

                    case "últimos 30 días":
                        formatoFecha = "FORMAT(FechaVenta, 'dd/MM')";
                        query = string.Format(@"
                            SELECT {0} as Periodo, 
                                   SUM(Total) as TotalVentas
                            FROM Ventas 
                            WHERE FechaVenta >= DATEADD(DAY, -30, GETDATE())
                            GROUP BY {0}, CAST(FechaVenta AS DATE)
                            ORDER BY CAST(FechaVenta AS DATE)", formatoFecha);
                        break;
                }

                DataTable dt = DatabaseManager.ExecuteQuery(query);

                if (dt.Rows.Count == 0)
                {
                    chartVentasTiempo.Series["Ventas"].Points.AddXY("Sin datos", 0);
                }
                else
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        string periodoTexto = row["Periodo"].ToString();
                        decimal ventas = Convert.ToDecimal(row["TotalVentas"]);
                        chartVentasTiempo.Series["Ventas"].Points.AddXY(periodoTexto, ventas);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar ventas por tiempo: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                // Mostrar datos de ejemplo en caso de error
                chartVentasTiempo.Series["Ventas"].Points.AddXY("Error", 0);
            }
        }

        private void CargarProductosMasVendidos()
        {
            if (chartProductosVendidos == null) return;

            chartProductosVendidos.Series["Productos"].Points.Clear();

            try
            {
                string query = @"
                    SELECT TOP 10 
                        i.Nombre AS Producto,
                        SUM(dv.Cantidad) AS CantidadVendida
                    FROM DetalleVentas dv
                    INNER JOIN Inventario i ON dv.ProductoId = i.Id
                    INNER JOIN Ventas v ON dv.VentaId = v.Id
                    GROUP BY i.Nombre
                    ORDER BY CantidadVendida DESC";

                DataTable dt = DatabaseManager.ExecuteQuery(query);

                Color[] colores = {
                    Color.FromArgb(231, 76, 60),
                    Color.FromArgb(52, 152, 219),
                    Color.FromArgb(46, 204, 113),
                    Color.FromArgb(155, 89, 182),
                    Color.FromArgb(230, 126, 34),
                    Color.FromArgb(241, 196, 15),
                    Color.FromArgb(149, 165, 166),
                    Color.FromArgb(192, 57, 43),
                    Color.FromArgb(41, 128, 185),
                    Color.FromArgb(39, 174, 96)
                };

                if (dt.Rows.Count == 0)
                {
                    chartProductosVendidos.Series["Productos"].Points.AddXY(0, "Sin datos");
                }
                else
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        DataRow row = dt.Rows[i];
                        string producto = row["Producto"].ToString();
                        int cantidad = Convert.ToInt32(row["CantidadVendida"]);

                        int pointIndex = chartProductosVendidos.Series["Productos"].Points.AddXY(cantidad, producto);
                        chartProductosVendidos.Series["Productos"].Points[pointIndex].Color = colores[i % colores.Length];
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar productos más vendidos: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                chartProductosVendidos.Series["Productos"].Points.AddXY(0, "Error al cargar");
            }
        }

        private void CargarVentasPorCategoria()
        {
            if (chartCategorias == null) return;

            chartCategorias.Series["Categorías"].Points.Clear();

            try
            {
                string query = @"
                    SELECT 
                        c.Nombre AS Categoria,
                        SUM(dv.Cantidad * dv.PrecioUnitario) AS TotalVentas
                    FROM DetalleVentas dv
                    INNER JOIN Inventario i ON dv.ProductoId = i.Id
                    INNER JOIN Categorias c ON i.CategoriaId = c.Id
                    GROUP BY c.Nombre
                    ORDER BY TotalVentas DESC";

                DataTable dt = DatabaseManager.ExecuteQuery(query);

                Color[] colores = {
                    Color.FromArgb(52, 152, 219),
                    Color.FromArgb(46, 204, 113),
                    Color.FromArgb(155, 89, 182),
                    Color.FromArgb(230, 126, 34),
                    Color.FromArgb(231, 76, 60),
                    Color.FromArgb(241, 196, 15),
                    Color.FromArgb(149, 165, 166),
                    Color.FromArgb(192, 57, 43)
                };

                if (dt.Rows.Count == 0)
                {
                    chartCategorias.Series["Categorías"].Points.AddXY("Sin datos", 0);
                }
                else
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        DataRow row = dt.Rows[i];
                        string categoria = row["Categoria"].ToString();
                        decimal ventas = Convert.ToDecimal(row["TotalVentas"]);

                        int pointIndex = chartCategorias.Series["Categorías"].Points.AddXY(categoria, ventas);
                        chartCategorias.Series["Categorías"].Points[pointIndex].Color = colores[i % colores.Length];
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar ventas por categoría: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                chartCategorias.Series["Categorías"].Points.AddXY("Error", 0);
            }
        }

        private void CargarTendenciaMensual()
        {
            if (chartTendenciaMensual == null) return;

            chartTendenciaMensual.Series["Tendencia"].Points.Clear();

            try
            {
                string query = @"
                    SELECT 
                        FORMAT(FechaVenta, 'MMM yyyy') AS MesTexto,
                        FORMAT(FechaVenta, 'yyyy-MM') AS MesOrden,
                        SUM(Total) AS TotalVentas
                    FROM Ventas 
                    WHERE FechaVenta >= DATEADD(MONTH, -6, GETDATE())
                    GROUP BY FORMAT(FechaVenta, 'MMM yyyy'), FORMAT(FechaVenta, 'yyyy-MM')
                    ORDER BY MesOrden";

                DataTable dt = DatabaseManager.ExecuteQuery(query);

                if (dt.Rows.Count == 0)
                {
                    chartTendenciaMensual.Series["Tendencia"].Points.AddXY("Sin datos", 0);
                }
                else
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        string mesTexto = row["MesTexto"].ToString();
                        decimal ventas = Convert.ToDecimal(row["TotalVentas"]);
                        chartTendenciaMensual.Series["Tendencia"].Points.AddXY(mesTexto, ventas);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar tendencia mensual: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                chartTendenciaMensual.Series["Tendencia"].Points.AddXY("Error", 0);
            }
        }

        private void CargarResumenVentas()
        {
            try
            {
                DataTable dt = ReporteVentasGenerator.GetResumen(fechaDesde, fechaHasta);

                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    decimal totalVentas = Convert.ToDecimal(row["TotalVentas"]);
                    int totalUnidades = Convert.ToInt32(row["TotalUnidades"]);
                    int totalTransacciones = Convert.ToInt32(row["TotalTransacciones"]);
                    decimal promedioVenta = Convert.ToDecimal(row["PromedioVenta"]);

                    if (lblTotalVentas != null)
                        lblTotalVentas.Text = string.Format("${0:N2}", totalVentas);

                    if (lblTotalUnidadesVendidas != null)
                        lblTotalUnidadesVendidas.Text = totalUnidades.ToString();

                    if (lblTotalTransacciones != null)
                        lblTotalTransacciones.Text = totalTransacciones.ToString();

                    if (lblPromedioVenta != null)
                        lblPromedioVenta.Text = string.Format("${0:N2}", promedioVenta);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar resumen de ventas: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                // Valores por defecto en caso de error
                if (lblTotalVentas != null)
                    lblTotalVentas.Text = "$0.00";

                if (lblTotalUnidadesVendidas != null)
                    lblTotalUnidadesVendidas.Text = "0";

                if (lblTotalTransacciones != null)
                    lblTotalTransacciones.Text = "0";

                if (lblPromedioVenta != null)
                    lblPromedioVenta.Text = "$0.00";
            }
        }

        private void cmbFiltroTiempo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbFiltroTiempo?.SelectedItem != null)
            {
                string periodo = cmbFiltroTiempo.SelectedItem.ToString();
                CargarEstadisticasVentas(periodo);
            }
        }

        private void btnExportarVentas_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog saveDialog = new SaveFileDialog
                {
                    Filter = "CSV files (*.csv)|*.csv",
                    Title = "Exportar Estadísticas de Ventas",
                    FileName = string.Format("EstadisticasVentas_{0:yyyyMMdd}.csv", DateTime.Now)
                };

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    // Obtener datos reales para exportar
                    string query = @"
                        SELECT 
                            ISNULL(SUM(v.Total), 0) AS TotalVentas,
                            ISNULL(SUM(dv.Cantidad), 0) AS TotalUnidades,
                            COUNT(DISTINCT v.Id) AS TotalTransacciones,
                            CASE 
                                WHEN COUNT(DISTINCT v.Id) > 0 
                                THEN ISNULL(SUM(v.Total), 0) / COUNT(DISTINCT v.Id)
                                ELSE 0 
                            END AS PromedioVenta
                        FROM Ventas v
                        LEFT JOIN DetalleVentas dv ON v.Id = dv.VentaId
                        WHERE v.FechaVenta >= DATEADD(DAY, -30, GETDATE())";

                    DataTable dtResumen = DatabaseManager.ExecuteQuery(query);

                    string contenidoCsv = "Estadísticas de Ventas - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm") + "\n\n";

                    if (dtResumen.Rows.Count > 0)
                    {
                        DataRow row = dtResumen.Rows[0];
                        decimal totalVentas = Convert.ToDecimal(row["TotalVentas"]);
                        int totalUnidades = Convert.ToInt32(row["TotalUnidades"]);
                        int totalTransacciones = Convert.ToInt32(row["TotalTransacciones"]);
                        decimal promedioVenta = Convert.ToDecimal(row["PromedioVenta"]);

                        contenidoCsv += "Concepto,Valor\n";
                        contenidoCsv += string.Format("Total Ventas,${0:N2}\n", totalVentas);
                        contenidoCsv += string.Format("Unidades Vendidas,{0}\n", totalUnidades);
                        contenidoCsv += string.Format("Transacciones,{0}\n", totalTransacciones);
                        contenidoCsv += string.Format("Promedio por Venta,${0:N2}\n\n", promedioVenta);
                    }

                    // Agregar productos más vendidos
                    string queryProductos = @"
                        SELECT TOP 10 
                            i.Nombre AS Producto,
                            SUM(dv.Cantidad) AS CantidadVendida
                        FROM DetalleVentas dv
                        INNER JOIN Inventario i ON dv.ProductoId = i.Id
                        GROUP BY i.Nombre
                        ORDER BY CantidadVendida DESC";

                    DataTable dtProductos = DatabaseManager.ExecuteQuery(queryProductos);

                    contenidoCsv += "Productos Más Vendidos\n";
                    contenidoCsv += "Producto,Cantidad Vendida\n";

                    foreach (DataRow row in dtProductos.Rows)
                    {
                        contenidoCsv += string.Format("{0},{1}\n", row["Producto"], row["CantidadVendida"]);
                    }

                    System.IO.File.WriteAllText(saveDialog.FileName, contenidoCsv);

                    MessageBox.Show("Estadísticas exportadas correctamente.", "Éxito",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al exportar: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            string periodo = cmbFiltroTiempo?.SelectedItem?.ToString() ?? "Hoy";
            CargarEstadisticasVentas(periodo);
            CargarResumenVentas();
            MessageBox.Show("Estadísticas actualizadas correctamente.", "Actualizado",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void EstablecerRango(DateTime desde, DateTime hasta)
        {
            if (hasta <= desde)
            {
                MessageBox.Show("Rango inválido", "Rango", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            fechaDesde = desde;
            fechaHasta = hasta;
            CargarResumenVentas();
        }

        private void btnExportarDetalle_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable detalle = ReporteVentasGenerator.GetVentasDetalle(fechaDesde, fechaHasta);
                if (detalle.Rows.Count == 0)
                {
                    MessageBox.Show("No hay datos para exportar", "Exportar", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "CSV (*.csv)|*.csv";
                sfd.FileName = string.Format("DetalleVentas_{0:yyyyMMdd}_{1:yyyyMMdd}.csv", fechaDesde, fechaHasta.AddDays(-1));
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    string csv = ReporteVentasGenerator.GenerarCsvDetalle(detalle);
                    System.IO.File.WriteAllText(sfd.FileName, csv, System.Text.Encoding.UTF8);
                    MessageBox.Show("Detalle exportado", "Exportar", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al exportar detalle: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
