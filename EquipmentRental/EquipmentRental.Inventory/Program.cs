using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using EquipmentRental.Common;
using EquipmentRental.Inventory.MessageHandlers;
using EquipmentRental.Inventory.Options;
using EquipmentRental.Inventory.Services;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EquipmentRental.Inventory
{
    class Program
    {
        public static IConfigurationRoot configuration;

        static async Task Main(string[] args)
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            using (ServiceProvider serviceProvider = services.BuildServiceProvider())
            {
                var bus = await InitializeRabbitMQ(serviceProvider);

                serviceProvider.GetService<IEquipmentInventoryService>().LoadInventory();

                try
                {
                    while (true)
                    {
                        await Task.Delay(10000);
                    }
                }
                finally
                {
                    await bus.StopAsync();
                }
            }
        }

        private static async Task<IBusControl> InitializeRabbitMQ (ServiceProvider provider)
        {
            var bus = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                var options = provider.GetService<IOptions<RabbitMqConnectionOptions>>().Value;
                cfg.Host(options.Host, "/", h =>
                {
                    h.Username(options.UserName);
                    h.Password(options.Password);
                });

                cfg.ReceiveEndpoint(options.EquipmentQueue,
                    e => { e.Consumer(provider.GetService<GetEquipmentMessageHandler>); });
                cfg.ReceiveEndpoint(options.InvoiceQueue,
                    e => { e.Consumer(provider.GetService<GenerateInvoiceMessageHandler>); });
            });

            await bus.StartAsync();

            return bus;
        }

        private static void ConfigureServices(ServiceCollection services)
        {
            configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                .AddJsonFile("appsettings.json", false)
                .Build();
            
            services.AddSingleton(configuration);
            
            services.Configure<EquipmentInventoryOptions>(configuration);
            services.Configure<RabbitMqConnectionOptions>(configuration.GetSection("RabbitMQ"));
            
            services.AddLogging(configure => configure.AddConsole());

            services.AddTransient<GetEquipmentMessageHandler>();
            services.AddTransient<GenerateInvoiceMessageHandler>();
            services.AddSingleton<IEquipmentInventoryService, EquipmentInventoryService>();
        }
    }
}