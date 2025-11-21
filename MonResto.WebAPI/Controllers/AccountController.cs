using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MonResto.Domain.DTOs;
using MonResto.WebAPI.Authentication;

namespace MonResto.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly JwtSettings _jwtSettings;

    public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, JwtSettings jwtSettings)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtSettings = jwtSettings;
    }

    [HttpPost("register")]
    public async Task<ActionResult> Register([FromBody] RegisterDto dto)
    {
        var user = new IdentityUser { UserName = dto.UserName, Email = dto.Email };
        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }
        return Ok();
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto dto)
    {
        var user = await _userManager.FindByNameAsync(dto.UserName);
        if (user is null) return Unauthorized();
        var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
        if (!result.Succeeded) return Unauthorized();

        var token = GenerateJwtToken(user);
        return Ok(token);
    }

    private AuthResponseDto GenerateJwtToken(IdentityUser user)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
            new Claim(ClaimTypes.NameIdentifier, user.Id)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );

        return new AuthResponseDto(user.UserName ?? string.Empty, new JwtSecurityTokenHandler().WriteToken(token), expires);
    }
}
