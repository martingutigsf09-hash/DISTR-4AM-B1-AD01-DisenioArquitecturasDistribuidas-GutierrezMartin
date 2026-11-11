
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Inventario.Api.Data;
using Inventario.Api.Models;
using System.Data;

namespace Inventario.Api.Controllers
{
    [Route("api/ReservasStock")]
    [ApiController]
    public class ReservasStockController : ControllerBase
    {
        private readonly InventarioDbContext _context;

        public ReservasStockController(InventarioDbContext context)
        {
            _context = context;
        }

        // GET: api/ReservasStock
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var reservas = await _context.ReservasStock
                .AsNoTracking()
                .ToListAsync();

            return Ok(reservas);
        }

        // GET: api/ReservasStock/1
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var reserva = await _context.ReservasStock
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.IdReserva == id);

            if (reserva == null)
                return NotFound("Reserva no encontrada");

            return Ok(reserva);
        }

        // POST: api/ReservasStock
        [HttpPost]
        public async Task<IActionResult> Post(ReservaStock reserva)
        {
            if (reserva.IdProducto <= 0 ||
                reserva.IdBodega <= 0 ||
                reserva.Cantidad <= 0)
            {
                return BadRequest("Datos de reserva inválidos");
            }

            await using var transaccion =
                await _context.Database.BeginTransactionAsync(
                    IsolationLevel.Serializable);

            var existencia = await _context.Existencias
                .FirstOrDefaultAsync(e =>
                    e.IdProducto == reserva.IdProducto &&
                    e.IdBodega == reserva.IdBodega);

            if (existencia == null)
                return NotFound("No existe stock para ese producto");

            var reservado = await _context.ReservasStock
                .Where(r =>
                    r.IdProducto == reserva.IdProducto &&
                    r.IdBodega == reserva.IdBodega &&
                    r.Estado == "Activa")
                .SumAsync(r => (int?)r.Cantidad) ?? 0;

            int disponible = existencia.Cantidad - reservado;

            if (reserva.Cantidad > disponible)
                return Conflict(
                    $"Stock insuficiente. Disponible: {disponible}");

            reserva.Estado = "Activa";
            reserva.FechaReserva = DateTime.UtcNow;

            _context.ReservasStock.Add(reserva);

            await _context.SaveChangesAsync();
            await transaccion.CommitAsync();

            return CreatedAtAction(
                nameof(Get),
                new { id = reserva.IdReserva },
                reserva);
        }

        // PUT: api/ReservasStock/1/cancelar
        [HttpPut("{id}/cancelar")]
        public async Task<IActionResult> Cancelar(int id)
        {
            var reserva = await _context.ReservasStock.FindAsync(id);

            if (reserva == null)
                return NotFound("Reserva no encontrada");

            if (reserva.Estado != "Activa")
                return Conflict("La reserva ya no está activa");

            reserva.Estado = "Cancelada";

            await _context.SaveChangesAsync();

            return Ok(reserva);
        }
    }
}
