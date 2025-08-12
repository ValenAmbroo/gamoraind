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
using Gamora_Indumentaria.Models;

namespace Gamora_Indumentaria
{
    public partial class ventas : Form
    {
        private List<ItemCarrito> carrito;
        private decimal totalVenta;

        // Controles definidos manualmente
        private Panel panel1;
        private Button button3;
        private GroupBox lblTitulo1;
        private Button btnAgregar1;
        private TextBox txtCodigoBarras1;
        private Label label1;
        private GroupBox groupBox2;
        private DataGridView dgvCarrito1;
        private Button button4;
        private Button button2;
        private GroupBox groupBox3;
        private Label label3;
        private Button ProcesarVentabtn;
        private ComboBox cmbMetodoPago1;
        private Label lblTotal;
        // Eliminado soporte de cliente (no se almacena)


        public ventas()
        {
            carrito = new List<ItemCarrito>();
            totalVenta = 0;
            DatabaseManager.InitializeDatabase(); // Usar el manager centralizado
            InitializeComponent();


        }

        private void AgregarProductoPorCodigo()
        {
            string codigoBarras = txtCodigoBarras1?.Text?.Trim();
            if (string.IsNullOrEmpty(codigoBarras))
            {
                MessageBox.Show("Ingrese un código de barras", "Atención",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string query = @"
                    SELECT i.Id, i.Nombre, i.Precio, i.Stock, c.Nombre as Categoria
                    FROM Inventario i
                    INNER JOIN Categorias c ON i.CategoriaId = c.Id
                    WHERE i.CodigoBarras = @codigo AND i.Stock > 0";

                SqlParameter[] parameters = {
                    new SqlParameter("@codigo", codigoBarras)
                };

                DataTable dt = DatabaseManager.ExecuteQuery(query, parameters);

                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    var producto = new Producto
                    {
                        Id = Convert.ToInt32(row["Id"]),
                        Nombre = row["Nombre"].ToString(),
                        Precio = Convert.ToDecimal(row["Precio"]),
                        Stock = Convert.ToInt32(row["Stock"]),
                        Categoria = row["Categoria"].ToString()
                    };

                    AgregarAlCarrito(producto);
                    txtCodigoBarras1.Clear();
                    txtCodigoBarras1.Focus();
                }
                else
                {
                    MessageBox.Show("Producto no encontrado o sin stock", "Producto no encontrado",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtCodigoBarras1.SelectAll();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar producto: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AgregarAlCarrito(Producto producto)
        {
            // Verificar si el producto ya está en el carrito
            var itemExistente = carrito.FirstOrDefault(x => x.ProductoId == producto.Id);

            if (itemExistente != null)
            {
                if (itemExistente.Cantidad < producto.Stock)
                {
                    itemExistente.Cantidad++;
                    itemExistente.Subtotal = itemExistente.Cantidad * itemExistente.PrecioUnitario;
                }
                else
                {
                    MessageBox.Show("No hay suficiente stock disponible", "Stock insuficiente",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            else
            {
                carrito.Add(new ItemCarrito
                {
                    ProductoId = producto.Id,
                    NombreProducto = producto.Nombre,
                    PrecioUnitario = producto.Precio,
                    Cantidad = 1,
                    Subtotal = producto.Precio
                });
            }

            ActualizarCarrito();
            ActualizarTotal();
        }

        private void ActualizarCarrito()
        {
            if (dgvCarrito1 != null)
            {
                dgvCarrito1.DataSource = null;
                dgvCarrito1.DataSource = carrito.Select(item => new
                {
                    Producto = item.NombreProducto,
                    Precio = item.PrecioUnitario.ToString("C"),
                    Cantidad = item.Cantidad,
                    Subtotal = item.Subtotal.ToString("C")
                }).ToList();

                dgvCarrito1.Columns["Producto"].HeaderText = "Producto";
                dgvCarrito1.Columns["Precio"].HeaderText = "Precio";
                dgvCarrito1.Columns["Cantidad"].HeaderText = "Cantidad";
                dgvCarrito1.Columns["Subtotal"].HeaderText = "Subtotal";
            }
        }

        private void ActualizarTotal()
        {
            totalVenta = carrito.Sum(x => x.Subtotal);
            if (lblTotal != null)
            {
                lblTotal.Text = string.Format("Total: {0:C}", totalVenta);
            }
        }

        private void ProcesarVenta()
        {
            if (carrito.Count == 0)
            {
                MessageBox.Show("No hay productos en el carrito", "Carrito vacío",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string metodoPago = cmbMetodoPago1?.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(metodoPago))
            {
                MessageBox.Show("Seleccione un método de pago", "Método de pago requerido",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                int ventaId = DatabaseManager.ProcesarVenta(carrito, metodoPago, totalVenta);

                MessageBox.Show(string.Format("Venta procesada exitosamente.\nVenta ID: {0}\nTotal: {1:C}\nMétodo: {2}",
                    ventaId, totalVenta, metodoPago),
                    "Venta Completada", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LimpiarCarrito();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al procesar venta: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LimpiarCarrito()
        {
            carrito.Clear();
            ActualizarCarrito();
            ActualizarTotal();
            txtCodigoBarras1?.Clear();
            txtCodigoBarras1?.Focus();
            if (cmbMetodoPago1 != null)
                cmbMetodoPago1.SelectedIndex = -1;
        }





        private void InitializeComponent()
        {
            this.button3 = new System.Windows.Forms.Button();
            this.lblTitulo1 = new System.Windows.Forms.GroupBox();
            this.btnAgregar1 = new System.Windows.Forms.Button();
            this.txtCodigoBarras1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.button4 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.dgvCarrito1 = new System.Windows.Forms.DataGridView();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.ProcesarVentabtn = new System.Windows.Forms.Button();
            this.cmbMetodoPago1 = new System.Windows.Forms.ComboBox();
            this.lblTotal = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lblTitulo1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCarrito1)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();

            // 
            // button3
            // 
            this.button3.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.button3.BackgroundImage = global::Gamora_Indumentaria.Properties.Resources.cerrrar_;
            this.button3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.button3.Dock = System.Windows.Forms.DockStyle.Right;
            this.button3.Location = new System.Drawing.Point(856, 0);
            this.button3.Margin = new System.Windows.Forms.Padding(2);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(33, 30);
            this.button3.TabIndex = 2;
            this.button3.UseVisualStyleBackColor = false;
            // 
            // lblTitulo1
            // 
            this.lblTitulo1.Controls.Add(this.btnAgregar1);
            this.lblTitulo1.Controls.Add(this.txtCodigoBarras1);
            this.lblTitulo1.Controls.Add(this.label1);
            this.lblTitulo1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitulo1.Location = new System.Drawing.Point(12, 50);
            this.lblTitulo1.Name = "lblTitulo1";
            this.lblTitulo1.Size = new System.Drawing.Size(848, 88);
            this.lblTitulo1.TabIndex = 3;
            this.lblTitulo1.TabStop = false;
            this.lblTitulo1.Text = "Escáner de Código de Barras";
            // 
            // btnAgregar1
            // 
            this.btnAgregar1.BackColor = System.Drawing.Color.LawnGreen;
            this.btnAgregar1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAgregar1.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.btnAgregar1.Location = new System.Drawing.Point(436, 32);
            this.btnAgregar1.Name = "btnAgregar1";
            this.btnAgregar1.Size = new System.Drawing.Size(88, 27);
            this.btnAgregar1.TabIndex = 2;
            this.btnAgregar1.Text = "+ agregar";
            this.btnAgregar1.UseVisualStyleBackColor = false;
            this.btnAgregar1.Click += new System.EventHandler(this.btnAgregar1_Click);
            // 
            // txtCodigoBarras1
            // 
            this.txtCodigoBarras1.Location = new System.Drawing.Point(166, 33);
            this.txtCodigoBarras1.Name = "txtCodigoBarras1";
            this.txtCodigoBarras1.Size = new System.Drawing.Size(219, 26);
            this.txtCodigoBarras1.TabIndex = 1;
            this.txtCodigoBarras1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TxtCodigoBarras1_KeyDown);
            this.txtCodigoBarras1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TxtCodigoBarras1_KeyPress);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(22, 38);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(138, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "Código de barras:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.button4);
            this.groupBox2.Controls.Add(this.button2);
            this.groupBox2.Controls.Add(this.dgvCarrito1);
            this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(12, 168);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(631, 287);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Carrito de compras";
            // 
            // button4
            // 
            this.button4.BackColor = System.Drawing.Color.Orange;
            this.button4.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button4.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.button4.Location = new System.Drawing.Point(184, 254);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(131, 27);
            this.button4.TabIndex = 4;
            this.button4.Text = "Limpiar Todo";
            this.button4.UseVisualStyleBackColor = false;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.Firebrick;
            this.button2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button2.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.button2.Location = new System.Drawing.Point(25, 254);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(135, 27);
            this.button2.TabIndex = 3;
            this.button2.Text = "- Eliminar Item";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // dgvCarrito1
            // 
            this.dgvCarrito1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCarrito1.Location = new System.Drawing.Point(25, 42);
            this.dgvCarrito1.Name = "dgvCarrito1";
            this.dgvCarrito1.Size = new System.Drawing.Size(576, 206);
            this.dgvCarrito1.TabIndex = 0;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.ProcesarVentabtn);
            this.groupBox3.Controls.Add(this.cmbMetodoPago1);
            this.groupBox3.Controls.Add(this.lblTotal);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox3.Location = new System.Drawing.Point(649, 168);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(211, 287);
            this.groupBox3.TabIndex = 5;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Procedimiento de Pago";
            // 
            // ProcesarVentabtn
            // 
            this.ProcesarVentabtn.BackColor = System.Drawing.Color.LawnGreen;
            this.ProcesarVentabtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ProcesarVentabtn.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.ProcesarVentabtn.Location = new System.Drawing.Point(9, 254);
            this.ProcesarVentabtn.Name = "ProcesarVentabtn";
            this.ProcesarVentabtn.Size = new System.Drawing.Size(196, 27);
            this.ProcesarVentabtn.TabIndex = 3;
            this.ProcesarVentabtn.Text = " Procesar Venta";
            this.ProcesarVentabtn.UseVisualStyleBackColor = false;
            this.ProcesarVentabtn.Click += new System.EventHandler(this.ProcesarVentabtn_Click);
            // 
            // cmbMetodoPago1
            // 
            this.cmbMetodoPago1.FormattingEnabled = true;
           
            this.cmbMetodoPago1.Location = new System.Drawing.Point(9, 83);
            this.cmbMetodoPago1.Name = "cmbMetodoPago1";
            this.cmbMetodoPago1.Size = new System.Drawing.Size(139, 28);
            this.cmbMetodoPago1.TabIndex = 5;
            // 
            // lblTotal
            // 
            this.lblTotal.AutoSize = true;
            this.lblTotal.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotal.ForeColor = System.Drawing.Color.Green;
            this.lblTotal.Location = new System.Drawing.Point(63, 42);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(49, 18);
            this.lblTotal.TabIndex = 4;
            this.lblTotal.Text = "$0,00";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.Green;
            this.label3.Location = new System.Drawing.Point(6, 42);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(51, 18);
            this.label3.TabIndex = 3;
            this.label3.Text = "Total:";
            // 
            // ventas
            // 
            this.ClientSize = new System.Drawing.Size(889, 512);
            this.ControlBox = false;
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.lblTitulo1);
            this.Name = "ventas";
            this.Load += new System.EventHandler(this.ventas_Load_1);
            this.lblTitulo1.ResumeLayout(false);
            this.lblTitulo1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvCarrito1)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        private void ventas_Load_1(object sender, EventArgs e)
        {
            // Configurar combo de métodos de pago
            if (cmbMetodoPago1 != null)
            {
                cmbMetodoPago1.Items.AddRange(new string[] {
                    "Efectivo",
                    "Tarjeta de Débito",
                    "Tarjeta de Crédito",
                    "Transferencia",
                    "Mercado Pago"
                });
            }

            txtCodigoBarras1?.Focus();
        }



        private void TxtCodigoBarras1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                AgregarProductoPorCodigo();
            }
        }

        private void btnAgregar1_Click(object sender, EventArgs e)
        {
            AgregarProductoPorCodigo();

        }

        private void ProcesarVentabtn_Click(object sender, EventArgs e)
        {
            ProcesarVenta();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("¿Está seguro de limpiar el carrito?", "Confirmar",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                LimpiarCarrito();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dgvCarrito1?.SelectedRows.Count > 0)
            {
                int index = dgvCarrito1.SelectedRows[0].Index;
                if (index >= 0 && index < carrito.Count)
                {
                    carrito.RemoveAt(index);
                    ActualizarCarrito();
                    ActualizarTotal();
                }
            }
        }

        private void TxtCodigoBarras1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnAgregar1_Click(sender, e);
            }
        }
    }
}
