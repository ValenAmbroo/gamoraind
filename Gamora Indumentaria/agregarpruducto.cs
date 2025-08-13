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
            if (categoriaSeleccionada == null)
            {
                MessageBox.Show("Por favor, selecciona una categoría.", "Validación",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("Por favor, ingresa el nombre del producto.", "Validación",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNombre.Focus();
                return false;
            }

            if (categoriaSeleccionada.TieneTalle && cboTalle.SelectedIndex == -1)
            {
                MessageBox.Show("Por favor, selecciona un talle.", "Validación",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cboTalle.Focus();
                return false;
            }

            if (EsCategoriaVaper(categoriaSeleccionada) && string.IsNullOrWhiteSpace(txtSabor.Text))
            {
                MessageBox.Show("Por favor, ingresa el sabor del vaper.", "Validación",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSabor.Focus();
                return false;
            }

            if (nudCantidad.Value <= 0)
            {
                MessageBox.Show("La cantidad debe ser mayor a cero.", "Validación",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                nudCantidad.Focus();
                return false;
            }

            // Precios requeridos
            if (string.IsNullOrWhiteSpace(txtPrecio.Text))
            {
                MessageBox.Show("Por favor, ingresa el precio de venta.", "Validación",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPrecio.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtPrecioCosto.Text))
            {
                MessageBox.Show("Por favor, ingresa el precio de compra.", "Validación",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPrecioCosto.Focus();
                return false;
            }

            // Validar precio si se ingresó
            if (!string.IsNullOrWhiteSpace(txtPrecio.Text))
            {
                decimal precio;
                if (!decimal.TryParse(txtPrecio.Text, out precio) || precio < 0)
                {
                    MessageBox.Show("El precio debe ser un número válido mayor o igual a cero.", "Validación",
                                  MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtPrecio.Focus();
                    return false;
                }
            }

            if (!string.IsNullOrWhiteSpace(txtPrecioCosto.Text))
            {
                decimal precioC;
                if (!decimal.TryParse(txtPrecioCosto.Text, out precioC) || precioC < 0)
                {
                    MessageBox.Show("El precio de compra debe ser un número válido >= 0.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtPrecioCosto.Focus();
                    return false;
                }
            }

            if (!string.IsNullOrWhiteSpace(txtPrecio.Text) && !string.IsNullOrWhiteSpace(txtPrecioCosto.Text))
            {
                decimal pv, pc;
                if (decimal.TryParse(txtPrecio.Text, out pv) && decimal.TryParse(txtPrecioCosto.Text, out pc))
                {
                    if (pc > pv)
                    {
                        var res = MessageBox.Show("El precio de compra es mayor que el de venta. ¿Desea continuar?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (res == DialogResult.No) return false;
                    }
                }
            }

            return true;
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
    }
}
