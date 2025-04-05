using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace MuseumService.Models.Services;

public class AuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;
    private const int Iterations = 100000; // Количество итераций PBKDF2
    private const int SaltSize = 16; // 128 бит соли
    private const int HashSize = 32; // 256 бит хеша
    
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

    // Генерация хеша пароля (используется при регистрации/смене пароля)
    public static string HashPassword(string password)
    {
        using var algorithm = new Rfc2898DeriveBytes(
            password,
            SaltSize,
            Iterations,
            HashAlgorithmName.SHA256);
            
        var salt = Convert.ToBase64String(algorithm.Salt);
        var hash = Convert.ToBase64String(algorithm.GetBytes(HashSize));
        
        return $"{Iterations}.{salt}.{hash}";
    }
    
    private bool VerifyPassword(string password, string storedHash)
    {
        try
        {
            var parts = storedHash.Split('.', 3);
            if (parts.Length != 3) return false;
            
            var iterations = int.Parse(parts[0]);
            var salt = Convert.FromBase64String(parts[1]);
            var expectedHash = Convert.FromBase64String(parts[2]);
            
            using var algorithm = new Rfc2898DeriveBytes(
                password,
                salt,
                iterations,
                HashAlgorithmName.SHA256);
                
            var actualHash = algorithm.GetBytes(HashSize);
            return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
        }
        catch
        {
            return false;
        }
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