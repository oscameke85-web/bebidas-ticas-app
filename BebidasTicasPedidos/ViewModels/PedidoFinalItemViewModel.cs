namespace BebidasTicasPedidos.ViewModels
{
    public class PedidoFinalItemViewModel
    {
        public int ProductoId { get; set; }

        public string Producto { get; set; } = string.Empty;

        public int Cantidad { get; set; }

        public decimal PrecioUnitario { get; set; }

        public decimal Subtotal { get; set; }
    }
}