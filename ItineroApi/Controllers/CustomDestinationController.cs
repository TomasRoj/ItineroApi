using ItineroApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ItineroApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomDestinationsController : ControllerBase
    {
        private MyContext _context = new MyContext();

        // GET: api/customdestinations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomDestination>>> GetAll()
        {
            return await _context.CustomDestinations.ToListAsync();
        }

        // GET: api/customdestinations/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CustomDestination>> Get(int id)
        {
            var destination = await _context.CustomDestinations.FindAsync(id);
            if (destination == null)
                return NotFound();
            return destination;
        }

        // POST: api/customdestinations
        [HttpPost]
        public async Task<ActionResult<CustomDestination>> Create(CustomDestination destination)
        {
            destination.CreatedAt = DateTime.UtcNow;
            destination.UpdatedAt = DateTime.UtcNow;
            _context.CustomDestinations.Add(destination);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = destination.Id }, destination);
        }

        // PUT: api/customdestinations/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, CustomDestination destination)
        {
            if (id != destination.Id)
                return BadRequest();

            destination.UpdatedAt = DateTime.UtcNow;
            _context.Entry(destination).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.CustomDestinations.AnyAsync(e => e.Id == id))
                    return NotFound();
                throw;
            }

            return NoContent();
        }

        // DELETE: api/customdestinations/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var destination = await _context.CustomDestinations.FindAsync(id);
            if (destination == null)
                return NotFound();

            _context.CustomDestinations.Remove(destination);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
