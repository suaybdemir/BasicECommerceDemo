using Microsoft.AspNetCore.Mvc;

namespace ECommerceDemo.Models.Concrete
{
    public class CartItem
    {
        public int Id { get; set; } // Sepet ürününün benzersiz ID'si
        public int ProductId { get; set; } // Ürünün ID'si (bağlantılı ürün)
        public string ProductName { get; set; } // Ürünün adı
        public decimal Price { get; set; } // Ürünün fiyatı
        public int Quantity { get; set; } // Sepetteki ürün miktarı
    }

}
