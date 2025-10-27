 using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Gamora_Indumentaria.Data;
using Gamora_Indumentaria.Models;

namespace Gamora_Indumentaria
{
    public partial class NotasForm : Form
    {
        private readonly NotasDAL dal = new NotasDAL();
        private List<Nota> cache = new List<Nota>();
        private Nota notaActual;

        public NotasForm()
        {
            InitializeComponent();
            DoubleBuffered = true;
        }

        private void NotasForm_Load(object sender, EventArgs e)
        {
            CargarNotas();
            AplicarEstilos();
        }

        private void AplicarEstilos()
        {
            if (dgvNotas != null)
            {
                dgvNotas.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(230, 235, 240);
                dgvNotas.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(50, 60, 70);
                dgvNotas.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                dgvNotas.DefaultCellStyle.SelectionBackColor = Color.FromArgb(210, 225, 245);
                dgvNotas.DefaultCellStyle.SelectionForeColor = Color.Black;
            }
        }

        private void CargarNotas()
        {
            cache = dal.ObtenerNotas();
            dgvNotas.DataSource = cache
                .OrderByDescending(n => n.Fecha)
                .ThenByDescending(n => n.Id)
                .Select(n => new { n.Id, n.Titulo, Fecha = n.Fecha.ToString("dd/MM/yyyy") })
                .ToList();
            if (dgvNotas.Columns.Contains("Id"))
            {
                dgvNotas.Columns["Id"].Visible = false;
            }
            dgvNotas.ClearSelection();
            LimpiarCampos();
        }

        private void LimpiarCampos()
        {
            notaActual = null;
            txtTitulo.Clear();
            dtpFecha.Value = DateTime.Now;
            txtTexto.Clear();
        }

        private void ClearToNew()
        {
            // Limpiar selección y preparar para nueva nota
            if (dgvNotas != null)
            {
                dgvNotas.ClearSelection();
            }
            LimpiarCampos();
        }

        // Click en el fondo del formulario: preparar para nueva nota
        private void NotasForm_MouseDown(object sender, MouseEventArgs e)
        {
            // Si el click fue sobre el formulario (no sobre controles hijos)
            var ctrl = this.GetChildAtPoint(e.Location);
            if (ctrl == null)
            {
                ClearToNew();
            }
        }

        // Click en el panel superior (fondo): preparar para nueva nota
        private void panelTop_Click(object sender, EventArgs e)
        {
            ClearToNew();
        }

        // Click en el panel derecho (fondo): preparar para nueva nota
        private void panelRight_Click(object sender, EventArgs e)
        {
            ClearToNew();
        }

        // Click en área vacía de la grilla: limpiar selección y preparar nueva nota
        private void dgvNotas_MouseDown(object sender, MouseEventArgs e)
        {
            var hit = dgvNotas.HitTest(e.X, e.Y);
            if (hit.RowIndex < 0)
            {
                ClearToNew();
            }
        }

        private void dgvNotas_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvNotas.CurrentRow == null) return;
            if (dgvNotas.CurrentRow.Cells["Id"] == null) return;
            int id = Convert.ToInt32(dgvNotas.CurrentRow.Cells["Id"].Value);
            notaActual = cache.FirstOrDefault(n => n.Id == id);
            if (notaActual == null) return;

            txtTitulo.Text = notaActual.Titulo;
            dtpFecha.Value = notaActual.Fecha;
            txtTexto.Text = notaActual.Texto;
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
            txtTitulo.Focus();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            string titulo = txtTitulo.Text.Trim();
            string texto = txtTexto.Text;
            if (string.IsNullOrWhiteSpace(titulo))
            {
                MessageBox.Show("Ingresá un título para la nota", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                if (notaActual == null)
                {
                    int newId = dal.AgregarNota(titulo, dtpFecha.Value, texto);
                }
                else
                {
                    notaActual.Titulo = titulo;
                    notaActual.Fecha = dtpFecha.Value;
                    notaActual.Texto = texto;
                    dal.ActualizarNota(notaActual);
                }
                CargarNotas();
                MessageBox.Show("Nota guardada", "Notas", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error guardando la nota: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (notaActual == null)
            {
                MessageBox.Show("Seleccioná una nota para eliminar", "Notas", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            var confirm = MessageBox.Show($"¿Eliminar la nota '{notaActual.Titulo}'?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirm != DialogResult.Yes) return;
            try
            {
                dal.EliminarNota(notaActual.Id);
                CargarNotas();
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo eliminar la nota: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
