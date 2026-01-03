using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using System.Windows.Forms.DataVisualization.Charting;
using Gamora_Indumentaria.Data;

namespace Gamora_Indumentaria
{
    public partial class EstadisticasVentas : Form
    {
        private DateTime fechaDesde;
        private DateTime fechaHasta;
        // Helper para asegurar que una serie exista en un chart
        private void EnsureSerie(Chart chart, string nombre, SeriesChartType tipo, Color? color = null)
        {
            if (chart == null) return;
            if (chart.Series.IndexOf(nombre) < 0)
            {
                // Si hay una única serie default sin nombre esperado, se puede renombrar
                if (chart.Series.Count == 1 && chart.Series[0].Points.Count == 0 && chart.Series[0].Name == "Series1")
                {
                    chart.Series[0].Name = nombre;
                    chart.Series[0].ChartType = tipo;
                    if (color.HasValue) chart.Series[0].Color = color.Value;
                }
                else
                {
                    Series s = new Series(nombre);
                    s.ChartType = tipo;
                    if (color.HasValue) s.Color = color.Value;
                    chart.Series.Add(s);
                }
            }
        }
        private readonly bool _isAdmin;

        public EstadisticasVentas(bool isAdmin = false)
        {
            try
            {
                _isAdmin = isAdmin;
                InitializeComponent();
                DatabaseManager.InitializeDatabase(); // Usar el manager centralizado
                fechaHasta = DateTime.Today.AddDays(1); // exclusivo
                fechaDesde = fechaHasta.AddDays(-30);
                ConfigurarFiltros();
                CargarEstadisticasVentas("Hoy");
                // Crear label de ganancia dinámicamente (si no existe en el diseñador)
                if (this.Controls.Find("lblGanancia", true).Length == 0)
                {
                    Label l = new Label();
                    l.Name = "lblGanancia";
                    l.AutoSize = true;
                    l.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
                    l.ForeColor = Color.FromArgb(231, 76, 60);
                    l.Text = "$0.00";
                    l.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
                    // Ubicarlo abajo del tableLayoutPanel dentro del groupBox1
                    l.Location = new System.Drawing.Point(238, 270);
                    l.Size = new System.Drawing.Size(122, 30);
                    // Añadir al groupBox1 para que sea visible
                    var gb = this.Controls.Find("groupBox1", true).FirstOrDefault() as GroupBox;
                    if (gb != null) gb.Controls.Add(l);
                    if (!_isAdmin) l.Visible = false;
                }
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
                // Nombres alineados con el resto de la app
                cmbFiltroTiempo.Items.Add("Hoy");
                cmbFiltroTiempo.Items.Add("Semana Actual");
                cmbFiltroTiempo.Items.Add("Mes Actual");
                cmbFiltroTiempo.Items.Add("Año Actual");
                cmbFiltroTiempo.Items.Add("Últimos 7 días");
                cmbFiltroTiempo.Items.Add("Últimos 30 días");
                cmbFiltroTiempo.Items.Add("Rango Personalizado");
                cmbFiltroTiempo.SelectedIndex = 0;
            }

            // Cargar años disponibles en la base de datos para el filtro por año
            CargarAniosDisponibles();

            // Deshabilitar selección de año salvo que el filtro sea "Este Año"
            try
            {
                if (labelAnio != null) labelAnio.Enabled = false;
                if (cmbAnio != null) cmbAnio.Enabled = false;
                if (labelDesde != null) labelDesde.Enabled = false;
                if (dtpDesde != null)
                {
                    dtpDesde.Enabled = false;
                    dtpDesde.Value = DateTime.Today;
                }
                if (labelHasta != null) labelHasta.Enabled = false;
                if (dtpHasta != null)
                {
                    dtpHasta.Enabled = false;
                    dtpHasta.Value = DateTime.Today;
                }
                if (btnAplicarRango != null) btnAplicarRango.Enabled = false;
            }
            catch { }

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
            chartVentasTiempo.Legends.Clear();

            ChartArea area = new ChartArea("VentasArea");
            // Sin títulos de ejes para no mostrar texto
            area.AxisX.Title = string.Empty;
            area.AxisY.Title = string.Empty;
            area.BackColor = Color.White;
            chartVentasTiempo.ChartAreas.Add(area);

            Series serie = new Series("Ventas");
            serie.ChartType = SeriesChartType.Column;
            serie.Color = Color.FromArgb(52, 152, 219);
            chartVentasTiempo.Series.Add(serie);

            // Sin título dentro del gráfico
            chartVentasTiempo.Titles.Clear();
        }

        private void ConfigurarGraficoProductos()
        {
            if (chartProductosVendidos == null) return;

            chartProductosVendidos.Series.Clear();
            chartProductosVendidos.ChartAreas.Clear();
            chartProductosVendidos.Legends.Clear();

            ChartArea area = new ChartArea("ProductosArea");
            // Sin títulos de ejes
            area.AxisX.Title = string.Empty;
            area.AxisY.Title = string.Empty;
            area.BackColor = Color.White;
            chartProductosVendidos.ChartAreas.Add(area);

            Series serie = new Series("Productos");
            serie.ChartType = SeriesChartType.Bar;
            // No mostrar valores sobre las barras para mantener los gráficos limpios
            serie.IsValueShownAsLabel = false;
            chartProductosVendidos.Series.Add(serie);

            // Sin título dentro del gráfico
            chartProductosVendidos.Titles.Clear();
        }

        private void ConfigurarGraficoCategorias()
        {
            if (chartCategorias == null) return;

            chartCategorias.Series.Clear();
            chartCategorias.ChartAreas.Clear();
            chartCategorias.Legends.Clear();

            ChartArea area = new ChartArea("CategoriasArea");
            area.BackColor = Color.White;
            chartCategorias.ChartAreas.Add(area);

            Series serie = new Series("Categorías");
            serie.ChartType = SeriesChartType.Pie;
            // No mostrar texto dentro del gráfico
            serie.IsValueShownAsLabel = false;
            serie.Label = string.Empty;
            // Referencia al lado del gráfico: nombre + porcentaje
            serie.ToolTip = "#VALX (#PERCENT{P1})";
            serie.LegendText = "#VALX (#PERCENT{P1})";
            chartCategorias.Series.Add(serie);

            // Mantener el gráfico sin título interno, pero con leyenda visible a la derecha
            chartCategorias.Titles.Clear();
            Legend legend = new Legend();
            legend.Docking = Docking.Right;
            chartCategorias.Legends.Add(legend);
        }

        private void ConfigurarGraficoTendencia()
        {
            if (chartTendenciaMensual == null) return;

            chartTendenciaMensual.Series.Clear();
            chartTendenciaMensual.ChartAreas.Clear();
            chartTendenciaMensual.Legends.Clear();

            ChartArea area = new ChartArea("TendenciaArea");
            // Sin títulos de ejes
            area.AxisX.Title = string.Empty;
            area.AxisY.Title = string.Empty;
            area.BackColor = Color.White;
            chartTendenciaMensual.ChartAreas.Add(area);

            Series serie = new Series("Tendencia");
            serie.ChartType = SeriesChartType.Line;
            serie.Color = Color.FromArgb(46, 204, 113);
            serie.BorderWidth = 3;
            serie.MarkerStyle = MarkerStyle.Circle;
            serie.MarkerSize = 8;
            chartTendenciaMensual.Series.Add(serie);

            // Sin título dentro del gráfico
            chartTendenciaMensual.Titles.Clear();
        }

        private void CargarEstadisticasVentas(string periodo)
        {
            try
            {
                CargarProductosMasVendidos();
                CargarVentasPorCategoria();
                // CargarTendenciaMensual(); // gráfico de tendencia mensual removido de la vista principal
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

            // Asegurar que la serie 'Ventas' exista
            if (chartVentasTiempo.Series.IndexOf("Ventas") < 0)
            {
                // Configurar nuevamente el gráfico si la serie no está
                ConfigurarGraficoVentasTiempo();
            }
            if (chartVentasTiempo.Series.IndexOf("Ventas") < 0 && chartVentasTiempo.Series.Count > 0)
            {
                // Renombrar primera serie como fallback
                chartVentasTiempo.Series[0].Name = "Ventas";
            }
            if (chartVentasTiempo.Series.IndexOf("Ventas") < 0)
            {
                chartVentasTiempo.Series.Add("Ventas");
            }

            chartVentasTiempo.Series["Ventas"].Points.Clear();

            try
            {
                // Determinar rango según filtro, actualizando fechaDesde/fechaHasta (hasta exclusivo)
                DateTime hoy = DateTime.Today;
                switch (periodo.ToLower())
                {
                    case "hoy":
                        fechaDesde = hoy;
                        fechaHasta = hoy.AddDays(1);
                        break;
                    case "esta semana":
                    case "semana actual":
                        int diff = (int)hoy.DayOfWeek; // domingo=0
                        DateTime inicioSemana = hoy.AddDays(-diff);
                        fechaDesde = inicioSemana;
                        fechaHasta = inicioSemana.AddDays(7);
                        break;
                    case "este mes":
                    case "mes actual":
                        fechaDesde = new DateTime(hoy.Year, hoy.Month, 1);
                        fechaHasta = fechaDesde.AddMonths(1);
                        break;
                    case "este año":
                    case "año actual":
                        fechaDesde = new DateTime(hoy.Year, 1, 1);
                        fechaHasta = fechaDesde.AddYears(1);
                        break;
                    case "últimos 7 días":
                        fechaHasta = hoy.AddDays(1);
                        fechaDesde = fechaHasta.AddDays(-7);
                        break;
                    case "últimos 30 días":
                        fechaHasta = hoy.AddDays(1);
                        fechaDesde = fechaHasta.AddDays(-30);
                        break;
                }

                // Agrupar por día dentro del rango
                string query = @"SELECT 
                                CONVERT(VARCHAR(5), FechaVenta, 108) AS Periodo,
                                SUM(Total) AS TotalVentas
                              FROM Ventas
                              WHERE FechaVenta >= @Desde AND FechaVenta < @Hasta AND DATEDIFF(DAY, @Desde, FechaVenta) = 0
                              GROUP BY DATEPART(HOUR, FechaVenta), CONVERT(VARCHAR(5), FechaVenta, 108)
                              ORDER BY DATEPART(HOUR, FechaVenta);";

                // Si rango mayor a un día, agrupar por fecha
                if ((fechaHasta - fechaDesde).TotalDays > 1)
                {
                    query = @"SELECT 
                                CONVERT(VARCHAR(5), MIN(FechaVenta), 103) AS Periodo,
                                SUM(Total) AS TotalVentas
                              FROM Ventas
                              WHERE FechaVenta >= @Desde AND FechaVenta < @Hasta
                              GROUP BY CAST(FechaVenta AS DATE)
                              ORDER BY CAST(FechaVenta AS DATE);";
                }

                SqlParameter[] parametros = {
                    new SqlParameter("@Desde", fechaDesde),
                    new SqlParameter("@Hasta", fechaHasta)
                };
                DataTable dt = DatabaseManager.ExecuteQuery(query, parametros);

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
            EnsureSerie(chartProductosVendidos, "Productos", SeriesChartType.Bar);
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
                    WHERE v.FechaVenta >= @Desde AND v.FechaVenta < @Hasta
                    GROUP BY i.Nombre
                    ORDER BY CantidadVendida DESC";

                SqlParameter[] parametros = {
                    new SqlParameter("@Desde", fechaDesde),
                    new SqlParameter("@Hasta", fechaHasta)
                };
                DataTable dt = DatabaseManager.ExecuteQuery(query, parametros);

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
                    // Etiqueta de categoría "Sin datos" con valor 0
                    chartProductosVendidos.Series["Productos"].Points.AddXY("Sin datos", 0);
                }
                else
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        DataRow row = dt.Rows[i];
                        string producto = row["Producto"].ToString();
                        int cantidad = Convert.ToInt32(row["CantidadVendida"]);

                        // Usar el nombre del producto como categoría (eje X) y la cantidad como valor
                        int pointIndex = chartProductosVendidos.Series["Productos"].Points.AddXY(producto, cantidad);
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
            EnsureSerie(chartCategorias, "Categorías", SeriesChartType.Pie);
            chartCategorias.Series["Categorías"].Points.Clear();

            try
            {
                // Construir WHERE dinámico para excluir ventas de regalo si existe esa columna
                bool tieneEsRegalo = DatabaseManager.ColumnExists("Ventas", "EsRegalo");
                string whereExtra = tieneEsRegalo ? " AND ISNULL(v.EsRegalo,0)=0" : string.Empty;

                string query = @"
                    SELECT 
                        c.Nombre AS Categoria,
                        SUM(dv.Cantidad * dv.PrecioUnitario) AS TotalVentas
                    FROM DetalleVentas dv
                    INNER JOIN Inventario i ON dv.ProductoId = i.Id
                    INNER JOIN Categorias c ON i.CategoriaId = c.Id
                    INNER JOIN Ventas v ON dv.VentaId = v.Id
                    WHERE v.FechaVenta >= @Desde AND v.FechaVenta < @Hasta" + whereExtra + @"
                    GROUP BY c.Nombre
                    ORDER BY TotalVentas DESC";

                SqlParameter[] parametros = {
                    new SqlParameter("@Desde", fechaDesde),
                    new SqlParameter("@Hasta", fechaHasta)
                };
                DataTable dt = DatabaseManager.ExecuteQuery(query, parametros);

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
                    // Para mejorar legibilidad, mostrar solo las principales categorías
                    // y agrupar el resto en "Otros" cuando haya muchas categorías.
                    int maxCategoriasIndividuales = 7;
                    decimal otrasVentas = 0m;

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        DataRow row = dt.Rows[i];
                        string categoria = row["Categoria"].ToString();
                        decimal ventas = Convert.ToDecimal(row["TotalVentas"]);

                        if (i < maxCategoriasIndividuales)
                        {
                            int pointIndex = chartCategorias.Series["Categorías"].Points.AddXY(categoria, ventas);
                            chartCategorias.Series["Categorías"].Points[pointIndex].Color = colores[i % colores.Length];
                        }
                        else
                        {
                            otrasVentas += ventas;
                        }
                    }

                    if (otrasVentas > 0)
                    {
                        int pointIndexOtros = chartCategorias.Series["Categorías"].Points.AddXY("Otros", otrasVentas);
                        // Usar un color neutro para "Otros"
                        chartCategorias.Series["Categorías"].Points[pointIndexOtros].Color = Color.FromArgb(189, 195, 199);
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
            EnsureSerie(chartTendenciaMensual, "Tendencia", SeriesChartType.Line, Color.FromArgb(46, 204, 113));
            chartTendenciaMensual.Series["Tendencia"].Points.Clear();

            try
            {
                // Tomar 6 meses hacia atrás desde fechaHasta
                DateTime inicio = new DateTime(fechaHasta.Year, fechaHasta.Month, 1).AddMonths(-5); // incluye mes actual (-5 = 6 meses)
                DateTime fin = new DateTime(fechaHasta.Year, fechaHasta.Month, 1).AddMonths(1); // siguiente mes exclusivo
                string query = @"SELECT 
                        DATENAME(MONTH, FechaVenta) + ' ' + CAST(YEAR(FechaVenta) AS VARCHAR) AS MesTexto,
                        FORMAT(FechaVenta, 'yyyy-MM') AS MesOrden,
                        SUM(Total) AS TotalVentas
                    FROM Ventas
                    WHERE FechaVenta >= @Inicio AND FechaVenta < @Fin
                    GROUP BY DATENAME(MONTH, FechaVenta), YEAR(FechaVenta), FORMAT(FechaVenta, 'yyyy-MM')
                    ORDER BY MesOrden";

                SqlParameter[] parametros = {
                    new SqlParameter("@Inicio", inicio),
                    new SqlParameter("@Fin", fin)
                };
                DataTable dt = DatabaseManager.ExecuteQuery(query, parametros);

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

                    // Obtener ganancia total y asignar al label (solo admin)
                    try
                    {
                        if (_isAdmin)
                        {
                            DataTable dg = ReporteVentasGenerator.GetGananciaTotal(fechaDesde, fechaHasta);
                            if (dg.Rows.Count > 0)
                            {
                                decimal ganancia = Convert.ToDecimal(dg.Rows[0]["Ganancia"]);
                                // buscar control existente (puede ser creado dinámicamente)
                                var matches = this.Controls.Find("lblGanancia", true);
                                if (matches.Length > 0)
                                {
                                    var lblG = matches[0] as Label;
                                    if (lblG != null) lblG.Text = string.Format("${0:N2}", ganancia);
                                }
                            }
                        }
                    }
                    catch { /* no bloquear resumen por fallo en ganancia */ }
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
                // Habilitar/deshabilitar selector de año según corresponda (acepta ambos nombres)
                bool esEsteAnio = string.Equals(periodo, "Este Año", StringComparison.OrdinalIgnoreCase)
                                   || string.Equals(periodo, "Año Actual", StringComparison.OrdinalIgnoreCase);
                if (labelAnio != null) labelAnio.Enabled = esEsteAnio;
                if (cmbAnio != null) cmbAnio.Enabled = esEsteAnio;
                bool esRango = string.Equals(periodo, "Rango Personalizado", StringComparison.OrdinalIgnoreCase);
                if (labelDesde != null) labelDesde.Enabled = esRango;
                if (dtpDesde != null) dtpDesde.Enabled = esRango;
                if (labelHasta != null) labelHasta.Enabled = esRango;
                if (dtpHasta != null) dtpHasta.Enabled = esRango;
                if (btnAplicarRango != null) btnAplicarRango.Enabled = esRango;
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
                    FileName = string.Format("EstadisticasVentas_{0:yyyyMMdd}_{1:yyyyMMdd}.csv", fechaDesde, fechaHasta.AddDays(-1))
                };

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    // Obtener datos reales para exportar
                    DataTable dtResumen = ReporteVentasGenerator.GetResumen(fechaDesde, fechaHasta);

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
                    string queryProductos = @"SELECT TOP 10 
                            i.Nombre AS Producto,
                            SUM(dv.Cantidad) AS CantidadVendida
                        FROM DetalleVentas dv
                        INNER JOIN Inventario i ON dv.ProductoId = i.Id
                        INNER JOIN Ventas v ON dv.VentaId = v.Id
                        WHERE v.FechaVenta >= @Desde AND v.FechaVenta < @Hasta
                        GROUP BY i.Nombre
                        ORDER BY CantidadVendida DESC";
                    SqlParameter[] paramProd = {
                        new SqlParameter("@Desde", fechaDesde),
                        new SqlParameter("@Hasta", fechaHasta)
                    };
                    DataTable dtProductos = DatabaseManager.ExecuteQuery(queryProductos, paramProd);

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

        // Cargar años para el filtro por año
        private void CargarAniosDisponibles()
        {
            try
            {
                if (cmbAnio == null) return;
                cmbAnio.Items.Clear();

                string query = @"SELECT DISTINCT YEAR(FechaVenta) AS Anio FROM Ventas ORDER BY Anio DESC";
                DataTable dt = DatabaseManager.ExecuteQuery(query);

                if (dt.Rows.Count == 0)
                {
                    // Si no hay ventas aún, mostrar el año actual
                    cmbAnio.Items.Add(DateTime.Today.Year);
                }
                else
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        cmbAnio.Items.Add(Convert.ToInt32(row["Anio"]));
                    }
                }

                if (cmbAnio.Items.Count > 0)
                {
                    // Seleccionar el primer año (más reciente)
                    cmbAnio.SelectedIndex = 0;
                }
            }
            catch
            {
                // En caso de error, al menos ofertar el año actual
                if (cmbAnio != null && cmbAnio.Items.Count == 0)
                {
                    cmbAnio.Items.Add(DateTime.Today.Year);
                    cmbAnio.SelectedIndex = 0;
                }
            }
        }

        private void cmbAnio_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbAnio?.SelectedItem == null) return;
            try
            {
                int anio = Convert.ToInt32(cmbAnio.SelectedItem);
                fechaDesde = new DateTime(anio, 1, 1);
                fechaHasta = fechaDesde.AddYears(1);
                // Alinear el filtro visual al periodo "Este Año" para consistencia
                try
                {
                    if (cmbFiltroTiempo != null)
                    {
                        if (cmbFiltroTiempo.Items.Contains("Año Actual"))
                            cmbFiltroTiempo.SelectedItem = "Año Actual";
                        else if (cmbFiltroTiempo.Items.Contains("Este Año"))
                            cmbFiltroTiempo.SelectedItem = "Este Año";
                    }
                }
                catch { }
                // Cargar todas las vistas con el rango definido por año
                CargarEstadisticasVentas("año"); // valor no manejado en switch => conserva fechaDesde/fechaHasta
                CargarResumenVentas();
            }
            catch { }
        }

        private void btnAplicarRango_Click(object sender, EventArgs e)
        {
            try
            {
                if (dtpDesde == null || dtpHasta == null)
                {
                    MessageBox.Show("Controles de fecha no disponibles", "Rango", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                DateTime d = dtpDesde.Value.Date;
                DateTime h = dtpHasta.Value.Date;
                if (h < d)
                {
                    MessageBox.Show("La fecha 'Hasta' no puede ser menor que 'Desde'", "Rango", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                fechaDesde = d;
                fechaHasta = h.AddDays(1); // exclusivo

                if (cmbFiltroTiempo != null && cmbFiltroTiempo.Items.Contains("Rango Personalizado"))
                {
                    cmbFiltroTiempo.SelectedItem = "Rango Personalizado";
                }

                CargarEstadisticasVentas("rango"); // no altera fechas en el switch
                CargarResumenVentas();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error aplicando rango: " + ex.Message, "Rango", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
