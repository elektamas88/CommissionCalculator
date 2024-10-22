using CommissionCalculator.Core.Entities;
using CommissionCalculator.Core.Interfaces;

namespace CommissionCalculator.Core.Strategies
{
    /// <summary>
    /// Calculator for determining commission based on a percentage rate.
    /// The commission is calculated based on different levels: Product or Invoice.
    /// This calculator supports commission rules that apply a percentage of the product price or the total invoice value.
    /// </summary>
    public class PercentageCommissionCalculator : ICommissionCalculator
    {
        /// <summary>
        /// Calculates the commission based on a percentage of either the price of the Product or the total of the Invoice,
        /// depending on the specified commission level in the rule.
        /// </summary>
        /// <param name="invoiceItem">The specific item in the invoice for which the commission is being calculated.</param>
        /// <param name="invoice">The invoice that contains the items sold.</param>
        /// <param name="rule">The commission rule that specifies the percentage and the level (Product or Invoice).</param>
        /// <returns>The calculated commission amount based on the rule.</returns>
        /// <exception cref="Exception">Thrown when an invalid commission level is provided for the percentage commission rule.</exception>

        public decimal Calculate(InvoiceItem invoiceItem, Invoice invoice, CommissionRule rule)
        {
            switch (rule.Level)
            {
                case Enums.CommissionLevel.Product:
                    return CalculateProductCommission(invoiceItem, rule);
                case Enums.CommissionLevel.Invoice:
                    return CalculateInvoiceCommission(invoice, rule);
                default:
                    throw new Exception("Invalid rule level for the percentage commission rule.");
            }
        }

        /// <summary>
        /// Calculates the commission for a SalesPerson as a percentage of the price of a specific product in the invoice.
        /// </summary>
        /// <param name="invoiceItem">The specific item in the invoice for which the commission is being calculated.</param>
        /// <param name="rule">The commission rule that specifies the percentage to be applied at the product level.</param>
        /// <returns>The calculated commission amount for the product.</returns>
        private decimal CalculateProductCommission(InvoiceItem invoiceItem, CommissionRule rule)
        {
            return invoiceItem.Quantity * invoiceItem.Product.Price * (rule.Percentage.GetValueOrDefault() / 100);
        }

        /// <summary>
        /// Calculates the commission for a SalesPerson as a percentage of the total value of the entire invoice.
        /// </summary>
        /// <param name="invoice">The invoice that contains the items sold.</param>
        /// <param name="rule">The commission rule that specifies the percentage to be applied at the invoice level.</param>
        /// <returns>The calculated commission amount for the entire invoice.</returns>
        private decimal CalculateInvoiceCommission(Invoice invoice, CommissionRule rule)
        {
            var totalInvoiceAmount = invoice.Items.Sum(i => i.Quantity * i.Product.Price);
            return totalInvoiceAmount * (rule.Percentage.GetValueOrDefault() / 100);
        }
    }
}
