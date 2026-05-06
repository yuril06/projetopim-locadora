-- ============================================================
-- Banco de dados: DriveNow Locadora
-- Projeto: PIM - Unip
-- ============================================================

CREATE DATABASE DriveNow;
GO

USE DriveNow;
GO

-- Usuarios do sistema (quem acessa o painel)
CREATE TABLE Usuarios (
    id_usuario   INT IDENTITY(1,1) PRIMARY KEY,
    nome         VARCHAR(100) NOT NULL,
    email        VARCHAR(100) NOT NULL,
    senha        VARCHAR(255) NOT NULL,
    criado_em    DATETIME DEFAULT GETDATE(),
    CONSTRAINT uq_email_usuario UNIQUE (email)
);
GO

-- Clientes que fazem locacao
CREATE TABLE Clientes (
    id_cliente    INT IDENTITY(1,1) PRIMARY KEY,
    nome          VARCHAR(150) NOT NULL,
    cpf           CHAR(11)     NOT NULL,
    telefone      VARCHAR(15),
    email         VARCHAR(100),
    endereco      VARCHAR(300),
    cnh           VARCHAR(20),
    data_nasc     DATE,
    ativo         BIT DEFAULT 1,
    data_cadastro DATETIME DEFAULT GETDATE(),
    CONSTRAINT uq_cpf UNIQUE (cpf)
);
GO

-- Veiculos da frota
CREATE TABLE Veiculos (
    id_veiculo   INT IDENTITY(1,1) PRIMARY KEY,
    marca        VARCHAR(80) NOT NULL,
    modelo       VARCHAR(80) NOT NULL,
    ano          INT,
    placa        VARCHAR(8)  NOT NULL,
    cor          VARCHAR(40),
    categoria    VARCHAR(30),  -- Basico, Intermediario, SUV, Luxo
    valor_diaria DECIMAL(10,2) NOT NULL,
    disponivel   BIT DEFAULT 1,
    CONSTRAINT uq_placa UNIQUE (placa)
);
GO

-- Tabela principal de locacoes
CREATE TABLE Locacoes (
    id_locacao     INT IDENTITY(1,1) PRIMARY KEY,
    id_cliente     INT NOT NULL,
    id_veiculo     INT NOT NULL,
    data_retirada  DATE NOT NULL,
    data_devolucao DATE NOT NULL,
    valor_diaria   DECIMAL(10,2) NOT NULL,
    valor_total    DECIMAL(10,2),
    status         VARCHAR(20) DEFAULT 'Ativa',  -- Ativa, Concluida, Cancelada
    observacao     VARCHAR(500),
    data_registro  DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (id_cliente) REFERENCES Clientes(id_cliente),
    FOREIGN KEY (id_veiculo) REFERENCES Veiculos(id_veiculo)
);
GO

-- ============================================================
-- Dados de exemplo
-- ============================================================

INSERT INTO Usuarios (nome, email, senha) VALUES
('Admin DriveNow', 'admin@drivenow.com', '202cb962ac59075b964b07152d234b70'); -- senha: 123 (md5)

INSERT INTO Clientes (nome, cpf, telefone, email, endereco, cnh, data_nasc) VALUES
('Joao Carlos Silva',      '12345678901', '(11) 98765-4321', 'joao@email.com',  'Rua das Flores, 123 - Sao Paulo/SP',   'SP123456789', '1990-05-15'),
('Maria Aparecida Santos', '98765432100', '(11) 91234-5678', 'maria@email.com', 'Av. Brasil, 456 - Campinas/SP',        'SP987654321', '1985-08-22'),
('Pedro Henrique Oliveira','11122233344', '(11) 99876-5432', 'pedro@email.com', 'Rua Sao Jose, 789 - Guarulhos/SP',     'SP111222333', '1995-03-10'),
('Ana Paula Ferreira',     '55566677788', '(11) 97654-3210', 'ana@email.com',   'Rua dos Ipes, 321 - Sao Paulo/SP',     'SP555666777', '1988-11-30'),
('Carlos Eduardo Lima',    '33344455566', '(11) 96543-2109', 'carlos@email.com','Av. Paulista, 1000 - Sao Paulo/SP',    'SP333444555', '1992-07-18');

INSERT INTO Veiculos (marca, modelo, ano, placa, cor, categoria, valor_diaria) VALUES
('Volkswagen', 'Gol',     2021, 'ABC1D23', 'Branco',  'Basico',        89.90),
('Chevrolet',  'Onix',    2022, 'DEF4G56', 'Prata',   'Basico',        99.90),
('Hyundai',    'HB20',    2022, 'GHI7J89', 'Vermelho','Basico',        95.00),
('Toyota',     'Corolla', 2023, 'JKL0M12', 'Preto',   'Intermediario', 189.90),
('Honda',      'Civic',   2022, 'NOP3Q45', 'Cinza',   'Intermediario', 179.90),
('Jeep',       'Compass', 2023, 'RST6U78', 'Branco',  'SUV',           259.90),
('Toyota',     'RAV4',    2022, 'VWX9Y01', 'Prata',   'SUV',           289.90),
('BMW',        '320i',    2023, 'ZAB2C34', 'Preto',   'Luxo',          499.90);

-- Alguns veiculos ja estao alugados
UPDATE Veiculos SET disponivel = 0 WHERE id_veiculo IN (2, 5);

INSERT INTO Locacoes (id_cliente, id_veiculo, data_retirada, data_devolucao, valor_diaria, valor_total, status, observacao) VALUES
(1, 2, '2025-04-01', '2025-04-05', 99.90,  399.60, 'Concluida', 'Cliente devolveu no prazo.'),
(2, 5, '2025-04-10', '2025-04-15', 179.90, 899.50, 'Ativa',     NULL),
(3, 1, '2025-04-20', '2025-04-22', 89.90,  179.80, 'Concluida', NULL),
(4, 4, '2025-04-25', '2025-04-28', 189.90, 569.70, 'Concluida', 'Pequeno arranhao relatado na entrega.');
GO
