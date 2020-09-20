using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EquipmentRental.Common;
using EquipmentRental.Common.DataModels;
using EquipmentRental.Common.Requests;
using EquipmentRental.Common.Responses;
using MassTransit;
using Microsoft.Extensions.Logging;
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

        public EquipmentService(ILogger<EquipmentService> logger, IBus bus)
        {
            _logger = logger;
            _bus = bus;
        }

        public async Task<List<Equipment>> GetAvailableEquipment()
        {
            var requestAddress = new Uri("rabbitmq://localhost/get_available_equipment");

            var client = _bus.CreateRequestClient<EquipmentInventoryRequest>(requestAddress);
            _logger.LogInformation($"Requesting Inventory!");
            var response = await client.GetResponse<EquipmentInventoryResponse>(new EquipmentInventoryRequest());
                //TODO handle failures!
            return response.Message.Equipment;
        }

        public async Task<Invoice> GenerateInvoice(List<EquipmentItem> equipment)
        {
            var requestAddress = new Uri("rabbitmq://localhost/generate_invoice");

            var client = _bus.CreateRequestClient<GenerateInvoiceRequest>(requestAddress);

            var response = await client.GetResponse<GenerateInvoiceResponse>(new GenerateInvoiceRequest
                {RentedEquipment = equipment});
            //TODO handle failures!
            return response.Message.Invoice;
        }
    }
}