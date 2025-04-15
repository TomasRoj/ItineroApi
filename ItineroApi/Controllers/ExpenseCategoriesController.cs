using ItineroApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ItineroApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExpenseCategoriesController : ControllerBase
    {
        private MyContext _context = new MyContext();

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExpenseCategory>>> GetAll()
        {
            return await _context.ExpenseCategory.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ExpenseCategory>> Get(int id)
        {
            var category = await _context.ExpenseCategory.FindAsync(id);
            if (category == null)
                return NotFound();
            return category;
        }

        [HttpPost]
        public async Task<ActionResult<ExpenseCategory>> Create(ExpenseCategory category)
        {
            _context.ExpenseCategory.Add(category);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = category.id }, category);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ExpenseCategory category)
        {
            if (id != category.id)
                return BadRequest();

            _context.Entry(category).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.ExpenseCategory.AnyAsync(e => e.id == id))
                    return NotFound();
                throw;
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _context.ExpenseCategory.FindAsync(id);
            if (category == null)
                return NotFound();

            _context.ExpenseCategory.Remove(category);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
