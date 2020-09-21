using System;

namespace EquipmentRental.Common
{
    public class RabbitMqConnectionOptions
    {
        public string UserName { get; set; }
        
        public string Password { get; set; }
        
        public string Host { get; set; }

        public string EquipmentQueue { get; set; }
        
        public string InvoiceQueue { get; set; }
    }
}