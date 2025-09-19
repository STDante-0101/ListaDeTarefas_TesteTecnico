using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ListaDeTarefas.Api.Data;
using ListaDeTarefas.Api.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace ListaDeTarefas.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly string _jwtKey = "MinhaChaveSuperSecreta123!";

        public UsuariosController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/Usuarios/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] Usuario usuario)
        {
            if (await _context.Usuarios.AnyAsync(u => u.Username == usuario.Username))
                return BadRequest("Usuário já existe.");

            usuario.PasswordHash = HashPassword(usuario.PasswordHash);
            usuario.CreatedAt = DateTime.UtcNow;

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return Ok(new { usuario.Id, usuario.Username });
        }

        // POST: api/Usuarios/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Usuario login)
        {
            var user = await _context.Usuarios.FirstOrDefaultAsync(u => u.Username == login.Username);
            if (user == null || !VerifyPassword(login.PasswordHash, user.PasswordHash))
                return Unauthorized("Usuário ou senha inválidos.");

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username)
                }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Ok(new { token = tokenHandler.WriteToken(token) });
        }

        private string HashPassword(string password)
        {
            byte[] salt = new byte[128 / 8]; // para simplificação, sem salt único
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));
        }

        private bool VerifyPassword(string password, string hashed)
        {
            return HashPassword(password) == hashed;
        }
    }
}
