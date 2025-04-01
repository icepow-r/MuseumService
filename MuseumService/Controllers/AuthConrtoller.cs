using Microsoft.AspNetCore.Mvc;
using MuseumService.Models;
using MuseumService.Models.Services;

namespace MuseumService.Controllers;

// AuthController.cs

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;
    
    public AuthController(AuthService authService)
    {
        _authService = authService;
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
    {
        var response = await _authService.Authenticate(loginModel);
        
        if (response == null)
            return Unauthorized();
            
        return Ok(response);
    }
}