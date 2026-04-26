using InvokerTraining.Application.Abstractions;
using InvokerTraining.Application.APIServices;
using InvokerTraining.Application.GameServices;
using InvokerTraining.Infrastructure;
using InvokerTraining.Infrastructure.Extentions;
using InvokerTraining.Infrastructure.JWT;
using InvokerTraining.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace InvokerTraining
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();
            builder.Services.AddSignalR();
            builder.Services.AddHttpClient();
            builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(nameof(JwtOptions)));
            builder.Services.AddSingleton<InvokeService>();
            builder.Services.AddSingleton<InvokeHub>();
            builder.Services.AddScoped<IHasher,PasswordHasher>();
            builder.Services.AddScoped<IJwtProvider,JwtProvider>();
            builder.Services.AddScoped<IPlayerRepository, PlayerRepository>();
            builder.Services.AddScoped<IPlayerService, PlayerService>();
            builder.Services.AddScoped<IGameSessionRepository, GameSessionRepository>();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<ICurrentUser, CurrentUserService>();
            builder.Services.AddApiAuth(builder.Configuration);
            builder.Services.ConfigureApplicationCookie(options => 
            { 

                options.LoginPath = "/Account/Login";
            });
            var DbConnectionString = builder.Configuration.GetConnectionString("Postgres");
            builder.Services.AddDbContext<AppDbContext>(op => op.UseNpgsql(DbConnectionString));
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Account}/{action=Register}/{id?}")
                .WithStaticAssets();
            app.MapHub<InvokeHub>("/invoker");
            app.Run();
        }
    }
}
