using CommissionCalculator.Core.Entities;
using CommissionCalculator.Core.Interfaces;

namespace CommissionCalculator.Core.Strategies
{
    /// <summary>
    /// Calculator for determining commission based on a flat rate.
    /// The commission is calculated based on different levels: Product, Multiples of Product, or Invoice.
    /// </summary>
    public class FlatRateCommissionCalculator : ICommissionCalculator
    {
        /// <summary>
        /// Calculates the commission based on the flat rate and the specified commission level.
        /// </summary>
        /// <param name="invoiceItem">The specific item in the invoice for which the commission is being calculated.</param>
        /// <param name="invoice">The invoice that contains the items sold. Used for Invoice-level calculations.</param>
        /// <param name="rule">The commission rule that specifies the flat amount and the level (Product, MultiplesOfProduct, or Invoice).</param>
        /// <returns>The calculated commission amount based on the rule.</returns>
        /// <exception cref="Exception">Thrown when an invalid commission level is provided for the flat commission rule.</exception>
        public decimal Calculate(InvoiceItem invoiceItem, Invoice invoice, CommissionRule rule)
        {
            switch (rule.Level)
            {
                case Enums.CommissionLevel.Product:
                    return CalculateProductCommission(invoiceItem, rule);
                case Enums.CommissionLevel.MultiplesOfProduct:
                    return CalculateMultiplesOfProductCommission(invoiceItem, rule);
                case Enums.CommissionLevel.Invoice:
                    return CalculateInvoiceCommission(rule);
                default:
                    throw new Exception("Invalid rule level for the flat commission rule.");
            }
        }

        /// <summary>
        /// Calculates the flat commission for a SalesPerson based on the number of units of a product sold.
        /// SalesPerson gets a set amount per unit sold.
        /// </summary>
        /// <param name="invoiceItem">The specific item in the invoice for which the commission is being calculated.</param>
        /// <param name="rule">The commission rule that specifies the flat amount to be applied per unit.</param>
        /// <returns>The calculated commission amount for the product.</returns>
        private static decimal CalculateProductCommission(InvoiceItem invoiceItem, CommissionRule rule)
        {
            return invoiceItem.Quantity * rule.FlatAmount.GetValueOrDefault();
        }

        /// <summary>
        /// Calculates the flat commission for a SalesPerson based on the sale of multiples or batches of a product.
        /// If a SalesPerson sells any multiples of a product, they get a set amount for each batch.
        /// </summary>
        /// <param name="invoiceItem">The specific item in the invoice for which the commission is being calculated.</param>
        /// <param name="rule">The commission rule that specifies the flat amount to be applied per batch of products.</param>
        /// <returns>The calculated commission amount for the multiples of the product.</returns>
        private decimal CalculateMultiplesOfProductCommission(InvoiceItem invoiceItem, CommissionRule rule)
        {
            var quantity = invoiceItem.Quantity;
            var batchSize = rule.MinQuantity.GetValueOrDefault(); // Assuming MinQuantity represents the batch size
            var numberOfBatches = quantity / batchSize;
            return numberOfBatches * rule.FlatAmount.GetValueOrDefault();
        }

        /// <summary>
        /// Calculates the flat commission for a SalesPerson based on the entire invoice.
        /// SalesPerson gets a set amount per invoice, regardless of the products or quantities sold.
        /// </summary>
        /// <param name="rule">The commission rule that specifies the flat amount to be applied per invoice.</param>
        /// <returns>The calculated commission amount for the invoice.</returns>
        private static decimal CalculateInvoiceCommission(CommissionRule rule)
        {
            return rule.FlatAmount.GetValueOrDefault();
        }
    }
}
