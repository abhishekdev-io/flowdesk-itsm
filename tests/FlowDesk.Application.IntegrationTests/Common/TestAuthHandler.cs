using Microsoft.Extensions.Logging;
using System.Text.Encodings.Web;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using FlowDesk.Application.IntegrationTests.Common.Helper;

namespace FlowDesk.Application.IntegrationTests.Common;

//Create a custom authentication handler which generates a fake user for testing purposes, allowing us to bypass real authentication during integration tests.
public class TestAuthHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    public const string SchemeName = "Test";

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, TestUserDataSeeder.TestUserId.ToString()),
            new Claim(ClaimTypes.Email, TestUserDataSeeder.TestUserEmail),
            new Claim(ClaimTypes.Name, TestUserDataSeeder.TestUserEmail)
        };

        var identity = new ClaimsIdentity(claims, SchemeName);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, SchemeName);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}

