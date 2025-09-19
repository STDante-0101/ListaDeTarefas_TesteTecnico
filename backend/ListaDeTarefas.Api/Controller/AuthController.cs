using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ListaDeTarefas.Api.Data;
using ListaDeTarefas.Api.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using BCrypt.Net;

namespace ListaDeTarefas.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public AuthController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // POST: api/Auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] Usuario model)
        {
            if (await _context.Usuarios.AnyAsync(u => u.Username == model.Username))
                return BadRequest("Usuário já existe.");

            model.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.PasswordHash);
            model.CreatedAt = DateTime.UtcNow;

            _context.Usuarios.Add(model);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Usuário registrado com sucesso" });
        }

        // POST: api/Auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Usuario model)
        {
            var user = await _context.Usuarios.FirstOrDefaultAsync(u => u.Username == model.Username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(model.PasswordHash, user.PasswordHash))
                return Unauthorized("Usuário ou senha inválidos.");

            var token = GenerateJwtToken(user);
            return Ok(new { token });
        }

        private string GenerateJwtToken(Usuario user)
        {
            var jwtSettings = _config.GetSection("Jwt");
            var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]);

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username)
                }),
                Expires = DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["ExpiresInMinutes"])),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
