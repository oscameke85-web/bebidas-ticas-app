namespace BebidasTicasPedidos.Models
{
    public class Pedido
    {
        public int Id { get; set; }

        public int ClienteId { get; set; }

        public Cliente? Cliente { get; set; }

        public DateTime Fecha { get; set; } = DateTime.Now;

        public decimal Total { get; set; }

        public string Estado { get; set; } = "Pendiente";

        public string? MetodoPago { get; set; }

        public string? Observaciones { get; set; }

        public List<DetallePedido> DetallePedido { get; set; } = new();
    }
}