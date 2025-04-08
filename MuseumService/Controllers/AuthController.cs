using Microsoft.AspNetCore.Mvc;
using MuseumService.Models;
using MuseumService.Models.Services;

namespace MuseumService.Controllers;

/// <summary>
/// Контроллер для аутентификации пользователей
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;
    
    public AuthController(AuthService authService)
    {
        _authService = authService;
    }
    
    /// <summary>
    /// Аутентификация пользователя
    /// </summary>
    /// <param name="loginModel">Данные для входа</param>
    /// <returns>JWT токен и информация о пользователе</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
    {
        var response = await _authService.Authenticate(loginModel);
        
        if (response == null)
            return Unauthorized();
            
        return Ok(response);
    }
}