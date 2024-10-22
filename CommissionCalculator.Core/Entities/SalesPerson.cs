namespace CommissionCalculator.Core.Entities
{
    public class SalesPerson
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Invoice> Invoices { get; set; }
        public ICollection<CommissionRule> CommissionRules { get; set; }
    }
}
