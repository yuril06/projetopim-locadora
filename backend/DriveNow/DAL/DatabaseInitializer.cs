using Microsoft.Data.Sqlite;

namespace DriveNow.DAL
{
    public static class DatabaseInitializer
    {
        public static void Inicializar(string connStr)
        {
            using var con = new SqliteConnection(connStr);
            con.Open();

            var cmdTabelas = new SqliteCommand(@"
                CREATE TABLE IF NOT EXISTS Usuarios (
                    id_usuario   INTEGER PRIMARY KEY AUTOINCREMENT,
                    nome         TEXT NOT NULL,
                    email        TEXT NOT NULL UNIQUE,
                    senha        TEXT NOT NULL,
                    criado_em    TEXT DEFAULT (datetime('now'))
                );

                CREATE TABLE IF NOT EXISTS Clientes (
                    id_cliente    INTEGER PRIMARY KEY AUTOINCREMENT,
                    nome          TEXT NOT NULL,
                    cpf           TEXT NOT NULL UNIQUE,
                    telefone      TEXT DEFAULT '',
                    email         TEXT DEFAULT '',
                    endereco      TEXT DEFAULT '',
                    cnh           TEXT DEFAULT '',
                    data_nasc     TEXT,
                    ativo         INTEGER DEFAULT 1,
                    data_cadastro TEXT DEFAULT (datetime('now'))
                );

                CREATE TABLE IF NOT EXISTS Veiculos (
                    id_veiculo   INTEGER PRIMARY KEY AUTOINCREMENT,
                    marca        TEXT NOT NULL,
                    modelo       TEXT NOT NULL,
                    ano          INTEGER,
                    placa        TEXT NOT NULL UNIQUE,
                    cor          TEXT DEFAULT '',
                    categoria    TEXT DEFAULT 'Basico',
                    valor_diaria REAL NOT NULL,
                    disponivel   INTEGER DEFAULT 1
                );

                CREATE TABLE IF NOT EXISTS Locacoes (
                    id_locacao     INTEGER PRIMARY KEY AUTOINCREMENT,
                    id_cliente     INTEGER NOT NULL,
                    id_veiculo     INTEGER NOT NULL,
                    data_retirada  TEXT NOT NULL,
                    data_devolucao TEXT NOT NULL,
                    valor_diaria   REAL NOT NULL,
                    valor_total    REAL,
                    status         TEXT DEFAULT 'Ativa',
                    observacao     TEXT DEFAULT '',
                    data_registro  TEXT DEFAULT (datetime('now')),
                    FOREIGN KEY (id_cliente) REFERENCES Clientes(id_cliente),
                    FOREIGN KEY (id_veiculo) REFERENCES Veiculos(id_veiculo)
                );
            ", con);
            cmdTabelas.ExecuteNonQuery();

            var cmdCheck = new SqliteCommand("SELECT COUNT(*) FROM Veiculos", con);
            long count = (long)cmdCheck.ExecuteScalar()!;

            if (count == 0)
            {
                var seed = new SqliteCommand(@"
                    INSERT INTO Usuarios (nome, email, senha) VALUES
                    ('Admin DriveNow', 'admin@drivenow.com', '123456');
                    -- acesso: admin@drivenow.com / 123456

                    INSERT INTO Clientes (nome, cpf, telefone, email, endereco, cnh) VALUES
                    ('Joao Carlos Silva',       '12345678901', '(11) 98765-4321', 'joao@email.com',  'Rua das Flores, 123 - Sao Paulo/SP', 'SP123456789'),
                    ('Maria Aparecida Santos',  '98765432100', '(11) 91234-5678', 'maria@email.com', 'Av. Brasil, 456 - Campinas/SP',       'SP987654321'),
                    ('Pedro Henrique Oliveira', '11122233344', '(11) 99876-5432', 'pedro@email.com', 'Rua Sao Jose, 789 - Guarulhos/SP',    'SP111222333'),
                    ('Ana Paula Ferreira',      '55566677788', '(11) 97654-3210', 'ana@email.com',   'Rua dos Ipes, 321 - Sao Paulo/SP',    'SP555666777');

                    INSERT INTO Veiculos (marca, modelo, ano, placa, cor, categoria, valor_diaria) VALUES
                    ('Volkswagen', 'Gol',     2021, 'ABC1D23', 'Branco',  'Basico',        89.90),
                    ('Chevrolet',  'Onix',    2022, 'DEF4G56', 'Prata',   'Basico',        99.90),
                    ('Hyundai',    'HB20',    2022, 'GHI7J89', 'Vermelho','Basico',        95.00),
                    ('Toyota',     'Corolla', 2023, 'JKL0M12', 'Preto',   'Intermediario', 189.90),
                    ('Honda',      'Civic',   2022, 'NOP3Q45', 'Cinza',   'Intermediario', 179.90),
                    ('Jeep',       'Compass', 2023, 'RST6U78', 'Branco',  'SUV',           259.90),
                    ('BMW',        '320i',    2023, 'ZAB2C34', 'Preto',   'Luxo',          499.90);

                    UPDATE Veiculos SET disponivel = 0 WHERE placa = 'DEF4G56';

                    INSERT INTO Locacoes (id_cliente, id_veiculo, data_retirada, data_devolucao, valor_diaria, valor_total, status, observacao) VALUES
                    (1, 2, '2025-04-01', '2025-04-05', 99.90,  399.60, 'Concluida', 'Cliente devolveu no prazo.'),
                    (2, 1, '2025-04-10', '2025-04-12', 89.90,  179.80, 'Ativa',     '');
                ", con);
                seed.ExecuteNonQuery();
            }
        }
    }
}
