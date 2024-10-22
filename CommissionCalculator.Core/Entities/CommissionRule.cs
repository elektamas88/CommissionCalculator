using CommissionCalculator.Core.Enums;
namespace CommissionCalculator.Core.Entities
{
    public class CommissionRule
    {
        public int Id { get; set; }
        public int? SalesPersonId { get; set; }
        public SalesPerson SalesPerson { get; set; }
        public int? ProductId { get; set; }
        public CommissionType Type { get; set; }
        public CommissionLevel Level { get; set; }
        public decimal? FlatAmount { get; set; }
        public decimal? Percentage { get; set; }
        public int? MinQuantity { get; set; }
        public int? MaxQuantity { get; set; }
        public decimal? CapAmount { get; set; }
        public decimal? CapPercentage { get; set; }
    }
}
