using CommissionCalculator.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace CommissionCalculator.Infrastructure.Context
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<SalesPerson> SalesPeople { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceItem> InvoiceItems { get; set; }
        public DbSet<CommissionRule> CommissionRules { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
            : base(options)
        { 
        }
    }
}
