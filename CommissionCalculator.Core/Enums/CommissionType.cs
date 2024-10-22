namespace CommissionCalculator.Core.Enums
{
    public enum CommissionType
    {
        /// <summary>
        /// A set amount. This can be applied to either an Invoice, Product, or multiples of a Product.
        /// </summary>
        Flat = 1,

        /// <summary>
        /// A percentage of the price of the Product or the total of the Invoice.
        /// </summary>
        Percentage = 2,

        /// <summary>
        /// Can be based on either the multiples of a Product sold, or the total value of the Product
        /// multiples, or the total of the Invoice.
        /// </summary>
        Tiered = 3
    }
}
