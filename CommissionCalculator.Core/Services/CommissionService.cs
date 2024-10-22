using CommissionCalculator.Core.Entities;
using CommissionCalculator.Core.Enums;
using CommissionCalculator.Core.Interfaces;
using CommissionCalculator.Core.Strategies;

namespace CommissionCalculator.Core.Services
{
    public class CommissionService : ICommissionService
    {
        private readonly ICommissionRepository _commissionRepository;
        private readonly CommissionCalculatorFactory _calculatorFactory;

        public CommissionService(ICommissionRepository commissionRepository, CommissionCalculatorFactory calculatorFactory)
        {
            _commissionRepository = commissionRepository;
            _calculatorFactory = calculatorFactory;
        }

        public async Task<decimal> CalculateTotalCommission(int invoiceId)
        {
            var invoice = await _commissionRepository.GetInvoice(invoiceId);

            if (invoice == null)
            {
                throw new ArgumentException("Invalid invoice ID");
            }

            decimal totalCommission = 0;
            var commissionRules = await _commissionRepository.GetCommissionRules(invoice.SalesPersonId);

            foreach (var rule in commissionRules)
            {
                var calculator = _calculatorFactory.CreateCalculator(rule);

                if (rule.Level == CommissionLevel.Invoice)
                {
                    // Calculate commission for the entire invoice (only once)
                    var commission = calculator.Calculate(null, invoice, rule);
                    totalCommission += commission;
                }
                else
                {
                    // Calculate commission for each invoice item
                    foreach (var invoiceItem in invoice.Items)
                    {
                        var commission = calculator.Calculate(invoiceItem, invoice, rule);
                        totalCommission += commission;
                    }
                }
            }

            // Apply any caps
            totalCommission = ApplyCommissionCaps(invoice, totalCommission, commissionRules);

            _commissionRepository.SaveTotalCommission(invoice, totalCommission);

            return totalCommission;
        }

        private decimal ApplyCommissionCaps(Invoice invoice, decimal totalCommission, IEnumerable<CommissionRule> rules)
        {
            foreach (var rule in rules)
            {
                if (rule.CapAmount.HasValue)
                {
                    totalCommission = Math.Min(totalCommission, rule.CapAmount.Value);
                }
                else if(rule.CapPercentage.HasValue)
                {
                    var maxCommission = invoice.Items.Sum(i => i.Quantity * i.Product.Price) * (rule.CapPercentage.GetValueOrDefault() / 100);
                    totalCommission = Math.Min(totalCommission, maxCommission);
                }
            }
            return totalCommission;
        }
    }
}
