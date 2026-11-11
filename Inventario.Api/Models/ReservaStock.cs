
using System.ComponentModel.DataAnnotations;

namespace Inventario.Api.Models
{
    public class ReservaStock
    {
        [Key]
        public int IdReserva { get; set; }

        public int IdProducto { get; set; }

        public int IdBodega { get; set; }

        [Range(1, int.MaxValue)]
        public int Cantidad { get; set; }

        public DateTime FechaReserva { get; set; } = DateTime.UtcNow;

        [Required]
        [MaxLength(30)]
        public string Estado { get; set; } = "Activa";
    }
}
