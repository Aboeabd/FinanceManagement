using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinanceManagement.Api.Controllers.Interfaces;
using FinanceManagement.Api.Database;
using FinanceManagement.Api.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Caching.Memory;

namespace FinanceManagement.Api.Controllers
{
    [ApiController]
    [Route("api/income")]
    [Authorize]
    public class IncomeController : ControllerBase
    {
        private readonly IIncomeRepository _incomeRepository;

        public IncomeController(IIncomeRepository incomeRepository)
        {
            _incomeRepository = incomeRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<IncomeComplete>>> GetAllIncome()
        {
            var incomes = await _incomeRepository.GetAllIncome();
            return Ok(incomes);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<IncomeComplete>> GetIncomeById(Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var income = await _incomeRepository.GetIncomeById(id);
            if (income == null)
            {
                return NotFound();
            }
            return Ok(income);
        }

        [HttpPost]
        public async Task<ActionResult<IncomeComplete>> AddNewIncome([FromBody] Income income)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newIncome = await _incomeRepository.AddNewIncome(income);
            return CreatedAtAction(nameof(GetIncomeById), new { id = newIncome.Id }, newIncome);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<IncomeComplete>> UpdateIncome(
            Guid id,
            [FromBody] Income income
        )
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updatedIncome = await _incomeRepository.UpdateIncome(id, income);
            if (updatedIncome == null)
            {
                return NotFound();
            }
            return Ok(updatedIncome);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteIncome(Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var deleted = await _incomeRepository.DeleteIncome(id);
            if (!deleted)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
