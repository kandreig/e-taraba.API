using e_taraba.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace e_taraba.API.DbContexts
{
    public class e_tarabaContext : DbContext
    {
        public DbSet<Order> Orders { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductOrder> ProductOrders { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public e_tarabaContext(DbContextOptions options)
            :base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>()
                .HasMany(p => p.Orders)
                .WithMany(p => p.Products)
                .UsingEntity<ProductOrder>(
                j => j.HasOne(pt => pt.Order).WithMany(t => t.ProductOrders).HasForeignKey(pt => pt.OrderId),
                j => j.HasOne(pt => pt.Product).WithMany(t => t.ProductOrders).HasForeignKey(pt => pt.ProductId),
                j =>
                {
                    j.HasKey(t => t.Id);
                }
                );

        }
    }
}
