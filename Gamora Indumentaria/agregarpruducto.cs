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
            // Activar vista previa de teclas para modo escáner
            this.KeyPreview = true;
            this.KeyPress += AgregarProducto_KeyPress_Global;
            InicializarErrorProviderYValidaciones();
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
            cboTalle.DataSource = null;
            cboTalle.Items.Clear();
            txtSabor.Text = "";

            // Mostrar/ocultar campos según la categoría
            bool tieneCategoria = categoriaSeleccionada != null;

            if (tieneCategoria)
            {
                // Configurar visibilidad de campos según la categoría
                bool esTalleVisible = categoriaSeleccionada.TieneTalle;
                bool esVaper = EsCategoriaVaper(categoriaSeleccionada);

                lblTalle.Visible = esTalleVisible;
                cboTalle.Visible = esTalleVisible;

                lblSabor.Visible = esVaper;
                txtSabor.Visible = esVaper;

                // Cargar talles si la categoría los tiene
                if (esTalleVisible)
                {
                    CargarTallesPorCategoria(categoriaSeleccionada.Id);
                }

                if (esVaper)
                {
                    CargarSaboresVaper();
                    ConfigurarAutocompleteSabor();
                }
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

                cboTalle.DataSource = tallesDisponibles;
                cboTalle.DisplayMember = "TalleValor";
                cboTalle.ValueMember = "Id";

                if (tallesDisponibles.Count > 0)
                {
                    cboTalle.SelectedIndex = 0;
                }
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

            try
            {
                var producto = new ProductoInventario
                {
                    CategoriaId = categoriaSeleccionada.Id,
                    Nombre = txtNombre.Text.Trim(),
                    Descripcion = string.IsNullOrWhiteSpace(txtDescripcion.Text) ? null : txtDescripcion.Text.Trim(),
                    CodigoBarra = string.IsNullOrWhiteSpace(txtCodigoBarra.Text) ? null : txtCodigoBarra.Text.Trim(),
                    TalleId = categoriaSeleccionada.TieneTalle && cboTalle.SelectedValue != null ?
                             (int?)cboTalle.SelectedValue : null,
                    Sabor = EsCategoriaVaper(categoriaSeleccionada) && !string.IsNullOrWhiteSpace(txtSabor.Text) ?
                           txtSabor.Text.Trim() : null,
                    Cantidad = (int)nudCantidad.Value,
                    PrecioVenta = string.IsNullOrWhiteSpace(txtPrecio.Text) ? null :
                                 (decimal?)Convert.ToDecimal(txtPrecio.Text)
                    ,
                    PrecioCosto = string.IsNullOrWhiteSpace(txtPrecioCosto.Text) ? null : (decimal?)Convert.ToDecimal(txtPrecioCosto.Text)
                };

                int nuevoId = inventarioDAL.AgregarProducto(producto);

                MessageBox.Show(string.Format("Producto agregado exitosamente con ID: {0}", nuevoId), "Éxito",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);

                LimpiarFormulario();
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Error al guardar el producto: {0}", ex.Message), "Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarSaboresVaper()
        {
            try
            {
                string q = "SELECT DISTINCT Sabor FROM Inventario WHERE Sabor IS NOT NULL AND LTRIM(RTRIM(Sabor)) <> '' ORDER BY Sabor";
                DataTable dt = DatabaseManager.ExecuteQuery(q);
                List<string> sabores = new List<string>();
                foreach (DataRow r in dt.Rows)
                {
                    sabores.Add(r["Sabor"].ToString());
                }
                // Si existe un ComboBox cboSabor lo llenamos; si solo TextBox, usamos AutoComplete
                if (this.Controls.Find("cboSabor", true).FirstOrDefault() is ComboBox cboSabor)
                {
                    cboSabor.Items.Clear();
                    cboSabor.Items.AddRange(sabores.ToArray());
                }
                saboresVaper = sabores;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error cargando sabores: " + ex.Message);
            }
        }

        private List<string> saboresVaper = new List<string>();
        private void ConfigurarAutocompleteSabor()
        {
            if (txtSabor == null) return;
            var source = new AutoCompleteStringCollection();
            source.AddRange(saboresVaper.ToArray());
            txtSabor.AutoCompleteSource = AutoCompleteSource.CustomSource;
            txtSabor.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            txtSabor.AutoCompleteCustomSource = source;
        }

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

            if (categoriaSeleccionada.TieneTalle && cboTalle.SelectedIndex == -1)
            {
                SetErr(cboTalle, "Seleccione un talle");
            }

            if (EsCategoriaVaper(categoriaSeleccionada) && string.IsNullOrWhiteSpace(txtSabor.Text))
            {
                SetErr(txtSabor, "Sabor requerido");
            }

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
            bool hayErrores = new Control[] { cboCategoria, txtNombre, cboTalle, txtSabor, nudCantidad, txtPrecio, txtPrecioCosto }
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
            txtSabor.Text = "";
            txtPrecio.Text = "";
            txtPrecioCosto.Text = "";
            nudCantidad.Value = 1;

            if (cboTalle.Items.Count > 0)
                cboTalle.SelectedIndex = 0;

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
    }
}
