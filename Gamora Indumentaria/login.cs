using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Gamora_Indumentaria
{
    public partial class login : Form
    {
        //private string contraseñaCorrecta = "gamora123"; // <- Cambiá por tu clave
        public login()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void button1_Click(object sender, EventArgs e)
        {

            this.WindowState = FormWindowState.Minimized;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Maximized)
                this.WindowState = FormWindowState.Normal;
            else
                this.WindowState = FormWindowState.Maximized;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void login_Load(object sender, EventArgs e)
        {

        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            //Verifica si el form actual (Form3) está en pantalla completa
            bool esPantallaCompleta = this.WindowState == FormWindowState.Maximized;




            

            string contraseñaCorrecta = "1234"; // Cambiá esto por tu contraseña real

            if (txtPassword.Text == contraseñaCorrecta)
            {
                Form1 form1 = new Form1("administrador");
               // Form1 form1 = new Form1(); // Abre Form1
                form1.Show();              // Muestra Form1
                this.Hide();               // Oculta el login
                                           // Aplica el mismo estado de ventana que tenía el anterior
                form1.WindowState = esPantallaCompleta ? FormWindowState.Maximized : FormWindowState.Normal;
            }
            else
            {
                MessageBox.Show("Contraseña incorrecta", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtPassword.Clear();
                txtPassword.Focus();


               
            }
    }

        private void txtPassword_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
