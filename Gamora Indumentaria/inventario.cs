using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Gamora_Indumentaria
{
    public partial class inventario : Form
    {
        private DataTable datos;
        private bool densidadCompacta = true;
        private Font fuenteNormal;
        private Font fuenteCompacta;
        private System.Windows.Forms.Timer debounceTimer;

        private bool esAdmin;

        public inventario(bool esAdmin = false)
        {
            InitializeComponent();
            this.esAdmin = esAdmin;
            ApplyFilterStyles();
            this.Load += Inventario_Load;
            this.KeyDown += Inventario_KeyDown;
            if (btnCerrar != null) btnCerrar.Click += (s, e) => this.Close();

            if (cboCategoria != null) cboCategoria.SelectedIndexChanged += (s, e) => { ActualizarCamposCategoria(); Filtrar(); };
            if (cboTalle != null) cboTalle.SelectedIndexChanged += (s, e) => Filtrar();

            if (chkSoloBajo != null) chkSoloBajo.CheckedChanged += (s, e) => Filtrar();
            if (nudStockMin != null) nudStockMin.ValueChanged += (s, e) => Filtrar();
            if (nudStockMax != null) nudStockMax.ValueChanged += (s, e) => Filtrar();
        }

        private void Inventario_Load(object sender, EventArgs e)
        {
            ConfigurarGrid();
            CargarCategorias();
            CargarInventario();
            PrepararFuentes();
        }

        private void ConfigurarGrid()
        {
            if (dgvInventario == null) return;
            dgvInventario.AutoGenerateColumns = true;
            dgvInventario.EnableHeadersVisualStyles = false;
            dgvInventario.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 58, 64);
            dgvInventario.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvInventario.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dgvInventario.DefaultCellStyle.SelectionBackColor = Color.FromArgb(0, 123, 255);
            dgvInventario.DefaultCellStyle.SelectionForeColor = Color.White;
            dgvInventario.RowTemplate.Height = 20;
            SetDoubleBuffer(dgvInventario, true);
        }

        private void CargarCategorias()
        {
            try
            {
                var dal = new Data.InventarioDAL();
                var categorias = dal.ObtenerCategorias();
                // Insertar opción "Todos" al inicio para permitir ver todos los productos sin filtrar por categoría
                categorias.Insert(0, new Data.Categoria { Id = 0, Nombre = "Todos", TieneTalle = false });
                cboCategoria.DataSource = categorias;
                cboCategoria.DisplayMember = "Nombre";
                cboCategoria.ValueMember = "Id";
                if (cboCategoria.Items.Count > 0) cboCategoria.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error cargando categorías: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarTalles(int categoriaId)
        {
            try
            {
                var dal = new Data.InventarioDAL();
                var talles = dal.ObtenerTallesPorCategoria(categoriaId);
                cboTalle.DataSource = talles;
                cboTalle.DisplayMember = "TalleValor";
                cboTalle.ValueMember = "Id";
                bool visible = talles.Count > 0;
                cboTalle.Visible = lblTalle.Visible = visible;
            }
            catch (Exception ex)
            {
                cboTalle.Visible = lblTalle.Visible = false;
                System.Diagnostics.Debug.WriteLine("Error cargando talles: " + ex.Message);
            }
        }

        private void ActualizarCamposCategoria()
        {
            if (cboCategoria?.SelectedItem is Data.Categoria cat)
            {
                // Si es "Todos" ocultamos campos dependientes
                if (!string.Equals(cat.Nombre, "Todos", StringComparison.OrdinalIgnoreCase) && cat.TieneTalle)
                {
                    CargarTalles(cat.Id);
                }
                else
                {
                    cboTalle.DataSource = null;
                    cboTalle.Visible = lblTalle.Visible = false;
                }
                bool esVaper = EsCategoriaVaper(cat.Nombre);

                if (!esVaper)
                {
                    // Limpiar texto para que no siga filtrando inadvertidamente

                }
            }
        }

        private bool EsCategoriaVaper(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre)) return false;
            var n = nombre.Trim().ToUpperInvariant();
            return n == "VAPER";
        }

        private void CargarInventario()
        {
            try
            {
                string sql = "SELECT * FROM vw_InventarioCompleto";
                datos = Data.DatabaseManager.ExecuteQuery(sql);
                dgvInventario.DataSource = datos;
                FormatearColumnas();
                Filtrar();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error cargando inventario: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FormatearColumnas()
        {
            if (dgvInventario.Columns.Contains("PrecioVenta"))
                dgvInventario.Columns["PrecioVenta"].DefaultCellStyle.Format = "C2";
            if (dgvInventario.Columns.Contains("PrecioCompra"))
            {
                // Mostrar costo solo si es admin
                dgvInventario.Columns["PrecioCompra"].Visible = esAdmin;
                if (esAdmin)
                    dgvInventario.Columns["PrecioCompra"].DefaultCellStyle.Format = "C2";
            }
            if (dgvInventario.Columns.Contains("FechaCreacion"))
                dgvInventario.Columns["FechaCreacion"].DefaultCellStyle.Format = "dd/MM/yyyy";
            var ocultar = new[] { "CategoriaId", "TalleId", "Activo" };
            foreach (var c in ocultar)
                if (dgvInventario.Columns.Contains(c)) dgvInventario.Columns[c].Visible = false;
        }

        private void DebounceFiltrar()
        {
            if (debounceTimer == null)
            {
                debounceTimer = new System.Windows.Forms.Timer();
                debounceTimer.Interval = 300;
                debounceTimer.Tick += (s, e) => { debounceTimer.Stop(); Filtrar(); };
            }
            debounceTimer.Stop();
            debounceTimer.Start();
        }

        private void Filtrar()
        {
            if (datos == null) return;
            string filtro = datos.Columns.Contains("Activo") ? "Activo = 1" : "1=1";
            if (!string.IsNullOrWhiteSpace(txtBuscar.Text))
            {
                string b = Escape(txtBuscar.Text.Trim());
                filtro += $" AND (Producto LIKE '%{b}%' OR Descripcion LIKE '%{b}%' OR CodigoBarras LIKE '%{b}%')";
            }
            if (cboCategoria.SelectedItem is Data.Categoria cat)
            {
                if (!string.Equals(cat.Nombre, "Todos", StringComparison.OrdinalIgnoreCase))
                {
                    string catEsc = Escape(cat.Nombre);
                    filtro += $" AND Categoria = '{catEsc}'";
                }
            }
            if (cboTalle.Visible && cboTalle.SelectedItem is Data.TallePorCategoria talle)
            {
                string t = Escape(talle.TalleValor);
                filtro += $" AND Talle = '{t}'";
            }

            if (chkSoloBajo.Checked)
            {
                filtro += " AND Cantidad <= 10";
            }
            if (nudStockMin.Value > 0)
            {
                filtro += $" AND Cantidad >= {nudStockMin.Value}";
            }
            if (nudStockMax.Value < 100000)
            {
                filtro += $" AND Cantidad <= {nudStockMax.Value}";
            }
            datos.DefaultView.RowFilter = filtro;
            ActualizarResumen();
        }

        private void ActualizarResumen()
        {
            if (lblResumen == null || datos == null) return;
            int visibles = datos.DefaultView.Count;
            decimal totalVal = 0;
            foreach (DataRowView rv in datos.DefaultView)
            {
                if (rv.Row["PrecioVenta"] != DBNull.Value && rv.Row["Cantidad"] != DBNull.Value)
                {
                    totalVal += Convert.ToDecimal(rv.Row["PrecioVenta"]) * Convert.ToInt32(rv.Row["Cantidad"]);
                }
            }
            lblResumen.Text = $"{visibles} registros | Valor total: {totalVal:C2}";
        }

        private string Escape(string s) => s.Replace("'", "''");

        private void PrepararFuentes()
        {
            if (dgvInventario == null) return;
            fuenteNormal = dgvInventario.DefaultCellStyle.Font ?? this.Font;
            fuenteCompacta = new Font(fuenteNormal.FontFamily, Math.Max(8f, fuenteNormal.Size - 1.2f), fuenteNormal.Style);
        }




        private void Inventario_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5) { CargarInventario(); e.Handled = true; }
            if (e.Control && e.KeyCode == Keys.E) { ExportarCsv(); e.Handled = true; }
        }

        private void txtBuscar_KeyDown(object sender, KeyEventArgs e)
        {
            // Si el usuario presiona Enter, buscar por código de barras exacto
            if (e.KeyCode == Keys.Enter && !string.IsNullOrWhiteSpace(txtBuscar.Text))
            {
                string codigo = txtBuscar.Text.Trim();
                if (datos != null)
                {
                    // Buscar coincidencia exacta en CodigoBarras
                    var rows = datos.Select($"CodigoBarras = '{Escape(codigo)}'");
                    if (rows.Length > 0)
                    {
                        datos.DefaultView.RowFilter = $"CodigoBarras = '{Escape(codigo)}'";
                        ActualizarResumen();
                    }
                    else
                    {
                        MessageBox.Show("No se encontró el producto con ese código de barras.", "Sin resultados", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                e.Handled = true;
            }
        }

        private void ExportarCsv()
        {
            try
            {
                if (datos == null || datos.DefaultView.Count == 0)
                {
                    MessageBox.Show("No hay datos para exportar", "Exportar", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                using (SaveFileDialog sfd = new SaveFileDialog())
                {
                    sfd.Filter = "CSV (*.csv)|*.csv";
                    sfd.FileName = "inventario.csv";
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        var sb = new StringBuilder();
                        // Encabezados visibles
                        var cols = dgvInventario.Columns.Cast<DataGridViewColumn>()
                            .Where(c => c.Visible && !string.Equals(c.DataPropertyName, "PrecioCompra", StringComparison.OrdinalIgnoreCase))
                            .ToList();
                        sb.AppendLine(string.Join(",", cols.Select(c => '"' + c.HeaderText.Replace("\"", "''") + '"')));
                        foreach (DataRowView rv in datos.DefaultView)
                        {
                            var line = new List<string>();
                            foreach (var c in cols)
                            {
                                var val = rv.Row[c.DataPropertyName];
                                string txt = val == null || val == DBNull.Value ? "" : val.ToString();
                                if (decimal.TryParse(txt, out decimal dec))
                                {
                                    if (c.DataPropertyName.Contains("Precio"))
                                        txt = dec.ToString("0.00");
                                }
                                line.Add('"' + txt.Replace("\"", "''") + '"');
                            }
                            sb.AppendLine(string.Join(",", line));
                        }
                        System.IO.File.WriteAllText(sfd.FileName, sb.ToString(), Encoding.UTF8);
                        MessageBox.Show("Exportación completada", "Exportar", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error exportando: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnAgregar_Click(object sender, EventArgs e)
        {
            var f = new agregarpruducto();
            f.ShowDialog(this);
            CargarInventario();
        }

        private void BtnEditar_Click(object sender, EventArgs e)
        {
            if (dgvInventario.CurrentRow == null) return;
            int id = Convert.ToInt32(dgvInventario.CurrentRow.Cells["Id"].Value);
            using (var f = new editarproducto(id))
            {
                if (f.ShowDialog(this) == DialogResult.OK)
                {
                    CargarInventario();
                }
            }
        }

        private void BtnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvInventario.CurrentRow == null) return;
            int id = Convert.ToInt32(dgvInventario.CurrentRow.Cells["Id"].Value);
            if (MessageBox.Show("¿Eliminar producto ID " + id + "?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    // Eliminación lógica: poner stock 0 (placeholder simplificado)
                    Data.DatabaseManager.ExecuteNonQuery("UPDATE Inventario SET Stock = 0 WHERE Id = " + id);
                    CargarInventario();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error eliminando: " + ex.Message);
                }
            }
        }

        private void SetDoubleBuffer(Control c, bool value)
        {
            if (SystemInformation.TerminalServerSession) return;
            var dgv = c as DataGridView;
            if (dgv == null) return;
            System.Reflection.PropertyInfo pi = typeof(DataGridView).GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            pi?.SetValue(dgv, value, null);
        }


        private void btnEliminar_Click_1(object sender, EventArgs e)
        {

        }

        private void txtBuscar_TextChanged(object sender, EventArgs e)
        {
            Filtrar();
        }

        private void ApplyFilterStyles()
        {
            // Colores base
            Color panelBg = Color.White;
            Color border = Color.FromArgb(230, 236, 240);
            Color label = Color.FromArgb(55, 71, 90);
            Color inputBack = Color.White;
            Color inputBorder = Color.FromArgb(206, 212, 218);
            Color inputFocus = Color.FromArgb(0, 123, 255);

            if (panelFiltros != null)
            {
                panelFiltros.BackColor = panelBg;
                panelFiltros.Padding = new Padding(10, 8, 10, 6);
                // Borde inferior suave
                panelFiltros.Paint += (s, e) =>
                {
                    using (var p = new Pen(border))
                        e.Graphics.DrawLine(p, 0, panelFiltros.Height - 1, panelFiltros.Width, panelFiltros.Height - 1);
                };
                panelFiltros.Resize += (s, e) => panelFiltros.Invalidate();
            }

            foreach (var lbl in new[] { lblBuscar, lblCategoria, lblTalle })
            {
                if (lbl == null) continue;
                lbl.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                lbl.ForeColor = label;
            }

            // Estilo básico del cuadro de búsqueda (sin placeholder)
            if (txtBuscar != null)
            {
                txtBuscar.BorderStyle = BorderStyle.FixedSingle;
                txtBuscar.BackColor = inputBack;
                txtBuscar.ForeColor = Color.Black;
                txtBuscar.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
                // Vincular evento para lector de barras
                txtBuscar.KeyDown -= txtBuscar_KeyDown;
                txtBuscar.KeyDown += txtBuscar_KeyDown;
            }

            void StyleCombo(ComboBox c)
            {
                if (c == null) return;
                c.FlatStyle = FlatStyle.Flat;
                c.BackColor = inputBack;
                c.ForeColor = Color.Black;
                c.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
                c.DrawMode = DrawMode.OwnerDrawFixed;
                c.DrawItem += (s, e) =>
                {
                    e.DrawBackground();
                    if (e.Index >= 0)
                    {
                        var txt = c.GetItemText(c.Items[e.Index]);
                        using (var br = new SolidBrush(e.ForeColor))
                        {
                            e.Graphics.DrawString(txt, c.Font, br, e.Bounds);
                        }
                    }
                    e.DrawFocusRectangle();
                };
            }
            StyleCombo(cboCategoria);
            StyleCombo(cboTalle);

            // Check y numeric
            foreach (var ctrl in new Control[] { chkSoloBajo, nudStockMin, nudStockMax })
            {
                if (ctrl == null) continue;
                ctrl.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            }
        }
    }
}
