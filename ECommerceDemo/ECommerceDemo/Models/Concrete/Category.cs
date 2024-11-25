using ECommerceDemo.Models.Abstract;

namespace ECommerceDemo.Models.Concrete
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Product> Products { get; set; }
    }
}
