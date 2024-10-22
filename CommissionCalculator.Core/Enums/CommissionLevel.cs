namespace CommissionCalculator.Core.Enums
{
    public enum CommissionLevel
    {
        /// <summary>
        /// Commission applied per unit of a specific product sold.
        /// </summary>
        Product = 1,

        /// <summary>
        /// Commission applied based on multiples or batches of a specific product sold.
        /// For example, a fixed amount per batch of 10 units.
        /// </summary>
        MultiplesOfProduct = 2,

        /// <summary>
        /// Commission applied based on the total value of multiples of a specific product sold.
        /// For example, a fixed amount for every $100 worth of a product sold.
        /// </summary>
        MultiplesOfProductValues = 3,

        /// <summary>
        /// Commission applied per invoice, regardless of the products or quantities included in the invoice.
        /// </summary>
        Invoice = 4,
    }
}
