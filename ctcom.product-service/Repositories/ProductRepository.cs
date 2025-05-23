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

        public async Task UpdateAsync(Product product)
        {
            _dbContext.Products.Update(product);
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
    }
}
