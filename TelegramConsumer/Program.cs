using Contracts;
using MassTransit;
using Microsoft.Extensions.Options;

namespace TelegramConsumer
{
    
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);
            var services = builder.Services;
            bool isContainer = builder.Configuration.GetValue<bool>("DOTNET_RUNNING_IN_CONTAINER");
            services.AddSingleton<TelegramService>();
            services.AddScoped<TelegramConsumer>();
            services.AddMassTransit(bus => 
            {
                var options = new RabbitMqOptions(); 
                builder.Configuration.GetSection("RabbitMqOptions").Bind(options);
                if (isContainer)
                {
                    options.Host = "rabbitmq";
                }
                bus.AddConsumer<TelegramConsumer>();
                bus.UsingRabbitMq((context,config) => 
                {
                    config.Host(options!.Host ?? "localhost", "/", h =>
                    {
                        h.Username(options.Username);
                        h.Password(options.Password);
                    });
                    config.ConfigureEndpoints(context);
                });
            });
            var host = builder.Build();
            host.Run();
        }
    }
}
