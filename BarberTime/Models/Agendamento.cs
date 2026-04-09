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
    public string telefone { get; set; } = string.Empty;

    [Required(ErrorMessage = "A data e hora são obrigatórios")]
    public DateTime DataHora { get; set; }

    [Required(ErrorMessage = "O serviço é obrigatório")]
    [StringLength(50)]
    public string Serviço { get; set; } = string.Empty;

    [StringLength(50)]
    public string? Observacoes {get; set;}
}