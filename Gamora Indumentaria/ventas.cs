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
using System.Media;
using System.Drawing.Printing;

namespace Gamora_Indumentaria
{
    public partial class ventas : Form
    {
        private List<ItemCarrito> carrito;
        private decimal totalVenta;
        private bool carritoGridConfigurado = false;
        private TextBox descuentoEditor; // referencia editor activo
        // Soporte escáner
        private StringBuilder scannerBuffer = new StringBuilder();
        private DateTime lastScannerCharTime = DateTime.MinValue;
        private const int SCAN_TIMEOUT_MS = 120; // intervalo máximo entre caracteres de un escaneo
        private Label lblUltimoCodigo;
        // Impresión de ticket
        private PrintDocument printDoc;
        private CheckBox chkImprimirTicket;
        private CheckBox chkEsRegalo; // Ocultar precios en ticket
        private VentaTicketData lastTicketData;
        private PrintPreviewDialog previewDlg;
        private Button btnVistaPrevia;

        private class VentaTicketData
        {
            public int VentaId { get; set; }
            public DateTime Fecha { get; set; }
            public string MetodoPago { get; set; }
            public decimal Total { get; set; }
            public List<ItemCarrito> Items { get; set; }
            public bool EsRegalo { get; set; }
        }

        // Controles definidos manualmente
        private Button button3;
        private GroupBox lblTitulo1;
        private Button btnAgregar1;
        private TextBox txtCodigoBarras1;
        private Label label1;
        private GroupBox groupBox2;
        private DataGridView dgvCarrito1; // lógica interna; se vinculará al dgvCarrito del diseñador
        private Button button4;
        private Button button2;
        private GroupBox grpPago;
        private Label label3;
        private Button ProcesarVentabtn;
        private ComboBox cmbMetodoPago1;
        private Panel panelTop;
        private Button btnCerrar;
        private Label lblTitulo;
        private Label lblTotal;
        // Eliminado soporte de cliente (no se almacena)


        public ventas()
        {
            carrito = new List<ItemCarrito>();
            totalVenta = 0;
            DatabaseManager.InitializeDatabase(); // Usar el manager centralizado
            InitializeComponent();

            // Añadir controles adicionales (ticket) si no existen aún
            if (chkImprimirTicket == null)
            {
                chkImprimirTicket = new CheckBox
                {
                    Text = "Imprimir ticket",
                    Checked = true,
                    AutoSize = true,
                    Location = new Point(20, 95)
                };
                grpPago.Controls.Add(chkImprimirTicket);
            }
            // Agregar checkbox de regalo (solo afecta impresión)
            if (chkEsRegalo == null)
            {
                chkEsRegalo = new CheckBox
                {
                    Text = "Es un regalo (ocultar precios)",
                    AutoSize = true,
                    Location = new Point(20, 125)
                };
                grpPago.Controls.Add(chkEsRegalo);
            }
            if (btnVistaPrevia == null)
            {
                btnVistaPrevia = new Button
                {
                    Text = "Vista previa",
                    Size = new Size(110, 30),
                    Location = new Point(360, 25),
                    BackColor = Color.LightSteelBlue,
                    FlatStyle = FlatStyle.Flat
                };
                btnVistaPrevia.Click += BtnVistaPrevia_Click;
                grpPago.Controls.Add(btnVistaPrevia);
            }
            this.KeyPreview = true;
            this.KeyPress += Ventas_KeyPress_Global;
            this.KeyDown += Ventas_KeyDown_Shortcuts;
            ConfigurarImpresion();


        }

        private void AgregarProductoPorCodigo()
        {
            string codigoBarras = txtCodigoBarras1?.Text?.Trim();
            if (string.IsNullOrEmpty(codigoBarras))
            {
                //MessageBox.Show("Ingrese un código de barras", "Atención",
                //   MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Tabla ya migrada: usar solo PrecioVenta
                string query = @"
              SELECT i.Id, i.Nombre,
                  i.PrecioVenta AS Precio,
                  i.Stock, c.Nombre as Categoria
              FROM Inventario i
              INNER JOIN Categorias c ON i.CategoriaId = c.Id
              WHERE i.CodigoBarras = @codigo AND i.Stock > 0";

                SqlParameter[] parameters = {
                    new SqlParameter("@codigo", codigoBarras)
                };

                DataTable dt = DatabaseManager.ExecuteQuery(query, parameters);

                if (dt.Rows.Count > 0)
                {
                    if (dt.Rows.Count == 1)
                    {
                        DataRow row = dt.Rows[0];
                        var producto = new Producto
                        {
                            Id = Convert.ToInt32(row["Id"]),
                            Nombre = row["Nombre"].ToString(),
                            Precio = Convert.ToDecimal(row["Precio"]),
                            Stock = Convert.ToInt32(row["Stock"]),
                            Categoria = row["Categoria"].ToString(),
                            CodigoBarras = codigoBarras
                        };

                        AgregarAlCarrito(producto);
                        txtCodigoBarras1.Clear();
                        txtCodigoBarras1.Focus();
                    }
                    else
                    {
                        // Construir lista de productos y pedir selección al usuario
                        var lista = new System.Collections.Generic.List<Producto>();
                        foreach (System.Data.DataRow row in dt.Rows)
                        {
                            lista.Add(new Producto
                            {
                                Id = Convert.ToInt32(row["Id"]),
                                Nombre = row["Nombre"].ToString(),
                                Precio = Convert.ToDecimal(row["Precio"]),
                                Stock = Convert.ToInt32(row["Stock"]),
                                Categoria = row["Categoria"].ToString(),
                                CodigoBarras = codigoBarras
                            });
                        }

                        using (var f = new SeleccionarProductoForm(lista))
                        {
                            var dr = f.ShowDialog(this);
                            if (dr == System.Windows.Forms.DialogResult.OK && f.ProductoSeleccionado != null)
                            {
                                AgregarAlCarrito(f.ProductoSeleccionado);
                                txtCodigoBarras1.Clear();
                                txtCodigoBarras1.Focus();
                            }
                            else
                            {
                                // Si el usuario cancela, volver a seleccionar el textbox
                                txtCodigoBarras1.SelectAll();
                                txtCodigoBarras1.Focus();
                            }
                        }
                    }
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
                    RecalcularItem(itemExistente);
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
                    Descuento = 0m,
                    Subtotal = producto.Precio // neto (sin descuento)
                });
            }

            ActualizarCarrito();
            ActualizarTotal();
            // Feedback UX: sonido y rotular último agregado
            try { SystemSounds.Asterisk.Play(); } catch { }
            if (lblUltimoCodigo != null)
            {
                lblUltimoCodigo.Text = $"Agregado: {producto.Nombre}";
            }
        }

        private void ConfigurarCarritoGrid(DataGridView grid)
        {
            grid.Columns.Clear();
            grid.AutoGenerateColumns = false;
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Producto", Name = "Producto", ReadOnly = true, Width = 220 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Precio", Name = "Precio", ReadOnly = true, Width = 70 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Cant", Name = "Cantidad", ReadOnly = true, Width = 50 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Desc.%", Name = "Descuento", ReadOnly = false, Width = 60 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Subtotal", Name = "Subtotal", ReadOnly = true, Width = 80 });
            carritoGridConfigurado = true;
        }

        private void ActualizarCarrito()
        {
            var grid = dgvCarrito1; // usar control real
            if (grid == null) return;
            if (!carritoGridConfigurado) ConfigurarCarritoGrid(grid);
            grid.Rows.Clear();
            foreach (var item in carrito)
            {
                grid.Rows.Add(
                    item.NombreProducto,
                    item.PrecioUnitario.ToString("C"),
                    item.Cantidad,
                    item.Descuento == 0 ? "" : item.Descuento.ToString("0.##"),
                    item.Subtotal.ToString("C"));
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

        private void dgvCarrito_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= carrito.Count) return;
            var item = carrito[e.RowIndex];
            // Descuento ahora es porcentaje (0-100)
            string input = MostrarPrompt("Descuento %", $"Ingrese % descuento para {item.NombreProducto} (0-100):", item.Descuento.ToString());
            if (input == null) return; // cancelado
            if (decimal.TryParse(input.Replace(',', '.'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal desc))
            {
                if (desc < 0) desc = 0;
                if (desc > 100) desc = 100;
                item.Descuento = desc;
                RecalcularItem(item);
                // Actualizar solo la fila en pantalla
                if (dgvCarrito1 != null && e.RowIndex < dgvCarrito1.Rows.Count)
                {
                    dgvCarrito1.Rows[e.RowIndex].Cells["Descuento"].Value = item.Descuento == 0 ? "" : item.Descuento.ToString("0.##");
                    dgvCarrito1.Rows[e.RowIndex].Cells["Subtotal"].Value = item.Subtotal.ToString("C");
                }
                else
                {
                    ActualizarCarrito();
                }
                ActualizarTotal();
            }
            else
            {
                MessageBox.Show("Valor inválido", "Descuento", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void dgvCarrito_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            var grid = sender as DataGridView;
            if (grid == null || e.RowIndex < 0 || e.ColumnIndex < 0) return;
            if (grid.Columns[e.ColumnIndex].Name != "Descuento") return;
            if (e.RowIndex >= carrito.Count) return;
            var item = carrito[e.RowIndex];
            var cellVal = grid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString().Trim();
            if (string.IsNullOrEmpty(cellVal)) item.Descuento = 0m;
            else
            {
                if (decimal.TryParse(cellVal.Replace(',', '.'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var d))
                {
                    if (d < 0) d = 0;
                    if (d > 100) d = 100;
                    item.Descuento = d;
                }
                else
                {
                    MessageBox.Show("Valor de descuento inválido", "Descuento", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            RecalcularItem(item);
            // Actualizar celdas editadas sin reconstruir todo el grid
            if (grid.Columns.Contains("Subtotal"))
            {
                grid.Rows[e.RowIndex].Cells["Subtotal"].Value = item.Subtotal.ToString("C");
            }
            ActualizarTotal();
        }

        private void dgvCarrito_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (dgvCarrito1.Columns[dgvCarrito1.CurrentCell.ColumnIndex].Name == "Descuento" && e.Control is TextBox tb)
            {
                if (descuentoEditor != null)
                {
                    descuentoEditor.KeyDown -= DescuentoEditor_KeyDown;
                }
                descuentoEditor = tb;
                descuentoEditor.KeyDown += DescuentoEditor_KeyDown;
            }
        }

        private void DescuentoEditor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && dgvCarrito1.CurrentCell != null)
            {
                int row = dgvCarrito1.CurrentCell.RowIndex;
                if (row >= 0 && row < carrito.Count)
                {
                    var txt = (sender as TextBox)?.Text?.Trim();
                    if (string.IsNullOrEmpty(txt)) carrito[row].Descuento = 0m;
                    else
                    {
                        if (decimal.TryParse(txt.Replace(',', '.'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var d))
                        {
                            if (d < 0) d = 0; if (d > 100) d = 100; carrito[row].Descuento = d;
                            RecalcularItem(carrito[row]);
                        }
                    }
                    // Refrescar solo fila
                    if (dgvCarrito1.Rows.Count > row)
                    {
                        dgvCarrito1.Rows[row].Cells["Descuento"].Value = carrito[row].Descuento == 0 ? "" : carrito[row].Descuento.ToString("0.##");
                        dgvCarrito1.Rows[row].Cells["Subtotal"].Value = carrito[row].Subtotal.ToString("C");
                    }
                    ActualizarTotal();
                }
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void RecalcularItem(ItemCarrito item)
        {
            decimal bruto = item.PrecioUnitario * item.Cantidad;
            decimal descPct = item.Descuento / 100m;
            if (descPct < 0) descPct = 0;
            if (descPct > 1) descPct = 1;
            item.Subtotal = Math.Round(bruto * (1 - descPct), 2);
        }

        private string MostrarPrompt(string titulo, string mensaje, string valorInicial)
        {
            Form prompt = new Form()
            {
                Width = 380,
                Height = 170,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = titulo,
                StartPosition = FormStartPosition.CenterParent
            };
            Label textLabel = new Label() { Left = 15, Top = 15, Width = 340, Text = mensaje };
            TextBox inputBox = new TextBox() { Left = 15, Top = 45, Width = 340, Text = valorInicial };
            Button confirmation = new Button() { Text = "OK", Left = 200, Width = 70, Top = 80, DialogResult = DialogResult.OK };
            Button cancel = new Button() { Text = "Cancelar", Left = 285, Width = 70, Top = 80, DialogResult = DialogResult.Cancel };
            confirmation.FlatStyle = FlatStyle.Flat;
            cancel.FlatStyle = FlatStyle.Flat;
            prompt.Controls.Add(textLabel);
            prompt.Controls.Add(inputBox);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(cancel);
            prompt.AcceptButton = confirmation;
            prompt.CancelButton = cancel;
            var result = prompt.ShowDialog(this);
            return result == DialogResult.OK ? inputBox.Text.Trim() : null;
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
                // Recalcular total neto por seguridad
                foreach (var it in carrito) RecalcularItem(it);
                totalVenta = carrito.Sum(c => c.Subtotal);
                // Guardar snapshot antes de limpiar
                var snapshotItems = carrito.Select(i => new ItemCarrito
                {
                    ProductoId = i.ProductoId,
                    NombreProducto = i.NombreProducto,
                    PrecioUnitario = i.PrecioUnitario,
                    Cantidad = i.Cantidad,
                    Descuento = i.Descuento,
                    Subtotal = i.Subtotal // ya neto
                }).ToList();

                // Ya no persistimos 'regalo' en BD. Solo afecta la impresión del ticket.
                int ventaId = DatabaseManager.ProcesarVenta(carrito, metodoPago, totalVenta);

                // Preparar datos para ticket
                lastTicketData = new VentaTicketData
                {
                    VentaId = ventaId,
                    Fecha = DateTime.Now,
                    MetodoPago = metodoPago,
                    Total = totalVenta,
                    Items = snapshotItems
                };
                lastTicketData.EsRegalo = chkEsRegalo?.Checked == true;

                MessageBox.Show(string.Format("Venta procesada exitosamente.\nVenta ID: {0}\nTotal: {1:C}\nMétodo: {2}",
                    ventaId, totalVenta, metodoPago),
                    "Venta Completada", MessageBoxButtons.OK, MessageBoxIcon.Information);

                if (chkImprimirTicket != null && chkImprimirTicket.Checked)
                {
                    try { printDoc.Print(); }
                    catch (Exception exPrint)
                    {
                        MessageBox.Show("Error al imprimir ticket: " + exPrint.Message, "Impresión", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }

                LimpiarCarrito();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al procesar venta: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Adaptador para el evento Load original del diseñador
        private void ventas_Load(object sender, EventArgs e)
        {
            ventas_Load_1(sender, e);
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
            this.grpPago = new System.Windows.Forms.GroupBox();
            this.ProcesarVentabtn = new System.Windows.Forms.Button();
            this.chkImprimirTicket = new System.Windows.Forms.CheckBox();
            this.btnVistaPrevia = new System.Windows.Forms.Button();
            this.cmbMetodoPago1 = new System.Windows.Forms.ComboBox();
            this.lblTotal = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtCodigoBarras1 = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.button4 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.dgvCarrito1 = new System.Windows.Forms.DataGridView();
            this.label1 = new System.Windows.Forms.Label();
            this.lblUltimoCodigo = new System.Windows.Forms.Label();
            this.panelTop = new System.Windows.Forms.Panel();
            this.btnCerrar = new System.Windows.Forms.Button();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.lblTitulo1.SuspendLayout();
            this.grpPago.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCarrito1)).BeginInit();
            this.panelTop.SuspendLayout();
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
            this.lblTitulo1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTitulo1.Controls.Add(this.btnAgregar1);
            this.lblTitulo1.Controls.Add(this.grpPago);
            this.lblTitulo1.Controls.Add(this.txtCodigoBarras1);
            this.lblTitulo1.Controls.Add(this.groupBox2);
            this.lblTitulo1.Controls.Add(this.label1);
            this.lblTitulo1.Controls.Add(this.lblUltimoCodigo);
            this.lblTitulo1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitulo1.Location = new System.Drawing.Point(12, 50);
            this.lblTitulo1.Name = "lblTitulo1";
            this.lblTitulo1.Size = new System.Drawing.Size(1181, 547);
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
            // grpPago
            // 
            this.grpPago.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.grpPago.Controls.Add(this.ProcesarVentabtn);
            this.grpPago.Controls.Add(this.chkImprimirTicket);
            this.grpPago.Controls.Add(this.btnVistaPrevia);
            this.grpPago.Controls.Add(this.cmbMetodoPago1);
            this.grpPago.Controls.Add(this.lblTotal);
            this.grpPago.Controls.Add(this.label3);
            this.grpPago.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpPago.Location = new System.Drawing.Point(776, 372);
            this.grpPago.Name = "grpPago";
            this.grpPago.Size = new System.Drawing.Size(390, 169);
            this.grpPago.TabIndex = 5;
            this.grpPago.TabStop = false;
            this.grpPago.Text = "Procedimiento de Pago";
            // 
            // ProcesarVentabtn
            // 
            this.ProcesarVentabtn.BackColor = System.Drawing.Color.LawnGreen;
            this.ProcesarVentabtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ProcesarVentabtn.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.ProcesarVentabtn.Location = new System.Drawing.Point(176, 138);
            this.ProcesarVentabtn.Name = "ProcesarVentabtn";
            this.ProcesarVentabtn.Size = new System.Drawing.Size(196, 27);
            this.ProcesarVentabtn.TabIndex = 3;
            this.ProcesarVentabtn.Text = " Procesar Venta";
            this.ProcesarVentabtn.UseVisualStyleBackColor = false;
            this.ProcesarVentabtn.Click += new System.EventHandler(this.ProcesarVentabtn_Click);
            // 
            // chkImprimirTicket
            // 
            this.chkImprimirTicket.AutoSize = true;
            this.chkImprimirTicket.Checked = true;
            this.chkImprimirTicket.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkImprimirTicket.Location = new System.Drawing.Point(6, 102);
            this.chkImprimirTicket.Name = "chkImprimirTicket";
            this.chkImprimirTicket.Size = new System.Drawing.Size(141, 24);
            this.chkImprimirTicket.TabIndex = 6;
            this.chkImprimirTicket.Text = "Imprimir ticket";
            this.chkImprimirTicket.UseVisualStyleBackColor = true;
            // 
            // btnVistaPrevia
            // 
            this.btnVistaPrevia.BackColor = System.Drawing.Color.LightSteelBlue;
            this.btnVistaPrevia.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.btnVistaPrevia.Location = new System.Drawing.Point(176, 114);
            this.btnVistaPrevia.Name = "btnVistaPrevia";
            this.btnVistaPrevia.Size = new System.Drawing.Size(196, 24);
            this.btnVistaPrevia.TabIndex = 7;
            this.btnVistaPrevia.Text = "Vista previa";
            this.btnVistaPrevia.UseVisualStyleBackColor = false;
            this.btnVistaPrevia.Click += new System.EventHandler(this.BtnVistaPrevia_Click);
            // 
            // cmbMetodoPago1
            // 
            this.cmbMetodoPago1.FormattingEnabled = true;
            this.cmbMetodoPago1.Location = new System.Drawing.Point(233, 58);
            this.cmbMetodoPago1.Name = "cmbMetodoPago1";
            this.cmbMetodoPago1.Size = new System.Drawing.Size(139, 28);
            this.cmbMetodoPago1.TabIndex = 5;
            // 
            // lblTotal
            // 
            this.lblTotal.AutoSize = true;
            this.lblTotal.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotal.ForeColor = System.Drawing.Color.Green;
            this.lblTotal.Location = new System.Drawing.Point(75, 62);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(60, 24);
            this.lblTotal.TabIndex = 4;
            this.lblTotal.Text = "$0,00";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.Green;
            this.label3.Location = new System.Drawing.Point(6, 66);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(51, 18);
            this.label3.TabIndex = 3;
            this.label3.Text = "Total:";
            // 
            // txtCodigoBarras1
            // 
            this.txtCodigoBarras1.Location = new System.Drawing.Point(201, 32);
            this.txtCodigoBarras1.Name = "txtCodigoBarras1";
            this.txtCodigoBarras1.Size = new System.Drawing.Size(219, 26);
            this.txtCodigoBarras1.TabIndex = 1;
            this.txtCodigoBarras1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TxtCodigoBarras1_KeyDown);
            this.txtCodigoBarras1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TxtCodigoBarras1_KeyPress);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.button4);
            this.groupBox2.Controls.Add(this.button2);
            this.groupBox2.Controls.Add(this.dgvCarrito1);
            this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(25, 72);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(1141, 294);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Carrito de compras";
            // 
            // button4
            // 
            this.button4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button4.BackColor = System.Drawing.Color.Orange;
            this.button4.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button4.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.button4.Location = new System.Drawing.Point(25, 261);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(131, 27);
            this.button4.TabIndex = 4;
            this.button4.Text = "Limpiar Todo";
            this.button4.UseVisualStyleBackColor = false;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button2.BackColor = System.Drawing.Color.Firebrick;
            this.button2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button2.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.button2.Location = new System.Drawing.Point(176, 261);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(135, 27);
            this.button2.TabIndex = 3;
            this.button2.Text = "- Eliminar Item";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // dgvCarrito1
            // 
            this.dgvCarrito1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvCarrito1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvCarrito1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCarrito1.Location = new System.Drawing.Point(25, 42);
            this.dgvCarrito1.Name = "dgvCarrito1";
            this.dgvCarrito1.RowHeadersWidth = 62;
            this.dgvCarrito1.Size = new System.Drawing.Size(576, 207);
            this.dgvCarrito1.TabIndex = 0;
            this.dgvCarrito1.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvCarrito_CellDoubleClick);
            this.dgvCarrito1.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvCarrito_CellEndEdit);
            this.dgvCarrito1.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.dgvCarrito_EditingControlShowing);
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
            // lblUltimoCodigo
            // 
            this.lblUltimoCodigo.AutoSize = true;
            this.lblUltimoCodigo.Location = new System.Drawing.Point(548, 35);
            this.lblUltimoCodigo.Name = "lblUltimoCodigo";
            this.lblUltimoCodigo.Size = new System.Drawing.Size(146, 20);
            this.lblUltimoCodigo.TabIndex = 5;
            this.lblUltimoCodigo.Text = "Último: (ninguno)";
            // 
            // panelTop
            // 
            this.panelTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(11)))), ((int)(((byte)(7)))), ((int)(((byte)(17)))));
            this.panelTop.Controls.Add(this.btnCerrar);
            this.panelTop.Controls.Add(this.lblTitulo);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(1222, 42);
            this.panelTop.TabIndex = 6;
            // 
            // btnCerrar
            // 
            this.btnCerrar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCerrar.FlatAppearance.BorderSize = 0;
            this.btnCerrar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCerrar.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnCerrar.ForeColor = System.Drawing.Color.DarkRed;
            this.btnCerrar.Location = new System.Drawing.Point(1180, 3);
            this.btnCerrar.Name = "btnCerrar";
            this.btnCerrar.Size = new System.Drawing.Size(39, 34);
            this.btnCerrar.TabIndex = 1;
            this.btnCerrar.Text = "✕";
            this.btnCerrar.UseVisualStyleBackColor = true;
            this.btnCerrar.Click += new System.EventHandler(this.btnCerrar_Click);
            // 
            // lblTitulo
            // 
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.ForeColor = System.Drawing.Color.White;
            this.lblTitulo.Location = new System.Drawing.Point(12, 8);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(63, 25);
            this.lblTitulo.TabIndex = 2;
            this.lblTitulo.Text = "Venta";
            // 
            // ventas
            // 
            this.ClientSize = new System.Drawing.Size(1222, 609);
            this.ControlBox = false;
            this.Controls.Add(this.panelTop);
            this.Controls.Add(this.lblTitulo1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "ventas";
            this.Load += new System.EventHandler(this.ventas_Load_1);
            this.lblTitulo1.ResumeLayout(false);
            this.lblTitulo1.PerformLayout();
            this.grpPago.ResumeLayout(false);
            this.grpPago.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvCarrito1)).EndInit();
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
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
                cmbMetodoPago1.SelectedIndexChanged += (s, ev) => UpdateProcesarEnabled();
            }

            txtCodigoBarras1?.Focus();
            UpdateProcesarEnabled();
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
                    UpdateProcesarEnabled();
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

        private void Ventas_KeyDown_Shortcuts(object sender, KeyEventArgs e)
        {
            // Atajos de teclado
            if (e.KeyCode == Keys.F2)
            {
                txtCodigoBarras1?.Focus();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.F5)
            {
                ProcesarVenta();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Delete)
            {
                button2_Click(sender, e); // eliminar seleccionado
                e.Handled = true;
            }
            else if (e.Control && e.KeyCode == Keys.L)
            {
                button4_Click(sender, e); // limpiar carrito
                e.Handled = true;
            }
            else if (e.Control && e.KeyCode == Keys.P)
            {
                BtnVistaPrevia_Click(sender, e);
                e.Handled = true;
            }
        }

        // Captura global de tecla para escáner cuando no se está en el textbox
        private void Ventas_KeyPress_Global(object sender, KeyPressEventArgs e)
        {

            // If user is actively typing in the barcode textbox or editing a grid cell (e.g. descuento), don't intercept
            if ((txtCodigoBarras1 != null && txtCodigoBarras1.Focused)
                || (descuentoEditor != null && descuentoEditor.Focused)
                || (dgvCarrito1 != null && dgvCarrito1.IsCurrentCellInEditMode))
            {
                return;
            }

            // Character of control different from Enter: handle scanner input
            if (e.KeyChar == (char)Keys.Enter)
            {
                if (scannerBuffer.Length > 0)
                {
                    txtCodigoBarras1.Text = scannerBuffer.ToString();
                    if (lblUltimoCodigo != null) lblUltimoCodigo.Text = "Último: " + txtCodigoBarras1.Text;
                    AgregarProductoPorCodigo();
                    scannerBuffer.Clear();
                    e.Handled = true;
                }
                return;
            }

            // Ignore other control keys
            if (char.IsControl(e.KeyChar)) return;

            var now2 = DateTime.Now;
            if ((now2 - lastScannerCharTime).TotalMilliseconds > SCAN_TIMEOUT_MS)
            {
                scannerBuffer.Clear();
            }
            lastScannerCharTime = now2;
            scannerBuffer.Append(e.KeyChar);
            e.Handled = true; // prevent falling through to other controls when treating input as scanner data
        }

        private void ConfigurarImpresion()
        {
            printDoc = new PrintDocument();
            // Intentar usar ancho reducido típico (e.g., 80mm ≈ 314 px @100dpi). Ajuste simple.
            try
            {
                printDoc.DefaultPageSettings.PaperSize = new PaperSize("Ticket", 300, 800); // alto se ajustará (se puede ignorar)
            }
            catch { }
            printDoc.PrintPage += PrintDoc_PrintPage;
            // Vista previa
            previewDlg = new PrintPreviewDialog();
            previewDlg.Document = printDoc;
            previewDlg.Width = 900;
            previewDlg.Height = 700;
        }

        private void BtnVistaPrevia_Click(object sender, EventArgs e)
        {
            // Preparar datos si aún no hay lastTicketData
            if (lastTicketData == null || (lastTicketData.Items == null || lastTicketData.Items.Count == 0))
            {
                if (carrito == null || carrito.Count == 0)
                {
                    MessageBox.Show("No hay datos para previsualizar.", "Vista previa", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                lastTicketData = new VentaTicketData
                {
                    VentaId = 0,
                    Fecha = DateTime.Now,
                    MetodoPago = cmbMetodoPago1?.SelectedItem?.ToString() ?? "-",
                    Total = carrito.Sum(x => x.Subtotal),
                    Items = carrito.Select(i => new ItemCarrito
                    {
                        ProductoId = i.ProductoId,
                        NombreProducto = i.NombreProducto,
                        PrecioUnitario = i.PrecioUnitario,
                        Cantidad = i.Cantidad,
                        Descuento = i.Descuento,
                        Subtotal = i.Subtotal
                    }).ToList()
                };
                lastTicketData.EsRegalo = chkEsRegalo?.Checked == true;
            }
            try { previewDlg.ShowDialog(this); }
            catch (Exception ex) { MessageBox.Show("No se pudo mostrar la vista previa: " + ex.Message, "Impresión", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        }

        private void PrintDoc_PrintPage(object sender, PrintPageEventArgs e)
        {
            if (lastTicketData == null)
            {
                e.Graphics.DrawString("Sin datos de ticket", SystemFonts.DefaultFont, Brushes.Black, 10, 10);
                return;
            }

            int y = 5;
            int left = e.MarginBounds.Left;
            int width = e.MarginBounds.Width > 0 ? e.MarginBounds.Width : 280;

            Font fHeader = new Font("Segoe UI", 9, FontStyle.Bold);
            Font fNormal = new Font("Consolas", 8);
            Font fBold = new Font("Consolas", 8, FontStyle.Bold);
            Font fTotal = new Font("Consolas", 10, FontStyle.Bold);

            // --- Helper: centrar texto ---
            void DrawCenter(string text, Font f)
            {
                e.Graphics.DrawString(text, f, Brushes.Black, CenterText(e.Graphics, text, f, width, left), y);
                y += (int)e.Graphics.MeasureString(text, f).Height + 2;
            }

            // --- Helper: medir ancho (TextRenderer) ---
            int TR_Width(string s, Font f) =>
                TextRenderer.MeasureText(s ?? string.Empty, f, new Size(int.MaxValue, int.MaxValue),
                    TextFormatFlags.NoPadding).Width;

            int lineH = TextRenderer.MeasureText("Ay", fNormal, new Size(int.MaxValue, int.MaxValue),
                    TextFormatFlags.NoPadding).Height;

            // --- Helper: dividir texto en líneas que entren en un ancho ---
            string[] WrapLeft(string text, Font font, int maxWidth)
            {
                var words = (text ?? string.Empty).Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                var lines = new List<string>();
                var cur = new StringBuilder();
                foreach (var w in words)
                {
                    string test = cur.Length == 0 ? w : cur + " " + w;
                    if (TR_Width(test, font) > maxWidth)
                    {
                        if (cur.Length > 0) { lines.Add(cur.ToString()); cur.Clear(); }
                        cur.Append(w);
                    }
                    else
                    {
                        if (cur.Length > 0) cur.Append(' ');
                        cur.Append(w);
                    }
                }
                if (cur.Length > 0) lines.Add(cur.ToString());
                if (lines.Count == 0) lines.Add(string.Empty);
                return lines.ToArray();
            }

            // --- Logo ---
            try
            {
                var logo = Gamora_Indumentaria.Properties.Resources.locoparaimprimir;
                if (logo != null)
                {
                    int logoW = 60;
                    int logoH = (int)(logo.Height * (logoW / (float)logo.Width));
                    e.Graphics.DrawImage(logo, left + (width - logoW) / 2, y, logoW, logoH);
                    y += logoH + 4;
                }
            }
            catch { }

            // --- Encabezado ---
            DrawCenter("GAMORA INDUMENTARIA", fHeader);
            DrawCenter("Av. Siempre Viva 123 - San Vicente", fNormal);
            DrawCenter("Tel: (011) 5555-5555", fNormal);
            DrawCenter("Instagram: @GamoraIndumentaria", fNormal);
            y += 2;
            DrawCenter($"Fecha: {lastTicketData.Fecha:dd/MM/yyyy HH:mm}", fNormal);
            DrawCenter($"Venta ID: {lastTicketData.VentaId}   Pago: {lastTicketData.MetodoPago}", fNormal);

            y = DibujarSeparadorCentrado(e.Graphics, y, width, left);
            DrawCenter("DETALLE", fBold);
            y = DibujarSeparadorCentrado(e.Graphics, y, width, left);

            int padding = 6;
            int usableWidth = width - padding * 2;
            int leftCol = left + padding;

            decimal totalBruto = 0m;
            decimal totalDesc = 0m;

            // --- Ítems ---
            foreach (var it in lastTicketData.Items)
            {
                string nombre = (it.NombreProducto ?? "").Trim();
                decimal bruto = it.PrecioUnitario * it.Cantidad;
                decimal descValor = Math.Round(bruto * (it.Descuento / 100m), 2);
                decimal neto = it.Subtotal;

                // Nombre multilínea (centrado)
                var nameLines = WrapLeft(nombre, fNormal, usableWidth);
                foreach (var ln in nameLines)
                {
                    DrawCenter(ln, fNormal);
                }

                if (!lastTicketData.EsRegalo)
                {
                    totalBruto += bruto;
                    totalDesc += descValor;

                    string detalle = $"{it.Cantidad} x {it.PrecioUnitario:C} = {bruto:C}";
                    DrawCenter(detalle, fNormal);

                    if (it.Descuento > 0)
                    {
                        string descTxt = $"-{it.Descuento:0.##}%  (-{descValor:C})";
                        DrawCenter(descTxt, fNormal);
                    }

                    string netoTxt = neto.ToString("C");
                    DrawCenter(netoTxt, fBold);
                    y += 6; // espacio extra
                }
                else
                {
                    string cantTxt = $"Cantidad: {it.Cantidad}";
                    DrawCenter(cantTxt, fNormal);
                    y += 6;
                }
            }

            y = DibujarSeparadorCentrado(e.Graphics, y, width, left);

            // --- Totales --- (omitidos en regalos)
            if (!lastTicketData.EsRegalo)
            {
                DrawCenter($"Subtotal: {totalBruto:C}", fNormal);
                if (totalDesc > 0) DrawCenter($"Descuentos: -{totalDesc:C}", fNormal);

                string totalTxt = "TOTAL: " + lastTicketData.Total.ToString("C");
                e.Graphics.DrawString(totalTxt, fTotal, Brushes.Black, CenterText(e.Graphics, totalTxt, fTotal, width, left), y);
                y += (int)e.Graphics.MeasureString(totalTxt, fTotal).Height + 4;
            }

            y = DibujarSeparadorCentrado(e.Graphics, y, width, left);

            // --- Pie ---
            if (lastTicketData.EsRegalo)
            {
                DrawCenter("Comprobante de regalo - sin precios", fNormal);
                DrawCenter("¡Gracias por su compra!", fNormal);
            }
            else
            {
                DrawCenter("Cambio válido dentro de 15 días con ticket", fNormal);
                DrawCenter("¡Gracias por su compra!", fNormal);
                DrawCenter("No válido como factura", fNormal);
            }

            e.HasMorePages = false;
        }






        private int DibujarSeparadorCentrado(Graphics g, int y, int width, int left)
        {
            int usable = width - 10;
            int seg = (int)(usable * 0.7); // 70% del ancho
            int start = left + (usable - seg) / 2;
            g.DrawLine(Pens.Black, start, y, start + seg, y);
            return y + 4;
        }

        // (Restaurado estilo simple, helpers avanzados removidos)

        private float CenterText(Graphics g, string text, Font f, int width, int left)
        {
            var size = g.MeasureString(text, f);
            return left + (width - size.Width) / 2f;
        }

        private float AlignRight(Graphics g, string text, Font f, int width, int left)
        {
            var size = g.MeasureString(text, f);
            return left + (width - size.Width) - 5;
        }

        private void UpdateProcesarEnabled()
        {
            bool puede = carrito != null && carrito.Count > 0 && cmbMetodoPago1 != null && cmbMetodoPago1.SelectedIndex >= 0;
            if (ProcesarVentabtn != null) ProcesarVentabtn.Enabled = puede;
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        private void txtCodigoBarras1_TextChanged(object sender, EventArgs e)
        {

        }


    }
}
