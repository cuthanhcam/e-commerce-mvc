using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ecommerce.Data;
using ecommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace ecommerce.Repositories
{
    public class EFOrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _context;

        public EFOrderRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            if (_context.Orders == null)
            {
                throw new InvalidOperationException("Orders table is not available.");
            }

            return await _context.Orders
                .Include(o => o.ApplicationUser)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Order> GetByIdAsync(int id)
        {
            if (_context.Orders == null)
            {
                throw new InvalidOperationException("Orders table is not available.");
            }

            var order = await _context.Orders
                .Include(o => o.ApplicationUser)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                throw new KeyNotFoundException($"Order with ID {id} not found.");
            }

            return order;
        }

        public async Task AddAsync(Order order)
        {
            if (order == null)
            {
                throw new ArgumentNullException(nameof(order));
            }

            if (_context.Orders == null)
            {
                throw new InvalidOperationException("Orders table is not available.");
            }

            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Order order)
        {
            if (order == null)
            {
                throw new ArgumentNullException(nameof(order));
            }

            if (_context.Orders == null)
            {
                throw new InvalidOperationException("Orders table is not available.");
            }

            var existingOrder = await _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.Id == order.Id);

            if (existingOrder == null)
            {
                throw new KeyNotFoundException($"Order with ID {order.Id} not found.");
            }

            // Update properties
            existingOrder.Status = order.Status;
            existingOrder.ShippingAddress = order.ShippingAddress;
            existingOrder.Notes = order.Notes;
            existingOrder.TotalPrice = order.TotalPrice;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            if (_context.Orders == null)
            {
                throw new InvalidOperationException("Orders table is not available.");
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                throw new KeyNotFoundException($"Order with ID {id} not found.");
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Order>> GetOrdersByUserIdAsync(string userId)
        {
            if (_context.Orders == null)
            {
                throw new InvalidOperationException("Orders table is not available.");
            }

            return await _context.Orders
                .Include(o => o.ApplicationUser)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .Where(o => o.UserId == userId)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}