using System.Collections.Generic;
using EquipmentRental.Common;
using EquipmentRental.Common.DataModels;

namespace EquipmentRentalClient.Models
{
    public class EquipmentViewModel
    {
        public List<Equipment> AvailableEquipment { get; set; }
    }
}