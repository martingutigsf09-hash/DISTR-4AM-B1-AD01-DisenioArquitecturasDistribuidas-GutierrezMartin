
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pedidos.Api.Data;
using Pedidos.Api.Models;

namespace Pedidos.Api.Controllers
{
    [Route("api/Clientes")]
    [ApiController]
    public class ClientesController : ControllerBase
    {
        private readonly PedidosDbContext _context;

        public ClientesController(PedidosDbContext context)
        {
            _context = context;
        }

        // GET: Listar clientes
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var clientes = await _context.Clientes
                .AsNoTracking()
                .ToListAsync();

            return Ok(clientes);
        }

        // GET: Buscar cliente por ID
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);

            if (cliente == null)
                return NotFound("Cliente no encontrado");

            return Ok(cliente);
        }

        // POST: Registrar cliente
        [HttpPost]
        public async Task<IActionResult> Post(Cliente cliente)
        {
            _context.Clientes.Add(cliente);

            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(Get),
                new { id = cliente.IdCliente },
                cliente);
        }

        // PUT: Actualizar cliente
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, Cliente cliente)
        {
            if (id != cliente.IdCliente)
                return BadRequest("Los ID no coinciden");

            var existe = await _context.Clientes
                .AnyAsync(c => c.IdCliente == id);

            if (!existe)
                return NotFound("Cliente no encontrado");

            _context.Entry(cliente).State =
                EntityState.Modified;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: Eliminar cliente
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);

            if (cliente == null)
                return NotFound("Cliente no encontrado");

            _context.Clientes.Remove(cliente);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
