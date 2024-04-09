using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinanceManagement.Api.Domain;

namespace FinanceManagement.Api.Controllers.Interfaces
{
    public interface IIncomeRepository
    {
        Task<IEnumerable<IncomeComplete>> GetAllIncome();
        Task<IncomeComplete> GetIncomeById(Guid id);
        Task<IncomeComplete> AddNewIncome(Income income);
        Task<IncomeComplete> UpdateIncome(Guid id, Income income);
        Task<bool> DeleteIncome(Guid id);
    }
}
