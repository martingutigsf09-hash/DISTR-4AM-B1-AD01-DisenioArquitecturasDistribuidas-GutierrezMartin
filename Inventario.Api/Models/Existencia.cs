
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventario.Api.Models
{
    public class Existencia
    {
        [Key]
        public int IdExistencia { get; set; }

        public int IdProducto { get; set; }

        public int IdBodega { get; set; }

        [Range(0, int.MaxValue)]
        public int Cantidad { get; set; }

        [ForeignKey(nameof(IdBodega))]
        public Bodega? Bodega { get; set; }
    }
}
