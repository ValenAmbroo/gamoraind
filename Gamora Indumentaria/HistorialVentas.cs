using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Gamora_Indumentaria.Data;
using System.ComponentModel;

namespace Gamora_Indumentaria
{
    public partial class HistorialVentas : Form
    {
        // Los controles se declaran en el archivo Designer

        public HistorialVentas()
        {
            InitializeComponent();
            this.Load += HistorialVentas_Load;
            this.txtBuscar.GotFocus += (s, e) => RemovePlaceholder();
            this.txtBuscar.LostFocus += (s, e) => ApplyPlaceholder();
            this.KeyDown += HistorialVentas_KeyDown;
            // Activar doble buffer para reducir parpadeo
            SetDoubleBuffer(dgvVentas, true);
            SetDoubleBuffer(dgvDetalles, true);
            if (btnDensidad != null)
                btnDensidad.Click += (s, e) => ToggleDensidad();
            if (btnFiltrar != null) btnFiltrar.Click += BtnFiltrar_Click;
            if (btnLimpiar != null) btnLimpiar.Click += BtnLimpiar_Click;
            if (btnExportar != null) btnExportar.Click += BtnExportar_Click;
            if (dgvVentas != null) dgvVentas.SelectionChanged += DgvVentas_SelectionChanged;
            if (dtpDesde != null) dtpDesde.ValueChanged += (s, e) => AutoFiltrar();
            if (dtpHasta != null) dtpHasta.ValueChanged += (s, e) => AutoFiltrar();
            if (cboMetodo != null) cboMetodo.SelectedIndexChanged += (s, e) => AutoFiltrar();
            if (txtBuscar != null)
            {
                txtBuscar.TextChanged += (s, e) => { if (!txtBuscar.Focused) return; DebounceBuscar(); };
                txtBuscar.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) { e.Handled = true; CargarVentas(); } };
            }
            // Preparar fuentes para densidad
            if (dgvVentas != null && dgvVentas.Font != null)
            {
                fuenteNormal = dgvVentas.DefaultCellStyle.Font ?? this.Font;
                float compactSize = Math.Max(8f, fuenteNormal.Size - 1.5f);
                fuenteCompacta = new Font(fuenteNormal.FontFamily, compactSize, fuenteNormal.Style);
                headerNormal = dgvVentas.ColumnHeadersDefaultCellStyle.Font ?? fuenteNormal;
                headerCompacta = new Font(headerNormal.FontFamily, Math.Max(8f, headerNormal.Size - 1f), headerNormal.Style);
            }
            // Placeholder inicial
            ApplyPlaceholder();
            // Aplicar densidad inicial (compacta por defecto)
            ApplyDensidad();
        }

        private void HistorialVentas_Load(object sender, EventArgs e)
        {
            if (EsDiseno()) return;
            dtpHasta.Value = DateTime.Today;
            dtpDesde.Value = DateTime.Today.AddDays(-30);
            CargarMetodosPago();
            CargarVentas();
        }

        private bool EsDiseno()
        {
            return LicenseManager.UsageMode == LicenseUsageMode.Designtime || DesignMode;
        }

        private void HistorialVentas_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                CargarVentas();
                e.Handled = true;
            }
            else if (e.Control && e.KeyCode == Keys.E)
            {
                ExportarCsv();
                e.Handled = true;
            }
            else if (e.Control && e.KeyCode == Keys.D)
            {
                ToggleDensidad();
                e.Handled = true;
            }
        }

        private string _placeholder = "Buscar producto o código...";
        private System.Windows.Forms.Timer debounceTimer;
        private void DebounceBuscar()
        {
            if (debounceTimer == null)
            {
                debounceTimer = new System.Windows.Forms.Timer();
                debounceTimer.Interval = 350; // ms
                debounceTimer.Tick += (s, e) => { debounceTimer.Stop(); if (txtBuscar.Text != _placeholder) CargarVentas(); };
            }
            debounceTimer.Stop();
            debounceTimer.Start();
        }

        private void AutoFiltrar()
        {
            if (EsDiseno()) return;
            CargarVentas();
        }
        private void ApplyPlaceholder()
        {
            if (EsDiseno()) return;
            if (string.IsNullOrWhiteSpace(txtBuscar.Text))
            {
                txtBuscar.ForeColor = Color.Gray;
                txtBuscar.Text = _placeholder;
            }
        }

        private bool densidadCompacta = true;
        private Font fuenteNormal;
        private Font fuenteCompacta;
        private Font headerNormal;
        private Font headerCompacta;

        private void ToggleDensidad()
        {
            densidadCompacta = !densidadCompacta;
            ApplyDensidad();
        }

        private void ApplyDensidad()
        {
            if (dgvVentas == null || dgvDetalles == null) return;
            int ventasHeight = densidadCompacta ? 18 : 24;
            int detallesHeight = densidadCompacta ? 16 : 22;

            // Ajustar alturas existentes
            foreach (DataGridViewRow r in dgvVentas.Rows)
            {
                if (!r.IsNewRow) r.Height = ventasHeight;
            }
            foreach (DataGridViewRow r in dgvDetalles.Rows)
            {
                if (!r.IsNewRow) r.Height = detallesHeight;
            }
            dgvVentas.RowTemplate.Height = ventasHeight;
            dgvDetalles.RowTemplate.Height = detallesHeight;

            // Ajustar fuentes
            if (fuenteNormal != null && fuenteCompacta != null)
            {
                var bodyFont = densidadCompacta ? fuenteCompacta : fuenteNormal;
                var headFont = densidadCompacta ? headerCompacta : headerNormal;
                dgvVentas.DefaultCellStyle.Font = bodyFont;
                dgvVentas.ColumnHeadersDefaultCellStyle.Font = headFont;
                dgvDetalles.DefaultCellStyle.Font = bodyFont;
                dgvDetalles.ColumnHeadersDefaultCellStyle.Font = headFont;
            }

            dgvVentas.Invalidate();
            dgvDetalles.Invalidate();
            if (btnDensidad != null)
                btnDensidad.Text = densidadCompacta ? "≡" : "≣"; // Ícono según modo
            if (toolTip1 != null)
            {
                toolTip1.SetToolTip(btnDensidad, densidadCompacta ? "Modo compacto (Ctrl+D)" : "Modo normal (Ctrl+D)");
            }
        }

        private void RemovePlaceholder()
        {
            if (txtBuscar.Text == _placeholder)
            {
                txtBuscar.Clear();
                txtBuscar.ForeColor = Color.Black;
            }
        }

        private void BtnFiltrar_Click(object sender, EventArgs e)
        {
            CargarVentas();
        }

        private void BtnLimpiar_Click(object sender, EventArgs e)
        {
            txtBuscar.Clear();
            if (cboMetodo.Items.Count > 0) cboMetodo.SelectedIndex = 0;
            dtpDesde.Value = DateTime.Today.AddDays(-30);
            dtpHasta.Value = DateTime.Today;
            CargarVentas();
        }

        private void BtnExportar_Click(object sender, EventArgs e)
        {
            ExportarCsv();
        }

        private void DgvVentas_SelectionChanged(object sender, EventArgs e)
        {
            CargarDetallesSeleccion();
        }

        private void CargarMetodosPago()
        {
            if (EsDiseno()) return; // Evita ejecución en diseñador
            try
            {
                cboMetodo.BeginUpdate();
                // Preserva el índice actual si posible
                object selected = cboMetodo.SelectedItem;
                if (cboMetodo.Items.Count == 0)
                    cboMetodo.Items.Add("(Todos)");
                string q = "SELECT DISTINCT MetodoPago FROM Ventas WHERE MetodoPago IS NOT NULL ORDER BY MetodoPago";
                DataTable dt = DatabaseManager.ExecuteQuery(q);
                foreach (DataRow r in dt.Rows)
                {
                    string m = r["MetodoPago"].ToString();
                    if (!string.IsNullOrWhiteSpace(m))
                    {
                        if (!cboMetodo.Items.Contains(m))
                            cboMetodo.Items.Add(m);
                    }
                }
                if (selected != null && cboMetodo.Items.Contains(selected))
                    cboMetodo.SelectedItem = selected;
                else
                    cboMetodo.SelectedIndex = 0;
                cboMetodo.EndUpdate();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error cargando métodos de pago: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarVentas()
        {
            if (EsDiseno()) return; // Evita ejecución en diseñador
            try
            {
                DateTime desde = dtpDesde.Value.Date;
                DateTime hasta = dtpHasta.Value.Date.AddDays(1); // exclusivo
                string metodo = (cboMetodo.SelectedIndex <= 0) ? string.Empty : cboMetodo.SelectedItem.ToString();
                string buscar = txtBuscar.Text == _placeholder ? string.Empty : txtBuscar.Text.Trim();

                string query = @"SELECT v.Id, v.FechaVenta, v.MetodoPago, v.Total,
                                        ISNULL(COUNT(dv.Id),0) AS Items,
                                        ISNULL(SUM(dv.Cantidad),0) AS Unidades
                                 FROM Ventas v
                                 LEFT JOIN DetalleVentas dv ON v.Id = dv.VentaId
                                 WHERE v.FechaVenta >= @Desde AND v.FechaVenta < @Hasta
                                   AND (@Metodo = '' OR v.MetodoPago = @Metodo)
                                   AND (@Buscar = '' OR EXISTS (SELECT 1 FROM DetalleVentas dv2 INNER JOIN Inventario i2 ON i2.Id = dv2.ProductoId
                                                               WHERE dv2.VentaId = v.Id AND (i2.Nombre LIKE @BuscarPatron OR i2.CodigoBarras LIKE @BuscarPatron)))
                                 GROUP BY v.Id, v.FechaVenta, v.MetodoPago, v.Total
                                 ORDER BY v.FechaVenta DESC";

                SqlParameter[] p = {
                    new SqlParameter("@Desde", desde),
                    new SqlParameter("@Hasta", hasta),
                    new SqlParameter("@Metodo", metodo),
                    new SqlParameter("@Buscar", buscar),
                    new SqlParameter("@BuscarPatron", "%" + buscar + "%")
                };

                DataTable dt = DatabaseManager.ExecuteQuery(query, p);
                dgvVentas.DataSource = dt;
                // Ajuste de columnas amigable
                if (dgvVentas.Columns.Contains("Id")) dgvVentas.Columns["Id"].HeaderText = "#";
                if (dgvVentas.Columns.Contains("FechaVenta")) dgvVentas.Columns["FechaVenta"].HeaderText = "Fecha";
                if (dgvVentas.Columns.Contains("MetodoPago")) dgvVentas.Columns["MetodoPago"].HeaderText = "Método";
                if (dgvVentas.Columns.Contains("Total")) dgvVentas.Columns["Total"].HeaderText = "Total";
                if (dgvVentas.Columns.Contains("Items")) dgvVentas.Columns["Items"].HeaderText = "Items";
                if (dgvVentas.Columns.Contains("Unidades")) dgvVentas.Columns["Unidades"].HeaderText = "Unidades";

                if (dt.Columns.Contains("Total"))
                {
                    dgvVentas.Columns["Total"].DefaultCellStyle.Format = "C2";
                    dgvVentas.Columns["FechaVenta"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
                }

                int count = dt.Rows.Count;
                decimal sumaTotal = 0m;
                long unidades = 0;
                foreach (DataRow r in dt.Rows)
                {
                    if (r["Total"] != DBNull.Value) sumaTotal += Convert.ToDecimal(r["Total"]);
                    if (r["Unidades"] != DBNull.Value) unidades += Convert.ToInt64(r["Unidades"]);
                }
                lblResumen.Text = string.Format("Ventas: {0} | Importe: {1:C2} | Unidades: {2}", count, sumaTotal, unidades);

                dgvDetalles.DataSource = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar ventas: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarDetallesSeleccion()
        {
            if (EsDiseno()) return;
            if (dgvVentas.CurrentRow == null || dgvVentas.CurrentRow.Cells["Id"].Value == null) return;
            if (!int.TryParse(dgvVentas.CurrentRow.Cells["Id"].Value.ToString(), out int ventaId)) return;
            try
            {
                string q = @"SELECT dv.Id, i.Nombre AS Producto, dv.Cantidad, dv.PrecioUnitario, dv.Subtotal
                             FROM DetalleVentas dv
                             INNER JOIN Inventario i ON dv.ProductoId = i.Id
                             WHERE dv.VentaId = @Id
                             ORDER BY dv.Id";
                DataTable dt = DatabaseManager.ExecuteQuery(q, new SqlParameter("@Id", ventaId));
                dgvDetalles.DataSource = dt;
                if (dgvDetalles.Columns.Contains("Id")) dgvDetalles.Columns["Id"].HeaderText = "#";
                if (dgvDetalles.Columns.Contains("Producto")) dgvDetalles.Columns["Producto"].HeaderText = "Producto";
                if (dgvDetalles.Columns.Contains("Cantidad")) dgvDetalles.Columns["Cantidad"].HeaderText = "Cant.";
                if (dgvDetalles.Columns.Contains("PrecioUnitario")) dgvDetalles.Columns["PrecioUnitario"].HeaderText = "P.Unit";
                if (dgvDetalles.Columns.Contains("Subtotal")) dgvDetalles.Columns["Subtotal"].HeaderText = "Subtotal";
                if (dt.Columns.Contains("PrecioUnitario"))
                {
                    dgvDetalles.Columns["PrecioUnitario"].DefaultCellStyle.Format = "C2";
                    dgvDetalles.Columns["Subtotal"].DefaultCellStyle.Format = "C2";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar detalles: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExportarCsv()
        {
            if (EsDiseno()) return;
            if (dgvVentas.DataSource == null)
            {
                MessageBox.Show("No hay datos para exportar", "Exportar", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "CSV (*.csv)|*.csv",
                FileName = string.Format("HistorialVentas_{0:yyyyMMdd}_{1:yyyyMMdd}.csv", dtpDesde.Value.Date, dtpHasta.Value.Date)
            };
            if (sfd.ShowDialog() != DialogResult.OK) return;
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

        private void SetDoubleBuffer(Control control, bool enable)
        {
            if (!enable) return;
            if (SystemInformation.TerminalServerSession) return; // Evita problemas en RDP
            var dgv = control as DataGridView;
            if (dgv != null)
            {
                typeof(Control).GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                    ?.SetValue(dgv, true, null);
            }
        }

        private void btnDensidad_Click(object sender, EventArgs e)
        {

        }

      

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();

        }
    }
}
