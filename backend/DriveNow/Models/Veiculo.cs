namespace DriveNow.Models
{
    public class Veiculo
    {
        public int IdVeiculo { get; set; }
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public int Ano { get; set; }
        public string Placa { get; set; }
        public string Cor { get; set; }
        public string Categoria { get; set; }
        public decimal ValorDiaria { get; set; }
        public bool Disponivel { get; set; }
    }
}
