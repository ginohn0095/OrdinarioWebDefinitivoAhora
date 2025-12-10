using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using OrdinarioWebDefinitivoAhora.Data;
using OrdinarioWebDefinitivoAhora.Models;

namespace OrdinarioWebDefinitivoAhora.Controllers
{
    public class AuthController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly PasswordHasher<UserModel> _hasher = new PasswordHasher<UserModel>();

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }
        //Redirecciona a la vista de login
        public IActionResult Login()
        {
            return View("~/Views/Home/Login.cshtml");
        }

        //funcion para autenticar al usuario
        [HttpPost]
        public IActionResult Login(string numEmpleado, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.NumEmpleado == numEmpleado);

            if (user == null)
            {
                ViewBag.Error = "Usuario no encontrado.";
                return View("~/Views/Home/Login.cshtml");
            }

            var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, password);

            if (result == PasswordVerificationResult.Failed)
            {
                ViewBag.Error = "Contraseña incorrecta.";
                return View("~/Views/Home/Login.cshtml");
            }

            HttpContext.Session.SetString("NumEmpleado", user.NumEmpleado);
            HttpContext.Session.SetString("Nombre", user.Nombre);
            HttpContext.Session.SetString("Rol", user.Role);

            return RedirectToAction("Index", "Home");
        }



    }
}
