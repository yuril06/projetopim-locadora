using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.Sqlite;
using DriveNow.DAL;
using DriveNow.Models;

namespace DriveNow.Controllers
{
    public class LocacoesController : BaseController
    {
        public IActionResult Index()
        {
            var lista = new List<Locacao>();

            var con = Banco.Conectar();
            con.Open();

            string sql = "SELECT l.*, c.nome AS nome_cliente, " +
                         "v.marca || ' ' || v.modelo AS nome_veiculo, v.placa " +
                         "FROM Locacoes l " +
                         "INNER JOIN Clientes c ON l.id_cliente = c.id_cliente " +
                         "INNER JOIN Veiculos v ON l.id_veiculo = v.id_veiculo " +
                         "ORDER BY l.data_registro DESC";

            var cmd = new SqliteCommand(sql, con);
            var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                var loc = new Locacao();
                loc.IdLocacao     = Convert.ToInt32(reader["id_locacao"]);
                loc.DataRetirada  = Convert.ToDateTime(reader["data_retirada"]);
                loc.DataDevolucao = Convert.ToDateTime(reader["data_devolucao"]);
                loc.ValorTotal    = reader["valor_total"] != DBNull.Value ? Convert.ToDecimal(reader["valor_total"]) : 0;
                loc.Status        = reader["status"].ToString();
                loc.NomeCliente   = reader["nome_cliente"].ToString();
                loc.NomeVeiculo   = reader["nome_veiculo"].ToString();
                loc.PlacaVeiculo  = reader["placa"].ToString();
                lista.Add(loc);
            }

            con.Close();
            return View(lista);
        }

        [HttpGet]
        public IActionResult Nova()
        {
            CarregarViewBags();
            return View();
        }

        [HttpPost]
        public IActionResult Nova(Locacao loc)
        {
            if (loc.IdCliente == 0 || loc.IdVeiculo == 0)
            {
                ViewBag.Erro = "Selecione o cliente e o veiculo.";
                CarregarViewBags();
                return View(loc);
            }

            if (loc.DataDevolucao <= loc.DataRetirada)
            {
                ViewBag.Erro = "A data de devolucao deve ser posterior a retirada.";
                CarregarViewBags();
                return View(loc);
            }

            var con = Banco.Conectar();
            con.Open();

            // pega o valor da diaria do veiculo selecionado
            var cmdDiaria = new SqliteCommand("SELECT valor_diaria FROM Veiculos WHERE id_veiculo = @id", con);
            cmdDiaria.Parameters.AddWithValue("@id", loc.IdVeiculo);
            decimal diaria = Convert.ToDecimal(cmdDiaria.ExecuteScalar());

            int dias = (loc.DataDevolucao - loc.DataRetirada).Days;
            decimal total = dias * diaria;

            string sql = "INSERT INTO Locacoes (id_cliente, id_veiculo, data_retirada, data_devolucao, valor_diaria, valor_total, observacao) " +
                         "VALUES (@cli, @vei, @ret, @dev, @diaria, @total, @obs)";

            var cmd = new SqliteCommand(sql, con);
            cmd.Parameters.AddWithValue("@cli",    loc.IdCliente);
            cmd.Parameters.AddWithValue("@vei",    loc.IdVeiculo);
            cmd.Parameters.AddWithValue("@ret",    loc.DataRetirada.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("@dev",    loc.DataDevolucao.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("@diaria", diaria);
            cmd.Parameters.AddWithValue("@total",  total);
            cmd.Parameters.AddWithValue("@obs",    loc.Observacao ?? "");
            cmd.ExecuteNonQuery();

            // bloqueia o veiculo
            var cmdBloqueia = new SqliteCommand("UPDATE Veiculos SET disponivel = 0 WHERE id_veiculo = @id", con);
            cmdBloqueia.Parameters.AddWithValue("@id", loc.IdVeiculo);
            cmdBloqueia.ExecuteNonQuery();

            con.Close();

            TempData["Sucesso"] = "Locacao registrada!";
            return RedirectToAction("Index");
        }

        public IActionResult Concluir(int id)
        {
            var con = Banco.Conectar();
            con.Open();

            // precisa do id do veiculo pra liberar depois
            var cmdGet = new SqliteCommand("SELECT id_veiculo FROM Locacoes WHERE id_locacao = @id", con);
            cmdGet.Parameters.AddWithValue("@id", id);
            int idVeiculo = Convert.ToInt32(cmdGet.ExecuteScalar());

            var cmd = new SqliteCommand("UPDATE Locacoes SET status = 'Concluida' WHERE id_locacao = @id", con);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();

            var cmdLibera = new SqliteCommand("UPDATE Veiculos SET disponivel = 1 WHERE id_veiculo = @id", con);
            cmdLibera.Parameters.AddWithValue("@id", idVeiculo);
            cmdLibera.ExecuteNonQuery();

            con.Close();

            TempData["Sucesso"] = "Locacao concluida. Veiculo liberado.";
            return RedirectToAction("Index");
        }

        private void CarregarViewBags()
        {
            var clientes = new List<SelectListItem>();
            var veiculos = new List<Veiculo>();

            var con = Banco.Conectar();
            con.Open();

            var cmdCli = new SqliteCommand("SELECT id_cliente, nome FROM Clientes WHERE ativo = 1 ORDER BY nome", con);
            var rCli = cmdCli.ExecuteReader();
            while (rCli.Read())
            {
                clientes.Add(new SelectListItem
                {
                    Value = rCli["id_cliente"].ToString(),
                    Text  = rCli["nome"].ToString()
                });
            }
            rCli.Close();

            var cmdVei = new SqliteCommand("SELECT id_veiculo, marca, modelo, placa, valor_diaria FROM Veiculos WHERE disponivel = 1 ORDER BY marca", con);
            var rVei = cmdVei.ExecuteReader();
            while (rVei.Read())
            {
                var v = new Veiculo();
                v.IdVeiculo   = Convert.ToInt32(rVei["id_veiculo"]);
                v.Marca       = rVei["marca"].ToString();
                v.Modelo      = rVei["modelo"].ToString();
                v.Placa       = rVei["placa"].ToString();
                v.ValorDiaria = Convert.ToDecimal(rVei["valor_diaria"]);
                veiculos.Add(v);
            }

            con.Close();

            ViewBag.Clientes     = clientes;
            ViewBag.VeiculosList = veiculos;
        }
    }
}
