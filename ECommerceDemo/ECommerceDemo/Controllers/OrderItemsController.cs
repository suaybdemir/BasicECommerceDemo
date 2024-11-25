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
    public class OrderItemsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderItemsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: OrderItems
        [HttpGet("Index")]
        public async Task<IActionResult> Index()
        {
            try
            {
                var orderItems = await _context.OrderItem.ToListAsync();
                return View(orderItems);
            }
            catch (Exception ex)
            {
                return Problem($"Error fetching order items: {ex.Message}");
            }
        }

        // GET: OrderItems/Details/5
        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return BadRequest("OrderItem ID must be provided.");
            }

            try
            {
                var orderItem = await _context.OrderItem.FirstOrDefaultAsync(m => m.Id == id);
                if (orderItem == null)
                {
                    return NotFound("OrderItem not found.");
                }

                return View(orderItem);
            }
            catch (Exception ex)
            {
                return Problem($"Error fetching order item details: {ex.Message}");
            }
        }

        // GET: OrderItems/Create
        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: OrderItems/Create
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProductId,ProductName,Price,Quantity")] OrderItem orderItem)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(orderItem);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error while creating OrderItem: {ex.Message}");
                }
            }

            return View(orderItem);
        }

        // GET: OrderItems/Edit/5
        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return BadRequest("OrderItem ID must be provided.");
            }

            try
            {
                var orderItem = await _context.OrderItem.FindAsync(id);
                if (orderItem == null)
                {
                    return NotFound("OrderItem not found.");
                }

                return View(orderItem);
            }
            catch (Exception ex)
            {
                return Problem($"Error fetching order item for editing: {ex.Message}");
            }
        }

        // POST: OrderItems/Edit/5
        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProductId,ProductName,Price,Quantity")] OrderItem orderItem)
        {
            if (id != orderItem.Id)
            {
                return BadRequest("Mismatched OrderItem ID.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(orderItem);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderItemExists(orderItem.Id))
                    {
                        return NotFound("OrderItem not found.");
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error while editing OrderItem: {ex.Message}");
                }
            }

            return View(orderItem);
        }

        // GET: OrderItems/Delete/5
        [HttpGet("Delete/{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return BadRequest("OrderItem ID must be provided.");
            }

            try
            {
                var orderItem = await _context.OrderItem.FirstOrDefaultAsync(m => m.Id == id);
                if (orderItem == null)
                {
                    return NotFound("OrderItem not found.");
                }

                return View(orderItem);
            }
            catch (Exception ex)
            {
                return Problem($"Error fetching order item for deletion: {ex.Message}");
            }
        }

        // POST: OrderItems/Delete/5
        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var orderItem = await _context.OrderItem.FindAsync(id);
                if (orderItem == null)
                {
                    return NotFound("OrderItem not found.");
                }

                _context.OrderItem.Remove(orderItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return Problem($"Error deleting OrderItem: {ex.Message}");
            }
        }

        // Check if an OrderItem exists by ID
        private bool OrderItemExists(int id)
        {
            return _context.OrderItem.Any(e => e.Id == id);
        }
    }
}
