using CommissionCalculator.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommissionCalculator.Core.Interfaces
{
    public interface ICommissionCalculator
    {
        decimal Calculate(InvoiceItem invoiceItem, Invoice invoice, CommissionRule rule);
    }
}
