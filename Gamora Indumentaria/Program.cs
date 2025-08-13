using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Gamora_Indumentaria.Data; // Para inicializar la base de datos

namespace Gamora_Indumentaria
{
    internal static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            // Inicializar/actualizar estructura de la base de datos (agrega columna PrecioCosto si falta)
            try
            {
                DatabaseManager.InitializeDatabase();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error inicializando la base de datos: " + ex.Message, "DB Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            Application.Run(new Form2());
        }
    }
}
