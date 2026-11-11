
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Inventario.Api.Data;
using Inventario.Api.Models;

namespace Inventario.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExistenciasController : ControllerBase
    {
        private readonly InventarioDbContext _context;

        public ExistenciasController(InventarioDbContext context)
        {
            _context = context;
        }

        // GET: api/Existencias
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var existencias = await _context.Existencias
                .AsNoTracking()
                .ToListAsync();

            return Ok(existencias);
        }

        // GET: api/Existencias/1
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var existencia = await _context.Existencias
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.IdExistencia == id);

            if (existencia == null)
                return NotFound("Existencia no encontrada");

            return Ok(existencia);
        }

        // POST: api/Existencias
        [HttpPost]
        public async Task<IActionResult> Post(Existencia existencia)
        {
            if (existencia.IdProducto <= 0)
                return BadRequest("Producto inválido");

            if (existencia.Cantidad < 0)
                return BadRequest("La cantidad no puede ser negativa");

            bool bodegaExiste = await _context.Bodegas
                .AnyAsync(b => b.IdBodega == existencia.IdBodega);

            if (!bodegaExiste)
                return BadRequest("La bodega no existe");

            bool yaExiste = await _context.Existencias
                .AnyAsync(e =>
                    e.IdProducto == existencia.IdProducto &&
                    e.IdBodega == existencia.IdBodega);

            if (yaExiste)
                return Conflict(
                    "El producto ya tiene existencias en esta bodega");

            _context.Existencias.Add(existencia);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(Get),
                new { id = existencia.IdExistencia },
                existencia);
        }

        // PUT: api/Existencias/1
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(
            int id, Existencia existencia)
        {
            if (id != existencia.IdExistencia)
                return BadRequest("El ID no coincide");

            if (existencia.Cantidad < 0)
                return BadRequest("La cantidad no puede ser negativa");

            var actual = await _context.Existencias.FindAsync(id);

            if (actual == null)
                return NotFound("Existencia no encontrada");

            actual.Cantidad = existencia.Cantidad;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Existencias/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existencia = await _context.Existencias.FindAsync(id);

            if (existencia == null)
                return NotFound("Existencia no encontrada");

            _context.Existencias.Remove(existencia);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
