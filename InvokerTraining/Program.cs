using InvokerTraining.Application.Abstractions;
using InvokerTraining.Application.APIServices;
using InvokerTraining.Application.GameServices;
using InvokerTraining.Infrastructure;
using InvokerTraining.Infrastructure.Extentions;
using InvokerTraining.Infrastructure.JWT;
using InvokerTraining.Infrastructure.Redis;
using InvokerTraining.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using Serilog.Sinks.Seq;
using Serilog;

namespace InvokerTraining
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var seq = builder.Configuration.GetConnectionString("Seq")!;
            builder.Host.UseSerilog((context, configuration) => configuration
                    .ReadFrom.Configuration(context.Configuration)
                    .Enrich.FromLogContext()
                    .WriteTo.Seq(seq)
                    .WriteTo.Console()
            );
            Serilog.Debugging.SelfLog.Enable(msg => Console.WriteLine(msg));
            Log.Fatal("!!!!! ТЕСТОВЫЙ ЛОГ ДЛЯ SEQ !!!!!");
            builder.Services.AddSingleton<IConnectionMultiplexer>(r =>
            {
                var rc = builder.Configuration.GetConnectionString("Redis");
                return ConnectionMultiplexer.Connect(rc!);
            });
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
            builder.Services.AddScoped<ICacher, CacheManager>();
            builder.Services.AddScoped<IInfoService, InfoService>();
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
            app.UseRouting();
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseTokenRefresh();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            
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
