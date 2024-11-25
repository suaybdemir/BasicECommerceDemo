namespace ECommerceDemo.Models.Concrete
{
    public class Order
    {
        public int Id { get; set; } // Siparişin benzersiz ID'si
        public string UserId { get; set; } // Kullanıcının ID'si
        public List<OrderItem> Items { get; set; } = new List<OrderItem>(); // Siparişin ürünleri
        public decimal TotalAmount { get; set; } // Siparişin toplam tutarı
        public string PaymentStatus { get; set; } // Ödeme durumu (örneğin "Başarılı", "Başarısız")
        public DateTime CreatedAt { get; set; } // Siparişin oluşturulma tarihi
        public string ShippingAddress { get; set; } // Kullanıcının teslimat adresi
    }
}
