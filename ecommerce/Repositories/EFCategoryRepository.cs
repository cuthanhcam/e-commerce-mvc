using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ecommerce.Data;
using ecommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace ecommerce.Repositories
{
    public class EFCategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public EFCategoryRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            if (_context.Categories == null)
            {
                throw new InvalidOperationException("Categories table is not available.");
            }

            return await _context.Categories
                .Include(c => c.Products)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Category> GetByIdAsync(int id)
        {
            if (_context.Categories == null)
            {
                throw new InvalidOperationException("Categories table is not available.");
            }

            var category = await _context.Categories
                .Include(c => c.Products)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                throw new KeyNotFoundException($"Category with ID {id} not found.");
            }

            return category;
        }

        public async Task AddAsync(Category category)
        {
            if (category == null)
            {
                throw new ArgumentNullException(nameof(category));
            }

            if (_context.Categories == null)
            {
                throw new InvalidOperationException("Categories table is not available.");
            }

            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Category category)
        {
            if (category == null)
            {
                throw new ArgumentNullException(nameof(category));
            }

            if (_context.Categories == null)
            {
                throw new InvalidOperationException("Categories table is not available.");
            }

            var existingCategory = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == category.Id);

            if (existingCategory == null)
            {
                throw new KeyNotFoundException($"Category with ID {category.Id} not found.");
            }

            // Update the existing category's properties
            existingCategory.Name = category.Name;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            if (_context.Categories == null)
            {
                throw new InvalidOperationException("Categories table is not available.");
            }

            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                throw new KeyNotFoundException($"Category with ID {id} not found.");
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
        }
    }
}