using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using Gamora_Indumentaria.Data;

namespace Gamora_Indumentaria
{
    public partial class editarproducto : Form
    {
        private readonly InventarioDAL dal = new InventarioDAL();
        private ProductoInventario producto;
        private bool cargando = true;

        public editarproducto()
        {
            InitializeComponent();
            ApplyStyles();
            CargarCategorias();
            CargarListaProductos();
            cargando = false;
        }

        public editarproducto(int productoId) : this()
        {
            SeleccionarProductoEnGrid(productoId);
        }

        private void CargarCategorias()
        {
            try
            {
                var cats = dal.ObtenerCategorias();
                cboCategoria.DataSource = cats;
                cboCategoria.DisplayMember = "Nombre";
                cboCategoria.ValueMember = "Id";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error cargando categorías: " + ex.Message);
            }
        }

        private void CargarProducto(int id)
        {
            producto = dal.ObtenerProductoPorId(id);
            if (producto == null)
            {
                MessageBox.Show("Producto no encontrado", "Editar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            txtNombre.Text = producto.Nombre;
            txtDescripcion.Text = producto.Descripcion;
            txtCodigoBarra.Text = producto.CodigoBarra;
            nudCantidad.Value = Math.Max(0, producto.Cantidad);
            txtPrecioVenta.Text = (producto.PrecioVenta ?? 0m).ToString("0.00");
            txtPrecioCosto.Text = (producto.PrecioCosto ?? 0m).ToString("0.00");
            if (producto.CategoriaId != 0)
                cboCategoria.SelectedValue = producto.CategoriaId;
        }

        private void CargarListaProductos()
        {
            try
            {
                var dt = dal.ObtenerInventarioCompleto();
                if (dt != null && dt.Columns.Contains("Id"))
                {
                    // Crear una vista ordenada por Id ascendente
                    DataView dv = dt.DefaultView;
                    dv.Sort = "Id ASC";
                    dgvProductos.DataSource = dv.ToTable();
                }
                else
                {
                    dgvProductos.DataSource = dt; // fallback sin ordenar
                }

                // Ajustar columnas básicas si existen
                if (dgvProductos.Columns.Contains("Id")) dgvProductos.Columns["Id"].Width = 50;
                string nombreCol = dgvProductos.Columns.Contains("Producto") ? "Producto" : (dgvProductos.Columns.Contains("Nombre") ? "Nombre" : null);
                if (nombreCol != null) dgvProductos.Columns[nombreCol].Width = 180;
                if (dgvProductos.Columns.Contains("Categoria")) dgvProductos.Columns["Categoria"].Width = 120;
                if (dgvProductos.Columns.Contains("Cantidad")) dgvProductos.Columns["Cantidad"].Width = 70;
                if (dgvProductos.Columns.Contains("PrecioVenta")) dgvProductos.Columns["PrecioVenta"].DefaultCellStyle.Format = "0.00";
                else if (dgvProductos.Columns.Contains("Precio")) dgvProductos.Columns["Precio"].DefaultCellStyle.Format = "0.00";
                // Estilos posteriores a asignación de DataSource
                dgvProductos.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error cargando lista: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvProductos_SelectionChanged(object sender, EventArgs e)
        {
            if (cargando) return;
            if (dgvProductos.CurrentRow == null) return;
            if (dgvProductos.Columns["Id"] == null) return;
            int id = Convert.ToInt32(dgvProductos.CurrentRow.Cells["Id"].Value);
            CargarProducto(id);
        }

        private void SeleccionarProductoEnGrid(int id)
        {
            foreach (DataGridViewRow row in dgvProductos.Rows)
            {
                if (dgvProductos.Columns["Id"] != null && Convert.ToInt32(row.Cells["Id"].Value) == id)
                {
                    row.Selected = true;
                    dgvProductos.CurrentCell = row.Cells.Cast<DataGridViewCell>().FirstOrDefault();
                    CargarProducto(id);
                    break;
                }
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (producto == null)
            {
                MessageBox.Show("Seleccione un producto", "Editar", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (!Validar()) return;
            try
            {
                producto.Nombre = txtNombre.Text.Trim();
                producto.Descripcion = string.IsNullOrWhiteSpace(txtDescripcion.Text) ? null : txtDescripcion.Text.Trim();
                producto.CodigoBarra = string.IsNullOrWhiteSpace(txtCodigoBarra.Text) ? null : txtCodigoBarra.Text.Trim();
                producto.CategoriaId = (int)cboCategoria.SelectedValue;
                producto.Cantidad = (int)nudCantidad.Value;
                if (decimal.TryParse(txtPrecioVenta.Text, out var pv)) producto.PrecioVenta = pv; else producto.PrecioVenta = null;
                if (decimal.TryParse(txtPrecioCosto.Text, out var pc)) producto.PrecioCosto = pc; else producto.PrecioCosto = null;
                dal.ActualizarProducto(producto, actualizarStock: true);
                MessageBox.Show("Producto actualizado", "Editar", MessageBoxButtons.OK, MessageBoxIcon.Information);
                // No cerrar el formulario: refrescar la lista y re-seleccionar el producto para continuar editando
                try
                {
                    CargarListaProductos();
                    if (producto != null && producto.Id != 0)
                    {
                        SeleccionarProductoEnGrid(producto.Id);
                    }
                }
                catch { /* no bloquear en refresco */ }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error actualizando: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool Validar()
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text)) { MessageBox.Show("Nombre requerido"); return false; }
            if (cboCategoria.SelectedItem == null) { MessageBox.Show("Seleccione categoría"); return false; }
            if (nudCantidad.Value < 0) { MessageBox.Show("Cantidad inválida"); return false; }
            if (!string.IsNullOrWhiteSpace(txtPrecioVenta.Text) && !decimal.TryParse(txtPrecioVenta.Text, out _)) { MessageBox.Show("Precio venta inválido"); return false; }
            if (!string.IsNullOrWhiteSpace(txtPrecioCosto.Text) && !decimal.TryParse(txtPrecioCosto.Text, out _)) { MessageBox.Show("Precio costo inválido"); return false; }
            return true;
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ApplyStyles()
        {
            // Activar doble buffer para reducir parpadeo
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
            this.UpdateStyles();

            BackColor = Color.FromArgb(245, 247, 250);
            // Mantener coherencia con otros formularios: panel superior blanco y título oscuro
            panelTop.BackColor = Color.White;
            lblTitulo.ForeColor = Color.FromArgb(40, 40, 40);

            var labelColor = Color.FromArgb(55, 71, 90);
            var inputBack = Color.White;
            var inputBorder = Color.FromArgb(200, 205, 210);
            var accent = Color.FromArgb(52, 152, 219);
            var danger = Color.FromArgb(231, 76, 60);
            var success = Color.FromArgb(46, 204, 113);

            Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            lblTitulo.Font = new Font("Segoe UI", 14F, FontStyle.Bold);

            // Etiquetas
            foreach (var lbl in new[] { lblNombre, lblDescripcion, lblCodigo, lblCategoria, lblCantidad, lblPrecioVenta, lblPrecioCosto })
            {
                lbl.ForeColor = labelColor;
                // FontStyle.SemiBold no existe en .NET Framework clásico, se usa Bold como alternativa.
                lbl.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            }

            // TextBoxes estilizados (borde simple simulando material light)
            void StyleTextBox(TextBox t)
            {
                t.BorderStyle = BorderStyle.FixedSingle;
                t.BackColor = inputBack;
                t.ForeColor = Color.Black;
            }
            StyleTextBox(txtNombre);
            StyleTextBox(txtDescripcion);
            StyleTextBox(txtCodigoBarra);
            StyleTextBox(txtPrecioVenta);
            StyleTextBox(txtPrecioCosto);

            // Combo y numeric
            cboCategoria.FlatStyle = FlatStyle.Flat;
            nudCantidad.BorderStyle = BorderStyle.FixedSingle;

            // Botones
            void StyleButton(Button b, Color back, Color fore)
            {
                b.FlatStyle = FlatStyle.Flat;
                b.FlatAppearance.BorderSize = 0;
                b.BackColor = back;
                b.ForeColor = fore;
                b.Cursor = Cursors.Hand;
            }
            StyleButton(btnGuardar, success, Color.White);
            StyleButton(btnCancelar, Color.FromArgb(149, 165, 166), Color.White);
            // Botón cerrar igual que en otros formularios (fondo blanco, texto rojo sin hover permanente)
            button1.FlatStyle = FlatStyle.Flat;
            button1.FlatAppearance.BorderSize = 0;
            button1.BackColor = Color.White;
            button1.ForeColor = Color.DarkRed;

            btnGuardar.MouseEnter += (s, e) => btnGuardar.BackColor = Color.FromArgb(39, 174, 96);
            btnGuardar.MouseLeave += (s, e) => btnGuardar.BackColor = success;
            btnCancelar.MouseEnter += (s, e) => btnCancelar.BackColor = Color.FromArgb(127, 140, 141);
            btnCancelar.MouseLeave += (s, e) => btnCancelar.BackColor = Color.FromArgb(149, 165, 166);
            button1.MouseEnter += (s, e) => button1.BackColor = Color.FromArgb(252, 238, 238);
            button1.MouseLeave += (s, e) => button1.BackColor = Color.White;

            // DataGridView styling
            dgvProductos.BorderStyle = BorderStyle.None;
            dgvProductos.BackgroundColor = Color.White;
            dgvProductos.EnableHeadersVisualStyles = false;
            dgvProductos.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgvProductos.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);
            dgvProductos.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(55, 71, 90);
            dgvProductos.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dgvProductos.DefaultCellStyle.BackColor = Color.White;
            dgvProductos.DefaultCellStyle.SelectionBackColor = Color.FromArgb(210, 230, 255);
            dgvProductos.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgvProductos.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);
            dgvProductos.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvProductos.GridColor = Color.FromArgb(230, 236, 240);
            dgvProductos.RowHeadersVisible = false;
            dgvProductos.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders;
            dgvProductos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;

            dgvProductos.SelectionChanged += (s, e) =>
            {
                // realzar fila seleccionada
                foreach (DataGridViewRow row in dgvProductos.Rows)
                {
                    if (!row.Selected)
                        row.DefaultCellStyle.BackColor = row.Index % 2 == 0 ? Color.White : Color.FromArgb(248, 250, 252);
                    else
                        row.DefaultCellStyle.BackColor = Color.FromArgb(224, 238, 252);
                }
            };
        }

        private void panelTop_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
