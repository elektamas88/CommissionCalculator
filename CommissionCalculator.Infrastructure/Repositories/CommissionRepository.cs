using CommissionCalculator.Core.Entities;
using CommissionCalculator.Core.Interfaces;
using CommissionCalculator.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace CommissionCalculator.Infrastructure.Repositories
{
    public class CommissionRepository : ICommissionRepository
    {
        private readonly ApplicationDbContext _context;

        public CommissionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get invoice by id
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <returns></returns>
        public Task<Invoice?> GetInvoice(int invoiceId)
        {
            return _context.Invoices
                .Include(i => i.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(i => i.Id == invoiceId);
        }

        /// <summary>
        /// Get commission rules for specific Sales Person plus get all default commission rules (SalesPersonId is not set)
        /// </summary>
        /// <param name="salesPersonId"></param>
        /// <returns></returns>
        public Task<List<CommissionRule>> GetCommissionRules(int salesPersonId)
        {
            var rules = _context.CommissionRules
                .Where(r => r.SalesPersonId == salesPersonId || r.SalesPersonId == null)
                .ToListAsync();

            return rules;
        }

        /// <summary>
        /// Save changes
        /// </summary>
        /// <param name="invoice"></param>
        /// <param name="totalCommission"></param>
        public void SaveTotalCommission(Invoice invoice, decimal totalCommission)
        {
            invoice.TotalCommission = totalCommission;

            _context.SaveChangesAsync();
        }
    }
}
