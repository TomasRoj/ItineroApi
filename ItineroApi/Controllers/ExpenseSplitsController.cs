using ItineroApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ItineroApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExpenseSplitsController : ControllerBase
    {
        private MyContext _context = new MyContext();

        // GET: api/expensesplits
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExpenseSplit>>> GetAll()
        {
            return await _context.ExpenseSplit.ToListAsync();
        }

        // GET: api/expensesplits/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ExpenseSplit>> Get(int id)
        {
            var split = await _context.ExpenseSplit.FindAsync(id);
            if (split == null)
                return NotFound();
            return split;
        }

        // GET: api/expensesplits/expense/5
        [HttpGet("expense/{expenseId}")]
        public async Task<ActionResult<IEnumerable<ExpenseSplit>>> GetByExpenseId(int expenseId)
        {
            return await _context.ExpenseSplit
                .Where(s => s.Expense_id == expenseId)
                .ToListAsync();
        }

        // GET: api/expensesplits/user/5
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<ExpenseSplit>>> GetByUserId(int userId)
        {
            return await _context.ExpenseSplit
                .Where(s => s.User_Id == userId)
                .ToListAsync();
        }

        // GET: api/expensesplits/trip/5
        [HttpGet("trip/{tripId}")]
        public async Task<ActionResult<IEnumerable<ExpenseSplit>>> GetByTripId(int tripId)
        {
            return await _context.ExpenseSplit
                .Where(s => s.trip_ID == tripId)
                .ToListAsync();
        }

        // POST: api/expensesplits
        [HttpPost]
        public async Task<ActionResult<ExpenseSplit>> Create(ExpenseSplit split)
        {
            _context.ExpenseSplit.Add(split);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = split.Id }, split);
        }

        // PUT: api/expensesplits/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ExpenseSplit split)
        {
            if (id != split.Id)
                return BadRequest();

            _context.Entry(split).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.ExpenseSplit.AnyAsync(e => e.Id == id))
                    return NotFound();
                throw;
            }
            return NoContent();
        }

        // PUT: api/expensesplits/settleexpense/5
        [HttpPut("settleexpense/{expenseId}")]
        public async Task<IActionResult> SettleExpense(int expenseId)
        {
            var splits = await _context.ExpenseSplit
                .Where(s => s.Expense_id == expenseId)
                .ToListAsync();

            if (!splits.Any())
                return NotFound();

            foreach (var split in splits)
            {
                split.Is_Settled = true;
                split.Settled_At = DateTime.UtcNow;
                _context.Entry(split).State = EntityState.Modified;
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/expensesplits/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var split = await _context.ExpenseSplit.FindAsync(id);
            if (split == null)
                return NotFound();

            _context.ExpenseSplit.Remove(split);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}