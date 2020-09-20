using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using EquipmentRental.Inventory.MessageHandlers;
using EquipmentRental.Inventory.Options;
using EquipmentRental.Inventory.Services;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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
                        await bus.StartAsync();
                    }
                    Console.WriteLine("Press enter to exit");

                    await Task.Run(() => Console.ReadLine());

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
                cfg.Host("rabbitmq", "/", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                cfg.ReceiveEndpoint("get_available_equipment",
                    e => { e.Consumer(provider.GetService<GetEquipmentMessageHandler>); });
                cfg.ReceiveEndpoint("generate_invoice",
                    e => { e.Consumer(provider.GetService<GenerateInvoiceMessageHandler>); });
            });

            //await bus.StartAsync();
            
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
            
            services.AddLogging(configure => configure.AddConsole());

            services.AddTransient<GetEquipmentMessageHandler>();
            services.AddTransient<GenerateInvoiceMessageHandler>();
            services.AddSingleton<IEquipmentInventoryService, EquipmentInventoryService>();
        }
    }
}