using Microsoft.EntityFrameworkCore;
using ctcom.ProductService.Models;

namespace ctcom.ProductService.Data
{
    public class ProductDbContext : DbContext
    {
        public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<ProductOption> ProductOptions { get; set; }
        public DbSet<ProductVariant> ProductVariants { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {



            base.OnModelCreating(modelBuilder);
            // Primary key for Product
            modelBuilder.Entity<Product>().HasKey(p => p.Id);

            // Define relationship between Product and Variants (explicit foreign key + cascade delete)
            modelBuilder.Entity<Product>()
                .HasMany(p => p.Variants)
                .WithOne(v => v.Product) // Specify the navigation property
                .HasForeignKey(v => v.ProductId) // Specify the foreign key
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProductVariant>()
                .HasOne(v => v.Product)
                .WithMany(p => p.Variants)
                .HasForeignKey(v => v.ProductId);


            modelBuilder.Entity<ProductVariant>(entity =>
                      {
                          entity.Property(p => p.Price)
                                  .HasPrecision(18, 2); // Precision 18, Scale 2 (e.g., 999,999,999,999.99)
                      });

            // Define relationship between Product and Options (explicit foreign key + cascade delete)
            modelBuilder.Entity<Product>()
                .HasMany(p => p.Options)
                .WithOne(o => o.Product) // Specify the navigation property
                .HasForeignKey(o => o.ProductId) // Specify the foreign key
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProductOption>()
                .HasOne(o => o.Product)
                .WithMany(p => p.Options)
                .HasForeignKey(o => o.ProductId);

            // Define relationship between Product and Images (explicit foreign key + cascade delete)
            modelBuilder.Entity<Product>()
                .HasMany(p => p.Images)
                .WithOne(i => i.Product) // Specify the navigation property
                .HasForeignKey(i => i.ProductId) // Specify the foreign key
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProductImage>()
                .HasOne(i => i.Product)
                .WithMany(p => p.Images)
                .HasForeignKey(i => i.ProductId);
        }
    }
}
