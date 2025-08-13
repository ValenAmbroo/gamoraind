namespace Gamora_Indumentaria.Models
{
    public class Producto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        // Precio de venta actual (mantener propiedad original para compatibilidad)
        public decimal Precio { get; set; }
        // Nuevo: Precio de venta explícito (alias de Precio) y precio de compra
        public decimal? PrecioVenta { get { return Precio; } set { if (value.HasValue) Precio = value.Value; } }
        public decimal? PrecioCosto { get; set; }
        public int Stock { get; set; }
        public string Categoria { get; set; }
        public string CodigoBarras { get; set; }
    }
}
