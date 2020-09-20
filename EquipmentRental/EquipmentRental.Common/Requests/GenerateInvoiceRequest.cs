using System.Collections.Generic;
using EquipmentRental.Common.DataModels;

namespace EquipmentRental.Common.Requests
{
    public class GenerateInvoiceRequest
    {
        public List<EquipmentItem> RentedEquipment { get; set; }
    }
}