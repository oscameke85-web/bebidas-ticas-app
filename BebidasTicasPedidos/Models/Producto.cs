using System.ComponentModel.DataAnnotations;

namespace BebidasTicasPedidos.Models
{
    public class Producto
    {
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        public string Categoria { get; set; } = string.Empty;

        public string? Presentacion { get; set; }

        public decimal Precio { get; set; }

        public bool Disponible { get; set; } = true;
    }
}