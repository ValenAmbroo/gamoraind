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
    public partial class productos : Form
    {
        private InventarioDAL inventarioDAL;

        public productos()
        {
            InitializeComponent();
            inventarioDAL = new InventarioDAL();
        }

        private void productos_Load(object sender, EventArgs e)
        {
            CargarCategorias();
            ActualizarGrid();
        }

        /// <summary>
        /// Carga todas las categorías en el ComboBox
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
        /// Actualiza el DataGridView con los productos
        /// </summary>
        private void ActualizarGrid()
        {
            try
            {
                DataTable dt;

                if (cboCategoria.SelectedItem != null)
                {
                    Categoria categoria = (Categoria)cboCategoria.SelectedItem;
                    dt = inventarioDAL.ObtenerInventarioPorCategoria(categoria.Nombre);
                }
                else
                {
                    dt = inventarioDAL.ObtenerInventarioCompleto();
                }

                dgvInventario.DataSource = dt;

                // Configurar el aspecto del DataGridView
                ConfigurarDataGridView();
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Error al cargar inventario: {0}", ex.Message), "Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Configura el aspecto y columnas del DataGridView
        /// </summary>
        private void ConfigurarDataGridView()
        {
            if (dgvInventario.Columns.Count > 0)
            {
                // Ocultar columna ID
                if (dgvInventario.Columns["Id"] != null)
                    dgvInventario.Columns["Id"].Visible = false;

                // Configurar anchos de columnas
                if (dgvInventario.Columns["Categoria"] != null)
                    dgvInventario.Columns["Categoria"].Width = 120;

                if (dgvInventario.Columns["Producto"] != null)
                    dgvInventario.Columns["Producto"].Width = 200;

                if (dgvInventario.Columns["Descripcion"] != null)
                    dgvInventario.Columns["Descripcion"].Width = 250;

                if (dgvInventario.Columns["Talle"] != null)
                    dgvInventario.Columns["Talle"].Width = 80;

                if (dgvInventario.Columns["Sabor"] != null)
                    dgvInventario.Columns["Sabor"].Width = 100;

                if (dgvInventario.Columns["Cantidad"] != null)
                {
                    dgvInventario.Columns["Cantidad"].Width = 80;
                    dgvInventario.Columns["Cantidad"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }

                if (dgvInventario.Columns["PrecioVenta"] != null)
                {
                    dgvInventario.Columns["PrecioVenta"].Width = 100;
                    dgvInventario.Columns["PrecioVenta"].DefaultCellStyle.Format = "C";
                    dgvInventario.Columns["PrecioVenta"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }

                // Ocultar columnas de fechas para simplificar la vista
                if (dgvInventario.Columns["FechaCreacion"] != null)
                    dgvInventario.Columns["FechaCreacion"].Visible = false;

                if (dgvInventario.Columns["FechaModificacion"] != null)
                    dgvInventario.Columns["FechaModificacion"].Visible = false;

                if (dgvInventario.Columns["Activo"] != null)
                    dgvInventario.Columns["Activo"].Visible = false;

                // Marcar productos con stock bajo en rojo
                MarcarStockBajo();
            }
        }

        /// <summary>
        /// Marca las filas con stock bajo en color rojo
        /// </summary>
        private void MarcarStockBajo()
        {
            foreach (DataGridViewRow row in dgvInventario.Rows)
            {
                if (row.Cells["Cantidad"] != null && row.Cells["Cantidad"].Value != null)
                {
                    int cantidad = Convert.ToInt32(row.Cells["Cantidad"].Value);
                    if (cantidad <= 5)
                    {
                        row.DefaultCellStyle.BackColor = Color.LightPink;
                        row.DefaultCellStyle.ForeColor = Color.DarkRed;
                    }
                }
            }
        }

        /// <summary>
        /// Maneja el cambio de categoría
        /// </summary>
        private void cboCategoria_SelectedIndexChanged(object sender, EventArgs e)
        {
            ActualizarGrid();
        }

        /// <summary>
        /// Abre el formulario para agregar un producto
        /// </summary>
        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if (cboCategoria.SelectedItem != null)
            {
                Categoria categoria = (Categoria)cboCategoria.SelectedItem;
                agregarpruducto form = new agregarpruducto(categoria.Nombre);

                // Usar ShowDialog para esperar a que se cierre el formulario
                if (form.ShowDialog() == DialogResult.OK)
                {
                    ActualizarGrid(); // Recargar después de agregar
                }
            }
            else
            {
                // Si no hay categoría seleccionada, abrir el formulario sin preselección
                agregarpruducto form = new agregarpruducto();
                if (form.ShowDialog() == DialogResult.OK)
                {
                    ActualizarGrid();
                }
            }
        }

        private void btnAgregar_Click_1(object sender, EventArgs e)
        {
            // Mantener la funcionalidad existente si se usa en otro lugar
            btnAgregar_Click(sender, e);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dgvInventario_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Aquí se puede agregar funcionalidad para editar productos al hacer click
        }

        private void btnRefrescar_Click(object sender, EventArgs e)
        {
            ActualizarGrid();
        }

        private void cboCategoria_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            // Mantener la funcionalidad existente
            cboCategoria_SelectedIndexChanged(sender, e);
        }
    }
}




