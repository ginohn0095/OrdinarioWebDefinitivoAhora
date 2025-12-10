using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrdinarioWebDefinitivoAhora.Data;
using OrdinarioWebDefinitivoAhora.Models;
using System;
using System.Diagnostics;

namespace Gestion2.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        //
        // CONSULTA DE MEMOS ACTIVOS
        //
        public IActionResult Consultas()
        {
            var activos = _context.Memos
                .Include(m => m.Folio)
                .Where(m => m.Estatus == "Activo")
                .OrderBy(m => m.Folio.Number)
                .ToList();

            return View(activos);
        }

        //
        // CONSULTA DE CANCELACIONES
        //
        public IActionResult Cancelar()
        {
            var cancelados = _context.Cancelaciones
                .Include(c => c.Memo)
                .ThenInclude(m => m.Folio)
                .OrderByDescending(c => c.Fecha)
                .ToList();

            return View(cancelados);
        }

        //
        // CREAR MEMORÁNDUM (GET)
        //
        public IActionResult Create()
        {
            return View();
        }

        //
        // CREAR MEMORÁNDUM (POST)
        //
        [HttpPost]
        public IActionResult Create(MemoModel memo)
        {
            try
            {
                int folioId = ObtenerFolioDisponible(); // obtiene ID del folio disponible

                memo.FolioId = folioId;
                memo.FechaRegistro = DateTime.Now;
                memo.Estatus = "Activo";
                memo.UsuarioRegistro = HttpContext.Session.GetString("NumEmpleado") ?? "Sistema";

                // Marcar folio como usado
                var folio = _context.Folios.First(f => f.Id == folioId);
                folio.Estado = FolioEstado.Ocupado;

                _context.Memos.Add(memo);
                _context.SaveChanges();

                return RedirectToAction("Details", new { id = memo.Id });
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(memo);
            }
        }

        
        // DETALLES DE MEMORÁNDUM
        
        public IActionResult Details(int id)
        {
            var memo = _context.Memos
                .Include(m => m.Folio)
                .Include(m => m.cancelados)
                .FirstOrDefault(m => m.Id == id);

            if (memo == null)
                return NotFound();

            return View(memo);
        }

        
        // LISTADO GENERAL DE CANCELACIONES
       
        public IActionResult Cancelados()
        {
            var cancelados = _context.Cancelaciones
                .Include(c => c.Memo)
                .ThenInclude(m => m.Folio)
                .ToList();

            return View(cancelados);
        }

        
        // ERROR
        
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        
        // OBTENER FOLIO DISPONIBLE
        
        private int ObtenerFolioDisponible()
        {
            int añoActual = DateTime.Now.Year;

            var folio = _context.Folios
                .Where(f => f.Year == añoActual && f.Estado == FolioEstado.Disponible)
                .OrderBy(f => f.Number)
                .FirstOrDefault();

            if (folio == null)
                throw new Exception("No hay folios disponibles para este año.");

            return folio.Id;
        }
    }
}
