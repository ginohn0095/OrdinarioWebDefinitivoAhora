using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrdinarioWebDefinitivoAhora.Data;
using OrdinarioWebDefinitivoAhora.Filters;
using OrdinarioWebDefinitivoAhora.Models;

namespace OrdinarioWebDefinitivoAhora.Controllers
{
    [AuthorizeSession]
    public class MemoController : Controller
    {
        private readonly ApplicationDbContext _db;

        public MemoController(ApplicationDbContext db)
        {
            _db = db;
        }
        // GET: Memo/Index
        public IActionResult Index()
        {
            var memos = _db.Memos
                .Include(m => m.Folio)
                .OrderByDescending(m => m.FechaRegistro)
                .ToList();

            return View(memos);
        }
        // GET: Memo/Create
        public IActionResult Create()
        {
            return View(new MemoModel());
        }
        // POST: Memo/Create
        [HttpPost]
        public IActionResult Create(MemoModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var nextFolio = _db.Folios.FirstOrDefault(f => f.Estado == FolioEstado.Disponible);

            if (nextFolio == null)
            {
                ViewBag.Error = "No quedan folios disponibles.";
                return View(model);
            }

            nextFolio.Estado = FolioEstado.Usado;
            model.FechaRegistro = DateTime.Now;
            model.FolioId = nextFolio.Id;

            _db.Memos.Add(model);
            _db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
