using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Gamora_Indumentaria
{
    public class AgregarCantidadPorTalleForm : Form
    {
        private DataGridView dgv;
        private Button btnOk;
        private Button btnCancel;
        private DataGridViewTextBoxColumn TalleId;
        private DataGridViewTextBoxColumn Talle;
        private DataGridViewTextBoxColumn Cantidad;
        private List<Tuple<int?, string, int>> items; // TalleId, TalleValor, Quantity

        public List<int> Quantities { get; private set; }

        public AgregarCantidadPorTalleForm(List<Tuple<int?, string, int>> entradas)
        {
            this.items = entradas ?? new List<Tuple<int?, string, int>>();
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.dgv = new System.Windows.Forms.DataGridView();
            this.TalleId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Talle = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Cantidad = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).BeginInit();
            this.SuspendLayout();
            // 
            // dgv
            // 
            this.dgv.AllowUserToAddRows = false;
            this.dgv.AllowUserToDeleteRows = false;
            this.dgv.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgv.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.TalleId,
            this.Talle,
            this.Cantidad});
            this.dgv.Dock = System.Windows.Forms.DockStyle.Top;
            this.dgv.Location = new System.Drawing.Point(0, 0);
            this.dgv.Name = "dgv";
            this.dgv.RowHeadersVisible = false;
            this.dgv.Size = new System.Drawing.Size(562, 260);
            this.dgv.TabIndex = 0;
            // 
            // TalleId
            // 
            this.TalleId.Name = "TalleId";
            this.TalleId.Visible = false;
            // 
            // Talle
            // 
            this.Talle.HeaderText = "Talle";
            this.Talle.Name = "Talle";
            this.Talle.ReadOnly = true;
            // 
            // Cantidad
            // 
            this.Cantidad.HeaderText = "Cantidad";
            this.Cantidad.Name = "Cantidad";
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(11)))), ((int)(((byte)(7)))), ((int)(((byte)(17)))));
            this.btnOk.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.btnOk.Location = new System.Drawing.Point(438, 300);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(99, 38);
            this.btnOk.TabIndex = 1;
            this.btnOk.Text = "Aceptar";
            this.btnOk.UseVisualStyleBackColor = false;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnCancel.Location = new System.Drawing.Point(318, 300);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(103, 38);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancelar";
            this.btnCancel.UseVisualStyleBackColor = false;
            // 
            // AgregarCantidadPorTalleForm
            // 
            this.ClientSize = new System.Drawing.Size(562, 356);
            this.Controls.Add(this.dgv);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AgregarCantidadPorTalleForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Asignar cantidades por talle";
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).EndInit();
            this.ResumeLayout(false);

        }

        private void LoadData()
        {
            dgv.Rows.Clear();
            foreach (var it in items)
            {
                var idx = dgv.Rows.Add();
                var row = dgv.Rows[idx];
                row.Cells[0].Value = it.Item1.HasValue ? (object)it.Item1.Value : DBNull.Value;
                row.Cells[1].Value = it.Item2 ?? string.Empty;
                row.Cells[2].Value = it.Item3;
            }
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            var cantidades = new List<int>();
            for (int i = 0; i < dgv.Rows.Count; i++)
            {
                var cell = dgv.Rows[i].Cells[2];
                if (cell.Value == null || string.IsNullOrWhiteSpace(cell.Value.ToString()))
                {
                    MessageBox.Show("Complete todas las cantidades (puede usar 0).", "Validadción", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (!int.TryParse(cell.Value.ToString(), out int q) || q < 0)
                {
                    MessageBox.Show("Ingrese cantidades válidas (enteros >= 0).", "Validadción", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                cantidades.Add(q);
            }

            this.Quantities = cantidades;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void AgregarCantidadPorTalleForm_Load(object sender, EventArgs e)
        {
            // Reposition buttons based on runtime sizes
            btnOk.Location = new Point(this.ClientSize.Width - 220, dgv.Bottom + 12);
            btnCancel.Location = new Point(this.ClientSize.Width - 110, dgv.Bottom + 12);
        }
    }
}
