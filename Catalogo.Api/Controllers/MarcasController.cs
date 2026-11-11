
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Catalogo.Api.Data;
using Catalogo.Api.Models;

namespace Catalogo.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MarcasController : ControllerBase
    {
        private readonly CatalogoDbContext _context;

        public MarcasController(CatalogoDbContext context)
        {
            _context = context;
        }

        // GET: api/Marcas
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var marcas = await _context.Marcas.ToListAsync();
            return Ok(marcas);
        }

        // GET: api/Marcas/1
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var marca = await _context.Marcas.FindAsync(id);

            if (marca == null)
                return NotFound("Marca no encontrada");

            return Ok(marca);
        }

        // POST: api/Marcas
        [HttpPost]
        public async Task<IActionResult> Post(Marca marca)
        {
            _context.Marcas.Add(marca);
            await _context.SaveChangesAsync();

            return Ok(marca);
        }

        // PUT: api/Marcas/1
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, Marca marca)
        {
            if (id != marca.IdMarca)
                return BadRequest("El ID no coincide");

            var actual = await _context.Marcas.FindAsync(id);

            if (actual == null)
                return NotFound("Marca no encontrada");

            actual.Nombre = marca.Nombre;
            actual.PaisOrigen = marca.PaisOrigen;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Marcas/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var marca = await _context.Marcas.FindAsync(id);

            if (marca == null)
                return NotFound("Marca no encontrada");

            _context.Marcas.Remove(marca);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
