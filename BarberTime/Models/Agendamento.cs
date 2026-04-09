using System.ComponentModel.DataAnnotations;

namespace BarberTime.Models;

public class Agendamento
{
    public int Id { get; set; }

    [Required(ErrorMessage = "O nome do cliente é obrigatório")]
    [StringLength(100)]
    public string NomeCliente { get; set; } = string.Empty;

    [Required(ErrorMessage = "O telefone é obrigatório")]
    [StringLength(20)]
    public string Telefone { get; set; } = string.Empty;

    [Required(ErrorMessage = "A data e hora são obrigatórios")]
    public DateTime DataHora { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Selecione um serviço")]
    [Display(Name = "Serviço")]
    public int ServicoId { get; set; }

    public Servico? Servico { get; set; }

    [StringLength(50)]
    public string? Observacoes { get; set; }

    [Required(ErrorMessage = "O status é obrigatório")]
    [StringLength(30)]
    public string Status { get; set; } = "Confirmado";
}