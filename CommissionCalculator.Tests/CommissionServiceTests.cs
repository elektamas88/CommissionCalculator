using CommissionCalculator.Core.Entities;
using CommissionCalculator.Core.Enums;
using CommissionCalculator.Core.Interfaces;
using CommissionCalculator.Core.Services;
using CommissionCalculator.Core.Strategies;
using Moq;

namespace CommissionCalculator.Tests
{
    public class CommissionServiceTests
    {
        private readonly Mock<ICommissionRepository> _mockRepo;
        private readonly CommissionService _service;
        private readonly Mock<CommissionCalculatorFactory> _mockFactory;

        public CommissionServiceTests()
        {
            _mockRepo = new Mock<ICommissionRepository>();
            _mockFactory = new Mock<CommissionCalculatorFactory>();
            _service = new CommissionService(_mockRepo.Object, _mockFactory.Object);
        }

        [Fact]
        public async Task CalculateCommission_FlatRateApplied()
        {
            // Arrange
            var product = new Product { Id = 1, Name = "Wheel", Price = 10.00m };
            var salesperson = new SalesPerson { Id = 1, Name = "Flora" };
            var invoice = new Invoice
            {
                Id = 1,
                SalesPersonId = salesperson.Id,
                SalesPerson = salesperson,
                Items = new List<InvoiceItem>
                {
                    new InvoiceItem { Product = product, Quantity = 5 }
                }
            };
            var rules = new List<CommissionRule>
            {
                new CommissionRule { Id = 1, ProductId = 1, FlatAmount = 2.00m, Type = CommissionType.Flat, Level = CommissionLevel.Product }
            };

            _mockRepo.Setup(r => r.GetInvoice(1)).ReturnsAsync(invoice);
            _mockRepo.Setup(r => r.GetCommissionRules(1)).ReturnsAsync(rules);

            var factory = new CommissionCalculatorFactory();

            var service = new CommissionService(_mockRepo.Object, factory);

            // Act
            var result = await service.CalculateTotalCommission(1);

            // Assert
            Assert.Equal(10.00m, result);
        }

        [Fact]
        public async Task CalculateCommission_PercentageOnInvoiceApplied()
        {
            // Arrange
            var product1 = new Product { Id = 1, Name = "Product1", Price = 100.00m };
            var product2 = new Product { Id = 2, Name = "Product2", Price = 50.00m };
            var salesperson = new SalesPerson { Id = 1, Name = "Veronica" };

            var invoice = new Invoice
            {
                Id = 1,
                SalesPersonId = salesperson.Id,
                SalesPerson = salesperson,
                Items = new List<InvoiceItem>
                {
                    new InvoiceItem { Product = product1, Quantity = 2 },
                    new InvoiceItem { Product = product2, Quantity = 3 }
                }
            };

            var rules = new List<CommissionRule>
            {
                new CommissionRule
                {
                    Type = CommissionType.Percentage,
                    Level = CommissionLevel.Invoice,
                    Percentage = 10.00m // 10% commission on total invoice amount
                }
            };

            _mockRepo.Setup(r => r.GetInvoice(1)).ReturnsAsync(invoice);
            _mockRepo.Setup(r => r.GetCommissionRules(1)).ReturnsAsync(rules);

            var factory = new CommissionCalculatorFactory();

            var service = new CommissionService(_mockRepo.Object, factory);

            // Act
            var result = await service.CalculateTotalCommission(1);

            // Assert
            // Total Invoice Amount = (2 * 100) + (3 * 50) = 200 + 150 = 350
            // Expected Commission = 10% of 350 = 35.00
            Assert.Equal(35.00m, result);
        }

        [Fact]
        public void Calculate_TieredCommissionBasedOnQuantity()
        {
            // Arrange
            var product = new Product { Id = 1, Name = "Wheel", Price = 10.00m };
            var invoiceItem = new InvoiceItem { Product = product, Quantity = 15 };

            var rule = new CommissionRule
            {
                Type = CommissionType.Tiered,
                Level = CommissionLevel.MultiplesOfProduct,
                FlatAmount = 0,
                Percentage = null,
                MinQuantity = 10,
                MaxQuantity = 20,
                CapAmount = null,
                CapPercentage = null
            };

            var calculator = new TieredCommissionCalculator();

            // Act
            var commission = calculator.Calculate(invoiceItem, null, rule);

            // Assert
            Assert.Equal(0, commission); // No commission expected because FlatAmount is 0
        }

        [Fact]
        public void Calculate_TieredCommissionWithFlatAmount()
        {
            // Arrange
            var product = new Product { Id = 1, Name = "Wheel", Price = 10.00m };
            var invoiceItem = new InvoiceItem { Product = product, Quantity = 15 };

            var rule = new CommissionRule
            {
                Type = CommissionType.Tiered,
                Level = CommissionLevel.MultiplesOfProduct,
                FlatAmount = 20.00m, // Flat amount for this tier
                Percentage = null,
                MinQuantity = 10,
                MaxQuantity = 20,
                CapAmount = null,
                CapPercentage = null
            };

            var calculator = new TieredCommissionCalculator();

            // Act
            var commission = calculator.Calculate(invoiceItem, null, rule);

            // Assert
            Assert.Equal(20.00m, commission); // Flat amount for this tier
        }

        [Fact]
        public void Calculate_TieredCommissionWithPercentage()
        {
            // Arrange
            var product = new Product { Id = 1, Name = "Wheel", Price = 10.00m };
            var invoiceItem = new InvoiceItem { Product = product, Quantity = 15 };

            var rule = new CommissionRule
            {
                Type = CommissionType.Tiered,
                Level = CommissionLevel.MultiplesOfProduct,
                FlatAmount = null,
                Percentage = 10.00m, // 10% commission
                MinQuantity = 10,
                MaxQuantity = 20,
                CapAmount = null,
                CapPercentage = null
            };

            var calculator = new TieredCommissionCalculator();

            // Act
            var commission = calculator.Calculate(invoiceItem, null, rule);

            // Assert
            var totalValue = invoiceItem.Quantity * invoiceItem.Product.Price; // 15 * 10 = 150
            var expectedCommission = totalValue * (rule.Percentage.GetValueOrDefault() / 100); // 10% of 150 = 15.00
            Assert.Equal(expectedCommission, commission);
        }

        [Fact]
        public void Calculate_TieredCommissionWithMultipleTiers()
        {
            // Arrange
            var product = new Product { Id = 1, Name = "Wheel", Price = 10.00m };
            var invoice = new Invoice
            {
                Id = 1,
                Items = new List<InvoiceItem>
                {
                    new InvoiceItem { Product = product, Quantity = 5 },
                    new InvoiceItem { Product = product, Quantity = 15 }
                }
            };

            var rule = new List<CommissionRule>
            {
                new CommissionRule
                {
                    Type = CommissionType.Tiered,
                    Level = CommissionLevel.MultiplesOfProduct,
                    FlatAmount = 10.00m, // For 1-10 items
                    Percentage = null,
                    MinQuantity = 1,
                    MaxQuantity = 10,
                    CapAmount = null,
                    CapPercentage = null
                },
                new CommissionRule
                {
                    Type = CommissionType.Tiered,
                    Level = CommissionLevel.MultiplesOfProduct,
                    FlatAmount = 20.00m, // For 11-20 items
                    Percentage = null,
                    MinQuantity = 11,
                    MaxQuantity = 20,
                    CapAmount = null,
                    CapPercentage = null
                }
            };

            var calculator = new TieredCommissionCalculator();

            // Act
            var commissionForItem1 = calculator.Calculate(invoice.Items.ToList()[0], invoice, rule[0]);
            var commissionForItem2 = calculator.Calculate(invoice.Items.ToList()[1], invoice, rule[1]);

            // Assert
            // For item1, 5 units fall in the 1-10 tier so should get 10.00 flat
            Assert.Equal(10.00m, commissionForItem1);

            // For item2, 15 units fall in the 11-20 tier so should get 20.00 flat
            Assert.Equal(20.00m, commissionForItem2);
        }

        [Fact]
        public async Task CalculateCommission_WithCapApplied()
        {
            // Arrange
            var product1 = new Product { Id = 1, Name = "Product1", Price = 50.00m };
            var product2 = new Product { Id = 2, Name = "Product2", Price = 100.00m };
            var salesperson = new SalesPerson { Id = 1, Name = "Bia" };

            var invoice = new Invoice
            {
                Id = 1,
                SalesPersonId = salesperson.Id,
                SalesPerson = salesperson,
                Items = new List<InvoiceItem>
                {
                    new InvoiceItem { Product = product1, Quantity = 3 }, // Total = 3 * 50 = 150
                    new InvoiceItem { Product = product2, Quantity = 2 }  // Total = 2 * 100 = 200
                }
            };

            // Total invoice amount = 150 + 200 = 350

            var rules = new List<CommissionRule>
            {
                new CommissionRule
                {
                    Type = CommissionType.Percentage,
                    Level = CommissionLevel.Invoice,
                    Percentage = 15.00m, // 15% commission
                    CapAmount = 30.00m   // Cap the commission to a maximum of 30.00
                }
            };

            // Set up mock repository methods
            _mockRepo.Setup(r => r.GetInvoice(1)).ReturnsAsync(invoice);
            _mockRepo.Setup(r => r.GetCommissionRules(1)).ReturnsAsync(rules);

            // Use the real CommissionCalculatorFactory
            var factory = new CommissionCalculatorFactory();

            // Inject the real factory into the service
            var service = new CommissionService(_mockRepo.Object, factory);

            // Act
            var result = await service.CalculateTotalCommission(1);

            // Assert
            // Total Commission before cap = 15% of 350 = 52.50
            // But with cap applied, the commission should be capped at 30.00
            Assert.Equal(30.00m, result);
        }
    }
}