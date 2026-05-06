using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using DriveNow.DAL;
using DriveNow.Models;

namespace DriveNow.Controllers
{
    public class ClientesController : BaseController
    {
        public IActionResult Index()
        {
            var lista = new List<Cliente>();

            var con = Banco.Conectar();
            con.Open();

            var cmd = new SqliteCommand("SELECT * FROM Clientes WHERE ativo = 1 ORDER BY nome", con);
            var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                var c = new Cliente();
                c.IdCliente    = Convert.ToInt32(reader["id_cliente"]);
                c.Nome         = reader["nome"].ToString();
                c.Cpf          = reader["cpf"].ToString();
                c.Telefone     = reader["telefone"].ToString();
                c.Email        = reader["email"].ToString();
                c.Cnh          = reader["cnh"].ToString();
                c.DataCadastro = Convert.ToDateTime(reader["data_cadastro"]);
                lista.Add(c);
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
        public IActionResult Novo(Cliente c)
        {
            if (string.IsNullOrEmpty(c.Nome) || string.IsNullOrEmpty(c.Cpf))
            {
                ViewBag.Erro = "Nome e CPF sao obrigatorios.";
                return View(c);
            }

            // remove mascara do cpf antes de salvar
            string cpf = c.Cpf.Replace(".", "").Replace("-", "");

            if (cpf.Length != 11)
            {
                ViewBag.Erro = "CPF invalido.";
                return View(c);
            }

            try
            {
                var con = Banco.Conectar();
                con.Open();

                string sql = "INSERT INTO Clientes (nome, cpf, telefone, email, endereco, cnh) " +
                             "VALUES (@nome, @cpf, @tel, @email, @end, @cnh)";

                var cmd = new SqliteCommand(sql, con);
                cmd.Parameters.AddWithValue("@nome",  c.Nome);
                cmd.Parameters.AddWithValue("@cpf",   cpf);
                cmd.Parameters.AddWithValue("@tel",   c.Telefone ?? "");
                cmd.Parameters.AddWithValue("@email", c.Email ?? "");
                cmd.Parameters.AddWithValue("@end",   c.Endereco ?? "");
                cmd.Parameters.AddWithValue("@cnh",   c.Cnh ?? "");
                cmd.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("UNIQUE"))
                    ViewBag.Erro = "Ja existe um cliente com esse CPF.";
                else
                    ViewBag.Erro = "Erro ao salvar. Tente novamente.";

                return View(c);
            }

            TempData["Sucesso"] = "Cliente cadastrado!";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Editar(int id)
        {
            var con = Banco.Conectar();
            con.Open();

            var cmd = new SqliteCommand("SELECT * FROM Clientes WHERE id_cliente = @id", con);
            cmd.Parameters.AddWithValue("@id", id);
            var reader = cmd.ExecuteReader();

            if (!reader.Read())
                return NotFound();

            var c = new Cliente();
            c.IdCliente = Convert.ToInt32(reader["id_cliente"]);
            c.Nome      = reader["nome"].ToString();
            c.Cpf       = reader["cpf"].ToString();
            c.Telefone  = reader["telefone"].ToString();
            c.Email     = reader["email"].ToString();
            c.Endereco  = reader["endereco"].ToString();
            c.Cnh       = reader["cnh"].ToString();

            con.Close();
            return View(c);
        }

        [HttpPost]
        public IActionResult Editar(Cliente c)
        {
            var con = Banco.Conectar();
            con.Open();

            string sql = "UPDATE Clientes SET nome=@nome, telefone=@tel, email=@email, endereco=@end, cnh=@cnh WHERE id_cliente=@id";
            var cmd = new SqliteCommand(sql, con);
            cmd.Parameters.AddWithValue("@nome",  c.Nome);
            cmd.Parameters.AddWithValue("@tel",   c.Telefone ?? "");
            cmd.Parameters.AddWithValue("@email", c.Email ?? "");
            cmd.Parameters.AddWithValue("@end",   c.Endereco ?? "");
            cmd.Parameters.AddWithValue("@cnh",   c.Cnh ?? "");
            cmd.Parameters.AddWithValue("@id",    c.IdCliente);
            cmd.ExecuteNonQuery();
            con.Close();

            TempData["Sucesso"] = "Cliente atualizado!";
            return RedirectToAction("Index");
        }

        public IActionResult Excluir(int id)
        {
            // soft delete pra nao perder historico de locacoes
            var con = Banco.Conectar();
            con.Open();
            var cmd = new SqliteCommand("UPDATE Clientes SET ativo = 0 WHERE id_cliente = @id", con);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
            con.Close();

            TempData["Sucesso"] = "Cliente removido.";
            return RedirectToAction("Index");
        }
    }
}
