using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace OrdinarioWebDefinitivoAhora.Filters
{
    public class AuthorizeSessionAttribute : ActionFilterAttribute
    {
        // Verifica si la sesión contiene "NumEmpleado"
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var session = context.HttpContext.Session;

            if (string.IsNullOrEmpty(session.GetString("NumEmpleado")))
            {
                context.Result = new RedirectToActionResult("Login", "Auth", null);
            }
        }
    }
}
