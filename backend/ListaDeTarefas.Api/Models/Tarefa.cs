using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ListaDeTarefas.Api.Models
{
    public class Tarefa
    {
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string Titulo { get; set; } = string.Empty;

        public string? Descricao { get; set; }

        public bool Concluida { get; set; } = false;

        public string? Categoria { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Relacionamento com Usuario
        public int UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public Usuario Usuario { get; set; } = null!;
    }
}
