using Microsoft.EntityFrameworkCore;

namespace MuseumService.Models.Services;

// AuthService.cs
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

public class AuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;
    
    public AuthService(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }
    
    public async Task<AuthResponse?> Authenticate(LoginModel loginModel)
    {
        var employee = await _context.Employees
            .FirstOrDefaultAsync(e => e.Username == loginModel.Username && e.IsActive);
            
        if (employee == null || !VerifyPassword(loginModel.Password, employee.PasswordHash))
            return null;
            
        var token = GenerateJwtToken(employee);
        
        return new AuthResponse
        {
            Token = token,
            Expiration = DateTime.UtcNow.AddMinutes(_configuration.GetValue<int>("Jwt:ExpiryInMinutes")),
            Employee = employee
        };
    }
    
    private bool VerifyPassword(string password, string storedHash)
    {
        // В реальном проекте используйте безопасное сравнение хэшей
        return BCrypt.Net.BCrypt.Verify(password, storedHash);
    }
    
    private string GenerateJwtToken(Employee employee)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, employee.Username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, employee.EmployeeId.ToString()),
            new Claim(ClaimTypes.Name, employee.FullName)
        };
        
        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_configuration.GetValue<int>("Jwt:ExpiryInMinutes")),
            signingCredentials: credentials);
            
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}