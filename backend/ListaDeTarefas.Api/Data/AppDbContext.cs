using Microsoft.EntityFrameworkCore;
using ListaDeTarefas.Api.Models;

namespace ListaDeTarefas.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; } = null!;
        public DbSet<Tarefa> Tarefas { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Username único
            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Username)
                .IsUnique();

            // Relacionamento 1:N entre Usuario e Tarefa
            modelBuilder.Entity<Tarefa>()
                .HasOne(t => t.Usuario)
                .WithMany(u => u.Tarefas)
                .HasForeignKey(t => t.UsuarioId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
