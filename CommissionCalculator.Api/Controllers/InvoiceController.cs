using CommissionCalculator.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CommissionCalculator.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InvoiceController : ControllerBase
    {
        private readonly ICommissionService _commissionService;

        public InvoiceController(ICommissionService commissionService)
        {
            _commissionService = commissionService;
        }

        [HttpPost("calculate/{invoiceId}")]
        public async Task<ActionResult<int>> CalculateCommission(int invoiceId)
        {
            var totalCommission = await _commissionService.CalculateTotalCommission(invoiceId);

            return Ok(totalCommission);
        }
    }
}
