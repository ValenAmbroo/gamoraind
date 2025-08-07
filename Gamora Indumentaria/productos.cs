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

namespace Gamora_Indumentaria
{
    public partial class productos : Form
    {
        string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\VentasDB.mdf;Integrated Security=True;";


        public productos()
        {
            InitializeComponent();
        }
      
        private void productos_Load(object sender, EventArgs e)
        {
            cboCategoria.Items.AddRange(new string[]
   {
        "BUZOS", "CAMPERAS", "REMERAS", "CHALECOS", "JOGGING",
        "JEANS HOMBRE", "JEANS DAMA", "BOXER", "ZAPATILLAS",
        "GORRAS", "CADENITAS", "VAPER", "RELOJ", "ANTEOJOS"
   });

            cboCategoria.SelectedIndex = 0; // selecciona por defecto
            ActualizarGrid();               // muestra los datos
        }

        // ✔️ Este método carga productos por categoría en el DataGridView
        private void ActualizarGrid()
        {
            string categoria = cboCategoria.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(categoria)) return;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                //con.Open();
                SqlDataAdapter da = new SqlDataAdapter("SELECT Id, Nombre, Talle, Cantidad FROM Inventario WHERE Categoria = @cat", con);
                da.SelectCommand.Parameters.AddWithValue("@cat", categoria);
                DataTable dt = new DataTable();
               // da.Fill(dt);
                dgvInventario.DataSource = dt;
            }
        }

        // 🔁 Cada vez que se cambia la categoría, se actualiza la grilla
        private void cboCategoria_SelectedIndexChanged(object sender, EventArgs e)
        {
            ActualizarGrid();
        }

        // ➕ Botón para abrir el formulario de agregar producto
        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if (cboCategoria.SelectedItem != null)
            {
                string categoria = cboCategoria.SelectedItem.ToString();
                agregarpruducto form = new agregarpruducto(categoria); // o FormAgregar si corregiste el nombre
                form.ShowDialog();

                // Recargar productos
                ActualizarGrid();
            }
           
        }

        private void btnAgregar_Click_1(object sender, EventArgs e)
        {
            agregarpruducto agregarpruducto = new agregarpruducto();
            this.Hide();
            agregarpruducto.FormClosed += (s, args) => this.Close(); // Cierra Form2 cuando se cierre Form3
            agregarpruducto.Show();
            //Verifica si el form actual (Form3) está en pantalla completa
            bool esPantallaCompleta = this.WindowState == FormWindowState.Maximized;




            // Aplica el mismo estado de ventana que tenía el anterior
            agregarpruducto.WindowState = esPantallaCompleta ? FormWindowState.Maximized : FormWindowState.Normal;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dgvInventario_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btnRefrescar_Click(object sender, EventArgs e)
        {
            ActualizarGrid();
        }

        private void cboCategoria_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }
    }
}




