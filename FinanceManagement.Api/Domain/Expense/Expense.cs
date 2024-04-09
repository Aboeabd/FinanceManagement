using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceManagement.Api.Domain
{
    public class Expense
    {
        public Guid Id { get; set; }

        [Range(0, (double)decimal.MaxValue, ErrorMessage = "Amount must be greater than or equal to 0")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "ExpenseType is required")]
        public string ExpenseType { get; set; }

        public DateTime? OneTimeExpenseDate { get; set; }
    }
}
