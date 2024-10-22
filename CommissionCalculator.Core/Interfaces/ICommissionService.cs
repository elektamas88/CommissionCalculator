namespace CommissionCalculator.Core.Interfaces
{
    public interface ICommissionService
    {
        Task<decimal> CalculateTotalCommission(int invoiceId);
    }
}
