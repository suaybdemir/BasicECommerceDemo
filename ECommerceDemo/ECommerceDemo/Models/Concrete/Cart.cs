using Microsoft.AspNetCore.Mvc;

namespace ECommerceDemo.Models.Concrete
{
    public class Cart 
    {
        
        public int Id { get; set; } // Sepetin benzersiz ID'si
        public string UserId { get; set; } // Kullanıcının ID'si
        public List<CartItem> Items { get; set; } = new List<CartItem>(); // Sepetteki ürünler
        public decimal TotalAmount => Items.Sum(item => item.Price * item.Quantity); // Sepetin toplam tutarı
        
    }
}
