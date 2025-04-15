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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExpenseSplit>>> GetAll()
        {
            return await _context.ExpenseSplit.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ExpenseSplit>> Get(int id)
        {
            var split = await _context.ExpenseSplit.FindAsync(id);
            if (split == null)
                return NotFound();
            return split;
        }

        [HttpPost]
        public async Task<ActionResult<ExpenseSplit>> Create(ExpenseSplit split)
        {
            _context.ExpenseSplit.Add(split);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = split.Id }, split);
        }

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
