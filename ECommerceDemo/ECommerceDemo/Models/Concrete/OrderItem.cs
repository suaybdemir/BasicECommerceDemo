using ECommerceDemo.Models.Abstract;

namespace ECommerceDemo.Models.Concrete
{
    public class OrderItem
    {
        public int Id { get; set; } // Sipariş ürününün benzersiz ID'si
        public int ProductId { get; set; } // Ürünün ID'si
        public string ProductName { get; set; } // Ürünün adı
        public decimal Price { get; set; } // Ürünün fiyatı
        public int Quantity { get; set; } // Ürünün siparişteki miktarı
    }
}
