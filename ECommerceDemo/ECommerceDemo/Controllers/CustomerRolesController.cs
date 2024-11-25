using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ECommerceDemo.Data;
using ECommerceDemo.Models.Concrete;

namespace ECommerceDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerRolesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CustomerRolesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/CustomerRoles
        [HttpGet("GetCustomerRoles")]
        public async Task<ActionResult<IEnumerable<CustomerRole>>> GetCustomerRoles()
        {
            return await _context.CustomerRole.ToListAsync();
        }

        // GET: api/CustomerRoles/5
        [HttpGet("GetCustomerRole")]
        public async Task<ActionResult<CustomerRole>> GetCustomerRole(int id)
        {
            var customerRole = await _context.CustomerRole.FindAsync(id);

            if (customerRole == null)
            {
                return NotFound();
            }

            return customerRole;
        }

        // POST: api/CustomerRoles
        [HttpPost("PostCustomerRole")]
        public async Task<ActionResult<CustomerRole>> PostCustomerRole([FromBody] CustomerRole customerRole)
        {
            if (ModelState.IsValid)
            {
                _context.CustomerRole.Add(customerRole);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetCustomerRole), new { id = customerRole.Id }, customerRole);
            }

            return BadRequest(ModelState);
        }

        // PUT: api/CustomerRoles/5
        [HttpPut("PutCustomerRole")]
        public async Task<IActionResult> PutCustomerRole(int id, [FromBody] CustomerRole customerRole)
        {
            if (id != customerRole.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Entry(customerRole).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerRoleExists(id))
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

            return BadRequest(ModelState);
        }

        // DELETE: api/CustomerRoles/5
        [HttpDelete("DeleteCustomerRole")]
        public async Task<IActionResult> DeleteCustomerRole(int id)
        {
            var customerRole = await _context.CustomerRole.FindAsync(id);
            if (customerRole == null)
            {
                return NotFound();
            }

            _context.CustomerRole.Remove(customerRole);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CustomerRoleExists(int id)
        {
            return _context.CustomerRole.Any(e => e.Id == id);
        }
    }
}
