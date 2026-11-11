
using System.ComponentModel.DataAnnotations;

namespace Inventario.Api.Models
{
    public class MovimientoInventario
    {
        [Key]
        public int IdMovimiento { get; set; }

        public int IdProducto { get; set; }

        public int IdBodega { get; set; }

        [Required]
        [MaxLength(20)]
        public string TipoMovimiento { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int Cantidad { get; set; }

        public DateTime Fecha { get; set; } = DateTime.UtcNow;

        [MaxLength(250)]
        public string Observacion { get; set; } = string.Empty;
    }
}
