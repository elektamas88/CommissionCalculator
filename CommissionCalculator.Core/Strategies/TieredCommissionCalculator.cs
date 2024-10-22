using CommissionCalculator.Core.Entities;
using CommissionCalculator.Core.Interfaces;

namespace CommissionCalculator.Core.Strategies
{
    /// <summary>
    /// Calculator for determining commission based on a tiered structure.
    /// The commission is calculated based on different levels: Multiples of Product, Multiples of Product Values, or Invoice.
    /// This calculator supports both flat rate and percentage-based tiered commissions.
    /// </summary>
    public class TieredCommissionCalculator : ICommissionCalculator
    {
        /// <summary>
        /// Calculates the commission based on a tiered structure and the specified commission level.
        /// </summary>
        /// <param name="invoiceItem">The specific item in the invoice for which the commission is being calculated.</param>
        /// <param name="invoice">The invoice that contains the items sold. Used for Invoice-level calculations.</param>
        /// <param name="rule">The commission rule that specifies the tiered structure, including the min/max quantities or values, and the flat amount or percentage.</param>
        /// <returns>The calculated commission amount based on the rule.</returns>
        /// <exception cref="Exception">Thrown when an invalid commission level is provided for the tiered commission rule.</exception>
        public decimal Calculate(InvoiceItem invoiceItem, Invoice invoice, CommissionRule rule)
        {
            switch (rule.Level)
            {
                case Enums.CommissionLevel.MultiplesOfProduct:
                    return CalculateMultiplesOfProductCommission(invoiceItem, rule);
                case Enums.CommissionLevel.MultiplesOfProductValues:
                    return CalculateMultiplesOfProductValuesCommission(invoiceItem, rule);
                case Enums.CommissionLevel.Invoice:
                    return CalculateInvoiceCommission(invoice, rule);
                default:
                    throw new Exception("Invalid rule level for the tiered commission rule.");
            }
        }

        /// <summary>
        /// Calculates the tiered commission for a SalesPerson based on the quantity of a product sold.
        /// The commission is determined by comparing the quantity to the tier's min and max quantity.
        /// If within the range, either a flat amount or a percentage is applied.
        /// </summary>
        /// <param name="invoiceItem">The specific item in the invoice for which the commission is being calculated.</param>
        /// <param name="rule">The commission rule that specifies the tier's min and max quantity, flat amount, or percentage.</param>
        /// <returns>The calculated commission amount for the product quantity.</returns>
        private decimal CalculateMultiplesOfProductCommission(InvoiceItem invoiceItem, CommissionRule rule)
        {
            var quantity = invoiceItem.Quantity;
            decimal commission = 0;
            if (quantity >= rule.MinQuantity.GetValueOrDefault() && quantity <= rule.MaxQuantity.GetValueOrDefault())
            {
                if (rule.FlatAmount.HasValue)
                {
                    commission = rule.FlatAmount.Value;
                }
                else if (rule.Percentage.HasValue)
                {
                    commission = quantity * invoiceItem.Product.Price * (rule.Percentage.GetValueOrDefault() / 100);
                }
            }
            return commission;
        }

        /// <summary>
        /// Calculates the tiered commission for a SalesPerson based on the total value of a product sold.
        /// The commission is determined by comparing the product value to the tier's min and max value.
        /// If within the range, either a flat amount or a percentage is applied.
        /// </summary>
        /// <param name="invoiceItem">The specific item in the invoice for which the commission is being calculated.</param>
        /// <param name="rule">The commission rule that specifies the tier's min and max value, flat amount, or percentage.</param>
        /// <returns>The calculated commission amount for the product values.</returns>
        private decimal CalculateMultiplesOfProductValuesCommission(InvoiceItem invoiceItem, CommissionRule rule)
        {
            var productValues = invoiceItem.Quantity * invoiceItem.Product.Price;
            decimal commission = 0;
            if (productValues >= rule.MinQuantity.GetValueOrDefault() && productValues <= rule.MaxQuantity.GetValueOrDefault())
            {
                if (rule.FlatAmount.HasValue)
                {
                    commission = rule.FlatAmount.Value;
                }
                else if (rule.Percentage.HasValue)
                {
                    commission = productValues * (rule.Percentage.GetValueOrDefault() / 100);
                }
            }
            return commission;
        }

        /// <summary>
        /// Calculates the tiered commission for a SalesPerson based on the total value of the entire invoice.
        /// The commission is determined by comparing the invoice total to the tier's min and max value.
        /// If within the range, either a flat amount or a percentage is applied.
        /// </summary>
        /// <param name="invoice">The invoice that contains the items sold.</param>
        /// <param name="rule">The commission rule that specifies the tier's min and max value, flat amount, or percentage.</param>
        /// <returns>The calculated commission amount for the entire invoice.</returns>
        private decimal CalculateInvoiceCommission(Invoice invoice, CommissionRule rule)
        {
            var totalInvoiceAmount = invoice.Items.Sum(i => i.Quantity * i.Product.Price);
            decimal commission = 0;
            if (totalInvoiceAmount >= rule.MinQuantity.GetValueOrDefault() && totalInvoiceAmount <= rule.MaxQuantity.GetValueOrDefault())
            {
                if (rule.FlatAmount.HasValue)
                {
                    commission = rule.FlatAmount.Value;
                }
                else if (rule.Percentage.HasValue)
                {
                    commission = totalInvoiceAmount * (rule.Percentage.GetValueOrDefault() / 100);
                }
            }
            return commission;
        }
    }
}
