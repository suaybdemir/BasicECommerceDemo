using ECommerceDemo.Models.Concrete;

namespace ECommerceDemo.Models.Abstract
{
    public abstract class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }

        // Görsel için URL
        public string ImageUrl { get; set; }

        // Video için URL (Opsiyonel)
        public string VideoUrl { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
