using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Gamora_Indumentaria.Models;

namespace Gamora_Indumentaria
{
    public class SeleccionarProductoForm : Form
    {
        public Producto ProductoSeleccionado { get; private set; }

        public SeleccionarProductoForm(List<Producto> productos)
        {
            // No designer used; construir controles en código
            CargarProductos(productos);
        }

        private void CargarProductos(List<Producto> productos)
        {
            var dgv = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            dgv.Columns.Add("Id", "Id");
            dgv.Columns.Add("Nombre", "Nombre");
            dgv.Columns.Add("Categoria", "Categoría");
            dgv.Columns.Add("Precio", "Precio");
            dgv.Columns.Add("Stock", "Stock");
            dgv.Columns.Add("CodigoBarras", "Código de barras");

            foreach (var p in productos)
            {
                dgv.Rows.Add(p.Id, p.Nombre, p.Categoria, p.Precio.ToString("F2"), p.Stock, p.CodigoBarras);
            }

            dgv.DoubleClick += (s, e) => AceptarSeleccion(dgv);

            var btnAceptar = new Button { Text = "Aceptar", Dock = DockStyle.Bottom, Height = 30 };
            btnAceptar.Click += (s, e) => AceptarSeleccion(dgv);

            var btnCancelar = new Button { Text = "Cancelar", Dock = DockStyle.Bottom, Height = 30 };
            btnCancelar.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };

            this.Controls.Add(dgv);
            this.Controls.Add(btnCancelar);
            this.Controls.Add(btnAceptar);
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Seleccionar producto";
            this.Width = 700;
            this.Height = 350;
        }

        private void AceptarSeleccion(DataGridView dgv)
        {
            if (dgv.SelectedRows.Count == 0)
                return;

            var row = dgv.SelectedRows[0];
            ProductoSeleccionado = new Producto
            {
                Id = Convert.ToInt32(row.Cells[0].Value),
                Nombre = row.Cells[1].Value?.ToString(),
                Categoria = row.Cells[2].Value?.ToString(),
                Precio = Convert.ToDecimal(row.Cells[3].Value),
                Stock = Convert.ToInt32(row.Cells[4].Value),
                CodigoBarras = row.Cells[5].Value?.ToString()
            };

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // SeleccionarProductoForm
            // 
            this.ClientSize = new System.Drawing.Size(457, 343);
            this.Name = "SeleccionarProductoForm";
            this.ResumeLayout(false);

        }
    }
}
