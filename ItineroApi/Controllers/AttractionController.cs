using ItineroApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ItineroApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AttractionsController : ControllerBase
    {
        private MyContext _context = new MyContext();

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Attraction>>> GetAll()
        {
            return await _context.Attractions.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Attraction>> Get(int id)
        {
            var attraction = await _context.Attractions.FindAsync(id);
            if (attraction == null) return NotFound();
            return attraction;
        }

        [HttpPost]
        public async Task<ActionResult<Attraction>> Create(Attraction attraction)
        {
            _context.Attractions.Add(attraction);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = attraction.Id }, attraction);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Attraction attraction)
        {
            if (id != attraction.Id) return BadRequest();
            _context.Entry(attraction).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Attractions.AnyAsync(a => a.Id == id))
                    return NotFound();
                throw;
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var attraction = await _context.Attractions.FindAsync(id);
            if (attraction == null) return NotFound();
            _context.Attractions.Remove(attraction);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
