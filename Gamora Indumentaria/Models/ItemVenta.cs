namespace Gamora_Indumentaria.Models
{
    /// <summary>
    /// Representa un ítem de venta genérico, usado tanto para el carrito como para el detalle de venta
    /// </summary>
    public class ItemVenta
    {
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Descuento { get; set; }
    }
}
