using System;
using System.Linq;
using System.Windows.Forms;
using Gamora_Indumentaria.Data;

namespace Gamora_Indumentaria
{
    public partial class agregarcategoria : Form
    {
        private InventarioDAL dal = new InventarioDAL();

        public int NuevaCategoriaId { get; private set; }

        public agregarcategoria()
        {
            InitializeComponent();
        }

        private void chkTieneTalle_CheckedChanged(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => chkTieneTalle_CheckedChanged(sender, e)));
                return;
            }
            txtTalles.Visible = chkTieneTalle.Checked;
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            string nombre = txtNombre.Text.Trim();
            if (string.IsNullOrWhiteSpace(nombre))
            {
                MessageBox.Show("Ingrese el nombre de la categoría", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                NuevaCategoriaId = dal.AgregarCategoria(nombre);
                // Si el usuario marcó que la categoría tiene talles, parsearlos e insertarlos
                if (chkTieneTalle.Checked)
                {
                    var raw = txtTalles.Text ?? string.Empty;
                    var parts = raw.Split(new[] { '\n', '\r', ',' }, StringSplitOptions.RemoveEmptyEntries)
                                        .Select(x => x.Trim())
                                        .Where(x => !string.IsNullOrWhiteSpace(x))
                                        .ToList();
                    if (parts.Count > 0)
                    {
                        dal.AgregarTalles(NuevaCategoriaId, parts);
                    }
                }
                MessageBox.Show("Categoría agregada correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                // Avisar al resto de la aplicación que hay una nueva categoría
                AppEvents.RaiseCategoryAdded(NuevaCategoriaId);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al agregar categoría:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
