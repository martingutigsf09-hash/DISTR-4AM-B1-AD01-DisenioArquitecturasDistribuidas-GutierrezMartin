
using System.ComponentModel.DataAnnotations;

namespace Inventario.Api.Models
{
    public class Bodega
    {
        [Key]
        public int IdBodega { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [MaxLength(200)]
        public string Ubicacion { get; set; } = string.Empty;

        public bool Estado { get; set; } = true;
    }
}
