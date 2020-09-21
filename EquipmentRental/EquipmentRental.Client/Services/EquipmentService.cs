using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EquipmentRental.Common;
using EquipmentRental.Common.DataModels;
using EquipmentRental.Common.Requests;
using EquipmentRental.Common.Responses;
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace EquipmentRentalClient.Services
{
    public interface IEquipmentService
    {
        Task<List<Equipment>> GetAvailableEquipment();
        Task<Invoice> GenerateInvoice(List<EquipmentItem> equipment);
    }
    
    public class EquipmentService : IEquipmentService
    {
        private readonly ILogger _logger;
        private readonly IBus _bus;
        private readonly RabbitMqConnectionOptions _options;

        public EquipmentService(ILogger<EquipmentService> logger, IBus bus, IOptions<RabbitMqConnectionOptions> options)
        {
            _logger = logger;
            _bus = bus;
            _options = options.Value;
        }

        public async Task<List<Equipment>> GetAvailableEquipment()
        {
            var requestAddress = new Uri($"rabbitmq://{_options.Host}/{_options.EquipmentQueue}");

            var request = new EquipmentInventoryRequest();
            _logger.LogInformation($"Sending InvoiceRequest for {JsonConvert.SerializeObject(request)} to {requestAddress}");

            var client = _bus.CreateRequestClient<EquipmentInventoryRequest>(requestAddress);
            return (await client.GetResponse<EquipmentInventoryResponse>(request)).Message.Equipment;
        }

        public async Task<Invoice> GenerateInvoice(List<EquipmentItem> equipment)
        {
            var requestAddress = new Uri($"rabbitmq://{_options.Host}/{_options.InvoiceQueue}");

            var request = new GenerateInvoiceRequest {RentedEquipment = equipment};
            _logger.LogInformation($"Sending InvoiceRequest for {JsonConvert.SerializeObject(request)} to {requestAddress}");
            
            var client = _bus.CreateRequestClient<GenerateInvoiceRequest>(requestAddress);
            return (await client.GetResponse<GenerateInvoiceResponse>(request)).Message.Invoice;
        }
    }
}