using System.Collections.Generic;
using System.Threading.Tasks;
using ecommerce.Models;

namespace ecommerce.Repositories
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetAllAsync();
        Task<Order> GetByIdAsync(int id);
        Task AddAsync(Order order);
        Task UpdateAsync(Order order);
        Task DeleteAsync(int id);
        Task<List<Order>> GetOrdersByUserIdAsync(string userId);
    }
}