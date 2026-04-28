using BebidasTicasPedidos.Data;
using BebidasTicasPedidos.Models;
using BebidasTicasPedidos.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BebidasTicasPedidos.Controllers
{
    public class PedidoFinalController : Controller
    {
        private readonly AppDbContext _context;

        public PedidoFinalController(AppDbContext context)
        {
            _context = context;
        }

        public static List<PedidoFinalItemViewModel> PedidoFinal = new();

        public async Task<IActionResult> Index()
        {
            ViewBag.Productos = await _context.Productos
                .Where(p => p.Disponible)
                .OrderBy(p => p.Nombre)
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = $"{p.Nombre} - ₡{p.Precio}"
                })
                .ToListAsync();

            ViewBag.TotalUnidades = PedidoFinal.Sum(x => x.Cantidad);
            ViewBag.TotalDinero = PedidoFinal.Sum(x => x.Subtotal);

            return View(PedidoFinal);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AgregarProducto(int productoId, int cantidad)
        {
            if (productoId <= 0 || cantidad <= 0)
                return RedirectToAction(nameof(Index));

            var producto = await _context.Productos.FindAsync(productoId);

            if (producto == null)
                return RedirectToAction(nameof(Index));

            var itemExistente = PedidoFinal.FirstOrDefault(x => x.ProductoId == productoId);

            if (itemExistente != null)
            {
                itemExistente.Cantidad += cantidad;
                itemExistente.Subtotal = itemExistente.Cantidad * itemExistente.PrecioUnitario;
            }
            else
            {
                PedidoFinal.Add(new PedidoFinalItemViewModel
                {
                    ProductoId = producto.Id,
                    Producto = producto.Nombre,
                    Cantidad = cantidad,
                    PrecioUnitario = producto.Precio,
                    Subtotal = producto.Precio * cantidad
                });
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult EliminarProducto(int productoId)
        {
            var item = PedidoFinal.FirstOrDefault(x => x.ProductoId == productoId);

            if (item != null)
                PedidoFinal.Remove(item);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Limpiar()
        {
            PedidoFinal.Clear();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GuardarPedidoFinal()
        {
            if (!PedidoFinal.Any())
                return RedirectToAction(nameof(Index));

            var pedidoFinal = new PedidoFinal
            {
                Fecha = DateTime.Now,
                TotalUnidades = PedidoFinal.Sum(x => x.Cantidad),
                TotalDinero = PedidoFinal.Sum(x => x.Subtotal),
                Detalles = new List<DetallePedidoFinal>()
            };

            foreach (var item in PedidoFinal)
            {
                pedidoFinal.Detalles.Add(new DetallePedidoFinal
                {
                    ProductoId = item.ProductoId,
                    Cantidad = item.Cantidad,
                    PrecioUnitario = item.PrecioUnitario,
                    Subtotal = item.Subtotal
                });
            }

            _context.PedidosFinales.Add(pedidoFinal);
            await _context.SaveChangesAsync();

            PedidoFinal.Clear();

            return RedirectToAction(nameof(Historial));
        }

        public async Task<IActionResult> Historial()
        {
            var pedidos = await _context.PedidosFinales
                .OrderByDescending(p => p.Fecha)
                .ToListAsync();

            return View(pedidos);
        }

        public async Task<IActionResult> Detalle(int id)
        {
            var pedido = await _context.PedidosFinales
                .Include(p => p.Detalles)
                    .ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pedido == null)
                return NotFound();

            return View(pedido);
        }
    }
}