using System.ComponentModel.DataAnnotations;

namespace CommissionCalculator.Core.Entities
{
    public class Invoice
    {
        public int Id { get; set; }
        
        [Required]
        public int SalesPersonId { get; set; }

        [Required]
        public SalesPerson SalesPerson { get; set; }

        public ICollection<InvoiceItem> Items { get; set; }

        public decimal? TotalCommission { get; set; }
    }
}
