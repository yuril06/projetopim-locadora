using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DriveNow.Controllers
{
    public class BaseController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var usuario = HttpContext.Session.GetString("usuario");
            if (string.IsNullOrEmpty(usuario))
            {
                context.Result = RedirectToAction("Login", "Usuarios");
                return;
            }

            ViewBag.UsuarioLogado = usuario;
            base.OnActionExecuting(context);
        }
    }
}
