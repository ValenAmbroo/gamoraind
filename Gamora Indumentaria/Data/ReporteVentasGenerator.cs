using System;
using System.Data;
using System.Data.SqlClient;

namespace Gamora_Indumentaria.Data
{
    /// <summary>
    /// Provee métodos centralizados para obtener datos de reportes y estadísticas de ventas.
    /// </summary>
    public static class ReporteVentasGenerator
    {
        public static DataTable GetResumen(DateTime desde, DateTime hasta)
        {
            string query = @"SELECT 
                                ISNULL(SUM(v.Total), 0) AS TotalVentas,
                                ISNULL(SUM(dv.Cantidad), 0) AS TotalUnidades,
                                COUNT(DISTINCT v.Id) AS TotalTransacciones,
                                CASE WHEN COUNT(DISTINCT v.Id) > 0 
                                     THEN ISNULL(SUM(v.Total),0)/COUNT(DISTINCT v.Id) ELSE 0 END AS PromedioVenta
                             FROM Ventas v
                             LEFT JOIN DetalleVentas dv ON v.Id = dv.VentaId
                             WHERE v.FechaVenta >= @Desde AND v.FechaVenta < @Hasta";

            SqlParameter[] parameters = {
                new SqlParameter("@Desde", desde),
                new SqlParameter("@Hasta", hasta)
            };
            return DatabaseManager.ExecuteQuery(query, parameters);
        }

        public static DataTable GetVentasPorDia(DateTime desde, DateTime hasta)
        {
            string query = @"SELECT 
                                CAST(FechaVenta AS DATE) AS Dia,
                                SUM(Total) AS TotalDia,
                                COUNT(*) AS CantidadVentas
                             FROM Ventas
                             WHERE FechaVenta >= @Desde AND FechaVenta < @Hasta
                             GROUP BY CAST(FechaVenta AS DATE)
                             ORDER BY Dia";

            SqlParameter[] parameters = {
                new SqlParameter("@Desde", desde),
                new SqlParameter("@Hasta", hasta)
            };
            return DatabaseManager.ExecuteQuery(query, parameters);
        }

        public static DataTable GetTopProductos(int top, DateTime desde, DateTime hasta)
        {
            string query = string.Format(@"SELECT TOP {0} 
                                i.Nombre AS Producto,
                                SUM(dv.Cantidad) AS CantidadVendida,
                                SUM(dv.Subtotal) AS Importe
                             FROM DetalleVentas dv
                             INNER JOIN Ventas v ON dv.VentaId = v.Id
                             INNER JOIN Inventario i ON dv.ProductoId = i.Id
                             WHERE v.FechaVenta >= @Desde AND v.FechaVenta < @Hasta
                             GROUP BY i.Nombre
                             ORDER BY CantidadVendida DESC", top);

            SqlParameter[] parameters = {
                new SqlParameter("@Desde", desde),
                new SqlParameter("@Hasta", hasta)
            };
            return DatabaseManager.ExecuteQuery(query, parameters);
        }

        public static DataTable GetVentasDetalle(DateTime desde, DateTime hasta)
        {
            string query = @"SELECT v.Id AS VentaId, v.FechaVenta, v.MetodoPago,
                                    i.Nombre AS Producto, dv.Cantidad, dv.PrecioUnitario, dv.Subtotal
                             FROM Ventas v
                             INNER JOIN DetalleVentas dv ON v.Id = dv.VentaId
                             INNER JOIN Inventario i ON dv.ProductoId = i.Id
                             WHERE v.FechaVenta >= @Desde AND v.FechaVenta < @Hasta
                             ORDER BY v.FechaVenta DESC, v.Id";

            SqlParameter[] parameters = {
                new SqlParameter("@Desde", desde),
                new SqlParameter("@Hasta", hasta)
            };
            return DatabaseManager.ExecuteQuery(query, parameters);
        }

        public static DataTable GetGananciaTotal(DateTime desde, DateTime hasta)
        {
            // Excluir ventas marcadas como regalo para que no sumen a la ganancia
            bool tieneEsRegalo = DatabaseManager.ColumnExists("Ventas", "EsRegalo");
            string whereExtra = tieneEsRegalo ? " AND ISNULL(v.EsRegalo,0)=0" : string.Empty;
            string query = $@"
                SELECT ISNULL(SUM((dv.PrecioUnitario - ISNULL(i.PrecioCompra,0)) * dv.Cantidad), 0) AS Ganancia
                FROM DetalleVentas dv
                INNER JOIN Ventas v ON dv.VentaId = v.Id
                INNER JOIN Inventario i ON dv.ProductoId = i.Id
                WHERE v.FechaVenta >= @Desde AND v.FechaVenta < @Hasta{whereExtra}";

            SqlParameter[] parameters = {
                new SqlParameter("@Desde", desde),
                new SqlParameter("@Hasta", hasta)
            };
            return DatabaseManager.ExecuteQuery(query, parameters);
        }

        public static string GenerarCsvDetalle(DataTable detalle)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine("VentaId;FechaVenta;MetodoPago;Producto;Cantidad;PrecioUnitario;Subtotal");
            foreach (System.Data.DataRow row in detalle.Rows)
            {
                sb.AppendFormat("{0};{1:yyyy-MM-dd HH:mm};{2};{3};{4};{5};{6}\n",
                    row["VentaId"], row["FechaVenta"], row["MetodoPago"], row["Producto"],
                    row["Cantidad"], row["PrecioUnitario"], row["Subtotal"]);
            }
            return sb.ToString();
        }
    }
}
