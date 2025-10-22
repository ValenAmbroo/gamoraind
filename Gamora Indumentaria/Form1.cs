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

        // Variables para arrastrar el formulario
        private bool arrastrando = false;
        private Point ultimaPosicion;

        public Form1(string perfil)
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(30, 30, 30); // Fondo oscuro para el formulario
            this.FormBorderStyle = FormBorderStyle.None; // Quitar borde si querés

            perfilUsuario = perfil; // ← ahora sí existe la variable "perfil"
            // Ocultar menú de administración si el usuario es empleado
            if (perfilUsuario != null && perfilUsuario.ToLower() == "empleado")
            {
                if (btnAdministracion != null)
                    btnAdministracion.Visible = false;
                if (panelClientes != null)
                    panelClientes.Visible = false;
            }


            // Crear panel de contenido principal
            CrearPanelContenido();

            // Configurar eventos para arrastrar el formulario desde panel1
            ConfigurarArrastre();




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
            try
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
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar formulario: " + ex.Message + "\n\nDetalles: " + ex.ToString(),
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Configura los eventos para permitir arrastrar el formulario desde panel1
        /// </summary>
        private void ConfigurarArrastre()
        {
            // Agregar eventos al panel1 (barra superior)
            panel1.MouseDown += Panel1_MouseDown;
            panel1.MouseMove += Panel1_MouseMove;
            panel1.MouseUp += Panel1_MouseUp;

            // También agregar eventos al label1 si está en panel1
            label1.MouseDown += Panel1_MouseDown;
            label1.MouseMove += Panel1_MouseMove;
            label1.MouseUp += Panel1_MouseUp;
        }

        private void Panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                arrastrando = true;
                ultimaPosicion = e.Location;
            }
        }

        private void Panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (arrastrando)
            {
                // Calcular la nueva posición del formulario
                Point nuevaPosicion = this.Location;
                nuevaPosicion.X += e.X - ultimaPosicion.X;
                nuevaPosicion.Y += e.Y - ultimaPosicion.Y;
                this.Location = nuevaPosicion;
            }
        }

        private void Panel1_MouseUp(object sender, MouseEventArgs e)
        {
            arrastrando = false;
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
            // Ocultar Form1 y mostrar el login
            this.Hide();

            // Crear una nueva instancia del login
            Form3 loginForm = new Form3();
            loginForm.Show();

            // Cuando se cierre el login, cerrar toda la aplicación
            loginForm.FormClosed += (s, args) => Application.Exit();
        }








        private void lblHora_Click(object sender, EventArgs e)
        {

        }






        private void button4_Click(object sender, EventArgs e)
        {
            CargarFormularioHijo(new ventas());
        }














        private void RegistrarMouseDownRecursivo(Control root)
        {
            if (root == null) return;
            root.MouseDown -= GlobalMouseDownCerrarSubmenus;
            root.MouseDown += GlobalMouseDownCerrarSubmenus;
            foreach (Control child in root.Controls)
            {
                RegistrarMouseDownRecursivo(child);
            }
        }

        private void GlobalMouseDownCerrarSubmenus(object sender, MouseEventArgs e)
        {


        }

        private bool PuntoDentroDe(Control ctrl, Point screenPoint)
        {
            if (ctrl == null) return false;
            Point local = ctrl.PointToClient(screenPoint);
            return local.X >= 0 && local.Y >= 0 && local.X < ctrl.Width && local.Y < ctrl.Height;
        }












        private void btnConsultas_Click(object sender, EventArgs e)
        {
            try
            {
                CargarFormularioHijo(new inventario(perfilUsuario == "administrador"));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al abrir el inventario: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button19_Click(object sender, EventArgs e)
        {
            try
            {
                CargarFormularioHijo(new CierreCaja(perfilUsuario != null && perfilUsuario.ToLower() == "administrador"));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al abrir Cierre De Caja: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAltas_Click(object sender, EventArgs e)
        {
            CargarFormularioHijo(new ventas());
        }

        private void button15_Click(object sender, EventArgs e)
        {

        }

        private void btnModificaciones_Click(object sender, EventArgs e)
        {
            CargarFormularioHijo(new HistorialVentas());
        }

        private void button20_Click(object sender, EventArgs e)
        {
            try
            {
                CargarFormularioHijo(new Estadisticas());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al abrir estadísticas: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button21_Click(object sender, EventArgs e)
        {
            try
            {
                CargarFormularioHijo(new EstadisticasVentas(perfilUsuario != null && perfilUsuario.ToLower() == "administrador"));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al abrir estadísticas: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button22_Click(object sender, EventArgs e)
        {
            try
            {
                // Abrir formulario de edición con selector integrado
                CargarFormularioHijo(new editarproducto());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al abrir Cierre De Caja: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAlta_Click(object sender, EventArgs e)
        {
            CargarFormularioHijo(new agregarpruducto());
        }

        private void panelClientes_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            CargarFormularioHijo(new agregarcategoria());

        }

        private void button5_Click(object sender, EventArgs e)
        {
            CargarFormularioHijo(new AdministrarCategorias());

        }

        private void button6_Click(object sender, EventArgs e)
        {
            CargarFormularioHijo(new NotasForm());
        }
    }
}







