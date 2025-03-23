using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ecommerce.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên danh mục không được để trống")]
        [StringLength(50, ErrorMessage = "Tên danh mục không được vượt quá 50 ký tự")]
        [Display(Name = "Tên danh mục")]
        public string Name { get; set; }

        [Display(Name = "Sản phẩm")]
        public List<Product>? Products { get; set; }
    }
}