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
            try
            {
                return await _context.Expenses
                    .OrderByDescending(e => e.Date)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all expenses");
                return StatusCode(500, "An error occurred while retrieving expenses");
            }
        }
        

        // GET: api/expenses/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Expense>> Get(int id)
        {
            try
            {
                var expense = await _context.Expenses.FindAsync(id);
                if (expense == null)
                {
                    return NotFound($"Expense with ID {id} not found");
                }
                return expense;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving expense with ID {ExpenseId}", id);
                return StatusCode(500, "An error occurred while retrieving the expense");
            }
        }

        // GET: api/expenses/trip/5
        [HttpGet("trip/{tripId}")]
        public async Task<ActionResult<IEnumerable<Expense>>> GetByTripId(int tripId)
        {
            try
            {
                // Validate that the trip exists
                var tripExists = await _context.Trips.AnyAsync(t => t.Id == tripId);
                if (!tripExists)
                {
                    return BadRequest($"Trip with ID {tripId} does not exist");
                }

                return await _context.Expenses
                    .Where(e => e.Trip_Id == tripId)
                    .OrderByDescending(e => e.Date)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving expenses for trip {TripId}", tripId);
                return StatusCode(500, "An error occurred while retrieving trip expenses");
            }
        }

        // GET: api/expenses/user/5
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<Expense>>> GetByUserId(int userId)
        {
            try
            {
                return await _context.Expenses
                    .Where(e => e.paid_by_user_id == userId)
                    .OrderByDescending(e => e.Date)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving expenses for user {UserId}", userId);
                return StatusCode(500, "An error occurred while retrieving user expenses");
            }
        }

        // GET: api/expenses/category/5
        [HttpGet("category/{categoryId}")]
        public async Task<ActionResult<IEnumerable<Expense>>> GetByCategoryId(int categoryId)
        {
            try
            {
                return await _context.Expenses
                    .Where(e => e.Category_Id == categoryId)
                    .OrderByDescending(e => e.Date)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving expenses for category {CategoryId}", categoryId);
                return StatusCode(500, "An error occurred while retrieving category expenses");
            }
        }

        // POST: api/expenses
        [HttpPost]
        [ProducesResponseType(typeof(Expense), StatusCodes.Status201Created)]
        public async Task<ActionResult<Expense>> Create([FromBody] CreateExpenseRequest request)
        {
            try
            {
                _logger.LogInformation("Received expense creation request: {RequestPayload}",
                    JsonSerializer.Serialize(request));

                // Validate required fields
                if (request == null)
                {
                    return BadRequest("Request body is required");
                }

                if (string.IsNullOrWhiteSpace(request.Name))
                {
                    return BadRequest("Expense name is required");
                }

                if (request.Trip_Id <= 0)
                {
                    return BadRequest("Valid Trip ID is required");
                }

                if (request.paid_by_user_id <= 0)
                {
                    return BadRequest("Valid User ID who paid is required");
                }

                if (request.Amount <= 0)
                {
                    return BadRequest("Amount must be greater than 0");
                }

                if (string.IsNullOrWhiteSpace(request.Currency_Code))
                {
                    return BadRequest("Currency code is required");
                }

                // Validate that the trip exists
                var tripExists = await _context.Trips.AnyAsync(t => t.Id == request.Trip_Id);
                if (!tripExists)
                {
                    return BadRequest($"Trip with ID {request.Trip_Id} does not exist");
                }

                // Validate that the user exists
                var userExists = await _context.Users.AnyAsync(u => u.Id == request.paid_by_user_id);
                if (!userExists)
                {
                    return BadRequest($"User with ID {request.paid_by_user_id} does not exist");
                }

                // Validate category if provided
                if (request.Category_Id.HasValue && request.Category_Id > 0)
                {
                    var categoryExists = await _context.ExpenseCategory.AnyAsync(c => c.id == request.Category_Id);
                    if (!categoryExists)
                    {
                        return BadRequest($"Category with ID {request.Category_Id} does not exist");
                    }
                }

                // Create the expense entity
                var expense = new Expense
                {
                    Name = request.Name.Trim(),
                    Trip_Id = request.Trip_Id,
                    Category_Id = request.Category_Id,
                    paid_by_user_id = request.paid_by_user_id,
                    Amount = request.Amount,
                    Currency_Code = request.Currency_Code.Trim().ToUpper(),
                    Description = request.Description?.Trim() ?? string.Empty,
                    Date = request.Date,
                    Receipt_image = request.Receipt_image?.Trim(),
                    Created_At = DateTime.UtcNow,
                    Updated_At = DateTime.UtcNow
                };

                _context.Expenses.Add(expense);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Created expense with ID {ExpenseId}", expense.Id);

                return CreatedAtAction(nameof(Get), new { id = expense.Id }, expense);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error creating expense");
                return StatusCode(500, $"Failed to create expense: {ex.InnerException?.Message ?? ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating expense");
                return StatusCode(500, "An unexpected error occurred while creating the expense");
            }
        }

        // PUT: api/expenses/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateExpenseRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest("Request body is required");
                }

                var existingExpense = await _context.Expenses.FindAsync(id);
                if (existingExpense == null)
                {
                    return NotFound($"Expense with ID {id} not found");
                }

                // Validate required fields
                if (string.IsNullOrWhiteSpace(request.Name))
                {
                    return BadRequest("Expense name is required");
                }

                if (request.Amount <= 0)
                {
                    return BadRequest("Amount must be greater than 0");
                }

                // Validate category if provided
                if (request.Category_Id.HasValue && request.Category_Id > 0)
                {
                    var categoryExists = await _context.ExpenseCategory.AnyAsync(c => c.id == request.Category_Id);
                    if (!categoryExists)
                    {
                        return BadRequest($"Category with ID {request.Category_Id} does not exist");
                    }
                }

                // Update the expense
                existingExpense.Name = request.Name.Trim();
                existingExpense.Category_Id = request.Category_Id;
                existingExpense.Amount = request.Amount;
                existingExpense.Currency_Code = request.Currency_Code?.Trim().ToUpper() ?? existingExpense.Currency_Code;
                existingExpense.Description = request.Description?.Trim() ?? string.Empty;
                existingExpense.Date = request.Date;
                existingExpense.Receipt_image = request.Receipt_image?.Trim();
                existingExpense.Updated_At = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Expenses.AnyAsync(e => e.Id == id))
                {
                    return NotFound($"Expense with ID {id} not found");
                }
                throw;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error updating expense {ExpenseId}", id);
                return StatusCode(500, $"Failed to update expense: {ex.InnerException?.Message ?? ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating expense {ExpenseId}", id);
                return StatusCode(500, "An unexpected error occurred while updating the expense");
            }
        }

        // DELETE: api/expenses/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var expense = await _context.Expenses.FindAsync(id);
                if (expense == null)
                {
                    return NotFound($"Expense with ID {id} not found");
                }

                // Delete related splits first (cascade delete)
                var relatedSplits = await _context.ExpenseSplit
                    .Where(s => s.Expense_id == id)
                    .ToListAsync();

                if (relatedSplits.Any())
                {
                    _context.ExpenseSplit.RemoveRange(relatedSplits);
                    _logger.LogInformation("Deleted {SplitCount} expense splits for expense {ExpenseId}",
                        relatedSplits.Count, id);
                }

                _context.Expenses.Remove(expense);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Deleted expense with ID {ExpenseId}", id);

                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error deleting expense {ExpenseId}", id);
                return StatusCode(500, $"Failed to delete expense: {ex.InnerException?.Message ?? ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting expense {ExpenseId}", id);
                return StatusCode(500, "An unexpected error occurred while deleting the expense");
            }
        }
    }

    // Request DTOs to handle the property mapping issues
    public class CreateExpenseRequest
    {
        public string Name { get; set; } = string.Empty;
        public int Trip_Id { get; set; }
        public int? Category_Id { get; set; }
        public int paid_by_user_id { get; set; }
        public decimal Amount { get; set; }
        public string Currency_Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string? Receipt_image { get; set; }
    }

    public class UpdateExpenseRequest
    {
        public string Name { get; set; } = string.Empty;
        public int? Category_Id { get; set; }
        public decimal Amount { get; set; }
        public string? Currency_Code { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string? Receipt_image { get; set; }
    }
}