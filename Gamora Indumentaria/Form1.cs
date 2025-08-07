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
    public partial class Form1 : Form
    {
        //string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\VentasDB.mdf;Integrated Security=True;";

        private string perfilUsuario;
        private Panel panelContenido; // Panel para contener los formularios hijos

        public Form1(string perfil)
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(30, 30, 30); // Fondo oscuro para el formulario
            this.FormBorderStyle = FormBorderStyle.None; // Quitar borde si querés

            perfilUsuario = perfil; // ← ahora sí existe la variable "perfil"
            panelsubprincipal.Visible = false;

            // Crear panel de contenido principal
            CrearPanelContenido();

            AplicarPerfil();
        }

        /// <summary>
        /// Crea el panel principal donde se cargarán los formularios hijos
        /// </summary>
        private void CrearPanelContenido()
        {
            panelContenido = new Panel();
            panelContenido.Name = "panelContenido";
            panelContenido.Dock = DockStyle.Fill;
            panelContenido.BackColor = Color.FromArgb(240, 240, 240);
            panelContenido.Margin = new Padding(200, 50, 0, 0); // Dejar espacio para el menú lateral

            // Posicionar el panel a la derecha del menú lateral
            panelContenido.Location = new Point(200, 40); // Después del menú lateral y barra superior
            panelContenido.Size = new Size(this.Width - 200, this.Height - 40);
            panelContenido.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            this.Controls.Add(panelContenido);
            panelContenido.BringToFront(); // Traer al frente para que no quede debajo de otros controles
        }

        /// <summary>
        /// Carga un formulario hijo dentro del panel de contenido
        /// </summary>
        private void CargarFormularioHijo(Form formularioHijo)
        {
            // Limpiar el panel de contenido
            panelContenido.Controls.Clear();

            // Configurar el formulario hijo
            formularioHijo.TopLevel = false;
            formularioHijo.FormBorderStyle = FormBorderStyle.None;
            formularioHijo.Dock = DockStyle.Fill;

            // Agregar al panel y mostrar
            panelContenido.Controls.Add(formularioHijo);
            formularioHijo.Show();
        }




        private void Form1_Load(object sender, EventArgs e)
        {

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

        private void timer1_Tick(object sender, EventArgs e)
        {
            DateTime ahora = DateTime.Now;
            lblHora.Text = ahora.ToString("dddd dd 'de' MMMM yyyy - HH:mm:ss");

        }

        private void button10_Click(object sender, EventArgs e)
        {
            this.Close();

        }

        private void button5_Click(object sender, EventArgs e)
        {
            CargarFormularioHijo(new inventario());
        }

        private void panel6_Paint(object sender, PaintEventArgs e)
        {
            button1.Text = "";
            button1.FlatStyle = FlatStyle.Flat;
            button1.FlatAppearance.BorderSize = 0;
            button1.BackColor = this.BackColor; // o cualquier color del fondo

        }

        private void btnsubprincipal_Click(object sender, EventArgs e)
        {
            panelsubprincipal.Visible = !panelsubprincipal.Visible;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            CargarFormularioHijo(new productos());
        }

        private void panel7_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnsubadministracion_Click(object sender, EventArgs e)
        {
            panelsubadministracion.Visible = !panelsubadministracion.Visible;
        }

        private void lblHora_Click(object sender, EventArgs e)
        {

        }

        private void btnsubprincipal_MouseEnter(object sender, EventArgs e)
        {
            // Button btn = sender as Button;
            //btn.BackColor = Color.FromArgb(0, 120, 215); // Color al pasar el mouse
        }

        private void btnsubprincipal_MouseLeave(object sender, EventArgs e)
        {
            // Button btn = sender as Button;
            // btn.BackColor = Color.FromArgb(255, 255, 255); // Vuelve al color original
        }
        private void AplicarPerfil()
        {
            if (perfilUsuario == "empleado")
            {
                // Mostrar solo los botones necesarios para empleados
                button5.Visible = true; // Ejemplo: Ventas
                button6.Visible = true; // Ejemplo: Perfil
                btnsubprincipal.Visible = true;
                // Ocultar el resto
                btnsubadministracion.Visible = false;
                //btnsubprincipal.Visible = false;
                button10.Visible = false;
                panel7.Visible = false;

                // Ocultá acá cualquier otro botón que no querés que el empleado vea
            }
            else if (perfilUsuario == "administrador")
            {
                // Mostrar todos los botones
                button5.Visible = true;
                button6.Visible = true;
                btnsubadministracion.Visible = true;
                btnsubprincipal.Visible = true;
                button10.Visible = true;
                // Asegurate de que todos los botones estén visibles
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            CargarFormularioHijo(new agregarpruducto());
        }

        private void button4_Click(object sender, EventArgs e)
        {
            CargarFormularioHijo(new ventas());
        }
    }
}







