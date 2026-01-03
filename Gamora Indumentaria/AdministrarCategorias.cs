using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Gamora_Indumentaria.Data;

namespace Gamora_Indumentaria
{
    public partial class AdministrarCategorias : Form
    {
        private readonly InventarioDAL dal = new InventarioDAL();
        private List<Categoria> cacheCategorias = new List<Categoria>();
        private Categoria categoriaActual;
        private bool cargando = true;

        public AdministrarCategorias()
        {
            InitializeComponent();
            DoubleBuffered = true;
            AplicarEstilosRuntime();
            CargarCategorias();
            cargando = false;
        }

        private void AdministrarCategorias_Load(object sender, EventArgs e)
        {
            AplicarEstilosRuntime();
        }

        private void AplicarEstilosRuntime()
        {
            if (dgvCategorias != null)
            {
                dgvCategorias.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(230, 235, 240);
                dgvCategorias.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(50, 60, 70);
                dgvCategorias.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                dgvCategorias.DefaultCellStyle.SelectionBackColor = Color.FromArgb(210, 225, 245);
                dgvCategorias.DefaultCellStyle.SelectionForeColor = Color.Black;

                // Asegurar columna visual y renumeración automática
                dgvCategorias.DataBindingComplete -= DgvCategorias_DataBindingComplete;
                dgvCategorias.DataBindingComplete += DgvCategorias_DataBindingComplete;
                dgvCategorias.Sorted -= (s, e) => { Renumerar(dgvCategorias); };
                dgvCategorias.Sorted += (s, e) => { Renumerar(dgvCategorias); };
                dgvCategorias.RowsAdded -= (s, e) => { Renumerar(dgvCategorias); };
                dgvCategorias.RowsAdded += (s, e) => { Renumerar(dgvCategorias); };
                dgvCategorias.RowsRemoved -= (s, e) => { Renumerar(dgvCategorias); };
                dgvCategorias.RowsRemoved += (s, e) => { Renumerar(dgvCategorias); };
            }
        }

        private void CargarCategorias()
        {
            try
            {
                cacheCategorias = dal.ObtenerCategorias();
                AplicarData(cacheCategorias);
                dgvCategorias.ClearSelection();
                categoriaActual = null;
                LimpiarEdicion();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error cargando categorías: " + ex.Message, "Categorías", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AplicarData(IEnumerable<Categoria> origen)
        {
            // Ordenar por Id ascendente (requerido por el usuario)
            dgvCategorias.DataSource = origen
                .OrderBy(c => c.Id)
                .Select(c => new { c.Id, c.Nombre, Talles = c.TieneTalle ? (string.IsNullOrEmpty(c.TipoTalle) ? "Sí" : c.TipoTalle) : "No" })
                .ToList();

            // Insertar columna de índice visual y numerar
            AsegurarColumnaIndice(dgvCategorias);
            Renumerar(dgvCategorias);
        }

        private void Filtrar()
        {
            if (cacheCategorias == null) return;
            var filtro = (txtBuscar.Text ?? string.Empty).Trim().ToUpperInvariant();
            if (string.IsNullOrEmpty(filtro))
            {
                AplicarData(cacheCategorias);
            }
            else
            {
                var res = cacheCategorias.Where(c => c.Nombre.ToUpperInvariant().Contains(filtro));
                AplicarData(res);
            }
        }

        private void DgvCategorias_SelectionChanged(object sender, EventArgs e)
        {
            if (cargando) return;
            if (dgvCategorias.CurrentRow == null) return;
            if (dgvCategorias.CurrentRow.Cells["Id"] == null) return;
            int id = Convert.ToInt32(dgvCategorias.CurrentRow.Cells["Id"].Value);
            categoriaActual = cacheCategorias.FirstOrDefault(c => c.Id == id);
            if (categoriaActual == null) return;

            txtNombre.Text = categoriaActual.Nombre;
            var talles = dal.ObtenerTallesPorCategoria(id);
            chkTieneTalle.Checked = talles.Count > 0 || categoriaActual.TieneTalle;
            txtTalles.Enabled = chkTieneTalle.Checked;
            txtTalles.BackColor = chkTieneTalle.Checked ? Color.White : Color.Gainsboro;
            txtTalles.Text = talles.Count > 0 ? string.Join(Environment.NewLine, talles.OrderBy(t => t.Orden).Select(t => t.TalleValor)) : string.Empty;
        }

        private void BtnNuevo_Click(object sender, EventArgs e)
        {
            categoriaActual = null;
            LimpiarEdicion();
            txtNombre.Focus();
        }

        private void LimpiarEdicion()
        {
            txtNombre.Clear();
            chkTieneTalle.Checked = false;
            txtTalles.Clear();
            txtTalles.Enabled = false;
            txtTalles.BackColor = Color.Gainsboro;
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            string nombre = txtNombre.Text.Trim();
            if (string.IsNullOrWhiteSpace(nombre))
            {
                MessageBox.Show("Ingrese un nombre de categoría", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                List<string> talles = null;
                if (chkTieneTalle.Checked)
                {
                    talles = (txtTalles.Text ?? string.Empty)
                        .Split(new[] { '\n', '\r', ',' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(x => x.Trim())
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .ToList();
                }

                if (categoriaActual == null)
                {
                    int newId = dal.AgregarCategoria(nombre);
                    if (chkTieneTalle.Checked && talles != null && talles.Count > 0)
                        dal.ReemplazarTalles(newId, talles);
                }
                else
                {
                    dal.ActualizarCategoria(categoriaActual.Id, nombre);
                    if (chkTieneTalle.Checked)
                        dal.ReemplazarTalles(categoriaActual.Id, talles ?? new List<string>());
                    else
                        dal.ReemplazarTalles(categoriaActual.Id, new List<string>());
                }
                CargarCategorias();
                MessageBox.Show("Categoría guardada", "Categorías", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error guardando categoría: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnEliminar_Click(object sender, EventArgs e)
        {
            if (categoriaActual == null)
            {
                MessageBox.Show("Seleccione una categoría", "Eliminar", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            // Ya no bloqueamos por productos asociados: la lógica interna hace limpieza/desasociación.
            var confirm = MessageBox.Show($"¿Eliminar la categoría '{categoriaActual.Nombre}'?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirm != DialogResult.Yes) return;
            try
            {
                dal.EliminarCategoria(categoriaActual.Id);
                CargarCategorias();
                MessageBox.Show("Categoría eliminada", "Categorías", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo eliminar: " + ex.Message, "Eliminar", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ChkTieneTalle_CheckedChanged(object sender, EventArgs e)
        {
            txtTalles.Enabled = chkTieneTalle.Checked;
            txtTalles.BackColor = chkTieneTalle.Checked ? Color.White : Color.Gainsboro;
        }

        private void TxtBuscar_TextChanged(object sender, EventArgs e) => Filtrar();
        private void BtnCerrar_Click(object sender, EventArgs e) => Close();

        // Mantiene una columna visual "#" con 1..N independiente del Id real
        private void AsegurarColumnaIndice(DataGridView dgv)
        {
            if (dgv == null) return;
            if (!dgv.Columns.Contains("IdVisual"))
            {
                var col = new DataGridViewTextBoxColumn
                {
                    Name = "IdVisual",
                    HeaderText = "#",
                    ReadOnly = true,
                    Width = 45,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                    SortMode = DataGridViewColumnSortMode.NotSortable
                };
                dgv.Columns.Insert(0, col);
            }

            // Mover la columna Id real para que quede después del índice visual
            if (dgv.Columns.Contains("Id"))
            {
                try
                {
                    dgv.Columns["Id"].DisplayIndex = Math.Min(dgv.Columns["Id"].DisplayIndex + 1, dgv.Columns.Count - 1);
                    // Ocultar el Id de base de datos en la grilla
                    dgv.Columns["Id"].Visible = false;
                }
                catch { /* ignorar si no aplica */ }
            }
        }

        private void Renumerar(DataGridView dgv)
        {
            if (dgv == null || !dgv.Columns.Contains("IdVisual")) return;
            int i = 1;
            foreach (DataGridViewRow row in dgv.Rows)
            {
                if (row.IsNewRow) continue;
                row.Cells["IdVisual"].Value = i++;
            }
        }

        private void DgvCategorias_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            AsegurarColumnaIndice(dgvCategorias);
            if (dgvCategorias.Columns.Contains("Id")) dgvCategorias.Columns["Id"].Visible = false;
            Renumerar(dgvCategorias);
        }
    }
}
