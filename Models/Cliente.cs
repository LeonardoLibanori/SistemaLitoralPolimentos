using System.ComponentModel.DataAnnotations;

namespace SistemaLitoralPolimentos.Models
{
    public class Cliente
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(200, ErrorMessage = "O nome deve ter no máximo 200 caracteres")]
        [Display(Name = "Nome completo")]
        public string Nome { get; set; } = string.Empty;

        [StringLength(18, ErrorMessage = "CPF/CNPJ inválido")]
        [Display(Name = "CPF / CNPJ")]
        public string? CpfCnpj { get; set; }

        [StringLength(20)]
        [Display(Name = "Telefone")]
        public string? Telefone { get; set; }

        [EmailAddress(ErrorMessage = "E-mail inválido")]
        [StringLength(200)]
        [Display(Name = "E-mail")]
        public string? Email { get; set; }

        [StringLength(500)]
        [Display(Name = "Endereço")]
        public string? Endereco { get; set; }

        [Display(Name = "Data de cadastro")]
        public DateTime DataCadastro { get; set; } = DateTime.Now;

        // Relacionamentos (um cliente pode ter muitos documentos)
        public ICollection<Documento> Documentos { get; set; } = new List<Documento>();
    }
}