using System.ComponentModel.DataAnnotations;

namespace ListaDeTarefas.Api.Models
{
    public class Usuario
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        // Navegação -> um usuário tem várias tarefas
        public ICollection<Tarefa> Tarefas { get; set; } = new List<Tarefa>();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
