using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Gamora_Indumentaria.Data;

namespace Gamora_Indumentaria
{
    public partial class ListadoVentas : Form
    {
        private DataGridView dgvVentas;
        private DataGridView dgvDetalles;
        private DateTimePicker dtpDesde;
        private DateTimePicker dtpHasta;
        private Button btnFiltrar;
        private Button btnExportar;
        private Label lblCantidadVentas;

        public ListadoVentas()
        {
            InitializeComponent();
            dtpHasta.Value = DateTime.Today;
            dtpDesde.Value = DateTime.Today.AddDays(-30);
            CargarVentas();
        }

        private void InitializeComponent()
        {
            this.Text = "Listado de Ventas";
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.White;

            dtpDesde = new DateTimePicker { Format = DateTimePickerFormat.Short, Width = 110 };
            dtpHasta = new DateTimePicker { Format = DateTimePickerFormat.Short, Width = 110 };
            btnFiltrar = new Button { Text = "Filtrar", Width = 80 };
            btnExportar = new Button { Text = "Exportar CSV", Width = 100 };
            lblCantidadVentas = new Label { AutoSize = true, Text = "Ventas: 0", Font = new Font("Segoe UI", 9, FontStyle.Bold) };

            btnFiltrar.Click += btnFiltrar_Click;
            btnExportar.Click += btnExportar_Click;

            FlowLayoutPanel topPanel = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 40, Padding = new Padding(8), BackColor = Color.FromArgb(245, 245, 245) };
            topPanel.Controls.Add(new Label { Text = "Desde:", AutoSize = true, TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(0, 8, 0, 0) });
            topPanel.Controls.Add(dtpDesde);
            topPanel.Controls.Add(new Label { Text = "Hasta:", AutoSize = true, TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(10, 8, 0, 0) });
            topPanel.Controls.Add(dtpHasta);
            topPanel.Controls.Add(btnFiltrar);
            topPanel.Controls.Add(btnExportar);
            topPanel.Controls.Add(lblCantidadVentas);

            dgvVentas = new DataGridView { Dock = DockStyle.Top, Height = 220, ReadOnly = true, AllowUserToAddRows = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect, MultiSelect = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
            dgvDetalles = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect, MultiSelect = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };

            dgvVentas.SelectionChanged += dgvVentas_SelectionChanged;

            SplitContainer split = new SplitContainer { Dock = DockStyle.Fill, Orientation = Orientation.Horizontal, SplitterDistance = 260 };
            split.Panel1.Controls.Add(dgvVentas);
            split.Panel2.Controls.Add(dgvDetalles);

            this.Controls.Add(split);
            this.Controls.Add(topPanel);
        }

        private void btnFiltrar_Click(object sender, EventArgs e)
        {
            CargarVentas();
        }

        private void CargarVentas()
        {
            try
            {
                DateTime desde = dtpDesde.Value.Date;
                DateTime hasta = dtpHasta.Value.Date.AddDays(1); // exclusivo
                string query = @"SELECT v.Id, v.FechaVenta, v.MetodoPago, v.Total,
                                        ISNULL(COUNT(dv.Id),0) AS Items,
                                        ISNULL(SUM(dv.Cantidad),0) AS Unidades
                                 FROM Ventas v
                                 LEFT JOIN DetalleVentas dv ON v.Id = dv.VentaId
                                 WHERE v.FechaVenta >= @Desde AND v.FechaVenta < @Hasta
                                 GROUP BY v.Id, v.FechaVenta, v.MetodoPago, v.Total
                                 ORDER BY v.FechaVenta DESC";
                SqlParameter[] p = { new SqlParameter("@Desde", desde), new SqlParameter("@Hasta", hasta) };
                DataTable dt = DatabaseManager.ExecuteQuery(query, p);
                dgvVentas.DataSource = dt;
                lblCantidadVentas.Text = string.Format("Ventas: {0}", dt.Rows.Count);
                dgvDetalles.DataSource = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar ventas: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvVentas_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvVentas.CurrentRow == null) return;
            int ventaId;
            if (!int.TryParse(dgvVentas.CurrentRow.Cells["Id"].Value.ToString(), out ventaId)) return;
            CargarDetalles(ventaId);
        }

        private void CargarDetalles(int ventaId)
        {
            try
            {
                string q = @"SELECT dv.Id, i.Nombre AS Producto, dv.Cantidad, dv.PrecioUnitario, dv.Subtotal
                            FROM DetalleVentas dv
                            INNER JOIN Inventario i ON dv.ProductoId = i.Id
                            WHERE dv.VentaId = @Id";
                SqlParameter p = new SqlParameter("@Id", ventaId);
                DataTable dt = DatabaseManager.ExecuteQuery(q, p);
                dgvDetalles.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar detalles: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnExportar_Click(object sender, EventArgs e)
        {
            if (dgvVentas.DataSource == null)
            {
                MessageBox.Show("No hay datos para exportar", "Exportar", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "CSV (*.csv)|*.csv";
            sfd.FileName = string.Format("Ventas_{0:yyyyMMdd}_{1:yyyyMMdd}.csv", dtpDesde.Value.Date, dtpHasta.Value.Date);
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    StringBuilder sb = new StringBuilder();
                    DataTable dtVentas = (DataTable)dgvVentas.DataSource;
                    sb.AppendLine("Id;FechaVenta;MetodoPago;Total;Items;Unidades");
                    foreach (DataRow row in dtVentas.Rows)
                    {
                        sb.AppendFormat("{0};{1:yyyy-MM-dd HH:mm};{2};{3};{4};{5}\n", row["Id"], row["FechaVenta"], row["MetodoPago"], row["Total"], row["Items"], row["Unidades"]);
                    }
                    sb.AppendLine();
                    sb.AppendLine("DETALLES");
                    sb.AppendLine("VentaId;DetalleId;Producto;Cantidad;PrecioUnitario;Subtotal");
                    foreach (DataGridViewRow ventaRow in dgvVentas.Rows)
                    {
                        if (ventaRow.Cells["Id"].Value == null) continue;
                        int ventaId = Convert.ToInt32(ventaRow.Cells["Id"].Value);
                        string qDet = @"SELECT dv.Id, i.Nombre AS Producto, dv.Cantidad, dv.PrecioUnitario, dv.Subtotal
                                         FROM DetalleVentas dv INNER JOIN Inventario i ON dv.ProductoId = i.Id
                                         WHERE dv.VentaId = @Id";
                        DataTable dtDet = DatabaseManager.ExecuteQuery(qDet, new SqlParameter("@Id", ventaId));
                        foreach (DataRow d in dtDet.Rows)
                        {
                            sb.AppendFormat("{0};{1};{2};{3};{4};{5}\n", ventaId, d["Id"], d["Producto"], d["Cantidad"], d["PrecioUnitario"], d["Subtotal"]);
                        }
                    }
                    System.IO.File.WriteAllText(sfd.FileName, sb.ToString(), Encoding.UTF8);
                    MessageBox.Show("Exportado correctamente", "Exportar", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al exportar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
