using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GidIndustrial.Gideon.WebApi.Models;

namespace WebApi.Features.Controllers
{
    [Produces("application/json")]
    [RequirePermission("ManageBilling")]
    [Route("GLAccounts")]
    public class GLAccountsController : Controller
    {
        private readonly AppDBContext _context;

        public GLAccountsController(AppDBContext context)
        {
            _context = context;
        }

        // GET: GLAccounts
        [HttpGet]
        public ListResult GetGLAccounts(
            [FromQuery] int skip = 0,
            [FromQuery] int perPage = 10
        ){
            var query = from glAccount in _context.GLAccounts select glAccount;

            query = query
                .OrderByDescending(item => item.CreatedAt);

            return new ListResult {
                Items = query.Skip(skip).Take(perPage),
                Count = query.Count()
            };
        }

        // GET: GLAccounts/SelectOptions
        [HttpGet("SelectOptions")]
        public async Task<IActionResult> GetGLAccountSelectOptions()
        {
            var glAccounts = await _context.GLAccounts.OrderBy(item => item.Name).Select(item => new {
                Id = item.Id,
                Value = item.Name
            }).ToListAsync();

            return Ok(glAccounts);
        }

        [HttpGet("DefaultGLAccountForCCTransaction")]
        public async Task<IActionResult> GetDefaultGLAccountForCCTransaction(){
            return (Ok(await _context.GLAccounts.FirstOrDefaultAsync(item => item.IsDefaultCCAccount == true)));
        }

        // GET: GLAccounts/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetGLAccount([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var glAccount = await _context.GLAccounts.SingleOrDefaultAsync(m => m.Id == id);

            if (glAccount == null)
            {
                return NotFound();
            }

            return Ok(glAccount);
        }

        // PUT: GLAccounts/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGLAccount([FromRoute] int id, [FromBody] GLAccount glAccount)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != glAccount.Id)
            {
                return BadRequest();
            }

            _context.Entry(glAccount).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GLAccountExists(id))
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

        // POST: GLAccounts
        [HttpPost]
        public async Task<IActionResult> PostGLAccount([FromBody] GLAccount glAccount)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            glAccount.CreatedAt = DateTime.UtcNow;
            _context.GLAccounts.Add(glAccount);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetGLAccount", new { id = glAccount.Id }, glAccount);
        }

        // DELETE: GLAccounts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGLAccount([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var glAccount = await _context.GLAccounts.SingleOrDefaultAsync(m => m.Id == id);
            if (glAccount == null)
            {
                return NotFound();
            }

            _context.GLAccounts.Remove(glAccount);
            await _context.SaveChangesAsync();

            return Ok(glAccount);
        }

        private bool GLAccountExists(int id)
        {
            return _context.GLAccounts.Any(e => e.Id == id);
        }
    }
}