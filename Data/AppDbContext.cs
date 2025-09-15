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
        }
    }
}