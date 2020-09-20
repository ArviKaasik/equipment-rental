using System.Collections.Generic;

namespace EquipmentRental.Common.DataModels
{
    public class Invoice
    {
        public string Title { get; set; }
        
        public List<InvoiceItem> InvoiceItems { get; set; }
        
        public string Price { get; set; }
        
        public int LoyalityPoints { get; set; }
    }

    public class InvoiceItem
    {
        public string ItemName { get; set; }
        
        // Important to use decimal type here to avoid rounding errors in calculations with floating-point numbers
        public decimal RentalPrice { get; set; }
    }
}