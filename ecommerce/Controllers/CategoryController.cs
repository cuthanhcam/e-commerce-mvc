using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ecommerce.Models;
using ecommerce.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ecommerce.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IProductRepository _productRepository;

        public CategoryController(ICategoryRepository categoryRepository, IProductRepository productRepository)
        {
            _categoryRepository = categoryRepository;
            _productRepository = productRepository;
        }

        // GET: Category/Index
        public async Task<IActionResult> Index()
        {
            var categories = await _categoryRepository.GetAllAsync();
            return View(categories);
        }

        // GET: Category/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Category/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category model)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra trùng lặp tên danh mục
                var existingCategory = (await _categoryRepository.GetAllAsync())
                    .FirstOrDefault(c => c.Name.Equals(model.Name, StringComparison.OrdinalIgnoreCase));

                if (existingCategory != null)
                {
                    ModelState.AddModelError("Name", "Tên danh mục đã tồn tại.");
                    return View(model);
                }

                await _categoryRepository.AddAsync(model);
                TempData["SuccessMessage"] = "Thêm danh mục thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Category/Edit/{id}
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null) return NotFound();
            return View(category);
        }

        // POST: Category/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Category model)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra trùng lặp tên danh mục (trừ danh mục hiện tại)
                var existingCategory = (await _categoryRepository.GetAllAsync())
                    .FirstOrDefault(c => c.Name.Equals(model.Name, StringComparison.OrdinalIgnoreCase) && c.Id != model.Id);

                if (existingCategory != null)
                {
                    ModelState.AddModelError("Name", "Tên danh mục đã tồn tại.");
                    return View(model);
                }

                await _categoryRepository.UpdateAsync(model);
                TempData["SuccessMessage"] = "Cập nhật danh mục thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Category/Delete/{id}
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var category = await _categoryRepository.GetByIdAsync(id.Value);
            if (category == null) return NotFound();
            return View(category);
        }

        // POST: Category/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null) return NotFound();

            // Kiểm tra xem category có sản phẩm liên quan không
            var hasProducts = (await _productRepository.GetAllAsync()).Any(p => p.CategoryId == id);

            if (hasProducts)
            {
                TempData["ErrorMessage"] = "Không thể xóa danh mục này vì vẫn còn sản phẩm liên quan. Vui lòng xóa hoặc chuyển sản phẩm sang danh mục khác trước.";
                return RedirectToAction(nameof(Index));
            }

            await _categoryRepository.DeleteAsync(id);
            TempData["SuccessMessage"] = "Xóa danh mục thành công!";
            return RedirectToAction(nameof(Index));
        }
    }
}