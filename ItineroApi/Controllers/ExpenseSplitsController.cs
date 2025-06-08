using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ItineroApi.Models;

namespace ItineroApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExpenseSplitsController : ControllerBase
    {
        private readonly MyContext _context;

        public ExpenseSplitsController(MyContext context)
        {
            _context = context;
        }

        // GET: api/ExpenseSplits
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExpenseSplit>>> GetExpenseSplits()
        {
            return await _context.ExpenseSplit.ToListAsync();
        }

        // GET: api/ExpenseSplits/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ExpenseSplit>> GetExpenseSplit(int id)
        {
            var expenseSplit = await _context.ExpenseSplit.FindAsync(id);

            if (expenseSplit == null)
            {
                return NotFound();
            }

            return expenseSplit;
        }

        // GET: api/ExpenseSplits/expense/5
        [HttpGet("expense/{expenseId}")]
        public async Task<ActionResult<IEnumerable<ExpenseSplit>>> GetExpenseSplitsByExpenseId(int expenseId)
        {
            return await _context.ExpenseSplit
                .Where(s => s.Expense_id == expenseId)
                .ToListAsync();
        }

        // GET: api/ExpenseSplits/user/5
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<ExpenseSplit>>> GetExpenseSplitsByUserId(int userId)
        {
            return await _context.ExpenseSplit
                .Where(s => s.User_Id == userId)
                .ToListAsync();
        }

        // GET: api/ExpenseSplits/trip/5
        [HttpGet("trip/{tripId}")]
        public async Task<ActionResult<IEnumerable<ExpenseSplit>>> GetExpenseSplitsByTripId(int tripId)
        {
            return await _context.ExpenseSplit
                .Where(s => s.trip_ID == tripId)
                .ToListAsync();
        }

        // POST: api/ExpenseSplits/CreateMultipleForExpense/5
        [HttpPost("CreateMultipleForExpense/{expenseId}")]
        public async Task<ActionResult<IEnumerable<ExpenseSplit>>> CreateMultipleForExpense(int expenseId, [FromBody] CreateMultipleExpenseSplitsRequest request)
        {
            // Validate that the expense exists
            var expense = await _context.Expenses.FindAsync(expenseId);
            if (expense == null)
            {
                return BadRequest($"Expense with ID {expenseId} does not exist");
            }

            if (request.UserIds == null || !request.UserIds.Any())
            {
                return BadRequest("At least one user must be specified");
            }

            var splits = new List<ExpenseSplit>();

            foreach (var userId in request.UserIds)
            {
                decimal amount;
                if (request.SplitType?.ToLower() == "equal")
                {
                    amount = Math.Round(request.TotalAmount / request.UserIds.Count, 2);
                }
                else if (request.UserAmounts != null && request.UserAmounts.TryGetValue(userId.ToString(), out var userAmount))
                {
                    amount = userAmount;
                }
                else
                {
                    amount = Math.Round(request.TotalAmount / request.UserIds.Count, 2);
                }

                var split = new ExpenseSplit
                {
                    Expense_id = expenseId,
                    User_Id = userId,
                    Amount = amount,
                    Is_Settled = false,
                    Settled_At = null,
                    trip_ID = expense.Trip_Id
                };

                splits.Add(split);
            }

            _context.ExpenseSplit.AddRange(splits);

            try
            {
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetExpenseSplitsByExpenseId), new { expenseId }, splits);
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"Failed to create expense splits: {ex.InnerException?.Message ?? ex.Message}");
            }
        }

        // PUT: api/ExpenseSplits/SettleExpense/5
        [HttpPut("SettleExpense/{expenseId}")]
        public async Task<IActionResult> SettleExpenseSplits(int expenseId)
        {
            var splits = await _context.ExpenseSplit
                .Where(s => s.Expense_id == expenseId && !s.Is_Settled)  // Find UNSETTLED splits
                .ToListAsync();

            if (!splits.Any())
            {
                return BadRequest("No unsettled splits found for this expense");
            }

            foreach (var split in splits)
            {
                split.Is_Settled = true;
                split.Settled_At = DateTime.UtcNow;
            }

            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"Failed to settle expense splits: {ex.InnerException?.Message ?? ex.Message}");
            }
        }

        // PUT: api/ExpenseSplits/UnsettleExpense/5
        [HttpPut("UnsettleExpense/{expenseId}")]
        public async Task<IActionResult> UnsettleExpenseSplits(int expenseId)
        {
            var splits = await _context.ExpenseSplit
                .Where(s => s.Expense_id == expenseId && s.Is_Settled)  // Find SETTLED splits
                .ToListAsync();

            if (!splits.Any())
            {
                return BadRequest("No settled splits found for this expense");
            }

            foreach (var split in splits)
            {
                split.Is_Settled = false;
                split.Settled_At = null;
            }

            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"Failed to unsettle expense splits: {ex.InnerException?.Message ?? ex.Message}");
            }
        }

        // PUT: api/ExpenseSplits/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateExpenseSplit(int id, ExpenseSplit split)
        {
            if (id != split.Id)
            {
                return BadRequest("ID mismatch");
            }

            // Validate that the expense exists
            var expenseExists = await _context.Expenses.AnyAsync(e => e.Id == split.Expense_id);
            if (!expenseExists)
            {
                return BadRequest($"Expense with ID {split.Expense_id} does not exist");
            }

            _context.Entry(split).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ExpenseSplitExists(id))
                {
                    return NotFound();
                }
                throw;
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"Failed to update expense split: {ex.InnerException?.Message ?? ex.Message}");
            }
        }

        // DELETE: api/ExpenseSplits/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExpenseSplit(int id)
        {
            var expenseSplit = await _context.ExpenseSplit.FindAsync(id);
            if (expenseSplit == null)
            {
                return NotFound();
            }

            _context.ExpenseSplit.Remove(expenseSplit);

            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"Failed to delete expense split: {ex.InnerException?.Message ?? ex.Message}");
            }
        }

        private bool ExpenseSplitExists(int id)
        {
            return _context.ExpenseSplit.Any(e => e.Id == id);
        }

    }

    public class CreateMultipleExpenseSplitsRequest
    {
        public List<int> UserIds { get; set; } = new List<int>();
        public decimal TotalAmount { get; set; }
        public string SplitType { get; set; } = "equal";
        public Dictionary<string, decimal>? UserAmounts { get; set; }
    }
}