using System;
using FinanceManagement.Api.Domain;
using FinanceManagement.Api.Domain.Dto;

namespace FinanceManagement.Api.Controllers.Interfaces
{
    public interface IExpenseRepository
    {
        Task<IEnumerable<ExpenseDto>> GetExpenses();
        Task<Expense> GetExpenseById(Guid id);
        Task<Expense> AddNewExpense(Expense expense);
        Task<Expense> UpdateExpense(Guid id, Expense expense);
        Task<bool> DeleteExpense(Guid id);
    }
}
