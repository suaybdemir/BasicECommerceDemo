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
    public class CartItemsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CartItemsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/CartItems
        [HttpGet]  // This is for fetching all cart items
        public async Task<ActionResult<IEnumerable<CartItem>>> GetCartItems()
        {
            var cartItems = await _context.CartItems.ToListAsync();
            return Ok(cartItems); // Wrap the result in Ok() to return a valid ActionResult
        }

        // GET: api/CartItems/5
        [HttpGet("{id}")]  // This is for fetching a single cart item by its ID
        public async Task<ActionResult<CartItem>> GetCartItem(int id)
        {
            var cartItem = await _context.CartItems.FindAsync(id);

            if (cartItem == null)
            {
                return NotFound();
            }

            return Ok(cartItem); // Return the cart item
        }

        // POST: api/CartItems
        [HttpPost]
        public async Task<ActionResult<CartItem>> PostCartItem([FromBody] ECommerceDemo.Models.Concrete.CartItem cartItem)
        {
            if (ModelState.IsValid)
            {
                _context.CartItems.Add(cartItem);
                await _context.SaveChangesAsync();

                // Returning the created resource with the correct type
                return CreatedAtAction(nameof(GetCartItem), new { id = cartItem.Id }, cartItem);
            }

            return BadRequest(ModelState);
        }


        // PUT: api/CartItems/5
        [HttpPut("{id}")]  // This is for updating an existing cart item
        public async Task<IActionResult> PutCartItem(int id, [FromBody] CartItem cartItem)
        {
            
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Entry(cartItem).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CartItemExists(id))
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

        // DELETE: api/CartItems/5
        [HttpDelete("{id}")]  // This is for deleting a cart item by ID
        public async Task<IActionResult> DeleteCartItem(int id)
        {
            var cartItem = await _context.CartItems.FindAsync(id);
            if (cartItem == null)
            {
                return NotFound();
            }

            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CartItemExists(int id)
        {
            return _context.CartItems.Any(e => e.Id == id);
        }
    }
}
