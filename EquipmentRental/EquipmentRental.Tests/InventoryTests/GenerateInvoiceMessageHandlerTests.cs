using System.Collections.Generic;
using EquipmentRental.Common.DataModels;
using EquipmentRental.Inventory.MessageHandlers;
using EquipmentRental.Inventory.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Xunit;

namespace EquipmentRental.Tests.InventoryTests
{
    public class GenerateInvoiceMessageHandlerTests
    {
        private readonly IOptions<InvoiceOptions> _options = Options.Create(new InvoiceOptions());

        private readonly ILogger<GenerateInvoiceMessageHandler> _logger =
            new NullLogger<GenerateInvoiceMessageHandler>();

        /// <summary>Customer should receive 2 loyality points for heavy equipment or 1 for other regardless of rental time</summary>
        [Theory]
        [InlineData(EquipmentType.Heavy, 1, 2)]
        [InlineData(EquipmentType.Heavy, 2, 2)]
        [InlineData(EquipmentType.Regular, 1, 1)]
        [InlineData(EquipmentType.Regular, 2, 1)]
        [InlineData(EquipmentType.Regular, int.MaxValue, 1)]
        [InlineData(EquipmentType.Specialized, 1, 1)]
        [InlineData(EquipmentType.Specialized, 2, 1)]
        public void GetLoyalityPoints_CorrectLoyalityPoints_AnyEquipmentAnyDays(EquipmentType type, int rentDays,
            int expected)
        {
            // Arrange
            var rentedItems = new List<EquipmentItem> {new EquipmentItem {EquipmentType = type, RentalDays = rentDays}};
            var handler = new GenerateInvoiceMessageHandler(_options, _logger);

            // Act
            var result = handler.GetLoyalityPoints(rentedItems);

            // Assert
            Assert.Equal(expected, result);
        }

        /// <summary>Invoice for heavy equipment should be one time rental fee + rental days number * premium fee</summary>
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(5)]
        [InlineData(int.MaxValue)]
        public void CreateInvoiceItem_OneTimePlusPremiumEach_HeavyEquipmentRented(int rentDays)
        {
            // Arrange
            var rentedItem = new EquipmentItem {EquipmentType = EquipmentType.Heavy, RentalDays = rentDays};
            var handler = new GenerateInvoiceMessageHandler(_options, _logger);

            // Act
            var result = handler.CreateInvoiceItem(rentedItem);

            // Assert
            Assert.Equal(_options.Value.OneTimeFee + rentDays * _options.Value.PremiumFee, result.RentalPrice);
        }

        /// <summary>Price for regular equipment should be one time fee + premium for first 2 days each</summary>
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public void CreateInvoiceItem_OneTimePlusPremiumEach_RegularEquipment(int rentDays)
        {
            // Arrange
            var rentedItem = new EquipmentItem {EquipmentType = EquipmentType.Regular, RentalDays = rentDays};
            var handler = new GenerateInvoiceMessageHandler(_options, _logger);

            // Act
            var result = handler.CreateInvoiceItem(rentedItem);

            // Assert
            Assert.Equal(_options.Value.OneTimeFee + rentDays * _options.Value.PremiumFee, result.RentalPrice);
        }

        /// <summary>
        /// Price for regular equipment should be one time fee + premium for first 2 days each and regular fee for next
        /// days each
        /// </summary>
        [Theory]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(5)]
        [InlineData(10)]
        public void CreateInvoiceItem_OneTimeTwoPremiumRegularEach_RegularEquipment(int rentDays)
        {
            // Arrange
            var rentedItem = new EquipmentItem {EquipmentType = EquipmentType.Regular, RentalDays = rentDays};
            var handler = new GenerateInvoiceMessageHandler(_options, _logger);

            // Act
            var result = handler.CreateInvoiceItem(rentedItem);

            // Assert
            var expected = _options.Value.OneTimeFee + 2 * _options.Value.PremiumFee +
                            (rentDays - 2) * _options.Value.RegularFee;
            Assert.Equal(expected, result.RentalPrice);
        }

        /// <summary>Price for specialized equipment should be one time fee + premium for first 3 days each</summary>
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void CreateInvoiceItem_PremiumEach_SpecializedEquipment(int rentDays)
        {
            // Arrange
            var rentedItem = new EquipmentItem {EquipmentType = EquipmentType.Specialized, RentalDays = rentDays};
            var handler = new GenerateInvoiceMessageHandler(_options, _logger);

            // Act
            var result = handler.CreateInvoiceItem(rentedItem);

            // Assert
            Assert.Equal(rentDays * _options.Value.PremiumFee, result.RentalPrice);
        }

        /// <summary>
        /// Price for specialized equipment should be one time fee + premium for first 3 days each and regular fee for
        /// next days each
        /// </summary>
        [Theory]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(7)]
        [InlineData(10)]
        public void CreateInvoiceItem_PremiumThreePlusRegularEach_SpecializedEquipment(int rentDays)
        {
            // Arrange
            var rentedItem = new EquipmentItem {EquipmentType = EquipmentType.Specialized, RentalDays = rentDays};
            var handler = new GenerateInvoiceMessageHandler(_options, _logger);

            // Act
            var result = handler.CreateInvoiceItem(rentedItem);

            // Assert
            var expected = 3 * _options.Value.PremiumFee + (rentDays - 3) * _options.Value.RegularFee;
            Assert.Equal(expected, result.RentalPrice);
        }
    }
}