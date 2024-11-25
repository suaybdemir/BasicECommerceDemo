using ECommerceDemo.Models.Concrete;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

[Route("api/[controller]")]
[ApiController]
public class CartController : ControllerBase
{
    private static List<Cart> _carts = new List<Cart>(); // Temp veri saklama (veritabanı yerine)

    // Sepete ürün ekleme
    [HttpPost("AddToCart")]
    public IActionResult AddToCart(int userId, int productId, string productName, decimal price, int quantity)
    {
        var cart = _carts.FirstOrDefault(c => c.UserId == userId.ToString());

        // Eğer sepet mevcut değilse, yeni bir sepet oluştur
        if (cart == null)
        {
            cart = new Cart
            {
                UserId = userId.ToString()
            };
            _carts.Add(cart);
        }

        // Sepet ürününü ekle
        var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == productId);
        if (existingItem != null)
        {
            existingItem.Quantity += quantity; // Ürün miktarını artır
        }
        else
        {
            cart.Items.Add(new CartItem
            {
                ProductId = productId,
                ProductName = productName,
                Price = price,
                Quantity = quantity
            });
        }

        // Sepet bilgilerini şifreleyerek sakla
        string cartJson = Newtonsoft.Json.JsonConvert.SerializeObject(cart);
        string encryptedCart = AesEncryptionHelper.Encrypt(cartJson);

        // Şifreli sepete kaydet (temp veri saklama yerine gerçek veritabanı kullanılmalı)
        // Bu örnekte sadece şifreli veriyi döndürüyoruz.
        return Ok(new { EncryptedCart = encryptedCart });
    }

    // Sepeti görüntüleme
    [HttpGet("GetCart")]
    public IActionResult GetCart(int userId)
    {
        var cart = _carts.FirstOrDefault(c => c.UserId == userId.ToString());

        if (cart == null)
        {
            return NotFound("Basket not found");
        }

        // Sepeti şifresini çözerek döndür
        string encryptedCart = cart.Items.FirstOrDefault()?.ProductName; // Burada sadece şifreli veriyi simüle ediyoruz
        string decryptedCart = AesEncryptionHelper.Decrypt(encryptedCart);

        var cartObject = Newtonsoft.Json.JsonConvert.DeserializeObject<Cart>(decryptedCart);
        return Ok(cartObject);
    }

    // Sepetten ürün silme
    [HttpDelete("RemoveFromCart")]
    public IActionResult RemoveFromCart(int userId, int productId)
    {
        var cart = _carts.FirstOrDefault(c => c.UserId == userId.ToString());

        if (cart == null)
        {
            return NotFound("Basket not found");
        }

        var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
        if (item != null)
        {
            cart.Items.Remove(item); // Ürünü sepetten sil
        }

        return Ok(cart);
    }
}
