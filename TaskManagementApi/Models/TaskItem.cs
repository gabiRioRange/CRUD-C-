using System.ComponentModel.DataAnnotations;

namespace TaskManagementApi.Models;

public class TaskItem
{
    public int Id { get; set; }

    [Required]
    [MaxLength(120)]
    public string Titulo { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Descricao { get; set; }

    // Valor padrão evita estado indefinido ao criar uma nova tarefa.
    public bool Concluida { get; set; } = false;
}
