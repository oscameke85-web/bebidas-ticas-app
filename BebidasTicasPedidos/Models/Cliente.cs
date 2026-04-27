using System.ComponentModel.DataAnnotations;

namespace BebidasTicasPedidos.Models
{
    public class Cliente
    {
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        public string Telefono { get; set; } = string.Empty;

        public string? Correo { get; set; }

        public string? Zona { get; set; }

        public string? TipoCliente { get; set; }

        public string? Observaciones { get; set; }

        public bool Activo { get; set; } = true;
    }
}
