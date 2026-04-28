namespace BebidasTicasPedidos.Models
{
    public class DetallePedidoFinal
    {
        public int Id { get; set; }

        public int PedidoFinalId { get; set; }

        public PedidoFinal? PedidoFinal { get; set; }

        public int ProductoId { get; set; }

        public Producto? Producto { get; set; }

        public int Cantidad { get; set; }

        public decimal PrecioUnitario { get; set; }

        public decimal Subtotal { get; set; }
    }
}