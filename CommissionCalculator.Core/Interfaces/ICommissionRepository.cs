using CommissionCalculator.Core.Entities;

namespace CommissionCalculator.Core.Interfaces
{
    public interface ICommissionRepository
    {
        Task<Invoice?> GetInvoice(int invoiceId);
        Task<List<CommissionRule>> GetCommissionRules(int salesPersonId);
        void SaveTotalCommission(Invoice invoice, decimal totalCommission);
    }
}
