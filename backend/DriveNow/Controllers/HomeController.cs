using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using DriveNow.DAL;

namespace DriveNow.Controllers
{
    public class HomeController : BaseController
    {
        public IActionResult Index()
        {
            using (var con = Banco.Conectar())
            {
                con.Open();

                var q1 = new SqliteCommand("SELECT COUNT(*) FROM Clientes WHERE ativo = 1", con);
                ViewBag.TotalClientes = Convert.ToInt32(q1.ExecuteScalar());

                var q2 = new SqliteCommand("SELECT COUNT(*) FROM Veiculos", con);
                ViewBag.TotalVeiculos = Convert.ToInt32(q2.ExecuteScalar());

                var q3 = new SqliteCommand("SELECT COUNT(*) FROM Veiculos WHERE disponivel = 1", con);
                ViewBag.Disponiveis = Convert.ToInt32(q3.ExecuteScalar());

                var q4 = new SqliteCommand("SELECT COUNT(*) FROM Locacoes WHERE status = 'Ativa'", con);
                ViewBag.LocacoesAtivas = Convert.ToInt32(q4.ExecuteScalar());
            }

            return View();
        }
    }
}
