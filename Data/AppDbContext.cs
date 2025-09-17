using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeSparkNET.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CodeSparkNET.Data
{
    public class AppDbContext : IdentityDbContext<AppUser, IdentityRole, string>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<ProductImage> ProductImages { get; set; } = null!;
        public DbSet<Catalog> Catalogs { get; set; } = null!;
        public DbSet<Course> Courses { get; set; } = null!;
        public DbSet<Template> Templates { get; set; } = null!;
        public DbSet<Diploma> Diplomas { get; set; } = null!;


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            var roles = new List<IdentityRole>
            {
                new IdentityRole
                {
                    Id = "a3f9c6d2-1f4b-4b8e-9f2a-111111111111",
                    Name = "Admin",
                    NormalizedName = "ADMIN",
                    ConcurrencyStamp = "c1f9c6d2-1f4b-4b8e-9f2a-111111111111"
                },
                new IdentityRole
                {
                    Id = "b4f9c6d2-2f4b-4b8e-9f2a-222222222222",
                    Name = "User",
                    NormalizedName = "USER",
                    ConcurrencyStamp = "d2f9c6d2-2f4b-4b8e-9f2a-222222222222"
                },
                new IdentityRole
                {
                    Id = "c5f9c6d2-3f4b-4b8e-9f2a-333333333333",
                    Name = "Prime",
                    NormalizedName = "PRIME",
                    ConcurrencyStamp = "e3f9c6d2-3f4b-4b8e-9f2a-333333333333"
                }
            };

            builder.Entity<IdentityRole>().HasData(roles);

            //Catalog
            builder.Entity<Catalog>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Name).IsRequired();
                entity.Property(c => c.Slug).IsRequired();
                entity.HasIndex(c => c.Slug).IsUnique();
                entity.Property(c => c.IsVisible).HasDefaultValue(true);
            });

            //Product
            builder.Entity<Product>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Name).IsRequired();
                entity.Property(p => p.Slug).IsRequired();
                entity.Property(p => p.Price).HasColumnType("decimal(18,2)");
                entity.HasIndex(p => p.Price);
                entity.HasIndex(p => new { p.CatalogId, p.Slug }).IsUnique();

                //Links
                entity.HasOne(p => p.Catalog)
                    .WithMany(p => p.Products)
                    .HasForeignKey(p => p.CatalogId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            //User Course
            builder.Entity<UserCourse>(entity =>
            {
                entity.HasKey(c => c.Id);
            });

            //Product Image
            builder.Entity<ProductImage>(entity =>
            {
                entity.HasKey(pi => pi.Id);
                entity.Property(pi => pi.ProductId).IsRequired();

                //Links
                entity.HasOne(pi => pi.Product)
                    .WithMany(pi => pi.ProductImages)
                    .HasForeignKey(pi => pi.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);
            });


            // Seed: Catalogs
            builder.Entity<Catalog>().HasData(
                new Catalog
                {
                    Id = "d1f9c6d2-4b4b-4b8e-9f2a-aaaa00000001",
                    Name = "It-Cubic",
                    Slug = "it-cubic",
                    IsVisible = true
                },
                new Catalog
                {
                    Id = "d1f9c6d2-4b4b-4b8e-9f2a-aaaa00000002",
                    Name = "Code Spark",
                    Slug = "code-spark",
                    IsVisible = true
                },
                new Catalog
                {
                    Id = "d1f9c6d2-4b4b-4b8e-9f2a-aaaa00000003",
                    Name = "Монтажка",
                    Slug = "montazhka",
                    IsVisible = true
                }
            );

            // Seed: Products (по одному продукту в каждом каталоге)
            builder.Entity<Product>().HasData(
                new Product
                {
                    Id = "e1f9c6d2-5b4b-4b8e-9f2a-bbbb00000001",
                    CatalogId = "d1f9c6d2-4b4b-4b8e-9f2a-aaaa00000001",
                    Name = "Курс C# с нуля",
                    Slug = "c#-for-beginners",
                    Price = 1999.99m,

                },
                new Product
                {
                    Id = "e1f9c6d2-5b4b-4b8e-9f2a-bbbb00000002",
                    CatalogId = "d1f9c6d2-4b4b-4b8e-9f2a-aaaa00000002",
                    Name = "Трехуровневая заготовка ASP.NET MVC",
                    Slug = "3-tier-web-template",
                    Price = 1299.00m,
                },
                new Product
                {
                    Id = "e1f9c6d2-5b4b-4b8e-9f2a-bbbb00000003",
                    CatalogId = "d1f9c6d2-4b4b-4b8e-9f2a-aaaa00000003",
                    Name = "Дипломгая работа",
                    Slug = "diploma-work",
                    Price = 30000m,
                }
            );

            builder.Entity<ProductImage>().HasData(
                new ProductImage
                {
                    Id = "f1f9c6d2-6b4b-4b8e-9f2a-cccc00000001",
                    ProductId = "e1f9c6d2-5b4b-4b8e-9f2a-bbbb00000001",
                    Name = "itcubic-main.jpg",
                    ImageData = null,
                    IsMain = true
                },
                new ProductImage
                {
                    Id = "f1f9c6d2-6b4b-4b8e-9f2a-cccc00000002",
                    ProductId = "e1f9c6d2-5b4b-4b8e-9f2a-bbbb00000002",
                    Name = "codespark-main.jpg",
                    ImageData = null,
                    IsMain = true
                },
                new ProductImage
                {
                    Id = "f1f9c6d2-6b4b-4b8e-9f2a-cccc00000003",
                    ProductId = "e1f9c6d2-5b4b-4b8e-9f2a-bbbb00000003",
                    Name = "montazhka-main.jpg",
                    ImageData = null,
                    IsMain = true
                }
            );

        }
    }
}