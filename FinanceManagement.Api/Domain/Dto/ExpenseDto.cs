using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace FinanceManagement.Api.Domain.Dto
{
    // We're just going to assume we don't want to expose the Id of the expense

    public class ExpenseDto
    {
        [Required(ErrorMessage = "ExpenseType is required")]
        public string ExpenseType { get; set; }

        [Range(0, (double)decimal.MaxValue, ErrorMessage = "Amount must be greater than or equal to 0")]
        public decimal Amount { get; set; }

        public DateTime OneTimeExpenseDate { get; set; }
    }
}
