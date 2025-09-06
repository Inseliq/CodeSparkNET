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
        public DbSet<CourseDetail> CourseDetails { get; set; } = null!;
        public DbSet<Module> Modules { get; set; } = null!;
        public DbSet<Lesson> Lessons { get; set; } = null!;


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

            // -----------------------
            // Product
            // -----------------------
            builder.Entity<Product>(entity =>
            {
                entity.HasKey(p => p.Id);

                // unique slug
                entity.HasIndex(p => p.Slug).IsUnique();

                // decimal precision (дублирует атрибут в модели)
                entity.Property(p => p.Price).HasColumnType("decimal(18,2)");

                // Enum to int
                entity.Property(p => p.ProductType).HasConversion<int>();

                // One-to-one: Product <-> CourseDetail (shared PK)
                // Исправлено: используем HasOne<CourseDetail>() — т.к. Product НЕ содержит навигационного свойства CourseDetail.
                builder.Entity<Product>()
                            .HasOne<CourseDetail>()                       // principal -> dependent type
                            .WithOne(cd => cd.Product)                    // dependent navigation -> principal navigation
                            .HasForeignKey<CourseDetail>(cd => cd.ProductId)
                            .OnDelete(DeleteBehavior.Cascade);
            });

            // -----------------------
            // CourseDetail (shared PK to Product.Id)
            // -----------------------
            builder.Entity<CourseDetail>(entity =>
            {
                entity.HasKey(cd => cd.ProductId);
            });

            // -----------------------
            // Module -> CourseDetail
            // -----------------------
            builder.Entity<Module>(entity =>
            {
                entity.HasKey(m => m.Id);

                entity.HasOne(m => m.Course)
                      .WithMany(cd => cd.Modules)
                      .HasForeignKey(m => m.CourseId)
                      .HasPrincipalKey(cd => cd.ProductId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.Property(m => m.Title).HasMaxLength(500);
                entity.Property(m => m.Order).HasColumnName("SortOrder");
            });

            // -----------------------
            // Lesson -> Module
            // -----------------------
            builder.Entity<Lesson>(entity =>
            {
                entity.HasKey(l => l.Id);

                entity.HasOne(l => l.Module)
                      .WithMany(m => m.Lessons)
                      .HasForeignKey(l => l.ModuleId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.Property(l => l.Content).HasColumnType("nvarchar(max)");
                entity.Property(l => l.Title).HasMaxLength(500);
            });

            // -----------------------
            // CourseAssignment -> Module AND -> Product (shadow FK ProductId)
            // -----------------------
            builder.Entity<CourseAssignment>(entity =>
            {
                entity.HasKey(ca => ca.Id);

                // связь к модулю — каскад оставляем
                entity.HasOne(ca => ca.Module)
                      .WithMany(m => m.Assignments)
                      .HasForeignKey(ca => ca.ModuleId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.Property(ca => ca.Title).HasMaxLength(500);
                entity.Property(ca => ca.Description).HasColumnType("nvarchar(max)");
            });

        }

    }
}