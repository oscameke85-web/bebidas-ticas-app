using Microsoft.AspNetCore.Mvc.Rendering;

namespace BebidasTicasPedidos.ViewModels
{
    public class PedidoCreateViewModel
    {
        public int ClienteId { get; set; }

        public string? MetodoPago { get; set; }

        public string? Observaciones { get; set; }

        public List<PedidoProductoInput> ProductosPedido { get; set; } = new();

        public List<SelectListItem> Clientes { get; set; } = new();

        public List<SelectListItem> Productos { get; set; } = new();
    }
}
