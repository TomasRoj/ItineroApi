using ItineroApi.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace ItineroApi.Controllers
{
    [EnableCors("AllowAngular")]
    [ApiController]
    [Route("api/[controller]")]
    public class TripsController : ControllerBase
    {
            private MyContext context = new MyContext();


            // GET: api/trips
            [HttpGet]
            public async Task<ActionResult<IEnumerable<Trip>>> GetTrips()
            {
                return  context.Trips.ToList();
            }

            // GET: api/trips/5
            [HttpGet("{id}")]
            public async Task<ActionResult<Trip>> GetTrip(int id)
            {
                var trip = await context.Trips.FindAsync(id);

                if (trip == null)
                    return NotFound();

                return trip;
            }

            // POST: api/trips
            [HttpPost]
            public async Task<ActionResult<Trip>> CreateTrip(Trip trip)
            {
                trip.created_at = DateTime.UtcNow;
                trip.updated_at = DateTime.UtcNow;

                context.Trips.Add(trip);
                await context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetTrip), new { id = trip.Id }, trip);
            }

            // PUT: api/trips/5
            [HttpPut("{id}")]
            public async Task<IActionResult> UpdateTrip(int id, Trip updatedTrip)
            {
                if (id != updatedTrip.Id)
                    return BadRequest();

                var trip = await context.Trips.FindAsync(id);
                if (trip == null)
                    return NotFound();

                // Aktualizace
                trip.Name = updatedTrip.Name;
                trip.Start_date = updatedTrip.Start_date;
                trip.End_date = updatedTrip.End_date;
                trip.Description = updatedTrip.Description;
                trip.Is_public = updatedTrip.Is_public;
                trip.Destination_city_id = updatedTrip.Destination_city_id;
                trip.updated_at = DateTime.UtcNow;

                await context.SaveChangesAsync();

                return NoContent();
            }

            // DELETE: api/trips/5
            [HttpDelete("{id}")]
            public async Task<IActionResult> DeleteTrip(int id)
            {
                var trip = await context.Trips.FindAsync(id);
                if (trip == null)
                    return NotFound();

                context.Trips.Remove(trip);
                await context.SaveChangesAsync();

                return NoContent();
            }
       
    }
}
