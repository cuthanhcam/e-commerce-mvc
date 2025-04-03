using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ecommerce.Data;
using ecommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace ecommerce.Repositories
{
    public class EFProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public EFProductRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            if (_context.Products == null)
            {
                throw new InvalidOperationException("Products table is not available.");
            }

            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Images)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Product> GetByIdAsync(int id)
        {
            if (_context.Products == null)
            {
                throw new InvalidOperationException("Products table is not available.");
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Images)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                throw new KeyNotFoundException($"Product with ID {id} not found.");
            }

            return product;
        }

        public async Task AddAsync(Product product)
        {
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product));
            }

            if (_context.Products == null)
            {
                throw new InvalidOperationException("Products table is not available.");
            }

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Product product)
        {
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product));
            }

            if (_context.Products == null)
            {
                throw new InvalidOperationException("Products table is not available.");
            }

            var existingProduct = await _context.Products
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == product.Id);

            if (existingProduct == null)
            {
                throw new KeyNotFoundException($"Product with ID {product.Id} not found.");
            }

            // Update the existing product's properties
            existingProduct.Name = product.Name;
            existingProduct.Price = product.Price;
            existingProduct.Description = product.Description;
            existingProduct.CategoryId = product.CategoryId;
            existingProduct.ImageUrl = product.ImageUrl;

            // Handle images
            if (product.Images != null && product.Images.Any())
            {
                // Remove images that are no longer in the updated product
                var imagesToRemove = existingProduct.Images?
                    .Where(ei => !product.Images.Any(i => i.Id == ei.Id))
                    .ToList();

                if (imagesToRemove != null && imagesToRemove.Any())
                {
                    _context.ProductImages.RemoveRange(imagesToRemove);
                }

                // Add new images
                var newImages = product.Images
                    .Where(i => existingProduct.Images == null || !existingProduct.Images.Any(ei => ei.Id == i.Id))
                    .ToList();

                if (newImages.Any())
                {
                    existingProduct.Images ??= new List<ProductImage>();
                    existingProduct.Images.AddRange(newImages);
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            if (_context.Products == null)
            {
                throw new InvalidOperationException("Products table is not available.");
            }

            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                throw new KeyNotFoundException($"Product with ID {id} not found.");
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Product>> GetFilteredProductsAsync(string searchString, int? categoryId, string sortOrder)
        {
            if (_context.Products == null)
            {
                throw new InvalidOperationException("Products table is not available.");
            }

            var query = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Images)
                .AsQueryable();

            // Tìm kiếm theo tên sản phẩm, không phân biệt chữ hoa/thường
            if (!string.IsNullOrEmpty(searchString))
            {
                // Sử dụng ToLower() để không phân biệt chữ hoa/thường
                query = query.Where(p => p.Name.ToLower().Contains(searchString.ToLower()));
            }

            // Lọc theo danh mục
            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            // Sắp xếp
            switch (sortOrder)
            {
                case "price_desc":
                    query = query.OrderByDescending(p => p.Price);
                    break;
                case "price_asc":
                    query = query.OrderBy(p => p.Price);
                    break;
                default:
                    query = query.OrderBy(p => p.Name); // Mặc định sắp xếp theo tên
                    break;
            }

            return await query.AsNoTracking().ToListAsync();
        }
    }
}