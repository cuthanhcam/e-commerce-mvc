using ecommerce.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ecommerce.Controllers
{
    [Authorize]
    public abstract class BaseController : Controller
    {
        protected bool IsAdmin => User.IsInRole(SD.Role_Admin);
        protected bool IsEmployee => User.IsInRole(SD.Role_Employee);
        protected bool IsCustomer => User.IsInRole(SD.Role_Customer);

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            if (IsAdmin)
            {
                ViewData["Layout"] = "~/Views/Shared/_AdminLayout.cshtml";
            }
            else
            {
                ViewData["Layout"] = "~/Views/Shared/_Layout.cshtml";
            }
        }

        protected IActionResult AccessDenied()
        {
            return View("~/Views/Shared/AccessDenied.cshtml");
        }
    }
}