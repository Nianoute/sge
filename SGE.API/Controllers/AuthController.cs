using Microsoft.AspNetCore.Mvc;
using SGE.Application.DTOs.Users;
using SGE.Application.Interfaces.Services;

namespace SGE.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto registerDto)
    {
        var result = await authService.RegisterAsync(registerDto);
        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginDto
        loginDto)
    {
        var result = await authService.LoginAsync(loginDto);
        return Ok(result);
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResponseDto>>
        RefreshToken(RefreshTokenDto refreshTokenDto)
    {
        var result = await
            authService.RefreshTokenAsync(refreshTokenDto);
        return Ok(result);
    }

    [HttpPost("logout/{userId:int}")]
    public async Task<ActionResult> Logout(int userId)
    {
        await authService.LogoutAsync(userId.ToString());
        return Ok(new { message = "Déconnexion réussie" });
    }
    
    [HttpPost("revoke")]
    public async Task<ActionResult> RevokeToken([FromBody] RevokeTokenDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Token))
            return BadRequest(new { message = "Token is required" });

        await authService.RevokeTokenAsync(dto.Token);

        return Ok(new { message = "Token révoqué avec succès" });
    }

}