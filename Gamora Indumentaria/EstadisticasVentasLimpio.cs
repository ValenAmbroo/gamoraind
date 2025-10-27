using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Gamora_Indumentaria
{
    public partial class EstadisticasVentas : Form
    {
        public EstadisticasVentas()
        {
            try
            {
                InitializeComponent();
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
                // Nombres alineados con la app
                cmbFiltroTiempo.Items.Add("Hoy");
                cmbFiltroTiempo.Items.Add("Semana Actual");
                cmbFiltroTiempo.Items.Add("Mes Actual");
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

            switch (periodo.ToLower())
            {
                case "hoy":
                    chartVentasTiempo.Series["Ventas"].Points.AddXY("09:00", 5000);
                    chartVentasTiempo.Series["Ventas"].Points.AddXY("12:00", 8500);
                    chartVentasTiempo.Series["Ventas"].Points.AddXY("15:00", 12000);
                    chartVentasTiempo.Series["Ventas"].Points.AddXY("18:00", 18500);
                    break;
                case "esta semana":
                    chartVentasTiempo.Series["Ventas"].Points.AddXY("Lun", 25000);
                    chartVentasTiempo.Series["Ventas"].Points.AddXY("Mar", 30000);
                    chartVentasTiempo.Series["Ventas"].Points.AddXY("Mié", 28000);
                    chartVentasTiempo.Series["Ventas"].Points.AddXY("Jue", 35000);
                    chartVentasTiempo.Series["Ventas"].Points.AddXY("Vie", 45000);
                    chartVentasTiempo.Series["Ventas"].Points.AddXY("Sáb", 55000);
                    break;
                default:
                    chartVentasTiempo.Series["Ventas"].Points.AddXY("Sem 1", 180000);
                    chartVentasTiempo.Series["Ventas"].Points.AddXY("Sem 2", 220000);
                    chartVentasTiempo.Series["Ventas"].Points.AddXY("Sem 3", 195000);
                    chartVentasTiempo.Series["Ventas"].Points.AddXY("Sem 4", 250000);
                    break;
            }
        }

        private void CargarProductosMasVendidos()
        {
            if (chartProductosVendidos == null) return;

            chartProductosVendidos.Series["Productos"].Points.Clear();

            Color[] colores = {
                Color.FromArgb(231, 76, 60),
                Color.FromArgb(52, 152, 219),
                Color.FromArgb(46, 204, 113),
                Color.FromArgb(155, 89, 182),
                Color.FromArgb(230, 126, 34)
            };

            string[] productos = { "Remera Básica", "Jean Clásico", "Vestido Casual", "Collar Dorado", "Zapatillas" };
            int[] cantidades = { 45, 38, 32, 28, 25 };

            for (int i = 0; i < productos.Length; i++)
            {
                int pointIndex = chartProductosVendidos.Series["Productos"].Points.AddXY(cantidades[i], productos[i]);
                chartProductosVendidos.Series["Productos"].Points[pointIndex].Color = colores[i];
            }
        }

        private void CargarVentasPorCategoria()
        {
            if (chartCategorias == null) return;

            chartCategorias.Series["Categorías"].Points.Clear();

            Color[] colores = {
                Color.FromArgb(52, 152, 219),
                Color.FromArgb(46, 204, 113),
                Color.FromArgb(155, 89, 182),
                Color.FromArgb(230, 126, 34),
                Color.FromArgb(231, 76, 60)
            };

            string[] categorias = { "Remeras", "Pantalones", "Vestidos", "Accesorios", "Calzado" };
            decimal[] ventas = { 125000, 95000, 80000, 45000, 35000 };

            for (int i = 0; i < categorias.Length; i++)
            {
                int pointIndex = chartCategorias.Series["Categorías"].Points.AddXY(categorias[i], ventas[i]);
                chartCategorias.Series["Categorías"].Points[pointIndex].Color = colores[i];
            }
        }

        private void CargarTendenciaMensual()
        {
            if (chartTendenciaMensual == null) return;

            chartTendenciaMensual.Series["Tendencia"].Points.Clear();

            string[] meses = { "Mar", "Abr", "May", "Jun", "Jul", "Ago" };
            decimal[] ventas = { 380000, 420000, 350000, 480000, 520000, 585000 };

            for (int i = 0; i < meses.Length; i++)
            {
                chartTendenciaMensual.Series["Tendencia"].Points.AddXY(meses[i], ventas[i]);
            }
        }

        private void CargarResumenVentas()
        {
            if (lblTotalVentas != null)
                lblTotalVentas.Text = "$585,000.00";

            if (lblTotalUnidadesVendidas != null)
                lblTotalUnidadesVendidas.Text = "245";

            if (lblTotalTransacciones != null)
                lblTotalTransacciones.Text = "38";

            if (lblPromedioVenta != null)
                lblPromedioVenta.Text = "$15,394.74";
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
                    System.IO.File.WriteAllText(saveDialog.FileName,
                        "Estadísticas de Ventas\n" +
                        "Total Ventas,$585,000.00\n" +
                        "Unidades Vendidas,245\n" +
                        "Transacciones,38\n" +
                        "Promedio por Venta,$15,394.74\n");

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
            MessageBox.Show("Estadísticas actualizadas correctamente.", "Actualizado",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
