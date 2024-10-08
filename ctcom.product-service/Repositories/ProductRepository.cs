using ctcom.ProductService.Models;
using Microsoft.EntityFrameworkCore;
using ctcom.ProductService.Data;
using ctcom.ProductService.Repositories;

namespace ctcom.ProductService.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ProductDbContext _dbContext;

        public ProductRepository(ProductDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _dbContext.Products
                        .Include(p => p.Variants)
                        .Include(p => p.Options)
                        .Include(p => p.Images)
                        .ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(Guid id)
        {
            return await _dbContext.Products
                      .Include(p => p.Variants)
                      .Include(p => p.Options)
                      .Include(p => p.Images)
                      .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task AddAsync(Product product)
        {
            await _dbContext.Products.AddAsync(product);
            await _dbContext.SaveChangesAsync();
        }


        public async Task DeleteAsync(Guid id)
        {
            var product = await _dbContext.Products.FindAsync(id);
            if (product != null)
            {
                _dbContext.Products.Remove(product);
                await _dbContext.SaveChangesAsync();
            }
        }

        // public async Task UpdateAsync(Product product)
        // {
        //     _dbContext.Products.Update(product);
        //     await _dbContext.SaveChangesAsync();
        // }
        public async Task UpdateAsync(Product product)
        {
            _dbContext.Products.Update(product);

            // Save any changes to the related images
            foreach (var image in product.Images)
            {
                if (_dbContext.Entry(image).State == EntityState.Detached)
                {
                    _dbContext.ProductImages.Add(image);
                }
            }

            await _dbContext.SaveChangesAsync();
        }

        // Method for fetching paginated and filtered products
        public async Task<(IEnumerable<Product>, int)> GetProductsAsync(int page, int pageSize, string? filter)
        {
            // Build the base query
            var query = _dbContext.Products
                                  .Include(p => p.Variants)
                                  .Include(p => p.Options)
                                  .Include(p => p.Images)
                                  .AsQueryable();

            // Apply filtering based on title or other fields as needed
            if (!string.IsNullOrEmpty(filter))
            {
                query = query.Where(p => p.Title.Contains(filter));
            }

            // Get the total count before applying pagination
            var totalRecords = await query.CountAsync();

            // Apply pagination (Skip and Take)
            var products = await query
                .OrderBy(p => p.Title) // Optional: Specify ordering (e.g., by Title)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Return the products along with total count for pagination
            return (products, totalRecords);
        }

    }
}
