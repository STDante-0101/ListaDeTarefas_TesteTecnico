using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ListaDeTarefas.Api.Data;
using ListaDeTarefas.Api.Models;
using System.Security.Claims;

namespace ListaDeTarefas.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // 🔒 Protege todos os endpoints
    public class TarefasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TarefasController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Tarefas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tarefa>>> GetTarefas()
        {
            var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var tarefas = await _context.Tarefas
                .Where(t => t.UsuarioId == usuarioId)
                .ToListAsync();

            return tarefas;
        }

        // GET: api/Tarefas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Tarefa>> GetTarefa(int id)
        {
            var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var tarefa = await _context.Tarefas
                .FirstOrDefaultAsync(t => t.Id == id && t.UsuarioId == usuarioId);

            if (tarefa == null)
                return NotFound();

            return tarefa;
        }

        // POST: api/Tarefas
        [HttpPost]
        public async Task<ActionResult<Tarefa>> CreateTarefa(Tarefa tarefa)
        {
            var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            tarefa.UsuarioId = usuarioId;
            tarefa.Concluida = false;
            tarefa.CreatedAt = DateTime.UtcNow;

            _context.Tarefas.Add(tarefa);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTarefa), new { id = tarefa.Id }, tarefa);
        }

        // PUT: api/Tarefas/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTarefa(int id, Tarefa tarefa)
        {
            var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (id != tarefa.Id)
                return BadRequest();

            // Garante que só o dono possa atualizar
            var tarefaExistente = await _context.Tarefas
                .FirstOrDefaultAsync(t => t.Id == id && t.UsuarioId == usuarioId);

            if (tarefaExistente == null)
                return NotFound();

            tarefaExistente.Titulo = tarefa.Titulo;
            tarefaExistente.Descricao = tarefa.Descricao;
            tarefaExistente.Categoria = tarefa.Categoria;
            tarefaExistente.Concluida = tarefa.Concluida;
            tarefaExistente.UpdatedAt = DateTime.UtcNow;

            _context.Entry(tarefaExistente).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TarefaExists(id, usuarioId))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // DELETE: api/Tarefas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTarefa(int id)
        {
            var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var tarefa = await _context.Tarefas
                .FirstOrDefaultAsync(t => t.Id == id && t.UsuarioId == usuarioId);

            if (tarefa == null)
                return NotFound();

            _context.Tarefas.Remove(tarefa);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/Tarefas/categoria/Trabalho
        [HttpGet("categoria/{categoria}")]
        public async Task<ActionResult<IEnumerable<Tarefa>>> GetTarefasByCategoria(string categoria)
        {
            var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var tarefas = await _context.Tarefas
                .Where(t => t.UsuarioId == usuarioId && t.Categoria == categoria)
                .ToListAsync();

            return tarefas;
        }

        private bool TarefaExists(int id, int usuarioId)
        {
            return _context.Tarefas.Any(e => e.Id == id && e.UsuarioId == usuarioId);
        }
    }
}
