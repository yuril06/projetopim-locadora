using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using DriveNow.DAL;
using DriveNow.Models;

namespace DriveNow.Controllers
{
    public class VeiculosController : BaseController
    {
        public IActionResult Index()
        {
            var lista = new List<Veiculo>();

            var con = Banco.Conectar();
            con.Open();

            var cmd = new SqliteCommand("SELECT * FROM Veiculos ORDER BY marca, modelo", con);
            var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                var v = new Veiculo();
                v.IdVeiculo   = Convert.ToInt32(reader["id_veiculo"]);
                v.Marca       = reader["marca"].ToString();
                v.Modelo      = reader["modelo"].ToString();
                v.Ano         = reader["ano"] != DBNull.Value ? Convert.ToInt32(reader["ano"]) : 0;
                v.Placa       = reader["placa"].ToString();
                v.Cor         = reader["cor"].ToString();
                v.Categoria   = reader["categoria"].ToString();
                v.ValorDiaria = Convert.ToDecimal(reader["valor_diaria"]);
                v.Disponivel  = Convert.ToInt32(reader["disponivel"]) == 1;
                lista.Add(v);
            }

            con.Close();
            return View(lista);
        }

        [HttpGet]
        public IActionResult Novo()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Novo(Veiculo v)
        {
            if (string.IsNullOrEmpty(v.Marca) || string.IsNullOrEmpty(v.Placa))
            {
                ViewBag.Erro = "Marca e placa sao obrigatorios.";
                return View(v);
            }

            if (v.ValorDiaria <= 0)
            {
                ViewBag.Erro = "Informe um valor de diaria valido.";
                return View(v);
            }

            try
            {
                var con = Banco.Conectar();
                con.Open();

                string sql = "INSERT INTO Veiculos (marca, modelo, ano, placa, cor, categoria, valor_diaria) " +
                             "VALUES (@marca, @modelo, @ano, @placa, @cor, @cat, @valor)";

                var cmd = new SqliteCommand(sql, con);
                cmd.Parameters.AddWithValue("@marca",  v.Marca);
                cmd.Parameters.AddWithValue("@modelo", v.Modelo ?? "");
                cmd.Parameters.AddWithValue("@ano",    v.Ano > 0 ? (object)v.Ano : DBNull.Value);
                cmd.Parameters.AddWithValue("@placa",  v.Placa.ToUpper());
                cmd.Parameters.AddWithValue("@cor",    v.Cor ?? "");
                cmd.Parameters.AddWithValue("@cat",    v.Categoria ?? "Basico");
                cmd.Parameters.AddWithValue("@valor",  v.ValorDiaria);
                cmd.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("UNIQUE"))
                    ViewBag.Erro = "Ja existe um veiculo com essa placa.";
                else
                    ViewBag.Erro = "Erro ao cadastrar.";

                return View(v);
            }

            TempData["Sucesso"] = "Veiculo cadastrado!";
            return RedirectToAction("Index");
        }

        public IActionResult AlterarDisponibilidade(int id, bool disponivel)
        {
            var con = Banco.Conectar();
            con.Open();

            var cmd = new SqliteCommand("UPDATE Veiculos SET disponivel = @disp WHERE id_veiculo = @id", con);
            cmd.Parameters.AddWithValue("@disp", disponivel ? 1 : 0);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
            con.Close();

            return RedirectToAction("Index");
        }
    }
}
