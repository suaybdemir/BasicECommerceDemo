using ECommerceDemo.Models.Abstract;
using ECommerceDemo.Models.Concrete;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ECommerceDemo.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }

        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<ECommerceDemo.Models.Concrete.Stationaery> Stationaery { get; set; } = default!;
        public DbSet<ECommerceDemo.Models.Concrete.OrderItem> OrderItem { get; set; } = default!;
        public DbSet<ECommerceDemo.Models.Concrete.CustomerRole> CustomerRole { get; set; } = default!;
        public DbSet<ECommerceDemo.Models.Concrete.Gift> Gift { get; set; } = default!;

    }

}
