using System;
using System.Linq;
using System.Threading.Tasks;
using ecommerce.Data;
using ecommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace ecommerce.Repositories
{
    public class EFCartRepository : ICartRepository
    {
        private readonly ApplicationDbContext _context;

        public EFCartRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Cart> GetCartByUserIdAsync(string userId)
        {
            if (_context.Carts == null)
            {
                throw new InvalidOperationException("Carts table is not available.");
            }

            var cart = await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart { UserId = userId, Items = new System.Collections.Generic.List<CartItem>() };
                await _context.Carts.AddAsync(cart);
                await _context.SaveChangesAsync();
            }

            return cart;
        }

        public async Task AddCartItemAsync(CartItem cartItem)
        {
            if (cartItem == null)
            {
                throw new ArgumentNullException(nameof(cartItem));
            }

            if (_context.CartItems == null)
            {
                throw new InvalidOperationException("CartItems table is not available.");
            }

            await _context.CartItems.AddAsync(cartItem);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCartItemAsync(CartItem cartItem)
        {
            if (cartItem == null)
            {
                throw new ArgumentNullException(nameof(cartItem));
            }

            if (_context.CartItems == null)
            {
                throw new InvalidOperationException("CartItems table is not available.");
            }

            var existingItem = await _context.CartItems.FindAsync(cartItem.Id);
            if (existingItem == null)
            {
                throw new KeyNotFoundException($"CartItem with ID {cartItem.Id} not found.");
            }

            existingItem.Quantity = cartItem.Quantity;
            await _context.SaveChangesAsync();
        }

        public async Task RemoveCartItemAsync(int cartItemId)
        {
            if (_context.CartItems == null)
            {
                throw new InvalidOperationException("CartItems table is not available.");
            }

            var cartItem = await _context.CartItems.FindAsync(cartItemId);
            if (cartItem == null)
            {
                throw new KeyNotFoundException($"CartItem with ID {cartItemId} not found.");
            }

            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();
        }

        public async Task ClearCartAsync(string userId)
        {
            if (_context.Carts == null)
            {
                throw new InvalidOperationException("Carts table is not available.");
            }

            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart != null && cart.Items.Any())
            {
                _context.CartItems.RemoveRange(cart.Items);
                await _context.SaveChangesAsync();
            }
        }
    }
}