using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ecommerce.Models;
using Microsoft.AspNetCore.Identity;

namespace ecommerce.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // Seed Roles
            string[] roles = { SD.Role_Admin, SD.Role_Employee, SD.Role_Customer };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Seed Admin User
            var adminEmail = "admin@example.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail, // UserName = Email
                    Email = adminEmail,
                    FullName = "Admin User",
                    Address = "123 Admin Street, Admin City", // Thêm Address vì là bắt buộc trong model
                    Age = 30, // Age có thể null, nhưng gán giá trị mẫu
                    EmailConfirmed = true // Để không cần xác nhận email
                };

                var result = await userManager.CreateAsync(adminUser, "Admin123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, SD.Role_Admin);
                }
                else
                {
                    // Log lỗi nếu cần
                    foreach (var error in result.Errors)
                    {
                        Console.WriteLine($"Error creating admin user: {error.Description}");
                    }
                }
            }

            // Seed Employee User (Tùy chọn)
            var employeeEmail = "employee@example.com";
            var employeeUser = await userManager.FindByEmailAsync(employeeEmail);
            if (employeeUser == null)
            {
                employeeUser = new ApplicationUser
                {
                    UserName = employeeEmail,
                    Email = employeeEmail,
                    FullName = "Employee User",
                    Address = "789 Employee Lane, Work Town",
                    Age = 25,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(employeeUser, "Employee123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(employeeUser, SD.Role_Employee);
                }
            }

            // Seed Customer User (Tùy chọn)
            var customerEmail = "customer@example.com";
            var customerUser = await userManager.FindByEmailAsync(customerEmail);
            if (customerUser == null)
            {
                customerUser = new ApplicationUser
                {
                    UserName = customerEmail,
                    Email = customerEmail,
                    FullName = "Customer User",
                    Address = "101 Customer Avenue, Shop City",
                    Age = null, // Age có thể null
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(customerUser, "Customer123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(customerUser, SD.Role_Customer);
                }
            }
        }
    }
}