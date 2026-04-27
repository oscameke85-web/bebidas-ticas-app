namespace BebidasTicasPedidos.ViewModels
{
    public class ResumenProductoViewModel
    {
        public DateTime Fecha { get; set; }
        public string Producto { get; set; } = string.Empty;
        public int CantidadTotal { get; set; }
        public decimal TotalDinero { get; set; }
    }
}
