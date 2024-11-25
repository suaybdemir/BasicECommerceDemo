using System;
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
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Orders
        [HttpGet("Index")]
        public async Task<IActionResult> Index()
        {
            try
            {
                var orders = await _context.Orders.ToListAsync();
                return View(orders);
            }
            catch (Exception ex)
            {
                return Problem($"Error fetching orders: {ex.Message}");
            }
        }

        // GET: Orders/Details/5
        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return BadRequest("Order ID must be provided.");
            }

            try
            {
                var order = await _context.Orders.FirstOrDefaultAsync(m => m.Id == id);
                if (order == null)
                {
                    return NotFound("Order not found.");
                }

                return View(order);
            }
            catch (Exception ex)
            {
                return Problem($"Error fetching order details: {ex.Message}");
            }
        }

        // GET: Orders/Create
        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Orders/Create
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,TotalAmount,PaymentStatus,CreatedAt,ShippingAddress")] Order order)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(order);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error creating order: {ex.Message}");
                }
            }

            return View(order);
        }

        // GET: Orders/Edit/5
        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return BadRequest("Order ID must be provided.");
            }

            try
            {
                var order = await _context.Orders.FindAsync(id);
                if (order == null)
                {
                    return NotFound("Order not found.");
                }

                return View(order);
            }
            catch (Exception ex)
            {
                return Problem($"Error fetching order for editing: {ex.Message}");
            }
        }

        // POST: Orders/Edit/5
        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,TotalAmount,PaymentStatus,CreatedAt,ShippingAddress")] Order order)
        {
            if (id != order.Id)
            {
                return BadRequest("Mismatched Order ID.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.Id))
                    {
                        return NotFound("Order not found.");
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error updating order: {ex.Message}");
                }
            }

            return View(order);
        }

        // GET: Orders/Delete/5
        [HttpGet("Delete/{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return BadRequest("Order ID must be provided.");
            }

            try
            {
                var order = await _context.Orders.FirstOrDefaultAsync(m => m.Id == id);
                if (order == null)
                {
                    return NotFound("Order not found.");
                }

                return View(order);
            }
            catch (Exception ex)
            {
                return Problem($"Error fetching order for deletion: {ex.Message}");
            }
        }

        // POST: Orders/Delete/5
        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var order = await _context.Orders.FindAsync(id);
                if (order == null)
                {
                    return NotFound("Order not found.");
                }

                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return Problem($"Error deleting order: {ex.Message}");
            }
        }

        // Check if an Order exists by ID
        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }
    }
}
