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
    public class IncomeRepository : IIncomeRepository
    {
        private readonly FinanceManagementContext _context;
        private readonly IMemoryCache _cache;

        public IncomeRepository(FinanceManagementContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<IEnumerable<IncomeComplete>> GetAllIncome()
        {
            try
            {
                var cacheKey = "GetAllIncome";
                if (!_cache.TryGetValue(cacheKey, out List<IncomeComplete> incomes))
                {
                    incomes = await _context.Incomes.AsNoTracking().ToListAsync();
                    _cache.Set(cacheKey, incomes);
                }
                return incomes;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in GetAllIncome: {ex.Message}");
                throw; // Rethrow the exception for global error handling
            }
        }

        public async Task<IncomeComplete> GetIncomeById(Guid id)
        {
            try
            {
                if (!_cache.TryGetValue(id, out IncomeComplete income))
                {
                    income = await _context.Incomes.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
                    if (income != null)
                    {
                        _cache.Set(id, income);
                    }
                }
                return income;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in GetIncomeById: {ex.Message}");
                throw; // Rethrow the exception for global error handling
            }
        }

        public async Task<IncomeComplete> AddNewIncome(Income income)
        {
            try
            {
                if (income.OneTimeIncomeDate.HasValue && await _context.Incomes.AnyAsync(x => x.OneTimeIncomeDate == income.OneTimeIncomeDate))
                {
                    throw new ArgumentException("Income cannot be added due to an existing OneTimeIncomeDate");
                }

                var newIncome = new IncomeComplete
                {
                    Id = Guid.NewGuid(),
                    Amount = income.Amount,
                    IncomeType = income.IncomeType,
                    OneTimeIncomeDate = income.OneTimeIncomeDate
                };

                await _context.Incomes.AddAsync(newIncome);
                await _context.SaveChangesAsync();

                _cache.Remove("GetAllIncome");

                return newIncome;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in AddNewIncome: {ex.Message}");
                throw; // Rethrow the exception for global error handling
            }
        }

        public async Task<IncomeComplete> UpdateIncome(Guid id, Income income)
        {
            try
            {
                var incomeToUpdate = await _context.Incomes.FirstOrDefaultAsync(x => x.Id == id);
                if (incomeToUpdate == null)
                {
                    return null;
                }

                if (income.OneTimeIncomeDate.HasValue && await _context.Incomes.AnyAsync(x => x.OneTimeIncomeDate == income.OneTimeIncomeDate && x.Id != id))
                {
                    throw new ArgumentException("Income cannot be updated due to an existing OneTimeIncomeDate");
                }

                incomeToUpdate.IncomeType = income.IncomeType;
                incomeToUpdate.Amount = income.Amount;
                incomeToUpdate.OneTimeIncomeDate = income.OneTimeIncomeDate;

                await _context.SaveChangesAsync();

                _cache.Remove("GetAllIncome");
                _cache.Remove(id);

                return incomeToUpdate;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in UpdateIncome: {ex.Message}");
                throw; // Rethrow the exception for global error handling
            }
        }

        public async Task<bool> DeleteIncome(Guid id)
        {
            try
            {
                var incomeToDelete = await _context.Incomes.FirstOrDefaultAsync(x => x.Id == id);
                if (incomeToDelete == null)
                {
                    return false;
                }

                _context.Remove(incomeToDelete);
                await _context.SaveChangesAsync();

                _cache.Remove("GetAllIncome");
                _cache.Remove(id);

                return true;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in DeleteIncome: {ex.Message}");
                throw; // Rethrow the exception for global error handling
            }
        }
    }
}
