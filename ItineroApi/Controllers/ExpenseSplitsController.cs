using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ItineroApi.Models;
using ItineroApi.Data;

namespace ItineroApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExpenseSplitsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ExpenseSplitsController(DbContext context)
        {
            _context = context;
        }

        // GET: api/ExpenseSplits
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExpenseSplit>>> GetExpenseSplits()
        {
            return await _context.ExpenseSplits.ToListAsync();
        }

        // GET: api/ExpenseSplits/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ExpenseSplit>> GetExpenseSplit(int id)
        {
            var expenseSplit = await _context.ExpenseSplits.FindAsync(id);

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
            return await _context.ExpenseSplits
                .Where(s => s.Expense_id == expenseId)
                .ToListAsync();
        }

        // POST: api/ExpenseSplits
        [HttpPost]
        public async Task<ActionResult<ExpenseSplit>> Create(ExpenseSplit split)
        {
            // Check if the expense exists
            var expense = await _context.Expenses.FindAsync(split.Expense_id);
            if (expense == null)
            {
                return BadRequest($"Expense with ID {split.Expense_id} does not exist");
            }

            // Ensure Id is 0 so the database will auto-assign it
            split.Id = 0;

            _context.ExpenseSplits.Add(split);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                // Log the exception details
                Console.WriteLine($"Error creating expense split: {ex.Message}");
                Console.WriteLine($"Inner exception: {ex.InnerException?.Message}");

                return StatusCode(500, $"Failed to create expense split: {ex.InnerException?.Message ?? ex.Message}");
            }

            return CreatedAtAction(nameof(GetExpenseSplit), new { id = split.Id }, split);
        }

        // POST: api/ExpenseSplits/CreateForExpense/5
        [HttpPost("CreateForExpense/{expenseId}")]
        public async Task<ActionResult<ExpenseSplit>> CreateForExpense(int expenseId, [FromBody] CreateExpenseSplitRequest request)
        {
            // Check if the expense exists
            var expense = await _context.Expenses.FindAsync(expenseId);
            if (expense == null)
            {
                return BadRequest($"Expense with ID {expenseId} does not exist");
            }

            // Create a new ExpenseSplit object
            var split = new ExpenseSplit
            {
                Id = 0, // Will be assigned by database
                Expense_id = expenseId,
                User_Id = request.UserId,
                Amount = request.Amount,
                Is_Settled = request.IsSettled ?? false,
                Settled_At = request.IsSettled == true ? DateTime.Now : null,
                trip_ID = expense.trip_id // Use the trip_id from the expense
            };

            _context.ExpenseSplits.Add(split);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                // Log the exception details
                Console.WriteLine($"Error creating expense split: {ex.Message}");
                Console.WriteLine($"Inner exception: {ex.InnerException?.Message}");

                return StatusCode(500, $"Failed to create expense split: {ex.InnerException?.Message ?? ex.Message}");
            }

            return CreatedAtAction(nameof(GetExpenseSplit), new { id = split.Id }, split);
        }

        // POST: api/ExpenseSplits/CreateMultipleForExpense/5
        [HttpPost("CreateMultipleForExpense/{expenseId}")]
        public async Task<ActionResult<IEnumerable<ExpenseSplit>>> CreateMultipleForExpense(int expenseId, [FromBody] CreateMultipleExpenseSplitsRequest request)
        {
            // Check if the expense exists
            var expense = await _context.Expenses.FindAsync(expenseId);
            if (expense == null)
            {
                return BadRequest($"Expense with ID {expenseId} does not exist");
            }

            if (request.UserIds == null || !request.UserIds.Any())
            {
                return BadRequest("At least one user must be specified");
            }

            // Create splits for each user
            var splits = new List<ExpenseSplit>();

            foreach (var userId in request.UserIds)
            {
                // Calculate amount based on split type
                decimal amount;
                if (request.SplitType == "equal")
                {
                    // Equal split
                    amount = Math.Round(request.TotalAmount / request.UserIds.Count, 2);
                }
                else if (request.UserAmounts != null && request.UserAmounts.TryGetValue(userId.ToString(), out var userAmount))
                {
                    // Custom split - use the specified amount for this user
                    amount = userAmount;
                }
                else
                {
                    // Default to equal split if no amount specified
                    amount = Math.Round(request.TotalAmount / request.UserIds.Count, 2);
                }

                var split = new ExpenseSplit
                {
                    Id = 0,
                    Expense_id = expenseId,
                    User_Id = userId,
                    Amount = amount,
                    Is_Settled = false,
                    Settled_At = null,
                    trip_ID = expense.trip_id
                };

                splits.Add(split);
            }

            // Add all splits to the context
            _context.ExpenseSplits.AddRange(splits);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                // Log the exception details
                Console.WriteLine($"Error creating expense splits: {ex.Message}");
                Console.WriteLine($"Inner exception: {ex.InnerException?.Message}");

                return StatusCode(500, $"Failed to create expense splits: {ex.InnerException?.Message ?? ex.Message}");
            }

            return CreatedAtAction(nameof(GetExpenseSplitsByExpenseId), new { expenseId }, splits);
        }

        // PUT: api/ExpenseSplits/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ExpenseSplit split)
        {
            if (id != split.Id)
            {
                return BadRequest("ID mismatch");
            }

            // Check if the expense exists
            var expense = await _context.Expenses.FindAsync(split.Expense_id);
            if (expense == null)
            {
                return BadRequest($"Expense with ID {split.Expense_id} does not exist");
            }

            _context.Entry(split).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ExpenseSplitExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"Failed to update expense split: {ex.InnerException?.Message ?? ex.Message}");
            }

            return NoContent();
        }

        // DELETE: api/ExpenseSplits/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var expenseSplit = await _context.ExpenseSplits.FindAsync(id);
            if (expenseSplit == null)
            {
                return NotFound();
            }

            _context.ExpenseSplits.Remove(expenseSplit);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ExpenseSplitExists(int id)
        {
            return _context.ExpenseSplits.Any(e => e.Id == id);
        }
    }

    // Request models
    public class CreateExpenseSplitRequest
    {
        public int UserId { get; set; }
        public decimal Amount { get; set; }
        public bool? IsSettled { get; set; }
    }

    public class CreateMultipleExpenseSplitsRequest
    {
        public List<int> UserIds { get; set; }
        public decimal TotalAmount { get; set; }
        public string SplitType { get; set; } = "equal"; // "equal" or "custom"
        public Dictionary<string, decimal> UserAmounts { get; set; } // Used for custom splits
    }
}