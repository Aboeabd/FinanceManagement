using System.ComponentModel.DataAnnotations;

namespace FinanceManagement.Api.Domain
{
    public class Income
    {
        [Required(ErrorMessage = "IncomeType is required")]
        public IncomeType IncomeType { get; set; }

        [Range(0, (double)decimal.MaxValue, ErrorMessage = "Amount must be greater than or equal to 0")]
        public decimal Amount { get; set; }

        public DateTime? OneTimeIncomeDate { get; set; }
    }
}
