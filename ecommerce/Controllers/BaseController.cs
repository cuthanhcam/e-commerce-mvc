using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ecommerce.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ecommerce.Controllers
{
    [Authorize]
    public abstract class BaseController : Controller
    {
        protected bool IsAdming => User.IsInRole(SD.Role_Admin);
        protected bool IsEmployee => User.IsInRole(SD.Role_Employee);
        protected bool IsCustomer => User.IsInRole(SD.Role_Customer);

        protected IActionResult AccssDenied()
        {
            return View("~/Views/Shared/AccessDenied.cshtml");
        }
    }
}