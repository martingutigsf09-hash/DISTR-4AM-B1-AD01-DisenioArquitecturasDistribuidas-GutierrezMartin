
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pedidos.Api.Data;
using Pedidos.Api.Models;
using System.Net;
using System.Net.Http.Json;

namespace Pedidos.Api.Controllers
{
    [Route("api/Pedidos")]
    [ApiController]
    public class PedidosController : ControllerBase
    {
        private readonly PedidosDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;

        public PedidosController(
            PedidosDbContext context,
            IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await _context.Pedidos
                .Include(p => p.Cliente)
                .Include(p => p.Detalles)
                .AsNoTracking()
                .ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var pedido = await _context.Pedidos
                .Include(p => p.Cliente)
                .Include(p => p.Detalles)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.IdPedido == id);

            return pedido == null
                ? NotFound("Pedido no encontrado")
                : Ok(pedido);
        }

        [HttpPost]
        public async Task<IActionResult> Post(Pedido pedido)
        {
            if (!await _context.Clientes
                .AnyAsync(c => c.IdCliente == pedido.IdCliente))
                return BadRequest("El cliente no existe");

            if (pedido.Detalles == null ||
                pedido.Detalles.Count == 0)
                return BadRequest("El pedido no tiene productos");

            if (pedido.Detalles.Any(d =>
                d.IdProducto <= 0 || d.Cantidad <= 0))
                return BadRequest("Producto o cantidad inválida");

            var catalogo = _httpClientFactory.CreateClient("CatalogoApi");
            var inventario = _httpClientFactory.CreateClient("InventarioApi");

            // Agrupar productos repetidos para reservar correctamente
            var cantidades = pedido.Detalles
                .GroupBy(d => d.IdProducto)
                .ToDictionary(
                    g => g.Key,
                    g => g.Sum(d => d.Cantidad));

            var reservasCreadas = new List<int>();

            try
            {
                // Validar productos y obtener precios oficiales
                foreach (var detalle in pedido.Detalles)
                {
                    var respuesta = await catalogo.GetAsync(
                        $"api/Productos/{detalle.IdProducto}");

                    if (!respuesta.IsSuccessStatusCode)
                        return BadRequest(
                            $"Producto {detalle.IdProducto} no encontrado");

                    var producto = await respuesta.Content
                        .ReadFromJsonAsync<ProductoCatalogo>();

                    if (producto == null || !producto.Estado)
                        return BadRequest(
                            $"Producto {detalle.IdProducto} inactivo");

                    detalle.PrecioUnitario = producto.Precio;
                    detalle.Subtotal =
                        detalle.Cantidad * producto.Precio;
                }

                // Consultar inventario
                var existencias = await inventario.GetFromJsonAsync<
                    List<ExistenciaInventario>>("api/Existencias");

                if (existencias == null)
                    return StatusCode(503, "Inventario no disponible");

                foreach (var item in cantidades)
                {
                    var existencia = existencias.FirstOrDefault(e =>
                        e.IdProducto == item.Key &&
                        e.IdBodega == 1);

                    if (existencia == null ||
                        existencia.Cantidad < item.Value)
                        return Conflict(
                            $"Stock insuficiente del producto {item.Key}");
                }

                // Crear reservas
                foreach (var item in cantidades)
                {
                    var respuesta = await inventario.PostAsJsonAsync(
                        "api/ReservasStock",
                        new
                        {
                            idProducto = item.Key,
                            idBodega = 1,
                            cantidad = item.Value
                        });

                    if (!respuesta.IsSuccessStatusCode)
                    {
                        await CancelarReservas(inventario, reservasCreadas);
                        return StatusCode((int)respuesta.StatusCode,
                            $"No se pudo reservar el producto {item.Key}");
                    }

                    var reserva = await respuesta.Content
                        .ReadFromJsonAsync<ReservaCreada>();

                    if (reserva == null || reserva.IdReserva <= 0)
                    {
                        await CancelarReservas(inventario, reservasCreadas);
                        return StatusCode(502,
                            "Respuesta inválida del servicio de reservas");
                    }

                    reservasCreadas.Add(reserva.IdReserva);
                }

                // Guardar pedido
                pedido.IdPedido = 0;
                pedido.Cliente = null;
                pedido.Estado = "Pendiente";
                pedido.FechaPedido = DateTime.UtcNow;
                pedido.Total = pedido.Detalles.Sum(d => d.Subtotal);

                foreach (var detalle in pedido.Detalles)
                {
                    detalle.IdDetallePedido = 0;
                    detalle.IdPedido = 0;
                    detalle.Pedido = null;
                }

                _context.Pedidos.Add(pedido);
                await _context.SaveChangesAsync();

                return CreatedAtAction(
                    nameof(Get),
                    new { id = pedido.IdPedido },
                    pedido);
            }
            catch (HttpRequestException)
            {
                await CancelarReservas(inventario, reservasCreadas);
                return StatusCode(503,
                    "No se pudo conectar con un microservicio");
            }
            catch (TaskCanceledException)
            {
                await CancelarReservas(inventario, reservasCreadas);
                return StatusCode(503,
                    "Tiempo de espera agotado");
            }
            catch (Exception)
            {
                await CancelarReservas(inventario, reservasCreadas);
                return StatusCode(500,
                    "No se pudo completar el pedido");
            }
        }

        private static async Task CancelarReservas(
            HttpClient inventario,
            List<int> reservas)
        {
            foreach (var id in reservas)
            {
                try
                {
                    await inventario.PutAsync(
                        $"api/ReservasStock/{id}/cancelar", null);
                }
                catch
                {
                    // Requiere revisión si falla la compensación
                }
            }
        }

        private class ProductoCatalogo
        {
            public decimal Precio { get; set; }
            public bool Estado { get; set; }
        }

        private class ExistenciaInventario
        {
            public int IdProducto { get; set; }
            public int IdBodega { get; set; }
            public int Cantidad { get; set; }
        }

        private class ReservaCreada
        {
            public int IdReserva { get; set; }
        }
    }
}
