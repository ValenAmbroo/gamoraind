using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Gamora_Indumentaria
{
    public partial class EstadisticasVentasSimple : Form
    {
        public EstadisticasVentasSimple()
        {
            try
            {
                InitializeComponent();
                ConfigurarFiltros();
                CargarEstadisticasVentas("Hoy"); // Cargar estadísticas del día por defecto
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al inicializar formulario de estadísticas de ventas: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConfigurarFiltros()
        {
            // Configurar ComboBox de filtros
            if (cmbFiltroTiempo != null)
            {
                cmbFiltroTiempo.Items.Clear();
                cmbFiltroTiempo.Items.Add("Hoy");
                cmbFiltroTiempo.Items.Add("Esta Semana");
                cmbFiltroTiempo.Items.Add("Este Mes");
                cmbFiltroTiempo.Items.Add("Últimos 7 días");
                cmbFiltroTiempo.Items.Add("Últimos 30 días");
                cmbFiltroTiempo.SelectedIndex = 0; // Seleccionar "Hoy" por defecto
            }

            // Configurar gráficos
            ConfigurarGraficos();
        }

        private void ConfigurarGraficos()
        {
            // Configurar gráfico de ventas por tiempo
            if (chartVentasTiempo != null)
            {
                chartVentasTiempo.Series.Clear();
                chartVentasTiempo.ChartAreas.Clear();

                ChartArea areaTiempo = new ChartArea("TiempoArea");
                areaTiempo.AxisX.Title = "Período";
                areaTiempo.AxisY.Title = "Ventas ($)";
                areaTiempo.BackColor = Color.White;
                areaTiempo.BorderColor = Color.LightGray;
                areaTiempo.BorderWidth = 1;
                chartVentasTiempo.ChartAreas.Add(areaTiempo);

                Series serieTiempo = new Series("Ventas");
                serieTiempo.ChartType = SeriesChartType.Column;
                serieTiempo.Color = Color.FromArgb(52, 152, 219);
                serieTiempo.BorderWidth = 1;
                serieTiempo.BorderColor = Color.FromArgb(41, 128, 185);
                chartVentasTiempo.Series.Add(serieTiempo);

                chartVentasTiempo.Titles.Clear();
                chartVentasTiempo.Titles.Add("📈 Ventas por Tiempo");
                chartVentasTiempo.Titles[0].Font = new Font("Segoe UI", 12, FontStyle.Bold);
                chartVentasTiempo.Titles[0].ForeColor = Color.FromArgb(44, 62, 80);
            }

            // Configurar gráfico de productos más vendidos
            if (chartProductosVendidos != null)
            {
                chartProductosVendidos.Series.Clear();
                chartProductosVendidos.ChartAreas.Clear();

                ChartArea areaProductos = new ChartArea("ProductosArea");
                areaProductos.AxisX.Title = "Cantidad Vendida";
                areaProductos.AxisY.Title = "Productos";
                areaProductos.BackColor = Color.White;
                areaProductos.BorderColor = Color.LightGray;
                areaProductos.BorderWidth = 1;
                chartProductosVendidos.ChartAreas.Add(areaProductos);

                Series serieProductos = new Series("Productos");
                serieProductos.ChartType = SeriesChartType.Bar;
                serieProductos.IsValueShownAsLabel = true;
                chartProductosVendidos.Series.Add(serieProductos);

                chartProductosVendidos.Titles.Clear();
                chartProductosVendidos.Titles.Add("🏆 Top 10 Productos Más Vendidos");
                chartProductosVendidos.Titles[0].Font = new Font("Segoe UI", 12, FontStyle.Bold);
                chartProductosVendidos.Titles[0].ForeColor = Color.FromArgb(44, 62, 80);
            }

            // Configurar gráfico de categorías (pie chart)
            if (chartCategorias != null)
            {
                chartCategorias.Series.Clear();
                chartCategorias.ChartAreas.Clear();

                ChartArea areaCategorias = new ChartArea("CategoriasArea");
                areaCategorias.BackColor = Color.White;
                chartCategorias.ChartAreas.Add(areaCategorias);

                Series serieCategorias = new Series("Categorías");
                serieCategorias.ChartType = SeriesChartType.Pie;
                serieCategorias.IsValueShownAsLabel = true;
                serieCategorias.LabelFormat = "{P1}";
                serieCategorias["PieLabelStyle"] = "Outside";
                serieCategorias.BorderWidth = 2;
                serieCategorias.BorderColor = Color.White;
                chartCategorias.Series.Add(serieCategorias);

                chartCategorias.Titles.Clear();
                chartCategorias.Titles.Add("🎯 Ventas por Categoría");
                chartCategorias.Titles[0].Font = new Font("Segoe UI", 12, FontStyle.Bold);
                chartCategorias.Titles[0].ForeColor = Color.FromArgb(44, 62, 80);

                // Habilitar leyenda
                chartCategorias.Legends.Clear();
                Legend legend = new Legend("CategoriaLegend");
                legend.Docking = Docking.Right;
                legend.Font = new Font("Segoe UI", 9);
                chartCategorias.Legends.Add(legend);
            }

            // Configurar gráfico de tendencia mensual
            if (chartTendenciaMensual != null)
            {
                chartTendenciaMensual.Series.Clear();
                chartTendenciaMensual.ChartAreas.Clear();

                ChartArea areaTendencia = new ChartArea("TendenciaArea");
                areaTendencia.AxisX.Title = "Mes";
                areaTendencia.AxisY.Title = "Ventas ($)";
                areaTendencia.BackColor = Color.White;
                areaTendencia.BorderColor = Color.LightGray;
                areaTendencia.BorderWidth = 1;
                chartTendenciaMensual.ChartAreas.Add(areaTendencia);

                Series serieTendencia = new Series("Tendencia");
                serieTendencia.ChartType = SeriesChartType.Line;
                serieTendencia.Color = Color.FromArgb(46, 204, 113);
                serieTendencia.BorderWidth = 3;
                serieTendencia.MarkerStyle = MarkerStyle.Circle;
                serieTendencia.MarkerSize = 8;
                serieTendencia.MarkerColor = Color.FromArgb(39, 174, 96);
                chartTendenciaMensual.Series.Add(serieTendencia);

                chartTendenciaMensual.Titles.Clear();
                chartTendenciaMensual.Titles.Add("📈 Tendencia Mensual");
                chartTendenciaMensual.Titles[0].Font = new Font("Segoe UI", 12, FontStyle.Bold);
                chartTendenciaMensual.Titles[0].ForeColor = Color.FromArgb(44, 62, 80);
            }
        }

        private void CargarEstadisticasVentas(string periodo)
        {
            try
            {
                // Cargar datos de ejemplo (simulados)
                CargarVentasPorTiempo(periodo);
                CargarProductosMasVendidos(periodo);
                CargarVentasPorCategoria(periodo);
                CargarTendenciaMensual(periodo);
                CargarResumenVentas(periodo);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar estadísticas de ventas: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarVentasPorTiempo(string periodo)
        {
            if (chartVentasTiempo == null) return;

            chartVentasTiempo.Series["Ventas"].Points.Clear();

            // Datos de ejemplo según el período
            switch (periodo.ToLower())
            {
                case "hoy":
                    chartVentasTiempo.Series["Ventas"].Points.AddXY("08:00h", 5000);
                    chartVentasTiempo.Series["Ventas"].Points.AddXY("10:00h", 7500);
                    chartVentasTiempo.Series["Ventas"].Points.AddXY("12:00h", 12000);
                    chartVentasTiempo.Series["Ventas"].Points.AddXY("14:00h", 8500);
                    chartVentasTiempo.Series["Ventas"].Points.AddXY("16:00h", 15000);
                    chartVentasTiempo.Series["Ventas"].Points.AddXY("18:00h", 22000);
                    break;
                case "esta semana":
                    chartVentasTiempo.Series["Ventas"].Points.AddXY("Lun", 25000);
                    chartVentasTiempo.Series["Ventas"].Points.AddXY("Mar", 30000);
                    chartVentasTiempo.Series["Ventas"].Points.AddXY("Mié", 28000);
                    chartVentasTiempo.Series["Ventas"].Points.AddXY("Jue", 35000);
                    chartVentasTiempo.Series["Ventas"].Points.AddXY("Vie", 45000);
                    chartVentasTiempo.Series["Ventas"].Points.AddXY("Sáb", 55000);
                    chartVentasTiempo.Series["Ventas"].Points.AddXY("Dom", 20000);
                    break;
                default:
                    chartVentasTiempo.Series["Ventas"].Points.AddXY("Sem 1", 180000);
                    chartVentasTiempo.Series["Ventas"].Points.AddXY("Sem 2", 220000);
                    chartVentasTiempo.Series["Ventas"].Points.AddXY("Sem 3", 195000);
                    chartVentasTiempo.Series["Ventas"].Points.AddXY("Sem 4", 250000);
                    break;
            }
        }

        private void CargarProductosMasVendidos(string periodo)
        {
            if (chartProductosVendidos == null) return;

            chartProductosVendidos.Series["Productos"].Points.Clear();

            // Datos de ejemplo
            string[] productos = { "Remera Básica", "Jean Clásico", "Vestido Casual", "Collar Dorado", "Zapatillas", "Campera", "Pantalón", "Blusa", "Bufanda", "Gorra" };
            int[] cantidades = { 45, 38, 32, 28, 25, 22, 18, 15, 12, 10 };

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

            for (int i = 0; i < productos.Length; i++)
            {
                int pointIndex = chartProductosVendidos.Series["Productos"].Points.AddXY(cantidades[i], productos[i]);
                chartProductosVendidos.Series["Productos"].Points[pointIndex].Color = colores[i];
            }
        }

        private void CargarVentasPorCategoria(string periodo)
        {
            if (chartCategorias == null) return;

            chartCategorias.Series["Categorías"].Points.Clear();

            // Datos de ejemplo
            string[] categorias = { "Remeras", "Pantalones", "Vestidos", "Accesorios", "Calzado" };
            decimal[] ventas = { 125000, 95000, 80000, 45000, 35000 };

            Color[] colores = {
                Color.FromArgb(52, 152, 219),
                Color.FromArgb(46, 204, 113),
                Color.FromArgb(155, 89, 182),
                Color.FromArgb(230, 126, 34),
                Color.FromArgb(231, 76, 60)
            };

            for (int i = 0; i < categorias.Length; i++)
            {
                int pointIndex = chartCategorias.Series["Categorías"].Points.AddXY(categorias[i], ventas[i]);
                chartCategorias.Series["Categorías"].Points[pointIndex].Color = colores[i];
            }
        }

        private void CargarTendenciaMensual(string periodo)
        {
            if (chartTendenciaMensual == null) return;

            chartTendenciaMensual.Series["Tendencia"].Points.Clear();

            // Datos de ejemplo de los últimos 6 meses
            string[] meses = { "Mar", "Abr", "May", "Jun", "Jul", "Ago" };
            decimal[] ventas = { 380000, 420000, 350000, 480000, 520000, 585000 };

            for (int i = 0; i < meses.Length; i++)
            {
                chartTendenciaMensual.Series["Tendencia"].Points.AddXY(meses[i], ventas[i]);
            }
        }

        private void CargarResumenVentas(string periodo)
        {
            // Datos de ejemplo para el resumen
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
                    Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
                    Title = "Exportar Estadísticas de Ventas",
                    DefaultExt = "csv",
                    FileName = string.Format("EstadisticasVentas_{0:yyyyMMdd}.csv", DateTime.Now)
                };

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    System.IO.File.WriteAllText(saveDialog.FileName,
                        "Estadísticas de Ventas - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm") + "\n" +
                        "Total Ventas,$585,000.00\n" +
                        "Unidades Vendidas,245\n" +
                        "Transacciones,38\n" +
                        "Promedio por Venta,$15,394.74\n");

                    MessageBox.Show("Estadísticas exportadas correctamente.", "Exportación Exitosa",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al exportar estadísticas: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            string periodo = cmbFiltroTiempo?.SelectedItem?.ToString() ?? "Hoy";
            CargarEstadisticasVentas(periodo);
            MessageBox.Show("Estadísticas de ventas actualizadas correctamente.", "Actualizado",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
