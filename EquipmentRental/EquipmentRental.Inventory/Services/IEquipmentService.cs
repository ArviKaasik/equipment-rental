using System.Collections.Generic;
using EquipmentRental.Common;
using EquipmentRental.Common.DataModels;

namespace EquipmentRental.Inventory.Services
{
    public interface IEquipmentInventoryService
    {
        void LoadInventory();
        
        List<Equipment> Inventory { get; }
    }
}