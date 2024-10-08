using ctcom.ProductService.Data;
using ctcom.ProductService.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ctcom.ProductService.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ProductDbContext _dbContext;

        public ProductRepository(ProductDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<(IEnumerable<Product>, int totalRecords)> GetProductsAsync(int page, int pageSize, string? filter, CancellationToken cancellationToken)
        {
            var query = _dbContext.Products
                                  .Include(p => p.Variants)
                                  .Include(p => p.Options)
                                  .Include(p => p.Images)
                                  .AsQueryable();

            if (!string.IsNullOrEmpty(filter))
            {
                query = query.Where(p => p.Title.Contains(filter));
            }

            var totalRecords = await query.CountAsync(cancellationToken);

            var products = await query
                .OrderBy(p => p.Title)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (products, totalRecords);
        }

        public async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _dbContext.Products
                .Include(p => p.Variants)
                .Include(p => p.Options)
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }

        public async Task<Guid> AddAsync(Product product, CancellationToken cancellationToken)
        {
            await _dbContext.Products.AddAsync(product, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return product.Id;

        }

        public async Task UpdateAsync(Product product, CancellationToken cancellationToken)
        {
            _dbContext.Products.Update(product);

            foreach (var image in product.Images)
            {
                if (_dbContext.Entry(image).State == EntityState.Detached)
                {
                    _dbContext.ProductImages.Add(image);
                }
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            var product = await _dbContext.Products.FindAsync(new object[] { id }, cancellationToken);
            if (product != null)
            {
                _dbContext.Products.Remove(product);
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
