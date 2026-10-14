using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaLitoralPolimentos.Models
{
    public class Documento
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O tipo do documento é obrigatório")]
        [Display(Name = "Tipo do documento")]
        public string TipoDocumento { get; set; } = string.Empty;
        // Valores possíveis: "Orcamento", "Recibo", "Contrato"

        [Required]
        [Display(Name = "Cliente")]
        public int ClienteId { get; set; }

        [ForeignKey("ClienteId")]
        public Cliente? Cliente { get; set; }

        [Display(Name = "Data de criação")]
        public DateTime DataCriacao { get; set; } = DateTime.Now;

        [Display(Name = "Valor")]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Valor { get; set; }

        // Armazena o JSON completo do documento (campos do orçamento/recibo/contrato)
        [Display(Name = "Conteúdo")]
        public string ConteudoJson { get; set; } = "{}";

        [Display(Name = "Status")]
        public string Status { get; set; } = "Pendente";
        // Valores possíveis: "Pendente", "Aprovado", "Cancelado"

        [Display(Name = "Data de aprovação")]
        public DateTime? DataAprovacao { get; set; }
    }
}