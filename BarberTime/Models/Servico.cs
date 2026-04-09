using System.ComponentModel.DataAnnotations;

namespace BarberTime.Models;

public class Servico
{
    public int Id { get; set; }

    [Required(ErrorMessage = "O nome do serviço é obrigatório")]
    [StringLength(100)]
    public string Nome { get; set; } = string.Empty;

    [Range(1, 1000, ErrorMessage = "O preço deve ser maior que zero")]
    public decimal Preco { get; set; }

    [Range(1, 300, ErrorMessage = "A duração deve estar entre 1 e 300 minutos")]
    public int DuracaoEmMinutos { get; set; }

    public bool Ativo { get; set; } = true;

    public ICollection<Agendamento>? Agendamentos { get; set; }
}