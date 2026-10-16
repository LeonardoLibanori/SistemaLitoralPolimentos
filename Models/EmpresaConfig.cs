namespace SistemaLitoralPolimentos.Models
{
    public class EmpresaConfig
    {
        public string Nome { get; set; } = "";
        public string Fantasia { get; set; } = "";
        public string Cnpj { get; set; } = "";
        public string Email { get; set; } = "";
        public string Telefone { get; set; } = "";
        public string Responsavel { get; set; } = "";
        public string CpfResponsavel { get; set; } = "";
        public EnderecoEmpresa Endereco { get; set; } = new();
        public BancoEmpresa Banco { get; set; } = new();
        public int ValidadeOrcamentoDias { get; set; } = 45;
    }

    public class EnderecoEmpresa
    {
        public string Logradouro { get; set; } = "";
        public string Numero { get; set; } = "";
        public string Bairro { get; set; } = "";
        public string Cidade { get; set; } = "";
        public string Uf { get; set; } = "";
        public string Cep { get; set; } = "";
        public string Completo => $"{Logradouro}, {Numero}, {Bairro}, {Cidade}/{Uf}, CEP {Cep}";
    }

    public class BancoEmpresa
    {
        public string Nome { get; set; } = "";
        public string Agencia { get; set; } = "";
        public string Conta { get; set; } = "";
        public string Titular { get; set; } = "";
        public string Pix { get; set; } = "";
    }
}