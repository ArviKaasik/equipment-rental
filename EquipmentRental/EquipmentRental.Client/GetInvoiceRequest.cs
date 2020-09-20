using System.Collections.Generic;
using EquipmentRental.Common;
using EquipmentRental.Common.DataModels;

namespace EquipmentRentalClient
{
    public class GetInvoiceRequest
    {
        public List<EquipmentItem> Equipment { get; set; }
    }
}