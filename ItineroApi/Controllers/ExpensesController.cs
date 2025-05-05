using ItineroApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ItineroApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExpensesController : ControllerBase
    {
        private MyContext _context = new MyContext();

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

        // GET: api/expenses/user/5
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<Expense>>> GetByUserId(int userId)
        {
            return await _context.Expenses
                .Where(e => e.paid_by_user_id == userId)
                .OrderByDescending(e => e.Date)
                .ToListAsync();
        }

        // GET: api/expenses/category/5
        [HttpGet("category/{categoryId}")]
        public async Task<ActionResult<IEnumerable<Expense>>> GetByCategoryId(int categoryId)
        {
            return await _context.Expenses
                .Where(e => e.Category_Id == categoryId)
                .OrderByDescending(e => e.Date)
                .ToListAsync();
        }

        // POST: api/expenses
        [HttpPost]
        public async Task<ActionResult<Expense>> Create(Expense expense)
        {
            // Set created and updated timestamps
            expense.Created_At = DateTime.UtcNow;
            expense.Updated_At = DateTime.UtcNow;

            _context.Expenses.Add(expense);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = expense.Id }, expense);
        }

        // PUT: api/expenses/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Expense expense)
        {
            if (id != expense.Id)
                return BadRequest();

            // Update the timestamp
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
            return NoContent();
        }

        // DELETE: api/expenses/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
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
    }
}