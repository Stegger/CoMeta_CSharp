using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CoMeta.Data;
using CoMeta.Models;
using Microsoft.AspNetCore.Authorization;

namespace CoMeta.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly CoMetaContext _context;
        private readonly IAuthorizationService _authorizationService;

        public MessageController(CoMetaContext context, IAuthorizationService authorizationService)
        {
            _context = context;
            _authorizationService = authorizationService;
        }

        // GET: api/Message
        [AllowAnonymous] //For debugging purpose only. Should require admin role!
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Message>>> GetMessages()
        {
            return await _context.Messages.ToListAsync();
        }

        // GET: api/Message/5
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<Message>> GetMessage(long id)
        {
            var message = await _context.Messages.FindAsync(id);
            if (message == null)
            {
                return NotFound();
            }
            
            var authorizationResult = await _authorizationService.AuthorizeAsync(User, message, "OwnerPolicy");
            if(authorizationResult.Succeeded)
            {
                return message;
            }
            else
            {
                return new ChallengeResult();
            }
        }

        // PUT: api/Message/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMessage(long id, Message message)
        {
            if (id != message.Id)
            {
                return BadRequest();
            }

            _context.Entry(message).State = EntityState.Modified;
            
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MessageExists(id))
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

        // POST: api/Message
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<Message>> PostMessage([FromBody] Message message)
        {
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            string sid = User.Claims.FirstOrDefault(type => type.Type.Equals(ClaimTypes.Sid)).Value;
            
            return CreatedAtAction("GetMessage", new { id = message.Id }, message);
        }

        // DELETE: api/Message/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMessage(long id)
        {
            var message = await _context.Messages.FindAsync(id);
            if (message == null)
            {
                return NotFound();
            }

            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MessageExists(long id)
        {
            return _context.Messages.Any(e => e.Id == id);
        }
    }
}
