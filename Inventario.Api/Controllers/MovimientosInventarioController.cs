
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Inventario.Api.Data;
using Inventario.Api.Models;

namespace Inventario.Api.Controllers
{
    [Route("api/MovimientosInventario")]
    [ApiController]
    public class MovimientosInventarioController : ControllerBase
    {
        private readonly InventarioDbContext _context;

        public MovimientosInventarioController(
            InventarioDbContext context)
        {
            _context = context;
        }

        // GET: api/MovimientosInventario
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var movimientos = await _context.MovimientosInventario
                .AsNoTracking()
                .OrderByDescending(m => m.Fecha)
                .ToListAsync();

            return Ok(movimientos);
        }

        // GET: api/MovimientosInventario/1
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var movimiento = await _context.MovimientosInventario
                .FindAsync(id);

            if (movimiento == null)
                return NotFound("Movimiento no encontrado");

            return Ok(movimiento);
        }

        // POST: api/MovimientosInventario
        [HttpPost]
        public async Task<IActionResult> Post(
            MovimientoInventario movimiento)
        {
            if (movimiento.Cantidad <= 0)
                return BadRequest("La cantidad debe ser mayor a cero");

            var tipo = movimiento.TipoMovimiento
                .Trim()
                .ToUpperInvariant();

            if (tipo != "ENTRADA" && tipo != "SALIDA")
                return BadRequest(
                    "El tipo debe ser ENTRADA o SALIDA");

            await using var transaccion =
                await _context.Database.BeginTransactionAsync(
                    System.Data.IsolationLevel.Serializable);

            var existencia = await _context.Existencias
                .FirstOrDefaultAsync(e =>
                    e.IdProducto == movimiento.IdProducto &&
                    e.IdBodega == movimiento.IdBodega);

            if (existencia == null)
                return NotFound(
                    "No existe stock de ese producto en la bodega");

            if (tipo == "SALIDA")
            {
                if (existencia.Cantidad < movimiento.Cantidad)
                    return BadRequest("Stock insuficiente");

                existencia.Cantidad -= movimiento.Cantidad;
            }
            else
            {
                existencia.Cantidad += movimiento.Cantidad;
            }

            movimiento.TipoMovimiento = tipo;
            movimiento.Fecha = DateTime.UtcNow;

            _context.MovimientosInventario.Add(movimiento);

            await _context.SaveChangesAsync();
            await transaccion.CommitAsync();

            return CreatedAtAction(
                nameof(Get),
                new { id = movimiento.IdMovimiento },
                movimiento);
        }
    }
}
