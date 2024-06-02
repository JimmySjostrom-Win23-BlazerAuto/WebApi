using Infrastructure.Contexts;
using Infrastructure.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscribeController : ControllerBase
    {
        private readonly ApiContext _context;

        public SubscribeController(ApiContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Subscribe(SubscribersEntity entity)
        {
            if (ModelState.IsValid)
            {
                if (await _context.Subscribers.AnyAsync(x => x.Email == entity.Email))
                    return Conflict();

                _context.Add(entity);
                await _context.SaveChangesAsync();
                return Ok();
            }

            return BadRequest();
        }

        [HttpDelete]
        public async Task<IActionResult> Unsubscribe(string email)
        {
            if (ModelState.IsValid)
            {
                var subscriberEntity = await _context.Subscribers.FirstOrDefaultAsync(x => x.Email == email);

                if (subscriberEntity == null)
                    return NotFound();

                _context.Remove(subscriberEntity);
                await _context.SaveChangesAsync();
                return Ok();
            }

            return BadRequest();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SubscribersEntity>>> GetAllSubscribers()
        {
            return await _context.Subscribers.ToListAsync();
        }

        [HttpGet("{email}")]
        public async Task<ActionResult<SubscribersEntity>> FindSubscriberByEmail(string email)
        {
            var subscriberEntity = await _context.Subscribers.FirstOrDefaultAsync(x => x.Email == email);

            if (subscriberEntity == null)
                return NotFound();

            return subscriberEntity;
        }

        [HttpPut("{email}")]
        public async Task<IActionResult> UpdateSubscriber(string email, SubscribersEntity entity)
        {
            if (email != entity.Email)
            {
                return BadRequest();
            }

            _context.Entry(entity).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SubscriberExists(email))
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

        private bool SubscriberExists(string email)
        {
            return _context.Subscribers.Any(e => e.Email == email);
        }
    }
}