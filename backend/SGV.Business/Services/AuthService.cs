using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SGV.Business.Interfaces;
using SGV.Business.Interfaces.Repositories;
using SGV.DTOs.Auth;
using BCrypt.Net;

namespace SGV.Business.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _repo;
    private readonly IConfiguration _configuration;

    public AuthService(IUserRepository repo, IConfiguration configuration)
    {
        _repo = repo;
        _configuration = configuration;
    }

    public async Task<AuthResponseDTO?> LoginAsync(LoginDTO dto)
    {
        var user = await _repo.GetByUsernameAsync(dto.Username);
        
        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
        {
            return null; // Invalid credentials
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"] ?? "default_super_secret_key_12345");
        
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            }),
            Expires = DateTime.UtcNow.AddHours(24),
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return new AuthResponseDTO
        {
            Token = tokenHandler.WriteToken(token),
            Username = user.Username,
            Role = user.Role
        };
    }
}
