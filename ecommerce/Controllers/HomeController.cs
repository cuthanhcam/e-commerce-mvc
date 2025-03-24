using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ecommerce.Models;
using ecommerce.Repositories;

namespace ecommerce.Controllers;

public class HomeController : Controller
{
    private readonly IProductRepository _productRepository;

    public HomeController(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<IActionResult> Index()
    {
        if (User.Identity.IsAuthenticated && User.IsInRole(SD.Role_Admin))
        {
            ViewData["Layout"] = "~/Views/Shared/_AdminLayout.cshtml";
        }
        else
        {
            ViewData["Layout"] = "~/Views/Shared/_Layout.cshtml";
        }

        var products = await _productRepository.GetAllAsync();
        return View(products);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View();
    }
}
