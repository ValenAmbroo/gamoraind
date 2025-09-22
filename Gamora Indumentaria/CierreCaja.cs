using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Gamora_Indumentaria.Data;

namespace Gamora_Indumentaria
{
    public partial class CierreCaja : Form
    {
        private readonly bool _isAdmin;

        public CierreCaja(bool isAdmin = false)
        {
            _isAdmin = isAdmin;
            InitializeComponent();
            dtpFecha.Value = DateTime.Today;
            ConfigurarGrid();
            // Ocultar ganancia para no administradores
            if (!_isAdmin)
            {
                if (lblGananciaDia != null) lblGananciaDia.Visible = false;
            }
        }

        private void ConfigurarGrid()
        {
            dgvDetalles.AutoGenerateColumns = false;
            dgvDetalles.Columns.Clear();
            dgvDetalles.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "VentaId", HeaderText = "Venta" });
            dgvDetalles.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Hora", HeaderText = "Hora" });
            dgvDetalles.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Total", HeaderText = "Total", DefaultCellStyle = { Format = "C2" } });
            // Columnas de costo y ganancia removidas para no mostrar precio de costo
            dgvDetalles.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Items", HeaderText = "Items" });
            dgvDetalles.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Metodo", HeaderText = "Pago" });
            dgvDetalles.EnableHeadersVisualStyles = false;
            dgvDetalles.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 58, 64);
            dgvDetalles.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvDetalles.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dgvDetalles.DefaultCellStyle.SelectionBackColor = Color.FromArgb(0, 123, 255);
            dgvDetalles.DefaultCellStyle.SelectionForeColor = Color.White;
        }

        private void btnCalcular_Click(object sender, EventArgs e)
        {
            CargarResumen(false);
        }

        private void btnCerrarCaja_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("Confirmar cierre de caja del día?", "Cierre", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
                DatabaseManager.CerrarCaja(dtpFecha.Value.Date);
                MessageBox.Show("Cierre registrado", "Cierre", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarResumen(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Cierre", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void CargarResumen(bool trasCierre)
        {
            DateTime dia = dtpFecha.Value.Date;
            string sql = @"SELECT v.Id AS VentaId, v.FechaVenta, v.Total, d.Cantidad, d.PrecioUnitario, ISNULL(d.CostoUnitario,0) CostoUnitario, v.MetodoPago FROM Ventas v INNER JOIN DetalleVentas d ON v.Id=d.VentaId WHERE CONVERT(date,v.FechaVenta)=@F ORDER BY v.FechaVenta";
            var dt = DatabaseManager.ExecuteQuery(sql, new System.Data.SqlClient.SqlParameter("@F", dia));
            if (dt.Rows.Count == 0)
            {
                dgvDetalles.DataSource = null;
                lblResumen.Text = "Sin ventas en el día";
                if (lblGananciaDia != null) lblGananciaDia.Text = "Ganancia: $0,00";
                return;
            }
            var agrupado = dt.AsEnumerable().GroupBy(r => r.Field<int>("VentaId")).Select(g => new
            {
                VentaId = g.Key,
                Hora = g.Min(r => r.Field<DateTime>("FechaVenta")).ToString("HH:mm"),
                Total = g.First().Field<decimal>("Total"),
                Items = g.Sum(r => r.Field<int>("Cantidad")),
                Metodo = g.First().Field<string>("MetodoPago")
            }).OrderBy(x => x.Hora).ToList();
            dgvDetalles.DataSource = agrupado;
            decimal total = agrupado.Sum(a => a.Total);
            int ventas = agrupado.Count;
            int items = agrupado.Sum(a => a.Items);
            lblResumen.Text = $"Ventas: {ventas} | Items: {items} | Total: {total:C2}";
            if (trasCierre) lblResumen.Text += " (Cierre registrado)";

            // Calcular ganancia del día (excluir regalos si la columna existe) solo para admin
            try
            {
                if (_isAdmin && lblGananciaDia != null)
                {
                    bool tieneEsRegalo = DatabaseManager.ColumnExists("Ventas", "EsRegalo");
                    string filtroRegalo = tieneEsRegalo ? " AND ISNULL(v.EsRegalo,0)=0" : string.Empty;
                    string sqlGan = $@"SELECT ISNULL(SUM((d.PrecioUnitario - ISNULL(i.PrecioCompra,0)) * d.Cantidad),0)
                                   FROM Ventas v
                                   INNER JOIN DetalleVentas d ON v.Id=d.VentaId
                                   INNER JOIN Inventario i ON d.ProductoId = i.Id
                                   WHERE CONVERT(date, v.FechaVenta)=@F{filtroRegalo}";
                    var val = DatabaseManager.ExecuteScalar(sqlGan, new System.Data.SqlClient.SqlParameter("@F", dia));
                    decimal gan = 0m;
                    if (val != null && val != DBNull.Value) gan = Convert.ToDecimal(val);
                    lblGananciaDia.Text = $"Ganancia: {gan:C2}";
                }
            }
            catch
            {
                if (_isAdmin && lblGananciaDia != null) lblGananciaDia.Text = "Ganancia: —";
            }
        }

        private void CierreCaja_Load(object sender, EventArgs e)
        {
            CargarResumen(false);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
