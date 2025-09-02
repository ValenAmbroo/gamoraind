using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Gamora_Indumentaria.Data;

namespace Gamora_Indumentaria
{
    public partial class agregarpruducto : Form
    {
        private InventarioDAL inventarioDAL;
        private Categoria categoriaSeleccionada;
        private List<TallePorCategoria> tallesDisponibles;
        private ErrorProvider err; // proveedor de errores para validaciones en línea
        // Buffer y control de tiempo para capturar lecturas rápidas de un escáner
        private readonly StringBuilder scannerBuffer = new StringBuilder();
        private DateTime lastScannerCharTime = DateTime.MinValue;
        private const int SCAN_TIMEOUT_MS = 120; // ms máximos entre caracteres para considerarlo parte del mismo escaneo

        public agregarpruducto()
        {
            InitializeComponent();
            inventarioDAL = new InventarioDAL();
            CargarCategorias();
            // Suscribirse a eventos de aplicación para recargar categorías dinámicamente
            AppEvents.CategoryAdded += OnCategoryAdded;
            // Activar vista previa de teclas para modo escáner
            this.KeyPreview = true;
            this.KeyPress += AgregarProducto_KeyPress_Global;
            InicializarErrorProviderYValidaciones();
        }

        private void OnCategoryAdded(int newCategoryId)
        {
            if (this.IsHandleCreated)
            {
                this.BeginInvoke(new Action(() =>
                {
                    // recargar categorías y seleccionar la nueva si existe
                    var prev = cboCategoria.SelectedItem as Categoria;
                    CargarCategorias();
                    for (int i = 0; i < cboCategoria.Items.Count; i++)
                    {
                        var c = (Categoria)cboCategoria.Items[i];
                        if (c.Id == newCategoryId)
                        {
                            cboCategoria.SelectedIndex = i;
                            return;
                        }
                    }
                }));
            }
        }

        public agregarpruducto(string nombreCategoria) : this()
        {
            // Si se pasa una categoría específica, seleccionarla
            for (int i = 0; i < cboCategoria.Items.Count; i++)
            {
                Categoria cat = (Categoria)cboCategoria.Items[i];
                if (cat.Nombre == nombreCategoria)
                {
                    cboCategoria.SelectedIndex = i;
                    break;
                }
            }
        }

        private void agregarpruducto_Load(object sender, EventArgs e)
        {
            // La inicialización se hace en el constructor
        }

        /// <summary>
        /// Carga todas las categorías disponibles en el ComboBox
        /// </summary>
        private void CargarCategorias()
        {
            try
            {
                var categorias = inventarioDAL.ObtenerCategorias();

                cboCategoria.DataSource = categorias;
                cboCategoria.DisplayMember = "Nombre";
                cboCategoria.ValueMember = "Id";

                if (categorias.Count > 0)
                {
                    cboCategoria.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Error al cargar categorías: {0}", ex.Message), "Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Maneja el cambio de categoría y actualiza los talles disponibles
        /// </summary>
        private void cboCategoria_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboCategoria.SelectedItem == null) return;

            categoriaSeleccionada = (Categoria)cboCategoria.SelectedItem;
            ActualizarCamposPorCategoria();
        }

        /// <summary>
        /// Actualiza los campos del formulario según la categoría seleccionada
        /// </summary>
        private void ActualizarCamposPorCategoria()
        {
            // Limpiar campos - usar DataSource = null para limpiar ComboBox con DataSource

            // txtSabor eliminado

            // Mostrar/ocultar campos según la categoría
            bool tieneCategoria = categoriaSeleccionada != null;

            if (tieneCategoria)
            {
                // Configurar visibilidad de campos según la categoría
                bool esTalleVisible = categoriaSeleccionada.TieneTalle;
                bool esVaper = EsCategoriaVaper(categoriaSeleccionada);

                lblTalle.Visible = esTalleVisible;


                // El campo Sabor ya no se utiliza: el sabor se ingresa dentro del Nombre del producto
                // lblSabor / txtSabor eliminados

                // Cargar talles si la categoría los tiene
                if (esTalleVisible)
                {
                    CargarTallesPorCategoria(categoriaSeleccionada.Id);
                    // cargar talles; `CargarTallesPorCategoria` decidirá si muestra u oculta el panel según existan talles
                }
                else
                {
                    // Si la categoría no tiene talle, ocultar y limpiar el panel de variantes
                    var panel = this.Controls.Find("panelVariantes", true).FirstOrDefault() as Panel;
                    if (panel != null)
                    {
                        // Eliminar únicamente los checkboxes dinámicos para no borrar controles fijos (por ejemplo panel1 con botones)
                        foreach (var cbOld in panel.Controls.OfType<CheckBox>().ToList())
                        {
                            panel.Controls.Remove(cbOld);
                        }
                        panel.Visible = false;
                    }
                    lblTalle.Visible = false;
                }

                // Se omite carga de sabores/autocomplete porque sabor va en el nombre
            }
        }

        /// <summary>
        /// Carga los talles disponibles para la categoría seleccionada
        /// </summary>
        private void CargarTallesPorCategoria(int categoriaId)
        {
            try
            {
                tallesDisponibles = inventarioDAL.ObtenerTallesPorCategoria(categoriaId);
                // Crear checkboxes dinámicos para talles (hasta 6) en panelVariantes
                try
                {
                    var panel = this.Controls.Find("panelVariantes", true).FirstOrDefault() as Panel;
                    if (panel != null)
                    {
                        // No eliminar el lblTalle si ya existe en el panel; eliminar solo checkboxes previos
                        var existingLabel = panel.Controls.OfType<Label>().FirstOrDefault(l => l.Name == "lblTalle");
                        foreach (var cbOld in panel.Controls.OfType<CheckBox>().ToList())
                        {
                            panel.Controls.Remove(cbOld);
                        }
                        if (existingLabel == null)
                        {
                            // asegurarse que el label esté en el panel
                            panel.Controls.Add(lblTalle);
                        }

                        bool tieneTalles = tallesDisponibles != null && tallesDisponibles.Count > 0;
                        panel.Visible = tieneTalles;
                        lblTalle.Visible = tieneTalles; // mostrar/ocultar etiqueta según corresponda
                        if (tieneTalles)
                        {
                            // encabezado claro
                            lblTalle.Text = "📏 Talles — marque los talles";

                            int x = 4;
                            int y = 30; // colocar checboxes por debajo del label
                            int spacingX = 120;
                            int spacingY = 30;
                            int maxWidth = panel.ClientSize.Width - 20;
                            for (int i = 0; i < tallesDisponibles.Count; i++)
                            {
                                var t = tallesDisponibles[i];
                                var chk = new CheckBox();
                                chk.AutoSize = true;
                                chk.Font = new System.Drawing.Font("Segoe UI", 9F);
                                chk.Location = new System.Drawing.Point(x, y);
                                chk.Name = "chkTalle_" + t.Id;
                                chk.Size = new System.Drawing.Size(100, 22);
                                chk.TabIndex = 100 + i;
                                chk.Text = t.TalleValor;
                                chk.Tag = t; // almacenar objeto talle
                                chk.UseVisualStyleBackColor = true;
                                panel.Controls.Add(chk);

                                x += spacingX;
                                if (x + spacingX > maxWidth)
                                {
                                    x = 4;
                                    y += spacingY;
                                }
                            }
                            // ajustar altura del panel para mostrar varias filas si es necesario
                            int rows = (int)Math.Ceiling((double)tallesDisponibles.Count * spacingX / Math.Max(1, maxWidth));
                            panel.Height = Math.Min(200, Math.Max(50, rows * spacingY + 40));
                            panel.AutoScroll = true;
                        }
                    }
                }
                catch { /* no bloquear si falla la UI dinámica */ }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Error al cargar talles: {0}", ex.Message), "Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Guarda el nuevo producto en la base de datos
        /// </summary>
        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!ValidarFormulario()) return;

            // Recolectar la lista de productos a crear: el principal + variantes marcadas
            var productosACrear = new List<ProductoInventario>();

            // Función local para parsear decimal seguro
            decimal? ParseDec(string s)
            {
                if (string.IsNullOrWhiteSpace(s)) return null;
                if (decimal.TryParse(s, out var v)) return v;
                return null;
            }

            try
            {
                var baseProducto = new ProductoInventario
                {
                    CategoriaId = categoriaSeleccionada.Id,
                    Nombre = txtNombre.Text.Trim(),
                    Descripcion = string.IsNullOrWhiteSpace(txtDescripcion.Text) ? null : txtDescripcion.Text.Trim(),
                    CodigoBarra = string.IsNullOrWhiteSpace(txtCodigoBarra.Text) ? null : txtCodigoBarra.Text.Trim(),
                    // TalleId se asigna cuando se crean productos por talle seleccionado
                    TalleId = null,
                    Sabor = null,
                    Cantidad = (int)nudCantidad.Value,
                    PrecioVenta = ParseDec(txtPrecio.Text),
                    PrecioCosto = ParseDec(txtPrecioCosto.Text)
                };

                // Si no hay talles disponibles, crear el producto base; si existen talles, se crearán
                // solo los productos por talles seleccionados.
                var panelChecks = this.Controls.Find("panelVariantes", true).FirstOrDefault() as Panel;
                bool hayChecks = panelChecks != null && panelChecks.Controls.OfType<CheckBox>().Any();
                if (!hayChecks)
                {
                    productosACrear.Add(baseProducto);
                }

                // Si hay checkboxes de talles creados en panelVariantes, crear un producto por cada uno marcado
                try
                {
                    var panel = this.Controls.Find("panelVariantes", true).FirstOrDefault() as Panel;
                    if (panel != null)
                    {
                        var checks = panel.Controls.OfType<CheckBox>().Where(c => c.Checked).ToList();
                        if (checks.Count == 0 && hayChecks)
                        {
                            // Si hay talles y no se seleccionó ninguno, ofrecer crear por todos los talles disponibles
                            var dr = MessageBox.Show("No seleccionaste ningún talle. ¿Crear productos para TODOS los talles disponibles?", "Crear todos los talles", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (dr == DialogResult.Yes)
                            {
                                // crear para todos los talles disponibles
                                if (tallesDisponibles != null)
                                {
                                    foreach (var talle in tallesDisponibles)
                                    {
                                        var pAll = CloneProducto(baseProducto);
                                        pAll.TalleId = talle.Id;
                                        pAll.Nombre = string.Format("{0} - {1}", baseProducto.Nombre, talle.TalleValor);
                                        productosACrear.Add(pAll);
                                    }
                                }
                            }
                            else
                            {
                                throw new Exception("Operación cancelada: seleccione al menos un talle.");
                            }
                        }
                        foreach (var c in checks)
                        {
                            var talle = c.Tag as TallePorCategoria;
                            if (talle != null)
                            {
                                var p = CloneProducto(baseProducto);
                                // Añadir el talle al nombre para diferenciarlos si procede
                                p.TalleId = talle.Id;
                                p.Nombre = string.Format("{0} - {1}", baseProducto.Nombre, talle.TalleValor);
                                productosACrear.Add(p);
                            }
                        }
                    }
                }
                catch { }

                // Si vamos a crear más de un producto, pedir cantidades por talle en una tabla editable
                var creados = new List<int>();
                if (productosACrear.Count > 1)
                {
                    // Preparar lista de talles seleccionados para el formulario: (TalleId, TalleValor, cantidadInicial)
                    var listas = new List<Tuple<int?, string, int>>();
                    foreach (var p in productosACrear)
                    {
                        string talleVal = p.TalleId.HasValue ? (tallesDisponibles.FirstOrDefault(t => t.Id == p.TalleId)?.TalleValor ?? "") : "";
                        listas.Add(Tuple.Create(p.TalleId, talleVal, (int)nudCantidad.Value));
                    }

                    using (var f = new AgregarCantidadPorTalleForm(listas))
                    {
                        var dr = f.ShowDialog(this);
                        if (dr != DialogResult.OK)
                        {
                            MessageBox.Show("Operación cancelada por el usuario.", "Cancelado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                        var cantidades = f.Quantities;
                        if (cantidades == null || cantidades.Count != productosACrear.Count)
                        {
                            MessageBox.Show("Error en las cantidades ingresadas.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        for (int i = 0; i < productosACrear.Count; i++)
                        {
                            productosACrear[i].Cantidad = cantidades[i];
                            int id = inventarioDAL.AgregarProducto(productosACrear[i]);
                            creados.Add(id);
                        }
                    }
                }
                else
                {
                    // único producto: usar la cantidad del numeric up-down
                    int cantidadAsignada = (int)nudCantidad.Value;
                    foreach (var prod in productosACrear)
                    {
                        prod.Cantidad = cantidadAsignada;
                        int id = inventarioDAL.AgregarProducto(prod);
                        creados.Add(id);
                    }
                }

                MessageBox.Show(string.Format("{0} producto(s) agregados exitosamente. IDs: {1}", creados.Count, string.Join(",", creados)), "Éxito",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);

                LimpiarFormulario();
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Error al guardar el/los producto(s): {0}", ex.Message), "Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private ProductoInventario CloneProducto(ProductoInventario original)
        {
            return new ProductoInventario
            {
                CategoriaId = original.CategoriaId,
                Descripcion = original.Descripcion,
                CodigoBarra = original.CodigoBarra,
                TalleId = original.TalleId,
                Sabor = original.Sabor,
                Cantidad = original.Cantidad,
                PrecioVenta = original.PrecioVenta,
                PrecioCosto = original.PrecioCosto
            };
        }

        // Métodos de sabores eliminados: el sabor se ingresa directamente en el nombre del producto para Vapers

        /// <summary>
        /// Valida que todos los campos requeridos estén completos
        /// </summary>
        private bool ValidarFormulario()
        {
            LimpiarErrores();
            if (categoriaSeleccionada == null)
            {
                SetErr(cboCategoria, "Seleccione una categoría");
            }

            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                SetErr(txtNombre, "Nombre requerido");
            }



            // Validación de sabor eliminada (el sabor se incorpora al nombre)

            if (nudCantidad.Value <= 0)
            {
                SetErr(nudCantidad, "Cantidad > 0");
            }

            // Precios requeridos
            if (string.IsNullOrWhiteSpace(txtPrecio.Text))
            {
                SetErr(txtPrecio, "Precio requerido");
            }

            if (string.IsNullOrWhiteSpace(txtPrecioCosto.Text))
            {
                SetErr(txtPrecioCosto, "Costo requerido");
            }

            // Validar precio si se ingresó
            if (!string.IsNullOrWhiteSpace(txtPrecio.Text))
            {
                decimal precio;
                if (!decimal.TryParse(txtPrecio.Text, out precio) || precio < 0)
                {
                    SetErr(txtPrecio, "Precio inválido");
                }
            }

            if (!string.IsNullOrWhiteSpace(txtPrecioCosto.Text))
            {
                decimal precioC;
                if (!decimal.TryParse(txtPrecioCosto.Text, out precioC) || precioC < 0)
                {
                    SetErr(txtPrecioCosto, "Costo inválido");
                }
            }

            if (!string.IsNullOrWhiteSpace(txtPrecio.Text) && !string.IsNullOrWhiteSpace(txtPrecioCosto.Text))
            {
                decimal pv, pc;
                if (decimal.TryParse(txtPrecio.Text, out pv) && decimal.TryParse(txtPrecioCosto.Text, out pc))
                {
                    if (pc > pv)
                    {
                        // Advertir visualmente, pero permitir continuar si usuario insiste (doble clic Guardar)
                        if (err.GetError(txtPrecioCosto) == string.Empty)
                            SetErr(txtPrecioCosto, "Costo > Precio (verificar)");
                    }
                }
            }
            // Si hay algún error marcado retornar false
            bool hayErrores = new Control[] { cboCategoria, txtNombre, nudCantidad, txtPrecio, txtPrecioCosto }
                .Any(c => c != null && err.GetError(c) != string.Empty);
            return !hayErrores;
        }

        /// <summary>
        /// Limpia todos los campos del formulario
        /// </summary>
        private void LimpiarFormulario()
        {
            txtNombre.Text = "";
            txtDescripcion.Text = "";
            txtCodigoBarra.Text = "";
            // txtSabor eliminado
            txtPrecio.Text = "";
            txtPrecioCosto.Text = "";
            nudCantidad.Value = 1;



            txtNombre.Focus();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            LimpiarFormulario();
        }

        private void button4_Click(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {
            this.Close();

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        // Helper para determinar si la categoría es un Vaper (acepta variantes VAPER / VAPERS)
        private bool EsCategoriaVaper(Categoria cat)
        {
            if (cat == null || string.IsNullOrWhiteSpace(cat.Nombre)) return false;
            var nombre = cat.Nombre.Trim().ToUpperInvariant();
            return nombre == "VAPER" || nombre == "VAPERS"; // permitir ambas denominaciones
        }

        // Captura global de teclas para poblar txtCodigoBarra mediante un escáner cuando el modo está activo
        private void AgregarProducto_KeyPress_Global(object sender, KeyPressEventArgs e)
        {
            // Buscar el checkbox (definido en el diseñador)
            var chkModoScanner = this.Controls.Find("chkModoScanner", true).FirstOrDefault() as CheckBox;
            if (chkModoScanner == null || !chkModoScanner.Checked) return; // no activo

            // Si el foco ya está en el textbox dejamos que funcione normalmente
            if (txtCodigoBarra != null && txtCodigoBarra.Focused) return;

            if (e.KeyChar == (char)Keys.Enter)
            {
                if (scannerBuffer.Length > 0)
                {
                    txtCodigoBarra.Text = scannerBuffer.ToString();
                    txtCodigoBarra.Focus();
                    txtCodigoBarra.SelectAll();
                    scannerBuffer.Clear();
                    e.Handled = true; // consumir Enter del escáner
                }
                return;
            }

            if (char.IsControl(e.KeyChar)) return; // ignorar otros controles distintos de Enter

            var now = DateTime.Now;
            if ((now - lastScannerCharTime).TotalMilliseconds > SCAN_TIMEOUT_MS)
            {
                scannerBuffer.Clear(); // inicio de una nueva lectura
            }
            lastScannerCharTime = now;
            scannerBuffer.Append(e.KeyChar);
            e.Handled = true; // evitar que el caracter vaya a otro control
        }

        // =================== NUEVAS VALIDACIONES =====================
        private void InicializarErrorProviderYValidaciones()
        {
            err = new ErrorProvider();
            err.BlinkStyle = ErrorBlinkStyle.NeverBlink;
            err.ContainerControl = this;

            // Configurar límites y eventos si los controles existen
            if (txtNombre != null)
            {
                txtNombre.MaxLength = 80;
                txtNombre.Validating += (s, ev) => { if (string.IsNullOrWhiteSpace(txtNombre.Text)) SetErr(txtNombre, "Requerido"); else ClearErr(txtNombre); };
                txtNombre.KeyPress += (s, ev) => { if (!char.IsControl(ev.KeyChar) && txtNombre.Text.Length >= txtNombre.MaxLength) ev.Handled = true; };
            }
            if (txtDescripcion != null)
            {
                txtDescripcion.MaxLength = 200;
            }
            if (txtCodigoBarra != null)
            {
                txtCodigoBarra.MaxLength = 60;
                txtCodigoBarra.KeyPress += TxtCodigoBarra_KeyPress;
                txtCodigoBarra.Validating += (s, ev) =>
                {
                    if (!string.IsNullOrWhiteSpace(txtCodigoBarra.Text) && txtCodigoBarra.Text.Length < 4)
                        SetErr(txtCodigoBarra, "Mínimo 4 caracteres");
                    else ClearErr(txtCodigoBarra);
                };
            }
            if (txtPrecio != null)
            {
                txtPrecio.KeyPress += TxtDecimal_KeyPress;
                txtPrecio.Validating += (s, ev) => { if (!ValidarDecimalPositivo(txtPrecio.Text)) SetErr(txtPrecio, "Monto inválido"); else ClearErr(txtPrecio); };
                txtPrecio.Leave += (s, ev) => FormatearDecimal(txtPrecio);
            }
            if (txtPrecioCosto != null)
            {
                txtPrecioCosto.KeyPress += TxtDecimal_KeyPress;
                txtPrecioCosto.Validating += (s, ev) => { if (!ValidarDecimalPositivo(txtPrecioCosto.Text)) SetErr(txtPrecioCosto, "Monto inválido"); else ClearErr(txtPrecioCosto); };
                txtPrecioCosto.Leave += (s, ev) => FormatearDecimal(txtPrecioCosto);
            }
            if (nudCantidad != null)
            {
                nudCantidad.Minimum = 0; // permitir 0 y validamos >0 aparte
                nudCantidad.Maximum = 100000;
                nudCantidad.ValueChanged += (s, ev) => { if (nudCantidad.Value <= 0) SetErr(nudCantidad, ">0"); else ClearErr(nudCantidad); };
            }
        }

        private void TxtCodigoBarra_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar)) return;
            // Permitir solo dígitos y letras, sin espacios
            if (!char.IsLetterOrDigit(e.KeyChar)) e.Handled = true;
        }

        private void TxtDecimal_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar)) return;
            var tb = sender as TextBox;
            char sep = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0];
            if (char.IsDigit(e.KeyChar)) return;
            if (e.KeyChar == sep && !tb.Text.Contains(sep)) return;
            e.Handled = true;
        }

        private bool ValidarDecimalPositivo(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto)) return false;
            if (decimal.TryParse(texto, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.CurrentCulture, out var val))
                return val >= 0;
            return false;
        }

        private void FormatearDecimal(TextBox tb)
        {
            if (tb == null) return;
            if (decimal.TryParse(tb.Text, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.CurrentCulture, out var val))
                tb.Text = val.ToString("0.00");
        }

        private void LimpiarErrores()
        {
            if (err == null) return;
            err.Clear();
        }

        private void SetErr(Control c, string mensaje)
        {
            if (c != null && err != null)
                err.SetError(c, mensaje);
        }
        private void ClearErr(Control c)
        {
            if (c != null && err != null)
                err.SetError(c, string.Empty);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnMarcarTodos_Click(object sender, EventArgs e)
        {
            var panel = this.Controls.Find("panelVariantes", true).FirstOrDefault() as Panel;
            if (panel == null) return;
            foreach (var chk in panel.Controls.OfType<CheckBox>()) chk.Checked = true;
        }

        private void btnDesmarcarTodos_Click(object sender, EventArgs e)
        {
            var panel = this.Controls.Find("panelVariantes", true).FirstOrDefault() as Panel;
            if (panel == null) return;
            foreach (var chk in panel.Controls.OfType<CheckBox>()) chk.Checked = false;
        }
    }
}
