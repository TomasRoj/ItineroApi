using ItineroApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;

namespace ItineroApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExpensesController : ControllerBase
    {
        private readonly MyContext _context;
        private readonly ILogger<ExpensesController> _logger;

        public ExpensesController(MyContext context, ILogger<ExpensesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/expenses
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Expense>>> GetAll()
        {
            return await _context.Expenses.ToListAsync();
        }

        // GET: api/expenses/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Expense>> Get(int id)
        {
            var expense = await _context.Expenses.FirstOrDefaultAsync(e => e.Id == id);
            if (expense == null)
                return NotFound();
            return expense;
        }

        // GET: api/expenses/trip/5
        [HttpGet("trip/{tripId}")]
        public async Task<ActionResult<IEnumerable<Expense>>> GetByTripId(int tripId)
        {
            return await _context.Expenses
                .Where(e => e.Trip_Id == tripId)
                .OrderByDescending(e => e.Date)
                .ToListAsync();
        }

        // GET: api/expenses/user/
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<Expense>>> GetByUserId(int userId)
        {
            return await _context.Expenses
                .Where(e => e.paid_by_user_id == userId)
                .OrderByDescending(e => e.Date)
                .ToListAsync();
        }

        [HttpGet("category/{categoryId}")]
        public async Task<ActionResult<IEnumerable<Expense>>> GetByCategoryId(int categoryId)
        {
            return await _context.Expenses
                .Where(e => e.Category_Id == categoryId)
                .OrderByDescending(e => e.Date)
                .ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<Expense>> Create([FromBody] Expense expense)
        {
            try
            {
                // Debug logging
                _logger.LogInformation("Received expense payload: {ExpensePayload}",
                    JsonSerializer.Serialize(expense));

                // Map the properties from Angular's naming convention if needed
                // This fixes the mismatch between tripId and Trip_Id
                if (expense.Trip_Id == 0 && Request.Headers.TryGetValue("X-TripId", out var tripIdHeader))
                {
                    if (int.TryParse(tripIdHeader, out int tripId))
                    {
                        expense.Trip_Id = tripId;
                    }
                }

                // Check for null required fields
                if (expense == null || string.IsNullOrEmpty(expense.Name))
                {
                    return BadRequest("Expense name is required");
                }

                // Check for either tripId or Trip_Id
                if (expense.Trip_Id <= 0)
                {
                    return BadRequest("Valid Trip ID is required");
                }

                // Check for paid by user id
                if (expense.paid_by_user_id <= 0)
                {
                    return BadRequest("Valid User ID who paid is required");
                }

                // Set created and updated timestamps
                expense.Created_At = DateTime.UtcNow;
                expense.Updated_At = DateTime.UtcNow;

                _context.Expenses.Add(expense);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(Get), new { id = expense.Id }, expense);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating expense");
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Expense expense)
        {
            if (id != expense.Id)
                return BadRequest();

            expense.Updated_At = DateTime.UtcNow;
            _context.Entry(expense).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Expenses.AnyAsync(e => e.Id == id))
                    return NotFound();
                throw;
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var expense = await _context.Expenses.FindAsync(id);
                if (expense == null)
                    return NotFound();

                var relatedSplits = await _context.ExpenseSplit.Where(s => s.Expense_id == id).ToListAsync();
                _context.ExpenseSplit.RemoveRange(relatedSplits);
                _context.Expenses.Remove(expense);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}