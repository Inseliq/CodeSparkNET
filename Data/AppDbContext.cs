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
        public DbSet<CourseAssignment> CourseAssignments { get; set; } = null!;
        public DbSet<CourseEnrollment> CourseEnrollments { get; set; } = null!;
        public DbSet<UserAssignment> UserAssignments { get; set; } = null!;


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

                // Игнорируем Assignments коллекцию в CourseDetail, т.к. в модели задания связаны с Module,
                // а не напрямую с CourseDetail. Если хочешь, чтобы CourseDetail.Assignments работал —
                // добавь ProductId в CourseAssignment и мапь его напрямую.
                entity.Ignore(cd => cd.Assignments);
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
                entity.Property(ca => ca.DeadLine).HasColumnName("DeadLine");

                // Shadow FK "ProductId" — делаем nullable и ставим Restrict (NoAction)
                entity.HasOne(ca => ca.Product)
                      .WithMany()
                      .HasForeignKey("ProductId")   // или .HasForeignKey(ca => ca.ProductId) если добавишь явное поле
                      .IsRequired(false)            // nullable
                      .OnDelete(DeleteBehavior.Restrict); // <-- ключ: запрет каскадного удаления
            });


            // -----------------------
            // UserAssignment -> CourseAssignment, AppUser
            // -----------------------
            builder.Entity<UserAssignment>(entity =>
            {
                entity.HasKey(ua => ua.Id);

                entity.HasOne(ua => ua.CourseAssignment)
                      .WithMany(ca => ca.UserAssignments)
                      .HasForeignKey(ua => ua.AssignmentId)
                      .OnDelete(DeleteBehavior.Cascade);

                // AppUser.Id is string by default; UserAssignment.UserId is string -> create FK
                entity.HasOne(ua => ua.AppUser)
                      .WithMany(u => u.Assignments)
                      .HasForeignKey(ua => ua.UserId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.Property(ua => ua.AnswerText).HasColumnType("nvarchar(max)");
                entity.Property(ua => ua.FileUrl).HasMaxLength(2000);
            });

            // -----------------------
            // CourseEnrollment -> AppUser, CourseDetail
            // -----------------------
            builder.Entity<CourseEnrollment>(entity =>
            {
                entity.HasKey(ce => ce.Id);

                entity.HasOne(ce => ce.AppUser)
                      .WithMany(u => u.Enrollments)
                      .HasForeignKey(ce => ce.UserId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(ce => ce.Course)
                      .WithMany()
                      .HasForeignKey(ce => ce.CourseId)
                      .HasPrincipalKey(cd => cd.ProductId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(nameof(CourseEnrollment.UserId), nameof(CourseEnrollment.CourseId))
                      .IsUnique()
                      .HasDatabaseName("UX_Enrollment_User_Course");
            });

        }

    }
}