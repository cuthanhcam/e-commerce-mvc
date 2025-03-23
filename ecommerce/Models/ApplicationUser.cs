using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace ecommerce.Models
{
    public class ApplicationUser : IdentityUser
    {   
        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        [Display(Name = "Họ tên")]
        [StringLength(100, ErrorMessage = "Họ tên không được vượt quá 100 ký tự")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ")]
        [Display(Name = "Địa chỉ")]
        [StringLength(200, ErrorMessage = "Địa chỉ không được vượt quá 200 ký tự")]
        public string Address { get; set; }

        [Display(Name = "Tuổi")]
        public int? Age { get; set; }
    }
}