using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ECommerceDemo.Models.Concrete;

namespace ECommerceDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly string _publishableKey;
        private readonly string _secretKey;

        public PaymentController(UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _publishableKey = configuration["Stripe:PublishableKey"];
            _secretKey = configuration["Stripe:SecretKey"];
        }

        // Get Payments by Role
        [HttpGet("GetUserPayments")]
        [Authorize]
        public async Task<IActionResult> GetUserPayments()
        {
            try
            {
                var currentUserId = User.Identity?.Name;

                if (string.IsNullOrEmpty(currentUserId))
                    return Unauthorized(new { message = "User is not authenticated" });

                // Check if the user is an Admin
                var user = await _userManager.FindByNameAsync(currentUserId);
                var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

                if (isAdmin)
                {
                    var allPayments = GetAllPayments();
                    return Ok(allPayments);
                }
                else
                {
                    var userPayments = GetUserPayments(currentUserId);
                    return Ok(userPayments);
                }
            }
            catch (System.Exception ex)
            {
                return Problem($"An error occurred: {ex.Message}");
            }
        }

        private IEnumerable<string> GetAllPayments()
        {
            return new List<string> { "Payment 1", "Payment 2", "Payment 3" };
        }

        private IEnumerable<string> GetUserPayments(string userId)
        {
            return new List<string> { $"Payment for User {userId} - Payment 1" };
        }

        // Add Item to Cart
        [HttpPost("AddToCart")]
        [Authorize]
        public IActionResult AddToCart([FromBody] CartItem cartItem)
        {
            if (cartItem == null || cartItem.ProductId <= 0 || cartItem.Quantity <= 0)
                return BadRequest(new { message = "Invalid cart item details" });

            var userId = User.Identity?.Name;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "User is not authenticated" });

            try
            {
                AddToCartDatabase(userId, cartItem);
                return Ok(new { message = "Item added to cart successfully" });
            }
            catch (System.Exception ex)
            {
                return Problem($"An error occurred while adding to cart: {ex.Message}");
            }
        }

        private void AddToCartDatabase(string userId, CartItem cartItem)
        {
            // Save the cart item in the database (implement your own logic)
        }

        // Purchase Items
        [HttpPost("Purchase")]
        [Authorize]
        public async Task<IActionResult> Purchase([FromBody] PurchaseRequest purchaseRequest)
        {
            if (purchaseRequest == null || string.IsNullOrEmpty(purchaseRequest.PaymentMethodId))
                return BadRequest(new { message = "Invalid purchase details" });

            var userId = User.Identity?.Name;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "User is not authenticated" });

            try
            {
                var cartItems = GetCartItems(userId);
                if (!cartItems.Any())
                    return BadRequest(new { message = "Cart is empty" });

                var paymentSuccess = await ProcessPayment(purchaseRequest);

                if (paymentSuccess)
                {
                    SaveOrder(userId, cartItems);
                    return Ok(new { message = "Purchase successful" });
                }

                return BadRequest(new { message = "Payment failed" });
            }
            catch (System.Exception ex)
            {
                return Problem($"An error occurred while processing the purchase: {ex.Message}");
            }
        }

        private async Task<bool> ProcessPayment(PurchaseRequest purchaseRequest)
        {
            try
            {
                StripeConfiguration.ApiKey = _secretKey;

                var paymentIntentService = new PaymentIntentService();
                var paymentIntent = await paymentIntentService.ConfirmAsync(
                    purchaseRequest.PaymentIntentId,
                    new PaymentIntentConfirmOptions
                    {
                        PaymentMethod = purchaseRequest.PaymentMethodId
                    });

                return paymentIntent.Status == "succeeded";
            }
            catch (StripeException ex)
            {
                // Log error and return failure
                return false;
            }
        }

        private IEnumerable<CartItem> GetCartItems(string userId)
        {
            return new List<CartItem>
            {
                new CartItem { ProductId = 1, Quantity = 2, Price = 20.0 }
            };
        }

        private void SaveOrder(string userId, IEnumerable<CartItem> cartItems)
        {
            // Save the order in the database (implement your own logic)
        }
    }

    // Models
    public class CartItem
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
    }

    public class PurchaseRequest
    {
        public string PaymentMethodId { get; set; }
        public string PaymentIntentId { get; set; }
    }
}
