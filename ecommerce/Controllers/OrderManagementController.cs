using System.Threading.Tasks;
using ecommerce.Models;
using ecommerce.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ecommerce.Controllers
{
    [Authorize(Roles = "Admin,Employee")]
    public class OrderManagementController : BaseController
    {
        private readonly IOrderRepository _orderRepository;

        public OrderManagementController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;

        }

        public async Task<IActionResult> Index()
        {
            var orders = await _orderRepository.GetAllAsync();
            return View(orders);
        }

        public async Task<IActionResult> Details(int id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        [HttpGet]
        public async Task<IActionResult> EditStatus(int id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditStatus(int id, string status)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            var validStatuses = new[] { "Chờ xử lý", "Đang giao", "Hoàn thành", "Hủy" };
            if (!validStatuses.Contains(status))
            {
                TempData["ErrorMessage"] = "Trạng thái không hợp lệ.";
                return RedirectToAction("Details", new { id });
            }

            order.Status = status;
            await _orderRepository.UpdateAsync(order);

            TempData["SuccessMessage"] = $"Cập nhật trạng thái đơn hàng #{id} thành công.";
            return RedirectToAction("Details", new { id });
        }
    }
}