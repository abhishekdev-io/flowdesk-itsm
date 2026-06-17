using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace FlowDesk.Infrastructure.Identity.Extensions;

public static class AuthExtension
{
    public static void AddAuthServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = config["Jwt:Issuer"],
                    ValidAudience = config["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:SecretKey"]!))
                };
                // options.Events = new JwtBearerEvents
                // {
                //     OnTokenValidated = context =>
                //     {
                //         Console.WriteLine("TOKEN VALIDATED");
                //         return Task.CompletedTask;
                //     },

                //     OnAuthenticationFailed = context =>
                //     {
                //         Console.WriteLine("AUTH FAILED");
                //         Console.WriteLine(context.Exception);
                //         return Task.CompletedTask;
                //     }
                // };
            });

    }
}
