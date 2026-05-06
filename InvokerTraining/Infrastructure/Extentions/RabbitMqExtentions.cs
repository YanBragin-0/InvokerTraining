using Contracts;
using MassTransit;
using Microsoft.Extensions.Options;


namespace InvokerTraining.Infrastructure.Extentions
{
    public static class RabbitMqExtentions
    {
        public static void AddRabbitMassTransitConfigurations(this IServiceCollection services,
            IConfiguration configuration,WebApplicationBuilder builder)
        {
            var rabbitOptions = new RabbitMqOptions();
            configuration.GetSection("RabbitMqOptions").Bind(rabbitOptions);
            bool isContainer = builder.Configuration.GetValue<bool>("DOTNET_RUNNING_IN_CONTAINER");
            services.AddMassTransit(x => 
            {
                x.UsingRabbitMq((context, config) =>
                {
                    if (isContainer)
                    {
                        rabbitOptions.Host = "rabbitmq";
                    }
                    config.Host(rabbitOptions?.Host ?? "localhost", "/", h =>
                    {
                        var user = rabbitOptions!.Username;
                        var pass = rabbitOptions.Password;
                        h.Username(user);
                        h.Password(pass);
                    });
                });
            });
        }
    }
}
