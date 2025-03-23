using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ecommerce.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên sản phẩm không được để trống")]
        [StringLength(100, ErrorMessage = "Tên sản phẩm không được vượt quá 100 ký tự")]
        [Display(Name = "Tên sản phẩm")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Giá không được để trống")]
        [Range(0.01, 10000.00, ErrorMessage = "Giá phải từ 0.01 đến 10000.00")]
        [Display(Name = "Giá")]
        public decimal Price { get; set; }

        [Display(Name = "Mô tả")]
        public string? Description { get; set; }

        [Display(Name = "Hình ảnh")]
        public string? ImageUrl { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn danh mục")]
        [Display(Name = "Danh mục")]
        public int CategoryId { get; set; }

        [Display(Name = "Danh mục")]
        public Category? Category { get; set; }

        [Display(Name = "Hình ảnh")]
        public List<ProductImage>? Images { get; set; }
    }
}