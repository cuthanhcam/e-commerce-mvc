using System.Threading.Tasks;
using ecommerce.Data;
using ecommerce.Models;
using ecommerce.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ecommerce.Controllers
{
    public class ShoppingCartController : BaseController
    {
        private readonly IProductRepository _productRepository;
        private readonly ICartRepository _cartRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public ShoppingCartController(
            IProductRepository productRepository,
            ICartRepository cartRepository,
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context)
        {
            _productRepository = productRepository;
            _cartRepository = cartRepository;
            _userManager = userManager;
            _context = context;
        }

        private async Task<Cart> GetCartAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return new Cart { Items = new System.Collections.Generic.List<CartItem>() };
            }

            return await _cartRepository.GetCartByUserIdAsync(user.Id);
        }

        public async Task<IActionResult> Index()
        {
            var cart = await GetCartAsync();
            return View(cart);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
            {
                TempData["ErrorMessage"] = "Sản phẩm không tồn tại.";
                return RedirectToAction("Index", "Product");
            }

            var cart = await GetCartAsync();
            if (cart.UserId == null)
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập để thêm sản phẩm vào giỏ hàng.";
                return RedirectToAction("Login", "Account");
            }

            var cartItem = cart.Items.FirstOrDefault(i => i.ProductId == productId);
            if (cartItem != null)
            {
                cartItem.Quantity += quantity;
                await _cartRepository.UpdateCartItemAsync(cartItem);
            }
            else
            {
                var newCartItem = new CartItem
                {
                    CartId = cart.Id,
                    ProductId = productId,
                    Name = product.Name,
                    Price = product.Price,
                    Quantity = quantity
                };
                await _cartRepository.AddCartItemAsync(newCartItem);
            }

            TempData["SuccessMessage"] = $"Đã thêm {product.Name} vào giỏ hàng!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int productId)
        {
            var cart = await GetCartAsync();
            var cartItem = cart.Items.FirstOrDefault(i => i.ProductId == productId);
            if (cartItem != null)
            {
                await _cartRepository.RemoveCartItemAsync(cartItem.Id);
                TempData["SuccessMessage"] = "Đã xóa sản phẩm khỏi giỏ hàng.";
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> IncreaseQuantity(int productId)
        {
            var cart = await GetCartAsync();
            var cartItem = cart.Items.FirstOrDefault(i => i.ProductId == productId);
            if (cartItem != null)
            {
                cartItem.Quantity++;
                await _cartRepository.UpdateCartItemAsync(cartItem);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DecreaseQuantity(int productId)
        {
            var cart = await GetCartAsync();
            var cartItem = cart.Items.FirstOrDefault(i => i.ProductId == productId);
            if (cartItem != null)
            {
                cartItem.Quantity--;
                if (cartItem.Quantity <= 0)
                {
                    await _cartRepository.RemoveCartItemAsync(cartItem.Id);
                }
                else
                {
                    await _cartRepository.UpdateCartItemAsync(cartItem);
                }
            }
            return RedirectToAction("Index");
        }

        [Authorize]
        public async Task<IActionResult> Checkout()
        {
            var cart = await GetCartAsync();
            if (!cart.Items.Any())
            {
                TempData["ErrorMessage"] = "Giỏ hàng trống. Vui lòng thêm sản phẩm trước khi thanh toán.";
                return RedirectToAction("Index");
            }

            var user = await _userManager.GetUserAsync(User);
            var order = new Order
            {
                UserId = user.Id,
                TotalPrice = cart.GetTotalPrice()
            };
            return View(order);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(Order order)
        {
            var cart = await GetCartAsync();
            if (!cart.Items.Any())
            {
                TempData["ErrorMessage"] = "Giỏ hàng trống.";
                return RedirectToAction("Index");
            }

            var user = await _userManager.GetUserAsync(User);
            order.UserId = user.Id;

            // Bỏ qua validation cho Notes nếu trống
            if (string.IsNullOrEmpty(order.Notes))
            {
                ModelState.Remove("Notes"); // Xóa lỗi validation cho Notes
            }

            if (ModelState.IsValid)
            {
                // Lấy thời gian hiện tại theo UTC+7 (Việt Nam)
                TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                order.OrderDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);

                order.TotalPrice = cart.GetTotalPrice();
                order.OrderDetails = cart.Items.Select(i => new OrderDetail
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    Price = i.Price
                }).ToList();

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                await _cartRepository.ClearCartAsync(user.Id);

                TempData["SuccessMessage"] = "Đặt hàng thành công!";
                return RedirectToAction("OrderCompleted", new { orderId = order.Id });
            }

            return View(order);
        }

        public IActionResult OrderCompleted(int orderId)
        {
            return View(orderId);
        }
    }
}