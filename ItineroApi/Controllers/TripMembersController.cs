using ItineroApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ItineroApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TripMembersController : ControllerBase
    {
        private MyContext _context = new MyContext();

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TripMember>>> GetAll()
        {
            return await _context.TripMembers.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TripMember>> GetById(int id)
        {
            var member = await _context.TripMembers.FindAsync(id);
            if (member == null) return NotFound();
            return member;
        }

        [HttpGet("for-trip/{tripId}")]
        public async Task<ActionResult<IEnumerable<TripMember>>> GetMembersForTrip(int tripId)
        {
            var members = await _context.TripMembers
                .Where(tm => tm.trip_id == tripId)
                .ToListAsync();

            return Ok(members);
        }


        [HttpPost]
        public async Task<ActionResult<TripMember>> AddTripMember([FromBody] TripMember tripMember)
        {
            // Volitelná kontrola existence tripu a uživatele
            _context.TripMembers.Add(tripMember);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = tripMember.Id }, tripMember);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var member = await _context.TripMembers.FindAsync(id);
            if (member == null) return NotFound();

            _context.TripMembers.Remove(member);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("for-member/{userId}")]
        public async Task<ActionResult<IEnumerable<Trip>>> GetTripsForMember(int userId)
        {
            var tripIds = await _context.TripMembers
                .Where(tm => tm.user_id == userId)
                .Select(tm => tm.trip_id)
                .ToListAsync();

            var trips = await _context.Trips
                .Where(t => tripIds.Contains(t.Id))
                .ToListAsync();

            return trips;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTripMember(int id, [FromBody] TripMember updatedMember)
        {
            if (id != updatedMember.Id)
                return BadRequest("ID mismatch");

            var existingMember = await _context.TripMembers.FindAsync(id);
            if (existingMember == null)
                return NotFound();

            // Optional: Validate that the referenced trip and user still exist
            var tripExists = await _context.Trips.AnyAsync(t => t.Id == updatedMember.trip_id);
            var userExists = await _context.Users.AnyAsync(u => u.Id == updatedMember.user_id); // assumes `Users` table

            if (!tripExists || !userExists)
                return BadRequest("Invalid trip_id or user_id");

            // Update allowed fields
            existingMember.trip_id = updatedMember.trip_id;
            existingMember.user_id = updatedMember.user_id;
            existingMember.Role = updatedMember.Role;
            existingMember.joined_at = updatedMember.joined_at;

            await _context.SaveChangesAsync();
            return NoContent();
        }


    }
}
