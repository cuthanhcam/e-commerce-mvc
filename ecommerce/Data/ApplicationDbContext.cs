using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ecommerce.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ecommerce.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>()
                .HasMany(p => p.Images)
                .WithOne(i => i.Product)
                .HasForeignKey(i => i.ProductId);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Category>()
                .Property(c => c.Name)
                .HasMaxLength(50);

            modelBuilder.Entity<Product>()
                .Property(p => p.Name)
                .HasMaxLength(100);

            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18,2)")
                .HasPrecision(18, 2);

            modelBuilder.Entity<ApplicationUser>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.Property(u => u.FullName).HasMaxLength(100);
                entity.Property(u => u.Address).HasMaxLength(200);
            });
        }
    }
}