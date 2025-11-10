using CodeSparkNET.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CodeSparkNET.Infrastructure
{
    public class AppDbContext : IdentityDbContext<AppUser, IdentityRole, string>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<ProductImage> ProductImages { get; set; } = null!;
        public DbSet<Catalog> Catalogs { get; set; } = null!;
        public DbSet<Course> Courses { get; set; } = null!;
        public DbSet<CourseModule> CourseModules { get; set; } = null!;
        public DbSet<Lesson> Lessons { get; set; } = null!;
        public DbSet<LessonResource> LessonResources { get; set; } = null!;
        public DbSet<Template> Templates { get; set; } = null!;
        public DbSet<Diploma> Diplomas { get; set; } = null!;
        public DbSet<UserCourse> UserCourses { get; set; } = null!;
        public DbSet<Product> Products { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // --- Roles seed ---
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

            // --- Catalog ---
            builder.Entity<Catalog>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Name).IsRequired();
                entity.Property(c => c.Slug).IsRequired();
                entity.HasIndex(c => c.Slug).IsUnique();
                entity.Property(c => c.IsVisible).HasDefaultValue(true);
            });

            // --- Product (base type) ---
            builder.Entity<Product>(entity =>
            {
                entity.HasKey(p => p.Id);

                entity.Property(p => p.Name).IsRequired();
                entity.Property(p => p.Slug).IsRequired();
                entity.HasIndex(p => p.Slug).IsUnique();

                entity.Property(p => p.Price).HasColumnType("decimal(18,2)");
                entity.HasIndex(p => p.Price);

                entity.HasIndex(p => new { p.CatalogId, p.Slug }).IsUnique();

                entity.HasOne(p => p.Catalog)
                      .WithMany(cat => cat.Products)
                      .HasForeignKey(p => p.CatalogId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(p => p.ProductImages)
                      .WithOne(pi => pi.Product)
                      .HasForeignKey(pi => pi.ProductId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasDiscriminator(p => p.ProductType)
                      .HasValue<Product>("Product")
                      .HasValue<Course>("Course")
                      .HasValue<Template>("Template")
                      .HasValue<Diploma>("Diploma");
            });

            // --- Course (derived) ---
            builder.Entity<Course>(entity =>
            {
                entity.Property(c => c.Level).HasMaxLength(100);

                entity.HasMany(c => c.UserCourses)
                      .WithOne(uc => uc.Course)
                      .HasForeignKey(uc => uc.CourseSlug)
                      .HasPrincipalKey(c => c.Slug);
            });

            // --- Template (derived) ---
            builder.Entity<Template>(entity =>
            {
            });

            // --- Diploma (derived) ---
            builder.Entity<Diploma>(entity =>
            {
                entity.Property(d => d.Issuer).HasMaxLength(200);
            });

            // --- Tutorial (derived) ---
            builder.Entity<Tutorial>(entity =>
            {
            });

            // --- UserCourse ---
            builder.Entity<UserCourse>(entity =>
            {
                entity.HasKey(uc => uc.Id);

                entity.Property(uc => uc.UserId).IsRequired();
                entity.Property(uc => uc.CourseSlug).IsRequired();

                entity.HasOne(uc => uc.User)
                      .WithMany()
                      .HasForeignKey(uc => uc.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(uc => new { uc.UserId, uc.CourseSlug }).IsUnique(false);
            });

            // --- ProductImage ---
            builder.Entity<ProductImage>(entity =>
            {
                entity.HasKey(pi => pi.Id);
                entity.Property(pi => pi.ProductId).IsRequired();
                entity.Property(pi => pi.Name).IsRequired(false);

                entity.HasOne(pi => pi.Product)
                      .WithMany(p => p.ProductImages)
                      .HasForeignKey(pi => pi.ProductId);
            });

            // --- CourseModule ---
            builder.Entity<CourseModule>(entity =>
            {
                entity.HasKey(m => m.Id);

                entity.Property(m => m.Title).IsRequired().HasMaxLength(300);
                entity.Property(m => m.Position).HasDefaultValue(0);

                // CourseId -> Course.Id
                entity.HasOne(m => m.Course)
                      .WithMany(c => c.Modules)
                      .HasForeignKey(m => m.CourseId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(m => m.CourseId);
            });

            // --- Lesson ---
            builder.Entity<Lesson>(entity =>
            {
                entity.HasKey(l => l.Id);

                entity.Property(l => l.Title).IsRequired().HasMaxLength(400);
                entity.Property(l => l.Slug).HasMaxLength(400);
                entity.Property(l => l.Position).HasDefaultValue(0);
                entity.Property(l => l.IsPublished).HasDefaultValue(true);
                entity.Property(l => l.IsFreePreview).HasDefaultValue(false);

                entity.HasOne(l => l.Module)
                      .WithMany(m => m.Lessons)
                      .HasForeignKey(l => l.ModuleId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(l => l.ModuleId);
            });

            // --- LessonResource ---
            builder.Entity<LessonResource>(entity =>
            {
                entity.HasKey(r => r.Id);

                entity.Property(r => r.Url).IsRequired().HasMaxLength(1000);
                entity.Property(r => r.ResourceType).IsRequired().HasMaxLength(100);
                entity.Property(r => r.Title).HasMaxLength(500);
                entity.Property(r => r.Position).HasDefaultValue(0);

                entity.HasOne(r => r.Lesson)
                      .WithMany(l => l.Resources)
                      .HasForeignKey(r => r.LessonId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(r => r.LessonId);
            });
        }
    }
}
