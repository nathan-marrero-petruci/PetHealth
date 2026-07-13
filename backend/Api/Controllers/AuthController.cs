using Api.Data;
using Api.DTOs.Auth;
using Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(AppDbContext db, IJwtService jwtService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var tutor = await db.Tutores.SingleOrDefaultAsync(t => t.Email == request.Email);

        if (tutor is null || !BCrypt.Net.BCrypt.Verify(request.Password, tutor.PasswordHash))
        {
            return Unauthorized(new { message = "Email ou senha inválidos." });
        }

        var token = jwtService.GenerateToken(tutor);

        return Ok(new LoginResponse(token));
    }
}
