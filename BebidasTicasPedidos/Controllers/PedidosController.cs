using BebidasTicasPedidos.Data;
using BebidasTicasPedidos.Models;
using BebidasTicasPedidos.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BebidasTicasPedidos.Controllers
{
    public class PedidosController : Controller
    {
        private readonly AppDbContext _context;

        public PedidosController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var pedidos = await _context.Pedidos
                .Include(p => p.Cliente)
                .OrderByDescending(p => p.Fecha)
                .ToListAsync();

            return View(pedidos);
        }

        public async Task<IActionResult> Create()
        {
            var viewModel = new PedidoCreateViewModel
            {
                Clientes = await ObtenerClientes(),
                Productos = await ObtenerProductos(),
                ProductosPedido = new List<PedidoProductoInput>()
            };

            viewModel.ProductosPedido.Add(new PedidoProductoInput());

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PedidoCreateViewModel viewModel)
        {
            viewModel.Clientes = await ObtenerClientes();
            viewModel.Productos = await ObtenerProductos();

            var productosValidos = viewModel.ProductosPedido
                .Where(p => p.ProductoId > 0 && p.Cantidad > 0)
                .ToList();

            if (viewModel.ClienteId <= 0)
            {
                ModelState.AddModelError("", "Debe seleccionar un cliente.");
                return View(viewModel);
            }

            if (!productosValidos.Any())
            {
                ModelState.AddModelError("", "Debe agregar al menos un producto con cantidad.");
                return View(viewModel);
            }

            var pedido = new Pedido
            {
                ClienteId = viewModel.ClienteId,
                Fecha = DateTime.Now,
                Estado = "Pendiente",
                MetodoPago = viewModel.MetodoPago,
                Observaciones = viewModel.Observaciones,
                Total = 0,
                DetallePedido = new List<DetallePedido>()
            };

            foreach (var item in productosValidos)
            {
                var producto = await _context.Productos.FindAsync(item.ProductoId);

                if (producto == null)
                {
                    continue;
                }

                decimal subtotal = producto.Precio * item.Cantidad;

                pedido.DetallePedido.Add(new DetallePedido
                {
                    ProductoId = producto.Id,
                    Cantidad = item.Cantidad,
                    PrecioUnitario = producto.Precio,
                    Subtotal = subtotal
                });

                pedido.Total += subtotal;
            }

            _context.Pedidos.Add(pedido);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Resumen()
        {
            var detalles = await _context.DetallePedido
                .Include(d => d.Pedido)
                .Include(d => d.Producto)
                .Where(d => d.Pedido != null && d.Producto != null)
                .ToListAsync(); // 🔥 CLAVE

            var resumen = detalles
                .GroupBy(d => new
                {
                    Fecha = d.Pedido!.Fecha.Date,
                    Producto = d.Producto!.Nombre
                })
                .Select(g => new ResumenProductoViewModel
                {
                    Fecha = g.Key.Fecha,
                    Producto = g.Key.Producto,
                    CantidadTotal = g.Sum(x => x.Cantidad),
                    TotalDinero = g.Sum(x => (decimal)x.Subtotal) // 🔥 CLAVE
                })
                .OrderByDescending(r => r.Fecha)
                .ThenBy(r => r.Producto)
                .ToList();

            return View(resumen);
        }

        public async Task<IActionResult> Details(int id)
        {
            var pedido = await _context.Pedidos
                .Include(p => p.Cliente)
                .Include(p => p.DetallePedido)
                    .ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pedido == null)
            {
                return NotFound();
            }

            return View(pedido);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var pedido = await _context.Pedidos
                .Include(p => p.DetallePedido)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pedido != null)
            {
                _context.DetallePedido.RemoveRange(pedido.DetallePedido);
                _context.Pedidos.Remove(pedido);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<List<SelectListItem>> ObtenerClientes()
        {
            return await _context.Clientes
                .OrderBy(c => c.Nombre)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Nombre
                })
                .ToListAsync();
        }

        private async Task<List<SelectListItem>> ObtenerProductos()
        {
            return await _context.Productos
                .Where(p => p.Disponible)
                .OrderBy(p => p.Nombre)
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.Nombre
                })
                .ToListAsync();
        }
    }
}