using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BookHaeven.Dtos.User;
using BookHaeven.Interface;
using BookHaeven.Mappers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace BookHaeven.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _repo;
        private readonly IConfiguration _config;

        public UserController(IUserRepository repo, IConfiguration config)
        {
            _repo = repo;
            _config = config;
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid input", errors = ModelState });
            }

            if (await _repo.GetByEmailAsync(registerDto.Email) != null)
            {
                return BadRequest(new { message = "Email already taken." });
            }

            if (await _repo.GetByUsernameAsync(registerDto.Username) != null)
            {
                return BadRequest(new { message = "Username already taken." });
            }

            var newUser = registerDto.ToUserFromRegisterDto();
            await _repo.AddAsync(newUser);

            return Ok(new { message = "User registered successfully." });

        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var user = await _repo.GetByUsernameAsync(loginDto.Username);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                return Unauthorized("Invalid email or password.");
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Convert.FromBase64String(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new { token = tokenString });

        }
    }
}
