using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UspgPOS.Data;
using UspgPOS.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebPOS.Controllers
{
    public class DetalleVentaController : Controller
    {
        private readonly AppDbContext _context;

        public DetalleVentaController(AppDbContext context)
        {
            _context = context;
        }

        // Listar los detalles de una venta específica
        public async Task<IActionResult> Index(long ventaId)
        {
            var detalles = await _context.DetallesVenta
                .Include(dv => dv.Producto)
                .Where(dv => dv.VentaId == ventaId)
                .ToListAsync();

            ViewBag.VentaId = ventaId; // Para referencia en la vista
            return View(detalles);
        }

        // Mostrar formulario de creacion de detalle de venta
        public IActionResult Create(long ventaId)
        {
            ViewBag.VentaId = ventaId; // Para saber a qué venta pertenece el detalle
            ViewBag.Productos = new SelectList(_context.Productos, "Id", "Nombre"); // Lista de productos
            return View();
        }

        // Crear un detalle de venta (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DetalleVenta detalleVenta)
        {
            if (ModelState.IsValid)
            {
                _context.DetallesVenta.Add(detalleVenta);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", new { ventaId = detalleVenta.VentaId });
            }
            ViewBag.Productos = new SelectList(_context.Productos, "Id", "Nombre");
            return View(detalleVenta);
        }

        // Mostrar formulario para editar un detalle de venta
        public async Task<IActionResult> Edit(long id)
        {
            var detalleVenta = await _context.DetallesVenta.FindAsync(id);
            if (detalleVenta == null)
            {
                return NotFound();
            }
            ViewBag.Productos = new SelectList(_context.Productos, "Id", "Nombre", detalleVenta.ProductoId);
            return View(detalleVenta);
        }

        // Actualizar un detalle de venta (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, DetalleVenta detalleVenta)
        {
            if (id != detalleVenta.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(detalleVenta);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DetalleVentaExists(detalleVenta.Id ?? 0))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index", new { ventaId = detalleVenta.VentaId });
            }
            ViewBag.Productos = new SelectList(_context.Productos, "Id", "Nombre", detalleVenta.ProductoId);
            return View(detalleVenta);
        }

        // Eliminar un detalle de venta
        public async Task<IActionResult> Delete(long id)
        {
            var detalleVenta = await _context.DetallesVenta
                .Include(dv => dv.Producto)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (detalleVenta == null)
            {
                return NotFound();
            }

            return View(detalleVenta);
        }

        // Confirmar la eliminacion (POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var detalleVenta = await _context.DetallesVenta.FindAsync(id);
            _context.DetallesVenta.Remove(detalleVenta);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", new { ventaId = detalleVenta.VentaId });
        }

        private bool DetalleVentaExists(long id)
        {
            return _context.DetallesVenta.Any(e => e.Id == id);
        }
    }
}
