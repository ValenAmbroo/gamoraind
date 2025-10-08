namespace Gamora_Indumentaria
{
    partial class Form1
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.panel1 = new System.Windows.Forms.Panel();
            this.button3 = new System.Windows.Forms.Button();
            this.lblHora = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.panelSideMenu = new System.Windows.Forms.Panel();
            this.panelClientes = new System.Windows.Forms.Panel();
            this.button4 = new System.Windows.Forms.Button();
            this.btnModificarProductos = new System.Windows.Forms.Button();
            this.btnEstadisticaVenta = new System.Windows.Forms.Button();
            this.btnEstadisticaInventario = new System.Windows.Forms.Button();
            this.btnAlta = new System.Windows.Forms.Button();
            this.btnAdministracion = new System.Windows.Forms.Button();
            this.panelLibroSubmenu = new System.Windows.Forms.Panel();
            this.btnCierreCaja = new System.Windows.Forms.Button();
            this.btnHistorialVenta = new System.Windows.Forms.Button();
            this.btnVenta = new System.Windows.Forms.Button();
            this.btnInventario = new System.Windows.Forms.Button();
            this.btnOperaciones = new System.Windows.Forms.Button();
            this.timerHora = new System.Windows.Forms.Timer(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.button10 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.panelSideMenu.SuspendLayout();
            this.panelClientes.SuspendLayout();
            this.panelLibroSubmenu.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel5.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.button3);
            this.panel1.Controls.Add(this.lblHora);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.button1);
            this.panel1.Controls.Add(this.button2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1323, 35);
            this.panel1.TabIndex = 0;
            // 
            // button3
            // 
            this.button3.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.button3.BackgroundImage = global::Gamora_Indumentaria.Properties.Resources.cerrrar_;
            this.button3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.button3.Dock = System.Windows.Forms.DockStyle.Right;
            this.button3.Location = new System.Drawing.Point(1290, 0);
            this.button3.Margin = new System.Windows.Forms.Padding(1);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(33, 35);
            this.button3.TabIndex = 2;
            this.button3.UseVisualStyleBackColor = false;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // lblHora
            // 
            this.lblHora.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblHora.AutoSize = true;
            this.lblHora.BackColor = System.Drawing.Color.White;
            this.lblHora.Font = new System.Drawing.Font("Impact", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHora.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.lblHora.Location = new System.Drawing.Point(771, 5);
            this.lblHora.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lblHora.Name = "lblHora";
            this.lblHora.Size = new System.Drawing.Size(0, 27);
            this.lblHora.TabIndex = 1;
            this.lblHora.Click += new System.EventHandler(this.lblHora_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("MingLiU_HKSCS-ExtB", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(9, 6);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(218, 19);
            this.label1.TabIndex = 3;
            this.label1.Text = "GAMORA INDUMENTARIA";
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.button1.BackgroundImage = global::Gamora_Indumentaria.Properties.Resources.nuevo_minimized_;
            this.button1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.button1.Location = new System.Drawing.Point(1210, 0);
            this.button1.Margin = new System.Windows.Forms.Padding(1);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(33, 32);
            this.button1.TabIndex = 0;
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.button2.BackgroundImage = global::Gamora_Indumentaria.Properties.Resources.maximized_n;
            this.button2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.button2.Location = new System.Drawing.Point(1253, 1);
            this.button2.Margin = new System.Windows.Forms.Padding(1);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(33, 32);
            this.button2.TabIndex = 1;
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // panelSideMenu
            // 
            this.panelSideMenu.AutoScroll = true;
            this.panelSideMenu.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(11)))), ((int)(((byte)(7)))), ((int)(((byte)(17)))));
            this.panelSideMenu.Controls.Add(this.panelClientes);
            this.panelSideMenu.Controls.Add(this.btnAdministracion);
            this.panelSideMenu.Controls.Add(this.panelLibroSubmenu);
            this.panelSideMenu.Controls.Add(this.btnOperaciones);
            this.panelSideMenu.Location = new System.Drawing.Point(0, 0);
            this.panelSideMenu.Margin = new System.Windows.Forms.Padding(1);
            this.panelSideMenu.Name = "panelSideMenu";
            this.panelSideMenu.Size = new System.Drawing.Size(215, 512);
            this.panelSideMenu.TabIndex = 11;
            // 
            // panelClientes
            // 
            this.panelClientes.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(32)))), ((int)(((byte)(39)))));
            this.panelClientes.Controls.Add(this.button5);
            this.panelClientes.Controls.Add(this.button4);
            this.panelClientes.Controls.Add(this.btnModificarProductos);
            this.panelClientes.Controls.Add(this.btnEstadisticaVenta);
            this.panelClientes.Controls.Add(this.btnEstadisticaInventario);
            this.panelClientes.Controls.Add(this.btnAlta);
            this.panelClientes.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelClientes.Location = new System.Drawing.Point(0, 257);
            this.panelClientes.Margin = new System.Windows.Forms.Padding(1);
            this.panelClientes.Name = "panelClientes";
            this.panelClientes.Size = new System.Drawing.Size(215, 254);
            this.panelClientes.TabIndex = 4;
            this.panelClientes.Paint += new System.Windows.Forms.PaintEventHandler(this.panelClientes_Paint);
            // 
            // button4
            // 
            this.button4.Dock = System.Windows.Forms.DockStyle.Top;
            this.button4.FlatAppearance.BorderSize = 0;
            this.button4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button4.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button4.ForeColor = System.Drawing.Color.LightGray;
            this.button4.Location = new System.Drawing.Point(0, 156);
            this.button4.Margin = new System.Windows.Forms.Padding(1);
            this.button4.Name = "button4";
            this.button4.Padding = new System.Windows.Forms.Padding(15, 0, 0, 0);
            this.button4.Size = new System.Drawing.Size(215, 39);
            this.button4.TabIndex = 6;
            this.button4.Text = "Agregar Categoria";
            this.button4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click_1);
            // 
            // btnModificarProductos
            // 
            this.btnModificarProductos.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnModificarProductos.FlatAppearance.BorderSize = 0;
            this.btnModificarProductos.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnModificarProductos.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnModificarProductos.ForeColor = System.Drawing.Color.LightGray;
            this.btnModificarProductos.Location = new System.Drawing.Point(0, 117);
            this.btnModificarProductos.Margin = new System.Windows.Forms.Padding(1);
            this.btnModificarProductos.Name = "btnModificarProductos";
            this.btnModificarProductos.Padding = new System.Windows.Forms.Padding(15, 0, 0, 0);
            this.btnModificarProductos.Size = new System.Drawing.Size(215, 39);
            this.btnModificarProductos.TabIndex = 5;
            this.btnModificarProductos.Text = "Modificar Productos";
            this.btnModificarProductos.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnModificarProductos.UseVisualStyleBackColor = true;
            this.btnModificarProductos.Click += new System.EventHandler(this.button22_Click);
            // 
            // btnEstadisticaVenta
            // 
            this.btnEstadisticaVenta.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnEstadisticaVenta.FlatAppearance.BorderSize = 0;
            this.btnEstadisticaVenta.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEstadisticaVenta.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEstadisticaVenta.ForeColor = System.Drawing.Color.LightGray;
            this.btnEstadisticaVenta.Location = new System.Drawing.Point(0, 78);
            this.btnEstadisticaVenta.Margin = new System.Windows.Forms.Padding(1);
            this.btnEstadisticaVenta.Name = "btnEstadisticaVenta";
            this.btnEstadisticaVenta.Padding = new System.Windows.Forms.Padding(15, 0, 0, 0);
            this.btnEstadisticaVenta.Size = new System.Drawing.Size(215, 39);
            this.btnEstadisticaVenta.TabIndex = 4;
            this.btnEstadisticaVenta.Text = "Estadisticas Ventas";
            this.btnEstadisticaVenta.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnEstadisticaVenta.UseVisualStyleBackColor = true;
            this.btnEstadisticaVenta.Click += new System.EventHandler(this.button21_Click);
            // 
            // btnEstadisticaInventario
            // 
            this.btnEstadisticaInventario.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnEstadisticaInventario.FlatAppearance.BorderSize = 0;
            this.btnEstadisticaInventario.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEstadisticaInventario.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEstadisticaInventario.ForeColor = System.Drawing.Color.LightGray;
            this.btnEstadisticaInventario.Location = new System.Drawing.Point(0, 39);
            this.btnEstadisticaInventario.Margin = new System.Windows.Forms.Padding(1);
            this.btnEstadisticaInventario.Name = "btnEstadisticaInventario";
            this.btnEstadisticaInventario.Padding = new System.Windows.Forms.Padding(15, 0, 0, 0);
            this.btnEstadisticaInventario.Size = new System.Drawing.Size(215, 39);
            this.btnEstadisticaInventario.TabIndex = 3;
            this.btnEstadisticaInventario.Text = "Estadisticas Inventario";
            this.btnEstadisticaInventario.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnEstadisticaInventario.UseVisualStyleBackColor = true;
            this.btnEstadisticaInventario.Click += new System.EventHandler(this.button20_Click);
            // 
            // btnAlta
            // 
            this.btnAlta.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnAlta.FlatAppearance.BorderSize = 0;
            this.btnAlta.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAlta.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAlta.ForeColor = System.Drawing.Color.LightGray;
            this.btnAlta.Location = new System.Drawing.Point(0, 0);
            this.btnAlta.Margin = new System.Windows.Forms.Padding(1);
            this.btnAlta.Name = "btnAlta";
            this.btnAlta.Padding = new System.Windows.Forms.Padding(15, 0, 0, 0);
            this.btnAlta.Size = new System.Drawing.Size(215, 39);
            this.btnAlta.TabIndex = 1;
            this.btnAlta.Text = "Agregar Producto";
            this.btnAlta.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAlta.UseVisualStyleBackColor = true;
            this.btnAlta.Click += new System.EventHandler(this.btnAlta_Click);
            // 
            // btnAdministracion
            // 
            this.btnAdministracion.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnAdministracion.FlatAppearance.BorderSize = 0;
            this.btnAdministracion.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAdministracion.ForeColor = System.Drawing.Color.Gainsboro;
            this.btnAdministracion.Location = new System.Drawing.Point(0, 212);
            this.btnAdministracion.Margin = new System.Windows.Forms.Padding(1);
            this.btnAdministracion.Name = "btnAdministracion";
            this.btnAdministracion.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.btnAdministracion.Size = new System.Drawing.Size(215, 45);
            this.btnAdministracion.TabIndex = 3;
            this.btnAdministracion.Text = "ADMINISTRACIÓN";
            this.btnAdministracion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAdministracion.UseVisualStyleBackColor = true;
            // 
            // panelLibroSubmenu
            // 
            this.panelLibroSubmenu.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(32)))), ((int)(((byte)(39)))));
            this.panelLibroSubmenu.Controls.Add(this.btnCierreCaja);
            this.panelLibroSubmenu.Controls.Add(this.btnHistorialVenta);
            this.panelLibroSubmenu.Controls.Add(this.btnVenta);
            this.panelLibroSubmenu.Controls.Add(this.btnInventario);
            this.panelLibroSubmenu.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelLibroSubmenu.Location = new System.Drawing.Point(0, 45);
            this.panelLibroSubmenu.Margin = new System.Windows.Forms.Padding(1);
            this.panelLibroSubmenu.Name = "panelLibroSubmenu";
            this.panelLibroSubmenu.Size = new System.Drawing.Size(215, 167);
            this.panelLibroSubmenu.TabIndex = 2;
            // 
            // btnCierreCaja
            // 
            this.btnCierreCaja.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnCierreCaja.FlatAppearance.BorderSize = 0;
            this.btnCierreCaja.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCierreCaja.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCierreCaja.ForeColor = System.Drawing.Color.LightGray;
            this.btnCierreCaja.Location = new System.Drawing.Point(0, 117);
            this.btnCierreCaja.Margin = new System.Windows.Forms.Padding(1);
            this.btnCierreCaja.Name = "btnCierreCaja";
            this.btnCierreCaja.Padding = new System.Windows.Forms.Padding(15, 0, 0, 0);
            this.btnCierreCaja.Size = new System.Drawing.Size(215, 39);
            this.btnCierreCaja.TabIndex = 3;
            this.btnCierreCaja.Text = "Cierre de Caja";
            this.btnCierreCaja.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCierreCaja.UseVisualStyleBackColor = true;
            this.btnCierreCaja.Click += new System.EventHandler(this.button19_Click);
            // 
            // btnHistorialVenta
            // 
            this.btnHistorialVenta.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnHistorialVenta.FlatAppearance.BorderSize = 0;
            this.btnHistorialVenta.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnHistorialVenta.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnHistorialVenta.ForeColor = System.Drawing.Color.LightGray;
            this.btnHistorialVenta.Location = new System.Drawing.Point(0, 78);
            this.btnHistorialVenta.Margin = new System.Windows.Forms.Padding(1);
            this.btnHistorialVenta.Name = "btnHistorialVenta";
            this.btnHistorialVenta.Padding = new System.Windows.Forms.Padding(15, 0, 0, 0);
            this.btnHistorialVenta.Size = new System.Drawing.Size(215, 39);
            this.btnHistorialVenta.TabIndex = 2;
            this.btnHistorialVenta.Text = "Historial de venta";
            this.btnHistorialVenta.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnHistorialVenta.UseVisualStyleBackColor = true;
            this.btnHistorialVenta.Click += new System.EventHandler(this.btnModificaciones_Click);
            // 
            // btnVenta
            // 
            this.btnVenta.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnVenta.FlatAppearance.BorderSize = 0;
            this.btnVenta.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnVenta.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnVenta.ForeColor = System.Drawing.Color.LightGray;
            this.btnVenta.Location = new System.Drawing.Point(0, 39);
            this.btnVenta.Margin = new System.Windows.Forms.Padding(1);
            this.btnVenta.Name = "btnVenta";
            this.btnVenta.Padding = new System.Windows.Forms.Padding(15, 0, 0, 0);
            this.btnVenta.Size = new System.Drawing.Size(215, 39);
            this.btnVenta.TabIndex = 1;
            this.btnVenta.Text = "Venta";
            this.btnVenta.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnVenta.UseVisualStyleBackColor = true;
            this.btnVenta.Click += new System.EventHandler(this.btnAltas_Click);
            // 
            // btnInventario
            // 
            this.btnInventario.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnInventario.FlatAppearance.BorderSize = 0;
            this.btnInventario.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnInventario.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnInventario.ForeColor = System.Drawing.Color.LightGray;
            this.btnInventario.Location = new System.Drawing.Point(0, 0);
            this.btnInventario.Margin = new System.Windows.Forms.Padding(1);
            this.btnInventario.Name = "btnInventario";
            this.btnInventario.Padding = new System.Windows.Forms.Padding(15, 0, 0, 0);
            this.btnInventario.Size = new System.Drawing.Size(215, 39);
            this.btnInventario.TabIndex = 0;
            this.btnInventario.Text = "Inventario";
            this.btnInventario.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnInventario.UseVisualStyleBackColor = true;
            this.btnInventario.Click += new System.EventHandler(this.btnConsultas_Click);
            // 
            // btnOperaciones
            // 
            this.btnOperaciones.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnOperaciones.FlatAppearance.BorderSize = 0;
            this.btnOperaciones.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOperaciones.ForeColor = System.Drawing.Color.Gainsboro;
            this.btnOperaciones.Location = new System.Drawing.Point(0, 0);
            this.btnOperaciones.Margin = new System.Windows.Forms.Padding(1);
            this.btnOperaciones.Name = "btnOperaciones";
            this.btnOperaciones.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.btnOperaciones.Size = new System.Drawing.Size(215, 45);
            this.btnOperaciones.TabIndex = 1;
            this.btnOperaciones.Text = "OPERACIONES";
            this.btnOperaciones.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnOperaciones.UseVisualStyleBackColor = true;
            // 
            // timerHora
            // 
            this.timerHora.Enabled = true;
            this.timerHora.Interval = 1000;
            this.timerHora.Tag = "hora";
            this.timerHora.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.SystemColors.Desktop;
            this.panel2.BackgroundImage = global::Gamora_Indumentaria.Properties.Resources.PHOTO_2024_02_05_15_39_30;
            this.panel2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Margin = new System.Windows.Forms.Padding(2);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(217, 142);
            this.panel2.TabIndex = 1;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.Black;
            this.panel3.Controls.Add(this.panel5);
            this.panel3.Controls.Add(this.panel2);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel3.Location = new System.Drawing.Point(0, 35);
            this.panel3.Margin = new System.Windows.Forms.Padding(2);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(217, 734);
            this.panel3.TabIndex = 1;
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.button10);
            this.panel5.Controls.Add(this.panelSideMenu);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel5.Location = new System.Drawing.Point(0, 142);
            this.panel5.Margin = new System.Windows.Forms.Padding(2);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(215, 592);
            this.panel5.TabIndex = 2;
            // 
            // button10
            // 
            this.button10.BackColor = System.Drawing.Color.DarkRed;
            this.button10.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.button10.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.button10.Location = new System.Drawing.Point(0, 561);
            this.button10.Margin = new System.Windows.Forms.Padding(1);
            this.button10.Name = "button10";
            this.button10.Size = new System.Drawing.Size(215, 31);
            this.button10.TabIndex = 6;
            this.button10.Text = "🔓 CERRAR SESIÓN";
            this.button10.UseVisualStyleBackColor = false;
            this.button10.Click += new System.EventHandler(this.button10_Click);
            // 
            // button5
            // 
            this.button5.Dock = System.Windows.Forms.DockStyle.Top;
            this.button5.FlatAppearance.BorderSize = 0;
            this.button5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button5.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button5.ForeColor = System.Drawing.Color.LightGray;
            this.button5.Location = new System.Drawing.Point(0, 195);
            this.button5.Margin = new System.Windows.Forms.Padding(1);
            this.button5.Name = "button5";
            this.button5.Padding = new System.Windows.Forms.Padding(15, 0, 0, 0);
            this.button5.Size = new System.Drawing.Size(215, 39);
            this.button5.TabIndex = 7;
            this.button5.Text = "Administrar Categoria";
            this.button5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnablePreventFocusChange;
            this.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(1323, 769);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panelSideMenu.ResumeLayout(false);
            this.panelClientes.ResumeLayout(false);
            this.panelLibroSubmenu.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.Timer timerHora;
        private System.Windows.Forms.Label lblHora;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.Panel panelSideMenu;
        private System.Windows.Forms.Panel panelClientes;
        private System.Windows.Forms.Button btnAlta;
        private System.Windows.Forms.Button btnAdministracion;
        private System.Windows.Forms.Panel panelLibroSubmenu;
        private System.Windows.Forms.Button btnHistorialVenta;
        private System.Windows.Forms.Button btnVenta;
        private System.Windows.Forms.Button btnInventario;
        private System.Windows.Forms.Button btnOperaciones;
        private System.Windows.Forms.Button btnModificarProductos;
        private System.Windows.Forms.Button btnEstadisticaVenta;
        private System.Windows.Forms.Button btnEstadisticaInventario;
        private System.Windows.Forms.Button btnCierreCaja;

        private System.Windows.Forms.Button btnsubadministracion;
        private System.Windows.Forms.Panel panelsubadministracion;
        private System.Windows.Forms.Button btnListadoVentas;
        private System.Windows.Forms.Button button12;
        private System.Windows.Forms.Button button13;
        private System.Windows.Forms.Button button14;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Button button10;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
    }
}

