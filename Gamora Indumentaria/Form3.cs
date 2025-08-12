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
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            login login = new login();
            this.Hide();
            login.FormClosed += (s, args) => this.Close(); // Cierra Form2 cuando se cierre Form3
            login.Show();
            //Verifica si el form actual (Form3) está en pantalla completa
            bool esPantallaCompleta = this.WindowState == FormWindowState.Maximized;




            // Aplica el mismo estado de ventana que tenía el anterior
            login.WindowState = esPantallaCompleta ? FormWindowState.Maximized : FormWindowState.Normal;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();

        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Maximized)
                this.WindowState = FormWindowState.Normal;
            else
                this.WindowState = FormWindowState.Maximized;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;

        }

        private void button2_Click(object sender, EventArgs e)
        {
           
            Form1 form1 = new Form1("empleado");

            //Form1 form1 = new Form1();
            this.Hide();
            form1.FormClosed += (s, args) => this.Close(); // Cierra Form2 cuando se cierre Form3
            form1.Show();
            //Verifica si el form actual (Form3) está en pantalla completa
            bool esPantallaCompleta = this.WindowState == FormWindowState.Maximized;




            // Aplica el mismo estado de ventana que tenía el anterior
            form1.WindowState = esPantallaCompleta ? FormWindowState.Maximized : FormWindowState.Normal;

        }

        private void Form3_Load(object sender, EventArgs e)
        {

        }
    }
}
