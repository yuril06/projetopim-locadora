using System;

namespace DriveNow.Models
{
    public class Locacao
    {
        public int IdLocacao { get; set; }
        public int IdCliente { get; set; }
        public int IdVeiculo { get; set; }
        public DateTime DataRetirada { get; set; }
        public DateTime DataDevolucao { get; set; }
        public decimal ValorDiaria { get; set; }
        public decimal ValorTotal { get; set; }
        public string Status { get; set; }
        public string Observacao { get; set; }
        public DateTime DataRegistro { get; set; }

        // usados so pra exibir nas listagens
        public string NomeCliente { get; set; }
        public string NomeVeiculo { get; set; }
        public string PlacaVeiculo { get; set; }
    }
}
