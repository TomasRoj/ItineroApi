using ItineroApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ItineroApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ItineraryController : ControllerBase
    {
        private MyContext _context = new MyContext();

        // GET: api/itinerary/days?tripId=1
        [HttpGet("days")]
        public async Task<ActionResult<IEnumerable<ItineraryDay>>> GetDays([FromQuery] int tripId)
        {
            return await _context.ItineraryDay
                .Where(d => d.trip_id == tripId)
                .ToListAsync();
        }

        // GET: api/itinerary/day/5
        [HttpGet("day/{id}")]
        public async Task<ActionResult<ItineraryDay>> GetDay(int id)
        {
            var day = await _context.ItineraryDay
                .FirstOrDefaultAsync(d => d.Id == id);

            if (day == null)
                return NotFound();

            return day;
        }

        // POST: api/itinerary/day
        [HttpPost("day")]
        public async Task<ActionResult<ItineraryDay>> CreateDay(ItineraryDay day)
        {
            _context.ItineraryDay.Add(day);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetDay), new { id = day.Id }, day);
        }

        // PUT: api/itinerary/day/5
        [HttpPut("day/{id}")]
        public async Task<IActionResult> UpdateDay(int id, ItineraryDay day)
        {
            if (id != day.Id)
                return BadRequest();

            _context.Entry(day).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.ItineraryDay.AnyAsync(d => d.Id == id))
                    return NotFound();
                throw;
            }

            return NoContent();
        }

        // DELETE: api/itinerary/day/5
        [HttpDelete("day/{id}")]
        public async Task<IActionResult> DeleteDay(int id)
        {
            var day = await _context.ItineraryDay.FindAsync(id);
            if (day == null)
                return NotFound();

            _context.ItineraryDay.Remove(day);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // CRUD pro itinerary_items

        // GET: api/itinerary/items?dayId=1
        [HttpGet("items")]
        public async Task<ActionResult<IEnumerable<ItineraryItem>>> GetItems([FromQuery] int dayId)
        {
            return await _context.ItineraryItem
                .Where(i => i.itinerary_day_id == dayId)
                .ToListAsync();
        }

        // GET: api/itinerary/item/5
        [HttpGet("item/{id}")]
        public async Task<ActionResult<ItineraryItem>> GetItem(int id)
        {
            var item = await _context.ItineraryItem.FindAsync(id);
            if (item == null)
                return NotFound();

            return item;
        }

        // POST: api/itinerary/item
        [HttpPost("item")]
        public async Task<ActionResult<ItineraryItem>> CreateItem(ItineraryItem item)
        {
            _context.ItineraryItem.Add(item);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetItem), new { id = item.id }, item);
        }

        // PUT: api/itinerary/item/5
        [HttpPut("item/{id}")]
        public async Task<IActionResult> UpdateItem(int id, ItineraryItem item)
        {
            if (id != item.id)
                return BadRequest();

            _context.Entry(item).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.ItineraryItem.AnyAsync(i => i.id == id))
                    return NotFound();
                throw;
            }

            return NoContent();
        }

        // DELETE: api/itinerary/item/5
        [HttpDelete("item/{id}")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            var item = await _context.ItineraryItem.FindAsync(id);
            if (item == null)
                return NotFound();

            _context.ItineraryItem.Remove(item);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
