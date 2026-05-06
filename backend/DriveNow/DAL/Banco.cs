using Microsoft.Data.Sqlite;

namespace DriveNow.DAL
{
    public class Banco
    {
        static string connString = "Data Source=drivenow.db";

        public static void Configurar(string conn)
        {
            connString = conn;
        }

        public static SqliteConnection Conectar()
        {
            return new SqliteConnection(connString);
        }
    }
}
