using System.Threading.Tasks;
using ecommerce.Models;
using ecommerce.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ecommerce.Controllers
{
    [Authorize]
    public class OrderController : BaseController
    {
        private readonly IOrderRepository _orderRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrderController(IOrderRepository orderRepository, UserManager<ApplicationUser> userManager)
        {
            _orderRepository = orderRepository;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            // Chỉ Admin được xem tất cả đơn hàng
            if (IsAdmin)
            {
                var allOrders = await _orderRepository.GetAllAsync();
                return View("AdminIndex", allOrders);
            }

            // Nếu là Customer hoặc Employee, chỉ hiển thị đơn hàng của họ (hoặc không hiển thị gì cho Employee nếu muốn)
            var orders = await _orderRepository.GetOrdersByUserIdAsync(user.Id);
            return View(orders);
        }

        public async Task<IActionResult> Details(int id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (!IsAdmin && !IsEmployee && order.UserId != user.Id)
            {
                return AccessDenied(); // Người dùng chỉ xem được đơn hàng của mình
            }

            return View(order);
        }
    }
}