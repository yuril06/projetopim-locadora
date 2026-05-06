using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using DriveNow.DAL;

namespace DriveNow.Controllers
{
    public class UsuariosController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("usuario")))
                return RedirectToAction("Index", "Home");

            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string senha)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha))
            {
                ViewBag.Erro = "Preencha o email e a senha.";
                return View();
            }

            using var con = Banco.Conectar();
            con.Open();

            var cmd = new SqliteCommand(
                "SELECT nome FROM Usuarios WHERE email = @email AND senha = @senha", con);
            cmd.Parameters.AddWithValue("@email", email.Trim().ToLower());
            cmd.Parameters.AddWithValue("@senha", senha);

            var nome = cmd.ExecuteScalar()?.ToString();

            if (nome == null)
            {
                ViewBag.Erro = "Email ou senha incorretos.";
                return View();
            }

            HttpContext.Session.SetString("usuario", nome);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData.Clear();
            return RedirectToAction("Login");
        }
    }
}
