using System.Threading.Tasks;
using EquipmentRental.Common.Requests;
using EquipmentRental.Common.Responses;
using EquipmentRental.Inventory.Services;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace EquipmentRental.Inventory.MessageHandlers
{
    public class GetEquipmentMessageHandler : IConsumer<EquipmentInventoryRequest>
    {
        private readonly IEquipmentInventoryService _inventoryService;
        private readonly ILogger _logger;

        public GetEquipmentMessageHandler(IEquipmentInventoryService inventoryService, ILogger<GetEquipmentMessageHandler> logger)
        {
            _inventoryService = inventoryService;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<EquipmentInventoryRequest> context)
        {
            await context.RespondAsync(new EquipmentInventoryResponse
            {
                Equipment = _inventoryService.Inventory
            });
        }
    }
}