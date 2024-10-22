using CommissionCalculator.Core.Entities;
using CommissionCalculator.Core.Enums;
using CommissionCalculator.Core.Interfaces;

namespace CommissionCalculator.Core.Strategies
{
    /// <summary>
    /// Factory class responsible for creating instances of commission calculators
    /// based on the type of commission rule provided.
    /// </summary>
    public class CommissionCalculatorFactory
    {
        /// <summary>
        /// Creates and returns an appropriate commission calculator instance based on the specified commission rule.
        /// </summary>
        /// <param name="rule">The commission rule that determines which type of calculator to create.</param>
        /// <returns>An instance of a class that implements the <see cref="ICommissionCalculator"/> interface, appropriate for the given rule type.</returns>
        /// <exception cref="NotImplementedException">Thrown if the provided commission type is not supported.</exception>
        public ICommissionCalculator CreateCalculator(CommissionRule rule)
        {
            return rule.Type switch
            {
                CommissionType.Flat => new FlatRateCommissionCalculator(),
                CommissionType.Percentage => new PercentageCommissionCalculator(),
                CommissionType.Tiered => new TieredCommissionCalculator(),
                _ => throw new NotImplementedException()
            };
        }
    }
}
