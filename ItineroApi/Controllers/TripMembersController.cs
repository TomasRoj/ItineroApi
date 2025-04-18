﻿using ItineroApi.Models;
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
            return await _context.TripMember.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TripMember>> Get(int id)
        {
            var member = await _context.TripMember.FindAsync(id);
            if (member == null)
                return NotFound();
            return member;
        }

        [HttpPost]
        public async Task<ActionResult<TripMember>> Create(TripMember member)
        {
            _context.TripMember.Add(member);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = member.id }, member);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, TripMember member)
        {
            if (id != member.id)
                return BadRequest();

            _context.Entry(member).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.TripMember.AnyAsync(e => e.id == id))
                    return NotFound();
                throw;
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var member = await _context.TripMember.FindAsync(id);
            if (member == null)
                return NotFound();

            _context.TripMember.Remove(member);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
