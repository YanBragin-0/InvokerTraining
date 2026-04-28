using InvokerTraining.Application.Abstractions;
using InvokerTraining.Application.DataTransfers;
using InvokerTraining.Infrastructure.JWT;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace InvokerTraining.Infrastructure.Extentions
{
    public static class AuthJwtExtention
    {
        public static void AddApiAuth(
            this IServiceCollection serviceCollection,
            IConfiguration configuration)
        {
            var jwtOptions = configuration.GetSection(nameof(JwtOptions)).Get<JwtOptions>();

            serviceCollection.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, op =>
                {
                    op.TokenValidationParameters = new()
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions!.SecretKey))
                    };
                    op.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            context.Token = context.Request.Cookies["aTo"];
                            return Task.CompletedTask;
                        }
                    };
                });
            serviceCollection.AddAuthorization();
        }
        public static IApplicationBuilder UseTokenRefresh(this WebApplication app)
        {
            return app.Use(async (context, next) =>
            {
                var access = context.Request.Cookies["aTo"];
                if(string.IsNullOrEmpty(access) && context.Request.Cookies.ContainsKey("rTo"))
                {
                    context.Request.Cookies.TryGetValue("rTo",out string? refresh); 
                    using(var scope = context.RequestServices.CreateScope())
                    {
                        var service = scope.ServiceProvider.GetRequiredService<IPlayerService>();
                        var options = scope.ServiceProvider.GetRequiredService<IOptions<JwtOptions>>();
                        var result = await service.TryRefresh(refresh!);
                        if(result != null)
                        {
                            var cookieOptions = new CookieOptions
                            {
                                HttpOnly = true,
                                Secure = false, //!!!!
                                SameSite = SameSiteMode.Lax,
                                Path = "/"
                            };
                            context.Response.Cookies.Append("aTo", result.accessToken, new CookieOptions(cookieOptions)
                            {
                                Expires = DateTime.UtcNow.AddMinutes(options.Value.Expires)
                            });
                            context.Response.Cookies.Append("rTo", result.refreshToken, new CookieOptions
                            {
                                Expires = DateTime.UtcNow.AddMinutes(2)//test!!!!
                            });
                            context.Response.Redirect(context.Request.Path + context.Request.QueryString);
                            return;
                        }
                    }
                }
                await next();
            });
        }
    }
}
