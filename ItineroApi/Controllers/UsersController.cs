using Microsoft.AspNetCore.Mvc;
using ItineroApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ItineroApi.Controllers
{
        [Route("api/[controller]")]
        [ApiController]
        public class UsersController : ControllerBase
        {
            private MyContext context = new MyContext();

            // GET: api/Users
            [HttpGet]
            public async Task<ActionResult<IEnumerable<User>>> GetUsers()
            {
                return await context.Users.ToListAsync();
            }

            // GET: api/Users/5
            [HttpGet("{id}")]
            public async Task<ActionResult<User>> GetUser(int id)
            {
                var user = await context.Users.FindAsync(id);

                if (user == null)
                {
                    return NotFound();
                }

                return user;
            }

            // PUT: api/Users/5
            [HttpPut("{id}")]
            public async Task<IActionResult> PutUser(int id, User user)
            {
                if (id != user.Id)
                {
                    return BadRequest();
                }

                context.Entry(user).State = EntityState.Modified;

                try
                {
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return NoContent();
            }

            // POST: api/Users
            [HttpPost]
            public async Task<ActionResult<User>> PostUser(User user)
            {
                context.Users.Add(user);
                await context.SaveChangesAsync();

                return CreatedAtAction("GetUser", new { id = user.Id }, user);
            }

            // DELETE: api/Users/5
            [HttpDelete("{id}")]
            public async Task<IActionResult> DeleteUser(int id)
            {
                var user = await context.Users.FindAsync(id);
                if (user == null)
                {
                    return NotFound();
                }

                context.Users.Remove(user);
                await context.SaveChangesAsync();

                return NoContent();
            }

            private bool UserExists(int id)
            {
                return context.Users.Any(e => e.Id == id);
            }
        }
    }