using ECommerceDemo.Models.Concrete;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

[Route("api/[controller]")]
[ApiController]
public class OrderController : ControllerBase
{
    private static List<Order> _orders = new List<Order>(); // Temp veri saklama (veritabanı yerine)
    private static List<Cart> _carts = new List<Cart>(); // Sepet verisi (Temp)

    // Sipariş oluşturma
    [HttpPost("CreateOrder")]
    public IActionResult CreateOrder(int userId, string shippingAddress)
    {
        // Kullanıcının sepetini al
        var cart = _carts.FirstOrDefault(c => c.UserId == userId.ToString());
        if (cart == null || !cart.Items.Any())
        {
            return BadRequest("Sepetinizde ürün bulunmamaktadır.");
        }

        // Sipariş oluştur
        var order = new Order
        {
            UserId = userId.ToString(),
            Items = cart.Items.Select(item => new OrderItem
            {
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                Price = item.Price,
                Quantity = item.Quantity
            }).ToList(),
            TotalAmount = cart.TotalAmount,
            PaymentStatus = "Beklemede", // Ödeme durumu başlangıçta "Beklemede"
            ShippingAddress = shippingAddress,
            CreatedAt = DateTime.UtcNow
        };

        // Siparişi listeye ekle
        _orders.Add(order);

        // Sepeti boşalt (satın alma tamamlandığında sepet temizlenir)
        cart.Items.Clear();

        return Ok(new { message = "Sipariş başarıyla oluşturuldu.", orderId = order.Id });
    }

    // Sipariş listesi (kullanıcı bazında)
    [HttpGet("GetUserOrders")]
    public IActionResult GetUserOrders(int userId)
    {
        var userOrders = _orders.Where(o => o.UserId == userId.ToString()).ToList();
        if (userOrders == null || !userOrders.Any())
        {
            return NotFound("Hiç siparişiniz bulunmamaktadır.");
        }

        return Ok(userOrders);
    }

    // Sipariş ödeme işlemi
    [HttpPost("ProcessPayment")]
    public IActionResult ProcessPayment(int orderId, string paymentMethod)
    {
        var order = _orders.FirstOrDefault(o => o.Id == orderId);
        if (order == null)
        {
            return NotFound("Sipariş bulunamadı.");
        }

        // Ödeme işlemi yapılır (örneğin kredi kartı, PayPal vb.)
        bool paymentSuccess = ProcessPaymentMethod(paymentMethod, order.TotalAmount);

        if (paymentSuccess)
        {
            order.PaymentStatus = "Başarılı";
            return Ok(new { message = "Ödeme başarıyla gerçekleştirildi.", orderId = order.Id });
        }

        order.PaymentStatus = "Başarısız";
        return BadRequest("Ödeme işlemi başarısız.");
    }

    // Ödeme işlemi (basit simülasyon)
    private bool ProcessPaymentMethod(string paymentMethod, decimal amount)
    {
        // Ödeme işleminin başarılı olup olmadığını simüle edelim
        // Gerçek ödeme işlemine API entegrasyonu yapılabilir
        return true; // Başarılı ödeme
    }
}
