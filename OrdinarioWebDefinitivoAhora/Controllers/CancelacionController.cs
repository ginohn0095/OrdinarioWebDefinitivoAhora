using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrdinarioWebDefinitivoAhora.Data;
using OrdinarioWebDefinitivoAhora.Models;

namespace OrdinarioWebDefinitivoAhora.Controllers
{
    public class CancelacionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CancelacionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Listado de memos cancelados
        public IActionResult Index()
        {
            var cancelaciones = _context.Cancelaciones
                .Include(c => c.Memo)
                .OrderByDescending(c => c.Fecha)
                .ToList();

            return View(cancelaciones);
        }

        // Formulario para cancelar un memo
        public IActionResult Cancelar(int id)
        {
            var memo = _context.Memos.Find(id);
            if (memo == null || memo.Estatus == "Cancelado")
                return NotFound();

            return View(memo);
        }

        [HttpPost]
        public IActionResult ConfirmarCancelacion(int id, string motivo)
        {
            var memo = _context.Memos
                .Include(m => m.Folio)
                .FirstOrDefault(m => m.Id == id);

            if (memo == null)
                return NotFound();

            // Cambiar estatus del memo
            memo.Estatus = "Cancelado";

            // Liberar folio si existe
            if (memo.Folio != null)
                memo.Folio.Estado = FolioEstado.Disponible;

            // Crear registro de cancelación
            var cancel = new CancelacionModel
            {
                MemoId = memo.Id,
                UsuarioCancela = HttpContext.Session.GetString("NumEmpleado") ?? "Sistema",
                Motivo = motivo,
                Fecha = DateTime.Now
            };

            _context.Cancelaciones.Add(cancel);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
