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

        public agregarpruducto()
        {
            InitializeComponent();
            inventarioDAL = new InventarioDAL();
            CargarCategorias();
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
                bool esVaper = categoriaSeleccionada.Nombre == "VAPER";

                lblTalle.Visible = esTalleVisible;
                cboTalle.Visible = esTalleVisible;

                lblSabor.Visible = esVaper;
                txtSabor.Visible = esVaper;

                // Cargar talles si la categoría los tiene
                if (esTalleVisible)
                {
                    CargarTallesPorCategoria(categoriaSeleccionada.Id);
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
                    Sabor = categoriaSeleccionada.Nombre == "VAPER" && !string.IsNullOrWhiteSpace(txtSabor.Text) ?
                           txtSabor.Text.Trim() : null,
                    Cantidad = (int)nudCantidad.Value,
                    PrecioVenta = string.IsNullOrWhiteSpace(txtPrecio.Text) ? null :
                                 (decimal?)Convert.ToDecimal(txtPrecio.Text)
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

            if (categoriaSeleccionada.Nombre == "VAPER" && string.IsNullOrWhiteSpace(txtSabor.Text))
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
    }
}
