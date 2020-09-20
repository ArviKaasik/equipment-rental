using System.Collections.Generic;
using EquipmentRental.Common;
using EquipmentRental.Common.DataModels;
using EquipmentRental.Inventory.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace EquipmentRental.Inventory.Services
{
    public class EquipmentInventoryService : IEquipmentInventoryService
    {
        private readonly ILogger _logger;
        private readonly EquipmentInventoryOptions _options;
        
        public List<Equipment> Inventory { private set; get; }

        public EquipmentInventoryService(IOptions<EquipmentInventoryOptions> options, ILogger<EquipmentInventoryService> logger)
        {
            _logger = logger;
            _options = options.Value;
        }
        
        public void LoadInventory()
        {
            var serializer = new JsonSerializer();
            using (var fileStreamReader = new System.IO.StreamReader(_options.InventorySourceFilePath))
            using (var jsonTextReader = new JsonTextReader(fileStreamReader))
            {
                Inventory = serializer.Deserialize<List<Equipment>>(jsonTextReader);
                _logger.LogInformation($"Deserialized inventory of {Inventory.Count} items");
            }
        }

    }
}