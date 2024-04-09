using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinanceManagement.Api.Controllers.Interfaces;
using FinanceManagement.Api.Database;
using FinanceManagement.Api.Domain;
using FinanceManagement.Api.Domain.Dto;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace FinanceManagement.Api.Repositories
{
    public class ExpenseRepository : IExpenseRepository
    {
        private readonly FinanceManagementContext _context;
        private readonly IMemoryCache _cache;

        public ExpenseRepository(FinanceManagementContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<IEnumerable<ExpenseDto>> GetExpenses()
        {
            try
            {
                var cacheKey = "GetExpenses";
                if (!_cache.TryGetValue(cacheKey, out List<ExpenseDto> expenseDtos))
                {
                    var expenses = await _context.Expenses.ToListAsync();
                    expenseDtos = expenses
                        .Select(e => new ExpenseDto
                        {
                            ExpenseType = e.ExpenseType,
                            Amount = e.Amount,
                            OneTimeExpenseDate = e.OneTimeExpenseDate ?? DateTime.MinValue
                        })
                        .ToList();

                    _cache.Set(cacheKey, expenseDtos);
                }
                return expenseDtos;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in GetExpenses: {ex.Message}");
                throw; // Rethrow the exception for global error handling
            }
        }

        public async Task<Expense> GetExpenseById(Guid id)
        {
            try
            {
                // Validate input parameter
                if (id == Guid.Empty)
                {
                    throw new ArgumentException("Expense id cannot be empty.", nameof(id));
                }

                var expense = await _context.Expenses.FirstOrDefaultAsync(x => x.Id == id);
                return expense;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in GetExpenseById: {ex.Message}");
                throw; // Rethrow the exception for global error handling
            }
        }

        public async Task<Expense> AddNewExpense(Expense expense)
        {
            try
            {
                expense.Id = Guid.NewGuid();
                await _context.Expenses.AddAsync(expense);
                await _context.SaveChangesAsync();

                _cache.Remove("GetExpenses");

                return expense;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in AddNewExpense: {ex.Message}");
                throw; // Rethrow the exception for global error handling
            }
        }

        public async Task<Expense> UpdateExpense(Guid id, Expense expense)
        {
            try
            {
                var expenseToUpdate = await _context.Expenses.FirstOrDefaultAsync(x => x.Id == id);
                if (expenseToUpdate == null)
                {
                    return null;
                }

                expenseToUpdate.ExpenseType = expense.ExpenseType;
                expenseToUpdate.Amount = expense.Amount;
                expenseToUpdate.OneTimeExpenseDate = expense.OneTimeExpenseDate;

                _context.Expenses.Update(expenseToUpdate);
                await _context.SaveChangesAsync();

                _cache.Remove("GetExpenses");
                _cache.Remove(id);

                return expenseToUpdate;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in UpdateExpense: {ex.Message}");
                throw; // Rethrow the exception for global error handling
            }
        }

        public async Task<bool> DeleteExpense(Guid id)
        {
            try
            {
                var expenseToDelete = await _context.Expenses.FirstOrDefaultAsync(x => x.Id == id);
                if (expenseToDelete == null)
                {
                    return false;
                }

                _context.Remove(expenseToDelete);
                await _context.SaveChangesAsync();

                _cache.Remove("GetExpenses");
                _cache.Remove(id);

                return true;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in DeleteExpense: {ex.Message}");
                throw; // Rethrow the exception for global error handling
            }
        }
    }
}
