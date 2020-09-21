using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EquipmentRental.Common;
using EquipmentRental.Common.DataModels;
using EquipmentRental.Common.Requests;
using EquipmentRental.Common.Responses;
using EquipmentRental.Inventory.Options;
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EquipmentRental.Inventory.MessageHandlers
{
    public class GenerateInvoiceMessageHandler : IConsumer<GenerateInvoiceRequest>
    {
        private readonly ILogger _logger;
        private readonly InvoiceOptions _options;

        public GenerateInvoiceMessageHandler(IOptions<InvoiceOptions> options, ILogger<GenerateInvoiceMessageHandler> logger)
        {
            _logger = logger;
            _options = options.Value;
        }
        
        public async Task Consume(ConsumeContext<GenerateInvoiceRequest> context)
        {
            _logger.LogInformation($"Received Invoice generate request!");
            var invoiceItems = CreateInvoiceItems(context.Message.RentedEquipment);
            await context.RespondAsync(new GenerateInvoiceResponse
            {
                Invoice = new Invoice
                {
                    Title = Guid.NewGuid().ToString(),
                    InvoiceItems = invoiceItems,
                    Price = String.Format("€{0:C}", GetTotalPrice(invoiceItems)),
                    LoyalityPoints = GetLoyalityPoints(context.Message.RentedEquipment)
                }
            });
        }

        public decimal GetTotalPrice(List<InvoiceItem> invoiceItems) => invoiceItems.Sum(item => item.RentalPrice);
        
        public int GetLoyalityPoints(List<EquipmentItem> equipmentItems)
        {
            var loyalityPoints = 0;
            foreach (var item in equipmentItems)
                loyalityPoints += item.EquipmentType == EquipmentType.Heavy ? 2 : 1;
            return loyalityPoints;
        }

        private List<InvoiceItem> CreateInvoiceItems(List<EquipmentItem> equipmentItems)
        {
            var invoiceItems = new List<InvoiceItem>();
            foreach (var equipment in equipmentItems)
                invoiceItems.Add(CreateInvoiceItem(equipment));

            return invoiceItems;
        }

        public InvoiceItem CreateInvoiceItem(EquipmentItem equipment)
        {
            ValidateEquipment(equipment);
            switch (equipment.EquipmentType)
            {
                case EquipmentType.Heavy:
                    return CreateHeavyEquipmentInvoice(equipment);
                case EquipmentType.Regular:
                    return CreateRegularEquipmentInvoice(equipment);
                case EquipmentType.Specialized:
                    return CreateSpecializedEquipmentInvoice(equipment);
                default:
                    throw new NotImplementedException($"No Invoice creation logic for {equipment.EquipmentType} implemented!");
            }
        }

        private InvoiceItem CreateHeavyEquipmentInvoice(EquipmentItem equipment) => new InvoiceItem
            {ItemName = equipment.Name, RentalPrice = _options.OneTimeFee + equipment.RentalDays * _options.PremiumFee};

        private InvoiceItem CreateRegularEquipmentInvoice(EquipmentItem equipment) => new InvoiceItem
            {ItemName = equipment.Name, RentalPrice = _options.OneTimeFee + CalculateDaysFee(equipment.RentalDays, 2)};

        private InvoiceItem CreateSpecializedEquipmentInvoice(EquipmentItem equipment) => new InvoiceItem
            {ItemName = equipment.Name, RentalPrice = CalculateDaysFee(equipment.RentalDays, 3)};

        /// <summary>
        /// First x premiumDays are billed at premium price, after that, regular fee is applied 
        /// </summary>
        private int CalculateDaysFee(int rentDays, int premiumDays) =>
            rentDays > premiumDays
                ? premiumDays * _options.PremiumFee + (rentDays - premiumDays) * _options.RegularFee
                : rentDays * _options.PremiumFee;

        private void ValidateEquipment(EquipmentItem equipment)
        {
            if (equipment.RentalDays < 1)
                throw new ArgumentOutOfRangeException($"equipment rental days have to be more than 1 day");
        }
    }
}