using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Gamora_Indumentaria.Models;

namespace Gamora_Indumentaria.Data
{
    public class NotasDAL
    {
        public NotasDAL()
        {
            DatabaseManager.InitializeDatabase();
        }

        public List<Nota> ObtenerNotas()
        {
            var notas = new List<Nota>();
            string query = "SELECT Id, Titulo, Fecha, Texto FROM Notas ORDER BY Fecha DESC, Id DESC";
            DataTable dt = DatabaseManager.ExecuteQuery(query);
            foreach (DataRow r in dt.Rows)
            {
                notas.Add(new Nota
                {
                    Id = Convert.ToInt32(r["Id"]),
                    Titulo = r["Titulo"].ToString(),
                    Fecha = Convert.ToDateTime(r["Fecha"]),
                    Texto = r["Texto"].ToString()
                });
            }
            return notas;
        }

        public int AgregarNota(string titulo, DateTime fecha, string texto)
        {
            string query = @"INSERT INTO Notas (Titulo, Fecha, Texto) VALUES (@t, @f, @x); SELECT SCOPE_IDENTITY();";
            object res = DatabaseManager.ExecuteScalar(query,
                new SqlParameter("@t", titulo ?? string.Empty),
                new SqlParameter("@f", fecha),
                new SqlParameter("@x", (object)texto ?? DBNull.Value));
            return Convert.ToInt32(res);
        }

        public void ActualizarNota(Nota n)
        {
            string query = @"UPDATE Notas SET Titulo=@t, Fecha=@f, Texto=@x WHERE Id=@id";
            DatabaseManager.ExecuteNonQuery(query,
                new SqlParameter("@t", n.Titulo ?? string.Empty),
                new SqlParameter("@f", n.Fecha),
                new SqlParameter("@x", (object)n.Texto ?? DBNull.Value),
                new SqlParameter("@id", n.Id));
        }

        public void EliminarNota(int id)
        {
            DatabaseManager.ExecuteNonQuery("DELETE FROM Notas WHERE Id=@id", new SqlParameter("@id", id));
        }
    }
}
