namespace CommissionCalculator.Core.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public ICollection<InvoiceItem> InvoiceItems { get; set; }
    }
}
