using ClosedXML.Excel;
using OrdinarioWebDefinitivoAhora.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrdinarioWebDefinitivoAhora.Data;

namespace GestionAvanzadas.Controllers
{
    public class RegistroController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RegistroController(ApplicationDbContext context)
        {
            _context = context;
        }

 
        // LISTADO / BUSQUEDA

        public IActionResult Index(string buscar)
        {
            var query = _context.Memos
                .Include(m => m.Folio)
                .AsQueryable();

            if (!string.IsNullOrEmpty(buscar))
            {
                query = query.Where(m => m.Asunto.Contains(buscar));
            }

            var lista = query
                .OrderBy(m => m.Folio.Number)
                .ToList();

            ViewBag.Buscar = buscar;

            return View(lista);
        }


        // PREVIEW DEL MEMO

        public IActionResult Preview(int id)
        {
            var memo = _context.Memos
                .Include(m => m.Folio)
                .Include(m => m.cancelados)
                .FirstOrDefault(m => m.Id == id);

            if (memo == null)
                return NotFound();

            if (memo.Estatus == "Cancelado" &&
                memo.cancelados != null &&
                memo.cancelados.Any())
            {
                var ultimaCancel = memo.cancelados
                    .OrderByDescending(c => c.Fecha)
                    .First();

                ViewBag.MotivoCancelacion = ultimaCancel.Motivo;
                ViewBag.UsuarioCancelo = ultimaCancel.UsuarioCancela;
                ViewBag.FechaCancelacion = ultimaCancel.Fecha;
            }

            return View(memo);
        }


        // FORM DE CANCELAR

        public IActionResult Cancel(int id)
        {
            var memo = _context.Memos
                .Include(m => m.Folio)
                .FirstOrDefault(m => m.Id == id);

            if (memo == null)
                return NotFound();

            return View(memo);
        }


        // CONFIRMAR CANCELACIÓN

        [HttpPost]
        public IActionResult Cancel(int id, string motivo)
        {
            var memo = _context.Memos
                .Include(m => m.Folio)
                .FirstOrDefault(m => m.Id == id);

            if (memo == null)
                return NotFound();

            memo.Estatus = "Cancelado";

            if (memo.Folio != null)
                memo.Folio.Estado = FolioEstado.Bloqueado;

            var cancelacion = new CancelacionModel
            {
                MemoId = memo.Id,
                UsuarioCancela = HttpContext.Session.GetString("NumEmpleado") ?? "Sistema",
                Motivo = motivo,
                Fecha = DateTime.Now
            };

            _context.Cancelaciones.Add(cancelacion);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }


        // ELIMINAR (solo admin)

        public IActionResult Delete(int id)
        {
            var memo = _context.Memos
                .Include(m => m.Folio)
                .FirstOrDefault(m => m.Id == id);

            if (memo == null)
                return NotFound();

            // marcamos el folio como bloqueado
            if (memo.Folio != null)
                memo.Folio.Estado = FolioEstado.Bloqueado;

            _context.Memos.Remove(memo);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }


        // EXPORTAR EXCEL

        public IActionResult ExportToExcel()
        {
            var memos = _context.Memos
                .Include(m => m.Folio)
                .Include(m => m.cancelados)
                .ToList();

            using (var workbook = new XLWorkbook())
            {
                var ws = workbook.Worksheets.Add("Memos");

                ws.Cell(1, 1).Value = "Folio";
                ws.Cell(1, 2).Value = "Año";
                ws.Cell(1, 3).Value = "De";
                ws.Cell(1, 4).Value = "Para";
                ws.Cell(1, 5).Value = "Fecha Registro";
                ws.Cell(1, 6).Value = "Asunto";
                ws.Cell(1, 7).Value = "Contenido";
                ws.Cell(1, 8).Value = "Estatus";
                ws.Cell(1, 9).Value = "Usuario Registro";
                ws.Cell(1, 10).Value = "Usuario Canceló";
                ws.Cell(1, 11).Value = "Motivo Cancelación";
                ws.Cell(1, 12).Value = "Fecha Cancelación";

                int row = 2;

                foreach (var memo in memos)
                {
                    ws.Cell(row, 1).Value = memo.Folio?.Number.ToString("D3") ?? "---";
                    ws.Cell(row, 2).Value = memo.Folio?.Year.ToString() ?? "---";
                    ws.Cell(row, 3).Value = memo.De;
                    ws.Cell(row, 4).Value = memo.Para;
                    ws.Cell(row, 5).Value = memo.FechaRegistro.ToString("dd/MM/yyyy");
                    ws.Cell(row, 6).Value = memo.Asunto;
                    ws.Cell(row, 7).Value = memo.Contenido;
                    ws.Cell(row, 8).Value = memo.Estatus;
                    ws.Cell(row, 9).Value = memo.UsuarioRegistro;

                    if (memo.Estatus == "Cancelado" &&
                        memo.cancelados != null &&
                        memo.cancelados.Any())
                    {
                        var cancel = memo.cancelados
                            .OrderByDescending(c => c.Fecha)
                            .First();

                        ws.Cell(row, 10).Value = cancel.UsuarioCancela ?? "";
                        ws.Cell(row, 11).Value = cancel.Motivo ?? "";
                        ws.Cell(row, 12).Value = cancel.Fecha.ToString("dd/MM/yyyy HH:mm");
                    }

                    row++;
                }

                ws.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var fileName = $"Memos_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                    return File(stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        fileName);
                }
            }
        }
 
        // LISTADO DE CANCELADOS

        public IActionResult Cancelados()
        {
            var cancelados = _context.Memos
                .Include(m => m.Folio)
                .Include(m => m.cancelados)
                .Where(m => m.Estatus == "Cancelado")
                .OrderByDescending(m => m.FechaRegistro)
                .ToList();

            return View(cancelados);
        }

    }
}
