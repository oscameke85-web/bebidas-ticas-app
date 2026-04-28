namespace BebidasTicasPedidos.Models
{
    public class PedidoFinal
    {
        public int Id { get; set; }

        public DateTime Fecha { get; set; } = DateTime.Now;

        public int TotalUnidades { get; set; }

        public decimal TotalDinero { get; set; }

        public List<DetallePedidoFinal> Detalles { get; set; } = new();
    }
}
