using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Gamora_Indumentaria
{
    public partial class TestDBConnection : Form
    {
        public TestDBConnection()
        {
            InitializeComponent();
            TestearConexion();
        }

        private void TestearConexion()
        {
            try
            {
                // Conexión sin archivos MDF
                string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=GamoraIndumentariaDB;Integrated Security=True;Connect Timeout=30;";

                // Crear la base de datos primero
                string masterConnectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True;Connect Timeout=30;";

                using (SqlConnection masterConnection = new SqlConnection(masterConnectionString))
                {
                    masterConnection.Open();

                    string createDbQuery = @"
                        IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'GamoraIndumentariaDB')
                        BEGIN
                            CREATE DATABASE GamoraIndumentariaDB;
                        END";

                    SqlCommand createDbCmd = new SqlCommand(createDbQuery, masterConnection);
                    createDbCmd.ExecuteNonQuery();
                }

                // Ahora conectar a la base de datos creada
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Crear una tabla de prueba
                    string createTableQuery = @"
                        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'TestTable')
                        BEGIN
                            CREATE TABLE TestTable (
                                Id INT IDENTITY(1,1) PRIMARY KEY,
                                Mensaje NVARCHAR(100)
                            );
                            INSERT INTO TestTable (Mensaje) VALUES ('Conexión exitosa!');
                        END";

                    SqlCommand createTableCmd = new SqlCommand(createTableQuery, connection);
                    createTableCmd.ExecuteNonQuery();

                    // Leer el mensaje
                    SqlCommand selectCmd = new SqlCommand("SELECT Mensaje FROM TestTable", connection);
                    string mensaje = selectCmd.ExecuteScalar().ToString();

                    MessageBox.Show(string.Format("¡Éxito! Base de datos funcionando: {0}", mensaje), "Prueba de Conexión",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Error en la prueba: {0}\n\nDetalle completo:\n{1}", ex.Message, ex), "Error de Prueba",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializeComponent()
        {
            this.Text = "Prueba de Conexión DB";
            this.Size = new System.Drawing.Size(300, 200);
            this.StartPosition = FormStartPosition.CenterScreen;
        }
    }
}
