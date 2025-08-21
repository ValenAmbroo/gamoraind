namespace Gamora_Indumentaria
{
    partial class ventas
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblTitulo = new System.Windows.Forms.Label();
            this.grpScanner = new System.Windows.Forms.GroupBox();
            this.btnAgregar = new System.Windows.Forms.Button();
            this.txtCodigoBarras = new System.Windows.Forms.TextBox();
            this.lblCodigoBarras = new System.Windows.Forms.Label();
            this.grpCarrito = new System.Windows.Forms.GroupBox();
            this.btnEliminarItem = new System.Windows.Forms.Button();
            this.btnLimpiarCarrito = new System.Windows.Forms.Button();
            this.dgvCarrito = new System.Windows.Forms.DataGridView();
            this.grpPago = new System.Windows.Forms.GroupBox();
            this.btnProcesarVenta = new System.Windows.Forms.Button();
            this.cmbMetodoPago = new System.Windows.Forms.ComboBox();
            this.lblMetodoPago = new System.Windows.Forms.Label();
            this.lblTotal = new System.Windows.Forms.Label();
            this.roundedPanel1 = new System.Windows.Forms.Panel();
            this.grpScanner.SuspendLayout();
            this.grpCarrito.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCarrito)).BeginInit();
            this.grpPago.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblTitulo
            // 
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitulo.ForeColor = System.Drawing.Color.FromArgb(33, 37, 41);
            this.lblTitulo.Location = new System.Drawing.Point(24, 18);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(320, 37);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "🛒 Punto de Venta Moderno";
            // 
            // grpScanner
            // 
            this.grpScanner.Controls.Add(this.btnAgregar);
            this.grpScanner.Controls.Add(this.txtCodigoBarras);
            this.grpScanner.Controls.Add(this.lblCodigoBarras);
            this.grpScanner.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpScanner.Location = new System.Drawing.Point(24, 70);
            this.grpScanner.Name = "grpScanner";
            this.grpScanner.Size = new System.Drawing.Size(760, 80);
            this.grpScanner.TabIndex = 1;
            this.grpScanner.TabStop = false;
            this.grpScanner.Text = "📱 Escáner de Productos";
            this.grpScanner.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);
            this.grpScanner.ForeColor = System.Drawing.Color.FromArgb(52, 58, 64);
            // 
            // btnAgregar
            // 
            this.btnAgregar.BackColor = System.Drawing.Color.FromArgb(0, 123, 255);
            this.btnAgregar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAgregar.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAgregar.ForeColor = System.Drawing.Color.White;
            this.btnAgregar.Location = new System.Drawing.Point(620, 30);
            this.btnAgregar.Name = "btnAgregar";
            this.btnAgregar.Size = new System.Drawing.Size(120, 35);
            this.btnAgregar.TabIndex = 2;
            this.btnAgregar.Text = "➕ AGREGAR";
            this.btnAgregar.UseVisualStyleBackColor = false;
            this.btnAgregar.FlatAppearance.BorderSize = 0;
            this.btnAgregar.Region = new System.Drawing.Region(new System.Drawing.Drawing2D.GraphicsPath(new System.Drawing.PointF[] { new System.Drawing.PointF(0, 10), new System.Drawing.PointF(10, 0), new System.Drawing.PointF(110, 0), new System.Drawing.PointF(120, 10), new System.Drawing.PointF(120, 25), new System.Drawing.PointF(110, 35), new System.Drawing.PointF(10, 35), new System.Drawing.PointF(0, 25) }, new byte[] { 1, 1, 1, 1, 1, 1, 1, 1 }));
            // 
            // txtCodigoBarras
            // 
            this.txtCodigoBarras.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCodigoBarras.Location = new System.Drawing.Point(200, 35);
            this.txtCodigoBarras.Name = "txtCodigoBarras";
            this.txtCodigoBarras.Size = new System.Drawing.Size(400, 26);
            this.txtCodigoBarras.TabIndex = 1;
            // 
            // lblCodigoBarras
            // 
            this.lblCodigoBarras.AutoSize = true;
            this.lblCodigoBarras.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCodigoBarras.Location = new System.Drawing.Point(20, 38);
            this.lblCodigoBarras.Name = "lblCodigoBarras";
            this.lblCodigoBarras.Size = new System.Drawing.Size(174, 17);
            this.lblCodigoBarras.TabIndex = 0;
            this.lblCodigoBarras.Text = "Código de Barras / Buscar:";
            // 
            // grpCarrito
            // 
            this.grpCarrito.Controls.Add(this.btnEliminarItem);
            this.grpCarrito.Controls.Add(this.btnLimpiarCarrito);
            this.grpCarrito.Controls.Add(this.dgvCarrito);
            this.grpCarrito.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpCarrito.Location = new System.Drawing.Point(24, 170);
            this.grpCarrito.Name = "grpCarrito";
            this.grpCarrito.Size = new System.Drawing.Size(760, 300);
            this.grpCarrito.TabIndex = 2;
            this.grpCarrito.TabStop = false;
            this.grpCarrito.Text = "🛍️ Carrito de Compras";
            this.grpCarrito.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);
            this.grpCarrito.ForeColor = System.Drawing.Color.FromArgb(52, 58, 64);
            // 
            // btnEliminarItem
            // 
            this.btnEliminarItem.BackColor = System.Drawing.Color.FromArgb(220, 53, 69);
            this.btnEliminarItem.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEliminarItem.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEliminarItem.ForeColor = System.Drawing.Color.White;
            this.btnEliminarItem.FlatAppearance.BorderSize = 0;
            this.btnEliminarItem.Location = new System.Drawing.Point(620, 250);
            this.btnEliminarItem.Name = "btnEliminarItem";
            this.btnEliminarItem.Size = new System.Drawing.Size(120, 35);
            this.btnEliminarItem.TabIndex = 2;
            this.btnEliminarItem.Text = "❌ ELIMINAR";
            this.btnEliminarItem.UseVisualStyleBackColor = false;
            this.btnEliminarItem.Click += new System.EventHandler(this.btnEliminarItem_Click);
            // 
            // btnLimpiarCarrito
            // 
            this.btnLimpiarCarrito.BackColor = System.Drawing.Color.FromArgb(255, 193, 7);
            this.btnLimpiarCarrito.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLimpiarCarrito.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLimpiarCarrito.ForeColor = System.Drawing.Color.Black;
            this.btnLimpiarCarrito.FlatAppearance.BorderSize = 0;
            this.btnLimpiarCarrito.Location = new System.Drawing.Point(480, 250);
            this.btnLimpiarCarrito.Name = "btnLimpiarCarrito";
            this.btnLimpiarCarrito.Size = new System.Drawing.Size(120, 35);
            this.btnLimpiarCarrito.TabIndex = 1;
            this.btnLimpiarCarrito.Text = "🗑️ LIMPIAR";
            this.btnLimpiarCarrito.UseVisualStyleBackColor = false;
            this.btnLimpiarCarrito.Click += new System.EventHandler(this.btnLimpiarCarrito_Click);
            // 
            // dgvCarrito
            // 
            this.dgvCarrito.AllowUserToAddRows = false;
            this.dgvCarrito.AllowUserToDeleteRows = false;
            this.dgvCarrito.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvCarrito.BackgroundColor = System.Drawing.Color.FromArgb(248, 249, 250);
            this.dgvCarrito.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvCarrito.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(0, 123, 255);
            this.dgvCarrito.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.White;
            this.dgvCarrito.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.dgvCarrito.EnableHeadersVisualStyles = false;
            this.dgvCarrito.GridColor = System.Drawing.Color.FromArgb(222, 226, 230);
            this.dgvCarrito.Location = new System.Drawing.Point(20, 30);
            this.dgvCarrito.MultiSelect = false;
            this.dgvCarrito.Name = "dgvCarrito";
            this.dgvCarrito.ReadOnly = false;
            this.dgvCarrito.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvCarrito.Size = new System.Drawing.Size(720, 200);
            this.dgvCarrito.TabIndex = 0;
            this.dgvCarrito.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvCarrito_CellDoubleClick);
            this.dgvCarrito.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvCarrito_CellEndEdit);
            this.dgvCarrito.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.dgvCarrito_EditingControlShowing);
            // 
            // grpPago
            // 
            this.grpPago.Controls.Add(this.btnProcesarVenta);
            this.grpPago.Controls.Add(this.cmbMetodoPago);
            this.grpPago.Controls.Add(this.lblMetodoPago);
            this.grpPago.Controls.Add(this.lblTotal);
            this.grpPago.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpPago.Location = new System.Drawing.Point(24, 490);
            this.grpPago.Name = "grpPago";
            this.grpPago.Size = new System.Drawing.Size(760, 120);
            this.grpPago.TabIndex = 3;
            this.grpPago.TabStop = false;
            this.grpPago.Text = "💳 Procesamiento de Pago";
            this.grpPago.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);
            this.grpPago.ForeColor = System.Drawing.Color.FromArgb(52, 58, 64);
            // 
            // btnProcesarVenta
            // 
            this.btnProcesarVenta.BackColor = System.Drawing.Color.FromArgb(40, 167, 69);
            this.btnProcesarVenta.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnProcesarVenta.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnProcesarVenta.ForeColor = System.Drawing.Color.White;
            this.btnProcesarVenta.FlatAppearance.BorderSize = 0;
            this.btnProcesarVenta.Location = new System.Drawing.Point(500, 30);
            this.btnProcesarVenta.Name = "btnProcesarVenta";
            this.btnProcesarVenta.Size = new System.Drawing.Size(240, 60);
            this.btnProcesarVenta.TabIndex = 3;
            this.btnProcesarVenta.Text = "💰 FINALIZAR VENTA";
            this.btnProcesarVenta.UseVisualStyleBackColor = false;
            this.btnProcesarVenta.Click += new System.EventHandler(this.btnProcesarVenta_Click);
            // 
            // cmbMetodoPago
            // 
            this.cmbMetodoPago.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMetodoPago.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbMetodoPago.FormattingEnabled = true;
            this.cmbMetodoPago.Location = new System.Drawing.Point(200, 65);
            this.cmbMetodoPago.Name = "cmbMetodoPago";
            this.cmbMetodoPago.Size = new System.Drawing.Size(250, 24);
            this.cmbMetodoPago.TabIndex = 2;
            // 
            // lblMetodoPago
            // 
            this.lblMetodoPago.AutoSize = true;
            this.lblMetodoPago.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMetodoPago.Location = new System.Drawing.Point(20, 68);
            this.lblMetodoPago.Name = "lblMetodoPago";
            this.lblMetodoPago.Size = new System.Drawing.Size(116, 17);
            this.lblMetodoPago.TabIndex = 1;
            this.lblMetodoPago.Text = "Método de Pago:";
            // 
            // lblTotal
            // 
            this.lblTotal.AutoSize = true;
            this.lblTotal.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotal.ForeColor = System.Drawing.Color.FromArgb(0, 123, 255);
            this.lblTotal.Location = new System.Drawing.Point(20, 30);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(133, 26);
            this.lblTotal.TabIndex = 0;
            this.lblTotal.Text = "Total: $0.00";
            // 
            // ventas
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(233, 236, 239);
            this.ClientSize = new System.Drawing.Size(820, 640);
            this.Controls.Add(this.grpPago);
            this.Controls.Add(this.grpCarrito);
            this.Controls.Add(this.grpScanner);
            this.Controls.Add(this.lblTitulo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "ventas";
            this.Text = "Punto de Venta Moderno - Gamora Indumentaria";
            this.Load += new System.EventHandler(this.ventas_Load);
            this.grpScanner.ResumeLayout(false);
            this.grpScanner.PerformLayout();
            this.grpCarrito.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvCarrito)).EndInit();
            this.grpPago.ResumeLayout(false);
            this.grpPago.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.GroupBox grpScanner;
        private System.Windows.Forms.Button btnAgregar;
        private System.Windows.Forms.TextBox txtCodigoBarras;
        private System.Windows.Forms.Label lblCodigoBarras;
        private System.Windows.Forms.GroupBox grpCarrito;
        private System.Windows.Forms.Button btnEliminarItem;
        private System.Windows.Forms.Button btnLimpiarCarrito;
        private System.Windows.Forms.DataGridView dgvCarrito;
        private System.Windows.Forms.GroupBox grpPago;
        private System.Windows.Forms.Button btnProcesarVenta;
        private System.Windows.Forms.ComboBox cmbMetodoPago;
        private System.Windows.Forms.Label lblMetodoPago;
        private System.Windows.Forms.Label lblTotal;
    }
}