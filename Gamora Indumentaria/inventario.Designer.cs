namespace Gamora_Indumentaria
{
    partial class inventario
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
            this.components = new System.ComponentModel.Container();
            this.panelTop = new System.Windows.Forms.Panel();
            this.btnCerrar = new System.Windows.Forms.Button();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.panelFiltros = new System.Windows.Forms.Panel();
            this.chkSoloBajo = new System.Windows.Forms.CheckBox();
            this.nudStockMax = new System.Windows.Forms.NumericUpDown();
            this.nudStockMin = new System.Windows.Forms.NumericUpDown();
            this.lblStockRango = new System.Windows.Forms.Label();
            this.cboTalle = new System.Windows.Forms.ComboBox();
            this.lblTalle = new System.Windows.Forms.Label();
            this.cboCategoria = new System.Windows.Forms.ComboBox();
            this.lblCategoria = new System.Windows.Forms.Label();
            this.txtBuscar = new System.Windows.Forms.TextBox();
            this.lblBuscar = new System.Windows.Forms.Label();
            this.panelAcciones = new System.Windows.Forms.FlowLayoutPanel();
            this.dgvInventario = new System.Windows.Forms.DataGridView();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lblResumen = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.txtCodigoBarras1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panelTop.SuspendLayout();
            this.panelFiltros.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudStockMax)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudStockMin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvInventario)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelTop
            // 
            this.panelTop.BackColor = System.Drawing.Color.White;
            this.panelTop.Controls.Add(this.btnCerrar);
            this.panelTop.Controls.Add(this.lblTitulo);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(1650, 65);
            this.panelTop.TabIndex = 0;
            // 
            // btnCerrar
            // 
            this.btnCerrar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCerrar.FlatAppearance.BorderSize = 0;
            this.btnCerrar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCerrar.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnCerrar.ForeColor = System.Drawing.Color.DarkRed;
            this.btnCerrar.Location = new System.Drawing.Point(1587, 5);
            this.btnCerrar.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnCerrar.Name = "btnCerrar";
            this.btnCerrar.Size = new System.Drawing.Size(58, 52);
            this.btnCerrar.TabIndex = 1;
            this.btnCerrar.Text = "✕";
            this.btnCerrar.UseVisualStyleBackColor = true;
            // 
            // lblTitulo
            // 
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.lblTitulo.Location = new System.Drawing.Point(18, 12);
            this.lblTitulo.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(152, 38);
            this.lblTitulo.TabIndex = 2;
            this.lblTitulo.Text = "Inventario";
            // 
            // panelFiltros
            // 
            this.panelFiltros.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panelFiltros.Controls.Add(this.label1);
            this.panelFiltros.Controls.Add(this.txtCodigoBarras1);
            this.panelFiltros.Controls.Add(this.chkSoloBajo);
            this.panelFiltros.Controls.Add(this.nudStockMax);
            this.panelFiltros.Controls.Add(this.nudStockMin);
            this.panelFiltros.Controls.Add(this.lblStockRango);
            this.panelFiltros.Controls.Add(this.cboTalle);
            this.panelFiltros.Controls.Add(this.lblTalle);
            this.panelFiltros.Controls.Add(this.cboCategoria);
            this.panelFiltros.Controls.Add(this.lblCategoria);
            this.panelFiltros.Controls.Add(this.txtBuscar);
            this.panelFiltros.Controls.Add(this.lblBuscar);
            this.panelFiltros.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelFiltros.Location = new System.Drawing.Point(0, 65);
            this.panelFiltros.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.panelFiltros.Name = "panelFiltros";
            this.panelFiltros.Padding = new System.Windows.Forms.Padding(15, 12, 15, 12);
            this.panelFiltros.Size = new System.Drawing.Size(1650, 148);
            this.panelFiltros.TabIndex = 1;
            // 
            // chkSoloBajo
            // 
            this.chkSoloBajo.AutoSize = true;
            this.chkSoloBajo.Location = new System.Drawing.Point(1137, 20);
            this.chkSoloBajo.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.chkSoloBajo.Name = "chkSoloBajo";
            this.chkSoloBajo.Size = new System.Drawing.Size(193, 24);
            this.chkSoloBajo.TabIndex = 11;
            this.chkSoloBajo.Text = "Solo stock bajo (<=10)";
            this.chkSoloBajo.UseVisualStyleBackColor = true;
            // 
            // nudStockMax
            // 
            this.nudStockMax.Location = new System.Drawing.Point(1154, 49);
            this.nudStockMax.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.nudStockMax.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.nudStockMax.Name = "nudStockMax";
            this.nudStockMax.Size = new System.Drawing.Size(105, 26);
            this.nudStockMax.TabIndex = 10;
            this.nudStockMax.Value = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            // 
            // nudStockMin
            // 
            this.nudStockMin.Location = new System.Drawing.Point(1023, 49);
            this.nudStockMin.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.nudStockMin.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.nudStockMin.Name = "nudStockMin";
            this.nudStockMin.Size = new System.Drawing.Size(105, 26);
            this.nudStockMin.TabIndex = 9;
            // 
            // lblStockRango
            // 
            this.lblStockRango.AutoSize = true;
            this.lblStockRango.Location = new System.Drawing.Point(1018, 20);
            this.lblStockRango.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblStockRango.Name = "lblStockRango";
            this.lblStockRango.Size = new System.Drawing.Size(106, 20);
            this.lblStockRango.TabIndex = 8;
            this.lblStockRango.Text = "Rango Stock:";
            // 
            // cboTalle
            // 
            this.cboTalle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTalle.FormattingEnabled = true;
            this.cboTalle.Location = new System.Drawing.Point(780, 46);
            this.cboTalle.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cboTalle.Name = "cboTalle";
            this.cboTalle.Size = new System.Drawing.Size(178, 28);
            this.cboTalle.TabIndex = 5;
            this.cboTalle.Visible = false;
            // 
            // lblTalle
            // 
            this.lblTalle.AutoSize = true;
            this.lblTalle.Location = new System.Drawing.Point(776, 18);
            this.lblTalle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTalle.Name = "lblTalle";
            this.lblTalle.Size = new System.Drawing.Size(42, 20);
            this.lblTalle.TabIndex = 4;
            this.lblTalle.Text = "Talle";
            this.lblTalle.Visible = false;
            // 
            // cboCategoria
            // 
            this.cboCategoria.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboCategoria.FormattingEnabled = true;
            this.cboCategoria.Location = new System.Drawing.Point(525, 46);
            this.cboCategoria.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cboCategoria.Name = "cboCategoria";
            this.cboCategoria.Size = new System.Drawing.Size(238, 28);
            this.cboCategoria.TabIndex = 3;
            // 
            // lblCategoria
            // 
            this.lblCategoria.AutoSize = true;
            this.lblCategoria.Location = new System.Drawing.Point(520, 20);
            this.lblCategoria.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCategoria.Name = "lblCategoria";
            this.lblCategoria.Size = new System.Drawing.Size(78, 20);
            this.lblCategoria.TabIndex = 2;
            this.lblCategoria.Text = "Categoría";
            // 
            // txtBuscar
            // 
            this.txtBuscar.Location = new System.Drawing.Point(20, 48);
            this.txtBuscar.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtBuscar.Name = "txtBuscar";
            this.txtBuscar.Size = new System.Drawing.Size(478, 26);
            this.txtBuscar.TabIndex = 1;
            this.txtBuscar.TextChanged += new System.EventHandler(this.txtBuscar_TextChanged);
            // 
            // lblBuscar
            // 
            this.lblBuscar.AutoSize = true;
            this.lblBuscar.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.lblBuscar.Location = new System.Drawing.Point(15, 20);
            this.lblBuscar.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblBuscar.Name = "lblBuscar";
            this.lblBuscar.Size = new System.Drawing.Size(59, 20);
            this.lblBuscar.TabIndex = 0;
            this.lblBuscar.Text = "Buscar";
            // 
            // panelAcciones
            // 
            this.panelAcciones.BackColor = System.Drawing.Color.White;
            this.panelAcciones.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelAcciones.Location = new System.Drawing.Point(0, 213);
            this.panelAcciones.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.panelAcciones.Name = "panelAcciones";
            this.panelAcciones.Padding = new System.Windows.Forms.Padding(15, 8, 0, 8);
            this.panelAcciones.Size = new System.Drawing.Size(1650, 15);
            this.panelAcciones.TabIndex = 2;
            // 
            // dgvInventario
            // 
            this.dgvInventario.AllowUserToAddRows = false;
            this.dgvInventario.AllowUserToDeleteRows = false;
            this.dgvInventario.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvInventario.BackgroundColor = System.Drawing.Color.White;
            this.dgvInventario.ColumnHeadersHeight = 34;
            this.dgvInventario.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvInventario.Location = new System.Drawing.Point(0, 228);
            this.dgvInventario.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.dgvInventario.MultiSelect = false;
            this.dgvInventario.Name = "dgvInventario";
            this.dgvInventario.ReadOnly = true;
            this.dgvInventario.RowHeadersVisible = false;
            this.dgvInventario.RowHeadersWidth = 62;
            this.dgvInventario.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvInventario.Size = new System.Drawing.Size(1650, 697);
            this.dgvInventario.TabIndex = 3;
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblResumen});
            this.statusStrip1.Location = new System.Drawing.Point(0, 925);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(2, 0, 21, 0);
            this.statusStrip1.Size = new System.Drawing.Size(1650, 32);
            this.statusStrip1.TabIndex = 4;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // lblResumen
            // 
            this.lblResumen.Name = "lblResumen";
            this.lblResumen.Size = new System.Drawing.Size(187, 25);
            this.lblResumen.Text = "0 registros mostrados";
            // 
            // txtCodigoBarras1
            // 
            this.txtCodigoBarras1.Location = new System.Drawing.Point(20, 114);
            this.txtCodigoBarras1.Name = "txtCodigoBarras1";
            this.txtCodigoBarras1.Size = new System.Drawing.Size(219, 26);
            this.txtCodigoBarras1.TabIndex = 12;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.label1.Location = new System.Drawing.Point(20, 86);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(134, 20);
            this.label1.TabIndex = 13;
            this.label1.Text = "Código de barras:";
            // 
            // inventario
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(1650, 957);
            this.Controls.Add(this.dgvInventario);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.panelAcciones);
            this.Controls.Add(this.panelFiltros);
            this.Controls.Add(this.panelTop);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "inventario";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Inventario";
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            this.panelFiltros.ResumeLayout(false);
            this.panelFiltros.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudStockMax)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudStockMin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvInventario)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Panel panelFiltros;
        private System.Windows.Forms.TextBox txtBuscar;
        private System.Windows.Forms.Label lblBuscar;
        private System.Windows.Forms.ComboBox cboCategoria;
        private System.Windows.Forms.Label lblCategoria;
        private System.Windows.Forms.ComboBox cboTalle;
        private System.Windows.Forms.Label lblTalle;
        private System.Windows.Forms.CheckBox chkSoloBajo;
        private System.Windows.Forms.NumericUpDown nudStockMax;
        private System.Windows.Forms.NumericUpDown nudStockMin;
        private System.Windows.Forms.Label lblStockRango;
        private System.Windows.Forms.FlowLayoutPanel panelAcciones;
        private System.Windows.Forms.DataGridView dgvInventario;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel lblResumen;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button btnCerrar;
        private System.Windows.Forms.TextBox txtCodigoBarras1;
        private System.Windows.Forms.Label label1;
    }
}