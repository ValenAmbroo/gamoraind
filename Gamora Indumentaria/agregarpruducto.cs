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
    public partial class agregarpruducto : Form
    {
        private string _categoria;
        private string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\VentasDB.mdf;Integrated Security=True;";
        private string categoria;

        public agregarpruducto()
        {
            InitializeComponent();
            
            _categoria = _categoria;
            
        }

        public agregarpruducto(string categoria)
        {
            this.categoria = categoria;
        }

        private void agregarpruducto_Load(object sender, EventArgs e)
        {
            InitializeComponent();
            _categoria = _categoria;
            //lblCategoriaValor.Text = _categoria;

            // Cargar talles en el comboBox
            cboTalle.Items.AddRange(new string[] { "S", "M", "L", "XL", "38", "40", "42", "44", "46", "N/A" });
            cboTalle.SelectedIndex = 0;
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text) || cboTalle.SelectedIndex == -1)
            {
                MessageBox.Show("Por favor, completá todos los campos.");
                return;
            }

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("INSERT INTO Inventario (Categoria, Nombre, Talle, Cantidad) VALUES (@cat, @nom, @talle, @cant)", con);
                cmd.Parameters.AddWithValue("@cat", _categoria);
                cmd.Parameters.AddWithValue("@nom", txtNombre.Text);
                cmd.Parameters.AddWithValue("@talle", cboTalle.Text);
                cmd.Parameters.AddWithValue("@cant", (int)nudCantidad.Value);

                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Producto agregado exitosamente.");
            this.Close(); // Cierra el formulario luego de guardar
        }

        private void lblCategoriaValor_Click(object sender, EventArgs e)
        {
           
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cboCategoria_SelectedIndexChanged(object sender, EventArgs e)
        {
            cboCategoria.Items.AddRange(new string[]
{
        "BUZOS", "CAMPERAS", "REMERAS", "CHALECOS", "JOGGING",
        "JEANS HOMBRE", "JEANS DAMA", "BOXER", "ZAPATILLAS",
        "GORRAS", "CADENITAS", "VAPER", "RELOJ", "ANTEOJOS"
});

            cboCategoria.SelectedIndex = 0; // selecciona por defecto
                        // muestra los datos
        }
    }
    }
