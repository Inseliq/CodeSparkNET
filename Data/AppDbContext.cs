using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CodeSparkNET.Models;
using CodeSparkNET.Models.Enum;
using Microsoft.AspNetCore.Identity;

namespace CodeSparkNET.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // Основные сущности
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<Catalog> Catalogs { get; set; }
        public DbSet<CatalogConfiguration> CatalogConfigurations { get; set; }

        // Курсы
        public DbSet<CourseDetail> CourseDetails { get; set; }
        public DbSet<Module> Modules { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<CourseAssignment> CourseAssignments { get; set; }
        public DbSet<UserAssignment> UserAssignments { get; set; }
        public DbSet<CourseEnrollment> CourseEnrollments { get; set; }

        // Веб-шаблоны
        public DbSet<WebTemplateDetail> WebTemplateDetails { get; set; }
        public DbSet<WebTemplateOrder> WebTemplateOrders { get; set; }
        public DbSet<WebTemplateReview> WebTemplateReviews { get; set; }

        // Дипломы
        public DbSet<DiplomaDetail> DiplomaDetails { get; set; }
        public DbSet<DiplomaOrder> DiplomaOrders { get; set; }

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
            // Применяем конфигурации
            ConfigureProduct(builder);
            ConfigureCategory(builder);
            ConfigureProductCategory(builder);
            ConfigureCatalog(builder);
            ConfigureCourse(builder);
            ConfigureWebTemplate(builder);
            ConfigureDiploma(builder);

            // Сидирование данных
            SeedData(builder);
        }

        private void ConfigureProduct(ModelBuilder builder)
        {
            builder.Entity<Product>(entity =>
            {
                // Основная конфигурация
                entity.HasKey(p => p.Id);
                entity.HasIndex(p => p.Slug).IsUnique();
                entity.HasIndex(p => p.ProductType);
                entity.HasIndex(p => p.IsPublished);
                entity.HasIndex(p => p.CreatedAt);
                entity.HasIndex(p => p.Price);
                entity.HasIndex(p => p.AverageRating);
                entity.HasIndex(p => p.ViewsCount);

                // Свойства
                entity.Property(p => p.Title)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(p => p.Slug)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(p => p.ShortDescription)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(p => p.ThumbnailUrl)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(p => p.Price)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                entity.Property(p => p.OriginalPrice)
                    .HasColumnType("decimal(18,2)");

                entity.Property(p => p.AverageRating)
                    .HasColumnType("decimal(3,2)")
                    .HasDefaultValue(0);

                // Значения по умолчанию
                entity.Property(p => p.CreatedAt)
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(p => p.ViewsCount)
                    .HasDefaultValue(0);

                entity.Property(p => p.SalesCount)
                    .HasDefaultValue(0);

                entity.Property(p => p.ReviewsCount)
                    .HasDefaultValue(0);
            });
        }

        private void ConfigureCategory(ModelBuilder builder)
        {
            builder.Entity<Category>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.HasIndex(c => c.Slug).IsUnique();
                entity.HasIndex(c => c.IsActive);
                entity.HasIndex(c => c.SortOrder);

                entity.Property(c => c.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(c => c.Slug)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(c => c.Description)
                    .HasMaxLength(500);

                entity.Property(c => c.ImageUrl)
                    .HasMaxLength(500);

                entity.Property(c => c.IconUrl)
                    .HasMaxLength(500);

                entity.Property(c => c.MetaTitle)
                    .HasMaxLength(200);

                entity.Property(c => c.MetaDescription)
                    .HasMaxLength(500);

                entity.Property(c => c.MetaKeywords)
                    .HasMaxLength(300);

                entity.Property(c => c.CreatedAt)
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(c => c.UpdatedAt)
                    .HasDefaultValueSql("GETUTCDATE()");

                // Самоссылающаяся связь для иерархии
                entity.HasOne(c => c.ParentCategory)
                    .WithMany(c => c.SubCategories)
                    .HasForeignKey(c => c.ParentCategoryId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }

        private void ConfigureProductCategory(ModelBuilder builder)
        {
            builder.Entity<ProductCategory>(entity =>
            {
                // Композитный ключ
                entity.HasKey(pc => new { pc.ProductId, pc.CategoryId });

                // Связи
                entity.HasOne(pc => pc.Product)
                    .WithMany(p => p.ProductCategories)
                    .HasForeignKey(pc => pc.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(pc => pc.Category)
                    .WithMany(c => c.ProductCategories)
                    .HasForeignKey(pc => pc.CategoryId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Индексы
                entity.HasIndex(pc => pc.IsPrimary);
                entity.HasIndex(pc => pc.AssignedAt);

                entity.Property(pc => pc.AssignedAt)
                    .HasDefaultValueSql("GETUTCDATE()");
            });
        }

        private void ConfigureCatalog(ModelBuilder builder)
        {
            builder.Entity<Catalog>(entity =>
            {
                entity.HasKey(c => c.Id);

                entity.Property(c => c.Name)
                    .IsRequired()
                    .HasMaxLength(200)
                    .HasDefaultValue("Каталог товаров");

                entity.Property(c => c.Description)
                    .HasMaxLength(1000);

                entity.Property(c => c.CreatedAt)
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(c => c.UpdatedAt)
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(c => c.TotalViews)
                    .HasDefaultValue(0);
            });
        }

        private void ConfigureCourse(ModelBuilder builder)
        {
            // CourseDetail
            builder.Entity<CourseDetail>(entity =>
            {
                entity.HasKey(cd => cd.ProductId);

                entity.HasOne(cd => cd.Product)
                    .WithOne()
                    .HasForeignKey<CourseDetail>(cd => cd.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Property(cd => cd.FullDescription)
                    .IsRequired()
                    .HasColumnType("text");

                entity.Property(cd => cd.EstimatedHours)
                    .IsRequired();
            });

            // Module
            builder.Entity<Module>(entity =>
            {
                entity.HasKey(m => m.Id);
                entity.HasIndex(m => new { m.CourseId, m.Order });

                entity.Property(m => m.Title)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.HasOne(m => m.Course)
                    .WithMany(cd => cd.Modules)
                    .HasForeignKey(m => m.CourseId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Lesson
            builder.Entity<Lesson>(entity =>
            {
                entity.HasKey(l => l.Id);
                entity.HasIndex(l => new { l.ModuleId, l.Order });

                entity.Property(l => l.Title)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(l => l.Content)
                    .IsRequired()
                    .HasColumnType("text");

                entity.HasOne(l => l.Module)
                    .WithMany(m => m.Lessons)
                    .HasForeignKey(l => l.ModuleId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // CourseAssignment
            builder.Entity<CourseAssignment>(entity =>
            {
                entity.HasKey(ca => ca.Id);

                entity.Property(ca => ca.Title)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(ca => ca.Description)
                    .IsRequired()
                    .HasColumnType("text");

                entity.HasOne(ca => ca.Module)
                    .WithMany(m => m.Assignments)
                    .HasForeignKey(ca => ca.ModuleId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // UserAssignment
            builder.Entity<UserAssignment>(entity =>
            {
                entity.HasKey(ua => ua.Id);

                entity.Property(ua => ua.AnswerText)
                    .HasColumnType("text");

                entity.Property(ua => ua.FileUrl)
                    .HasMaxLength(500);

                entity.Property(ua => ua.SubmittedAt)
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.HasOne(ua => ua.CourseAssignment)
                    .WithMany(ca => ca.UserAssignments)
                    .HasForeignKey(ua => ua.AssignmentId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ua => ua.AppUser)
                    .WithMany()
                    .HasForeignKey(ua => ua.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // CourseEnrollment
            builder.Entity<CourseEnrollment>(entity =>
            {
                entity.HasKey(ce => ce.Id);
                entity.HasIndex(ce => new { ce.UserId, ce.CourseId }).IsUnique();

                entity.Property(ce => ce.EnrolledAt)
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.HasOne(ce => ce.AppUser)
                    .WithMany()
                    .HasForeignKey(ce => ce.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ce => ce.Course)
                    .WithMany()
                    .HasForeignKey(ce => ce.CourseId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }

        private void ConfigureWebTemplate(ModelBuilder builder)
        {
            // WebTemplateDetail
            builder.Entity<WebTemplateDetail>(entity =>
            {
                entity.HasKey(wtd => wtd.ProductId);

                entity.HasOne(wtd => wtd.Product)
                    .WithOne()
                    .HasForeignKey<WebTemplateDetail>(wtd => wtd.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Property(wtd => wtd.FullDescription)
                    .IsRequired()
                    .HasColumnType("text");

                entity.Property(wtd => wtd.Framework)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(wtd => wtd.Technologies)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(wtd => wtd.BrowserSupport)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(wtd => wtd.SourceCodeUrl)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(wtd => wtd.DemoUrl)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(wtd => wtd.DocumentationUrl)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(wtd => wtd.FigmaUrl)
                    .HasMaxLength(500);

                entity.Property(wtd => wtd.ColorScheme)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(wtd => wtd.FontsUsed)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(wtd => wtd.Features)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(wtd => wtd.Dependencies)
                    .HasMaxLength(500);

                entity.Property(wtd => wtd.InstallationInstructions)
                    .IsRequired()
                    .HasColumnType("text");

                entity.Property(wtd => wtd.License)
                    .HasMaxLength(100);
            });

            // WebTemplateOrder
            builder.Entity<WebTemplateOrder>(entity =>
            {
                entity.HasKey(wto => wto.Id);

                entity.Property(wto => wto.OrderDate)
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(wto => wto.Status)
                    .HasDefaultValue(OrderStatus.Pending);

                entity.Property(wto => wto.CustomerNotes)
                    .HasColumnType("text");

                entity.Property(wto => wto.AdminNotes)
                    .HasColumnType("text");

                entity.Property(wto => wto.PaidAmount)
                    .HasColumnType("decimal(18,2)");

                entity.HasOne(wto => wto.AppUser)
                    .WithMany()
                    .HasForeignKey(wto => wto.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(wto => wto.Template)
                    .WithMany(wtd => wtd.Orders)
                    .HasForeignKey(wto => wto.TemplateId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // WebTemplateReview
            builder.Entity<WebTemplateReview>(entity =>
            {
                entity.HasKey(wtr => wtr.Id);
                entity.HasIndex(wtr => new { wtr.TemplateId, wtr.UserId }).IsUnique();

                entity.Property(wtr => wtr.Rating)
                    .IsRequired()
                    .HasAnnotation("Range", new[] { 1, 5 });

                entity.Property(wtr => wtr.ReviewText)
                    .HasMaxLength(1000);

                entity.Property(wtr => wtr.CreatedAt)
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.HasOne(wtr => wtr.AppUser)
                    .WithMany()
                    .HasForeignKey(wtr => wtr.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(wtr => wtr.Template)
                    .WithMany(wtd => wtd.Reviews)
                    .HasForeignKey(wtr => wtr.TemplateId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }

        private void ConfigureDiploma(ModelBuilder builder)
        {
            // DiplomaDetail
            builder.Entity<DiplomaDetail>(entity =>
            {
                entity.HasKey(dd => dd.ProductId);

                entity.HasOne(dd => dd.Product)
                    .WithOne()
                    .HasForeignKey<DiplomaDetail>(dd => dd.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Property(dd => dd.FullDescription)
                    .IsRequired()
                    .HasColumnType("text");

                entity.Property(dd => dd.Subject)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(dd => dd.Specialization)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(dd => dd.Keywords)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(dd => dd.DocumentUrl)
                    .HasMaxLength(500);

                entity.Property(dd => dd.PresentationUrl)
                    .HasMaxLength(500);

                entity.Property(dd => dd.SourceCodeUrl)
                    .HasMaxLength(500);

                entity.Property(dd => dd.Bibliography)
                    .HasColumnType("text");

                entity.Property(dd => dd.Requirements)
                    .HasColumnType("text");

                entity.Property(dd => dd.PlagiarismPercentage)
                    .HasColumnType("decimal(5,2)");
            });

            // DiplomaOrder
            builder.Entity<DiplomaOrder>(entity =>
            {
                entity.HasKey(d => d.Id);

                entity.Property(d => d.OrderDate)
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(d => d.Status)
                    .HasDefaultValue(OrderStatus.Pending);

                entity.Property(d => d.CustomerNotes)
                    .HasColumnType("text");

                entity.Property(d => d.AdminNotes)
                    .HasColumnType("text");

                entity.Property(d => d.PaidAmount)
                    .HasColumnType("decimal(18,2)");

                entity.HasOne(d => d.AppUser)
                    .WithMany()
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Diploma)
                    .WithMany(dd => dd.Orders)
                    .HasForeignKey(d => d.DiplomaId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }

        private void SeedData(ModelBuilder builder)
        {
            // Сидирование каталога (singleton)
            builder.Entity<Catalog>().HasData(
                new Catalog
                {
                    Id = 1,
                    Name = "Каталог товаров",
                    Description = "Каталог курсов, веб-шаблонов и дипломных работ",
                    TotalViews = 0,
                    CreatedAt = new DateTime(2024, 10, 12),
                    UpdatedAt = new DateTime(2024, 10, 12)
                }
            );

            // Сидирование основных категорий
            var categoryIds = new
            {
                Programming = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                WebDevelopment = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                MobileDevelopment = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                DataScience = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                WebTemplates = Guid.Parse("55555555-5555-5555-5555-555555555555"),
                Diplomas = Guid.Parse("66666666-6666-6666-6666-666666666666")
            };

            builder.Entity<Category>().HasData(
                new Category
                {
                    Id = categoryIds.Programming,
                    Name = "Программирование",
                    Slug = "programming",
                    Description = "Курсы по программированию и разработке ПО",
                    IsActive = true,
                    IsVisible = true,
                    SortOrder = 1,
                    MetaTitle = "Курсы программирования",
                    MetaDescription = "Изучите программирование с нашими курсами",
                    CreatedAt = new DateTime(2024, 10, 12),
                    UpdatedAt = new DateTime(2024, 10, 12)
                },
                new Category
                {
                    Id = categoryIds.WebDevelopment,
                    Name = "Веб-разработка",
                    Slug = "web-development",
                    Description = "Курсы по созданию веб-сайтов и веб-приложений",
                    ParentCategoryId = categoryIds.Programming,
                    IsActive = true,
                    IsVisible = true,
                    SortOrder = 1,
                    MetaTitle = "Курсы веб-разработки",
                    MetaDescription = "Освойте создание современных веб-приложений",
                    CreatedAt = new DateTime(2024, 10, 12),
                    UpdatedAt = new DateTime(2024, 10, 12)
                },
                new Category
                {
                    Id = categoryIds.MobileDevelopment,
                    Name = "Мобильная разработка",
                    Slug = "mobile-development",
                    Description = "Курсы по созданию мобильных приложений",
                    ParentCategoryId = categoryIds.Programming,
                    IsActive = true,
                    IsVisible = true,
                    SortOrder = 2,
                    MetaTitle = "Курсы мобильной разработки",
                    MetaDescription = "Создавайте мобильные приложения для iOS и Android",
                    CreatedAt = new DateTime(2024, 10, 12),
                    UpdatedAt = new DateTime(2024, 10, 12)
                },
                new Category
                {
                    Id = categoryIds.DataScience,
                    Name = "Data Science",
                    Slug = "data-science",
                    Description = "Курсы по анализу данных и машинному обучению",
                    IsActive = true,
                    IsVisible = true,
                    SortOrder = 2,
                    MetaTitle = "Курсы Data Science",
                    MetaDescription = "Изучите анализ данных и машинное обучение",
                    CreatedAt = new DateTime(2024, 10, 12),
                    UpdatedAt = new DateTime(2024, 10, 12)
                },
                new Category
                {
                    Id = categoryIds.WebTemplates,
                    Name = "Веб-шаблоны",
                    Slug = "web-templates",
                    Description = "Готовые шаблоны для веб-сайтов",
                    IsActive = true,
                    IsVisible = true,
                    SortOrder = 3,
                    MetaTitle = "Веб-шаблоны",
                    MetaDescription = "Скачайте готовые шаблоны для ваших проектов",
                    CreatedAt = new DateTime(2024, 10, 12),
                    UpdatedAt = new DateTime(2024, 10, 12)
                },
                new Category
                {
                    Id = categoryIds.Diplomas,
                    Name = "Дипломные работы",
                    Slug = "diploma-works",
                    Description = "Готовые дипломные и курсовые работы",
                    IsActive = true,
                    IsVisible = true,
                    SortOrder = 4,
                    MetaTitle = "Дипломные работы",
                    MetaDescription = "Качественные дипломные работы по программированию",
                    CreatedAt = new DateTime(2024, 10, 12),
                    UpdatedAt = new DateTime(2024, 10, 12)
                }
            );
        }
    }
}