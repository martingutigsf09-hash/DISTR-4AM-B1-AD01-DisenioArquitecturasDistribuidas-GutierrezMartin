
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Inventario.Api.Data;
using Inventario.Api.Models;

namespace Inventario.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BodegasController : ControllerBase
    {
        private readonly InventarioDbContext _context;

        public BodegasController(InventarioDbContext context)
        {
            _context = context;
        }

        // GET: api/Bodegas
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var bodegas = await _context.Bodegas.ToListAsync();
            return Ok(bodegas);
        }

        // GET: api/Bodegas/1
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var bodega = await _context.Bodegas.FindAsync(id);

            if (bodega == null)
                return NotFound("Bodega no encontrada");

            return Ok(bodega);
        }

        // POST: api/Bodegas
        [HttpPost]
        public async Task<IActionResult> Post(Bodega bodega)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.Bodegas.Add(bodega);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(Get),
                new { id = bodega.IdBodega },
                bodega);
        }

        // PUT: api/Bodegas/1
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, Bodega bodega)
        {
            if (id != bodega.IdBodega)
                return BadRequest("El ID no coincide");

            var actual = await _context.Bodegas.FindAsync(id);

            if (actual == null)
                return NotFound("Bodega no encontrada");

            actual.Nombre = bodega.Nombre;
            actual.Ubicacion = bodega.Ubicacion;
            actual.Estado = bodega.Estado;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Bodegas/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var bodega = await _context.Bodegas.FindAsync(id);

            if (bodega == null)
                return NotFound("Bodega no encontrada");

            bool tieneExistencias = await _context.Existencias
                .AnyAsync(e => e.IdBodega == id);

            if (tieneExistencias)
                return Conflict(
                    "No se puede eliminar una bodega con existencias");

            _context.Bodegas.Remove(bodega);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
