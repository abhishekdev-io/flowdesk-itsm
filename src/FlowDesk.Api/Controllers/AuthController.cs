using FlowDesk.Application.Features.Auth.Register;
using FlowDesk.Application.Features.Auth.Login;
using FlowDesk.Application.Common.Dto;
using FlowDesk.Api.Dto.Auth;
using FlowDesk.Application.Features.Auth.RefreshToken;
using Microsoft.Extensions.Options;
using FlowDesk.Infrastructure.Identity.Config;

namespace FlowDesk.Api.Controllers;

[ApiController]
[Route("api/auth")]
[Produces("application/json")]
public class AuthController(IMediator mediator, IOptions<JwtSettings> jwtOptions, IOptions<RefreshTokenCookieSettings> cookieOptions) : ControllerBase
{
    private readonly JwtSettings _jwtSettings = jwtOptions.Value;
    private readonly RefreshTokenCookieSettings _cookieSettings = cookieOptions.Value;
    
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthResponseDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> RegisterAsync(
        [FromBody] RegisterRequest request,
        CancellationToken cancellationToken)
    {
        var command = new RegisterCommand(
            request.Email, 
            request.Username,
            request.AvatarUrl, 
            request.Password, 
            request.ConfirmPassword);
            
        var result = await mediator.Send(command, cancellationToken);
        SetRefreshTokenCookie(result.RefreshToken);
        return Ok(result with { RefreshToken = "" });
    }

    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthResponseDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> LoginAsync(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        var command = new LoginCommand(request.Email, request.Password);
        var result = await mediator.Send(command,cancellationToken);
        SetRefreshTokenCookie(result.RefreshToken);
        return Ok(result with { RefreshToken = "" });
    }

    [HttpPost("refresh-token")]
    [ProducesResponseType<AuthResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Refresh(CancellationToken ct)
    {
        if (!Request.Cookies.TryGetValue(_cookieSettings.Name, out var refreshToken) || string.IsNullOrEmpty(refreshToken))
            return Unauthorized();

        var result = await mediator.Send(new RefreshTokenCommand(refreshToken), ct);
        SetRefreshTokenCookie(result.RefreshToken);
        return Ok(result with { RefreshToken = "" });
    }

    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public IActionResult Logout()
    {
        Response.Cookies.Delete(_cookieSettings.Name, new CookieOptions { Path = _cookieSettings.Path });
        return NoContent();
    }

    private void SetRefreshTokenCookie(string? refreshToken)
    {
        if (string.IsNullOrEmpty(refreshToken)) return;

        Response.Cookies.Append(_cookieSettings.Name, refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = Enum.Parse<SameSiteMode>(_cookieSettings.SameSite, ignoreCase: true),
            Path = _cookieSettings.Path,
            Expires = DateTimeOffset.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryInDays)
        });
    }
}

  