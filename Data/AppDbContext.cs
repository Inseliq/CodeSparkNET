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
                      .HasForeignKey(pi => pi.ProductId)
                      .OnDelete(DeleteBehavior.Restrict);
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


            // ---------------------------
            // Seed data: Catalogs, Course/Template/Diploma, ProductImages
            // ---------------------------
            // Catalogs
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

            // Seed: Course 
            builder.Entity<Course>().HasData(
                new
                {
                    Id = "e1f9c6d2-5b4b-4b8e-9f2a-bbbb00000001",
                    CatalogId = "d1f9c6d2-4b4b-4b8e-9f2a-aaaa00000001",
                    Name = "Курс C# с нуля",
                    Slug = "c#-for-beginners",
                    Price = 1999.99m,
                    Level = "Beginner",
                    ProductType = "Course",
                    Currency = "RUB",
                    InStock = 10,
                    IsPublished = true
                }
            );

            // Seed: Template
            builder.Entity<Template>().HasData(
                new
                {
                    Id = "e1f9c6d2-5b4b-4b8e-9f2a-bbbb00000002",
                    CatalogId = "d1f9c6d2-4b4b-4b8e-9f2a-aaaa00000002",
                    Name = "Трехуровневая заготовка ASP.NET MVC",
                    Slug = "3-tier-web-template",
                    Price = 1299.00m,
                    ProductType = "Template",
                    Currency = "RUB",
                    InStock = 10,
                    IsPublished = true
                }
            );

            // Seed: Diploma
            builder.Entity<Diploma>().HasData(
                new
                {
                    Id = "e1f9c6d2-5b4b-4b8e-9f2a-bbbb00000003",
                    CatalogId = "d1f9c6d2-4b4b-4b8e-9f2a-aaaa00000003",
                    Name = "Дипломная работа",
                    Slug = "diploma-work",
                    Price = 30000m,
                    Issuer = "Code Spark Academy",
                    ProductType = "Diploma",
                    Currency = "RUB",
                    InStock = 10,
                    IsPublished = true
                }
            );

            // ProductImage seeds 
            builder.Entity<ProductImage>().HasData(
                new ProductImage
                {
                    Id = "f1f9c6d2-6b4b-4b8e-9f2a-cccc00000001",
                    ProductId = "e1f9c6d2-5b4b-4b8e-9f2a-bbbb00000001",
                    Name = "itcubic-main.jpg",
                    Url = "https://cdn.example.com/courses/csharp/itcubic-main.jpg",
                    IsMain = true,
                    Position = 0
                },

            new ProductImage
                {
                    Id = "f1f9c6d2-6b4b-4b8e-9f2a-cccc00000002",
                    ProductId = "e1f9c6d2-5b4b-4b8e-9f2a-bbbb00000002",
                    Name = "codespark-main.jpg",
                    Url = null,
                    IsMain = true,
                    Position = 0
                },
                new ProductImage
                {
                    Id = "f1f9c6d2-6b4b-4b8e-9f2a-cccc00000003",
                    ProductId = "e1f9c6d2-5b4b-4b8e-9f2a-bbbb00000003",
                    Name = "montazhka-main.jpg",
                    Url = null,
                    IsMain = true,
                    Position = 0
                }
            );

            // ---------- Seed sample module + lesson + resource for the seeded course ----------
            var moduleId = "a1111111-1111-1111-1111-aaaaaaaaaaa1";
            var lessonId = "b2222222-2222-2222-2222-bbbbbbbbbbb2";
            var resourceId = "c3333333-3333-3333-3333-ccccccccccc3";

            // CourseId
            var seededCourseId = "e1f9c6d2-5b4b-4b8e-9f2a-bbbb00000001";

            // Модули
            builder.Entity<CourseModule>().HasData(
                new
                {
                    Id = "a1111111-1111-1111-1111-aaaaaaaaaaa2",
                    CourseId = seededCourseId,
                    Title = "Основы C#",
                    Description = "Переменные, типы, операторы и базовые конструкции управления потоком.",
                    Position = 1
                },
                new
                {
                    Id = "a1111111-1111-1111-1111-aaaaaaaaaaa3",
                    CourseId = seededCourseId,
                    Title = "Объектно-ориентированное программирование",
                    Description = "Классы, наследование, полиморфизм, интерфейсы и SOLID-принципы.",
                    Position = 2
                },
                new
                {
                    Id = "a1111111-1111-1111-1111-aaaaaaaaaaa4",
                    CourseId = seededCourseId,
                    Title = "Коллекции и LINQ",
                    Description = "Списки, словари, множества и мощный синтаксис LINQ для обработки данных.",
                    Position = 3
                },
                new
                {
                    Id = "a1111111-1111-1111-1111-aaaaaaaaaaa5",
                    CourseId = seededCourseId,
                    Title = "Асинхронность и ввод/вывод",
                    Description = "async/await, Task, работа с файлами и сетью.",
                    Position = 4
                },
                new
                {
                    Id = "a1111111-1111-1111-1111-aaaaaaaaaaa6",
                    CourseId = seededCourseId,
                    Title = "Веб и Entity Framework Core",
                    Description = "Создание простого веб-приложения на ASP.NET Core и работа с EF Core.",
                    Position = 5
                }
            );

            // Уроки для модуля "Основы C#"
            builder.Entity<Lesson>().HasData(
                new
                {
                    Id = "b2222222-2222-2222-2222-bbbbbbbbbbb3",
                    ModuleId = "a1111111-1111-1111-1111-aaaaaaaaaaa2",
                    Title = "Переменные и типы данных",
                    Slug = "variables-and-types",
                    Body = "<h4>Переменные и типы</h4><p>В C# есть примитивные типы: <strong>int, long, double, bool, char</strong> и ссылочные типы: <em>string, object</em>. Объявление: <code>int x = 5;</code>. Приведение типов: <code>(int)3.14</code> или <code>Convert.ToInt32()</code>.</p><p>Практика: создайте консольное приложение, попросите пользователя ввести два числа и выведите их сумму.</p>",
                    Position = 0,
                    DurationMinutes = 12,
                    IsPublished = true,
                    IsFreePreview = true
                },
                new
                {
                    Id = "b2222222-2222-2222-2222-bbbbbbbbbbb4",
                    ModuleId = "a1111111-1111-1111-1111-aaaaaaaaaaa2",
                    Title = "Управляющие конструкции (if, switch, loops)",
                    Slug = "control-flow",
                    Body = "<h4>Управляющие конструкции</h4><p>if/else, switch, while, for, foreach. Примеры и типичные ошибки (бесконечные циклы, неверные границы).</p><p>Задача: напишите программу, которая считает факториал числа с помощью цикла и рекурсии.</p>",
                    Position = 1,
                    DurationMinutes = 18,
                    IsPublished = true,
                    IsFreePreview = true
                },
                new
                {
                    Id = "b2222222-2222-2222-2222-bbbbbbbbbbb5",
                    ModuleId = "a1111111-1111-1111-1111-aaaaaaaaaaa2",
                    Title = "Методы и области видимости",
                    Slug = "methods-and-scope",
                    Body = "<h4>Методы и области видимости</h4><p>Определение методов, возвращаемые типы, параметры (ref, out), локальные переменные, статические методы.</p><p>Пример: <code>public int Sum(int a, int b) => a + b;</code></p>",
                    Position = 2,
                    DurationMinutes = 14,
                    IsPublished = true,
                    IsFreePreview = false
                },
                new
                {
                    Id = "b2222222-2222-2222-2222-bbbbbbbbbbb6",
                    ModuleId = "a1111111-1111-1111-1111-aaaaaaaaaaa2",
                    Title = "Строки и работа с текстом",
                    Slug = "strings-and-text",
                    Body = "<h4>Строки и форматирование</h4><p>Класс <code>string</code>, интерполяция, StringBuilder для больших объёмов, регулярные выражения, базовые операции (Split, Replace, Trim).</p>",
                    Position = 3,
                    DurationMinutes = 16,
                    IsPublished = true,
                    IsFreePreview = false
                }
            );

            // Уроки для модуля "ОOP"
            builder.Entity<Lesson>().HasData(
                new
                {
                    Id = "b2222222-2222-2222-2222-bbbbbbbbbbb7",
                    ModuleId = "a1111111-1111-1111-1111-aaaaaaaaaaa3",
                    Title = "Классы и объекты",
                    Slug = "classes-and-objects",
                    Body = "<h4>Классы и объекты</h4><p>Определение класса, поля, свойства, конструкторы. Создание экземпляров и доступ к членам класса.</p><p>Пример: класс <code>Person</code> с именем и методом <code>Greet()</code>.</p>",
                    Position = 0,
                    DurationMinutes = 20,
                    IsPublished = true,
                    IsFreePreview = false
                },
                new
                {
                    Id = "b2222222-2222-2222-2222-bbbbbbbbbbb8",
                    ModuleId = "a1111111-1111-1111-1111-aaaaaaaaaaa3",
                    Title = "Наследование и полиморфизм",
                    Slug = "inheritance-and-polymorphism",
                    Body = "<h4>Наследование</h4><p>Базовые и производные классы, модификаторы доступа, virtual/override, абстрактные классы и интерфейсы.</p><p>Задача: реализовать иерархию <code>Animal</code> -> <code>Dog</code>, <code>Cat</code> и метод <code>MakeSound()</code>.</p>",
                    Position = 1,
                    DurationMinutes = 22,
                    IsPublished = true,
                    IsFreePreview = false
                },
                new
                {
                    Id = "b2222222-2222-2222-2222-bbbbbbbbbbb9",
                    ModuleId = "a1111111-1111-1111-1111-aaaaaaaaaaa3",
                    Title = "Интерфейсы и композиция",
                    Slug = "interfaces-and-composition",
                    Body = "<h4>Интерфейсы и композиция</h4><p>Когда использовать интерфейсы, композицию vs наследование, dependency inversion.</p>",
                    Position = 2,
                    DurationMinutes = 18,
                    IsPublished = true,
                    IsFreePreview = false
                },
                new
                {
                    Id = "b2222222-2222-2222-2222-bbbbbbbbb10",
                    ModuleId = "a1111111-1111-1111-1111-aaaaaaaaaaa3",
                    Title = "SOLID-принципы в примерах",
                    Slug = "solid-principles",
                    Body = "<h4>SOLID</h4><p>Краткий обзор каждого принципа с примерами: Single Responsibility, Open/Closed, Liskov, Interface Segregation, Dependency Inversion.</p>",
                    Position = 3,
                    DurationMinutes = 24,
                    IsPublished = true,
                    IsFreePreview = false
                }
            );

            // Уроки для модуля "Коллекции и LINQ"
            builder.Entity<Lesson>().HasData(
                new
                {
                    Id = "b2222222-2222-2222-2222-bbbbbbbbb11",
                    ModuleId = "a1111111-1111-1111-1111-aaaaaaaaaaa4",
                    Title = "List, Dictionary, HashSet",
                    Slug = "collections-basic",
                    Body = "<h4>Коллекции</h4><p>List<T>, Dictionary<TKey,TValue>, HashSet<T> — сценарии использования, сложности операций, типичные методы (Add, Remove, Contains).</p>",
                    Position = 0,
                    DurationMinutes = 20,
                    IsPublished = true,
                    IsFreePreview = true
                },
                new
                {
                    Id = "b2222222-2222-2222-2222-bbbbbbbbb12",
                    ModuleId = "a1111111-1111-1111-1111-aaaaaaaaaaa4",
                    Title = "Основы LINQ",
                    Slug = "linq-intro",
                    Body = "<h4>LINQ</h4><p>Методы расширения: Where, Select, OrderBy, GroupBy. Синтаксис query vs method chain. Примеры выборок.</p>",
                    Position = 1,
                    DurationMinutes = 26,
                    IsPublished = true,
                    IsFreePreview = true
                },
                new
                {
                    Id = "b2222222-2222-2222-2222-bbbbbbbbb13",
                    ModuleId = "a1111111-1111-1111-1111-aaaaaaaaaaa4",
                    Title = "Продвинутый LINQ и производительность",
                    Slug = "linq-advanced",
                    Body = "<h4>Продвинутый LINQ</h4><p>Оператор Deferred Execution, разные реализации (Enumerable vs Queryable), оптимизация выражений и предотвращение N+1 в EF.</p>",
                    Position = 2,
                    DurationMinutes = 28,
                    IsPublished = true,
                    IsFreePreview = false
                },
                new
                {
                    Id = "b2222222-2222-2222-2222-bbbbbbbbb14",
                    ModuleId = "a1111111-1111-1111-1111-aaaaaaaaaaa4",
                    Title = "Практика: обработка CSV и выборки",
                    Slug = "linq-practice",
                    Body = "<h4>Практическое задание</h4><p>Парсинг CSV в объекты, фильтрация, группировка и вывод топ-N записей с помощью LINQ.</p>",
                    Position = 3,
                    DurationMinutes = 30,
                    IsPublished = true,
                    IsFreePreview = false
                }
            );

            // Уроки для модуля "Асинхронность и ввод/вывод"
            builder.Entity<Lesson>().HasData(
                new
                {
                    Id = "b2222222-2222-2222-2222-bbbbbbbbb15",
                    ModuleId = "a1111111-1111-1111-1111-aaaaaaaaaaa5",
                    Title = "Task, async/await — основы",
                    Slug = "async-await-basics",
                    Body = "<h4>Async/Await</h4><p>Понимание Task, async/await, возвращаемые типы Task/Task&lt;T&gt;, блокировки потоков, примеры асинхронных методов.</p>",
                    Position = 0,
                    DurationMinutes = 22,
                    IsPublished = true,
                    IsFreePreview = true
                },
                new
                {
                    Id = "b2222222-2222-2222-2222-bbbbbbbbb16",
                    ModuleId = "a1111111-1111-1111-1111-aaaaaaaaaaa5",
                    Title = "Работа с файлами и потоками",
                    Slug = "file-io",
                    Body = "<h4>Файловый ввод/вывод</h4><p>FileStream, StreamReader/Writer, async-версии методов (ReadAsync, WriteAsync), буферизация и ошибки IO.</p>",
                    Position = 1,
                    DurationMinutes = 20,
                    IsPublished = true,
                    IsFreePreview = false
                },
                new
                {
                    Id = "b2222222-2222-2222-2222-bbbbbbbbb17",
                    ModuleId = "a1111111-1111-1111-1111-aaaaaaaaaaa5",
                    Title = "HTTP-запросы и HttpClient",
                    Slug = "http-httpclient",
                    Body = "<h4>HttpClient</h4><p>Как делать запросы, парсить JSON, правильное повторное использование HttpClient и обработка ошибок/таймаутов.</p>",
                    Position = 2,
                    DurationMinutes = 26,
                    IsPublished = true,
                    IsFreePreview = false
                },
                new
                {
                    Id = "b2222222-2222-2222-2222-bbbbbbbbb18",
                    ModuleId = "a1111111-1111-1111-1111-aaaaaaaaaaa5",
                    Title = "Отмена задач и таймауты",
                    Slug = "cancellation-and-timeouts",
                    Body = "<h4>Отмена задач</h4><p>CancellationToken, примеры отмены долгих операций, паттерны правильной обработки таймаутов.</p>",
                    Position = 3,
                    DurationMinutes = 18,
                    IsPublished = true,
                    IsFreePreview = false
                }
            );

            // Уроки для модуля "Веб и EF Core"
            builder.Entity<Lesson>().HasData(
                new
                {
                    Id = "b2222222-2222-2222-2222-bbbbbbbbb19",
                    ModuleId = "a1111111-1111-1111-1111-aaaaaaaaaaa6",
                    Title = "Введение в ASP.NET Core",
                    Slug = "intro-aspnetcore",
                    Body = "<h4>ASP.NET Core</h4><p>Структура проекта, Startup/Program, middleware pipeline, контроллеры и Razor Pages.</p>",
                    Position = 0,
                    DurationMinutes = 30,
                    IsPublished = true,
                    IsFreePreview = true
                },
                new
                {
                    Id = "b2222222-2222-2222-2222-bbbbbbbbb20",
                    ModuleId = "a1111111-1111-1111-1111-aaaaaaaaaaa6",
                    Title = "Entity Framework Core — модели и миграции",
                    Slug = "efcore-models-migrations",
                    Body = "<h4>EF Core</h4><p>DbContext, сущности, миграции, отношения, проекции и отладка SQL.</p>",
                    Position = 1,
                    DurationMinutes = 28,
                    IsPublished = true,
                    IsFreePreview = false
                },
                new
                {
                    Id = "b2222222-2222-2222-2222-bbbbbbbbb21",
                    ModuleId = "a1111111-1111-1111-1111-aaaaaaaaaaa6",
                    Title = "Dependency Injection и конфигурация",
                    Slug = "di-and-configuration",
                    Body = "<h4>DI и конфигурация</h4><p>Регистрация сервисов, позволяют тестировать и упрощать код, IConfiguration и привязка настроек.</p>",
                    Position = 2,
                    DurationMinutes = 20,
                    IsPublished = true,
                    IsFreePreview = false
                },
                new
                {
                    Id = "b2222222-2222-2222-2222-bbbbbbbbb22",
                    ModuleId = "a1111111-1111-1111-1111-aaaaaaaaaaa6",
                    Title = "Развертывание и базовые рекомендации по безопасности",
                    Slug = "deployment-and-security",
                    Body = "<h4>Развертывание</h4><p>Публикация, конфигурации окружения, HTTPS, настройка CORS и основы безопасности (хранение секретов, защита от XSS/CSRF).</p>",
                    Position = 3,
                    DurationMinutes = 32,
                    IsPublished = true,
                    IsFreePreview = false
                }
            );

            // Ресурсы (несколько примеров)
            builder.Entity<LessonResource>().HasData(
                new
                {
                    Id = "c3333333-3333-3333-3333-ccccccccccc4",
                    LessonId = "b2222222-2222-2222-2222-bbbbbbbbbbb4", // control-flow
                    Url = "https://docs.microsoft.com/dotnet/csharp/programming-guide/inside-a-program",
                    ResourceType = "article",
                    Title = "Документация Microsoft: Inside a Program",
                    Position = 0
                },
                new
                {
                    Id = "c3333333-3333-3333-3333-ccccccccccc5",
                    LessonId = "b2222222-2222-2222-2222-bbbbbbbbbbb8", // inheritance
                    Url = "https://docs.microsoft.com/dotnet/csharp/fundamentals/object-oriented/",
                    ResourceType = "article",
                    Title = "OOP в C# — официальная документация",
                    Position = 0
                },
                new
                {
                    Id = "c3333333-3333-3333-3333-ccccccccccc6",
                    LessonId = "b2222222-2222-2222-2222-bbbbbbbbb12", // linq-intro
                    Url = "https://docs.microsoft.com/dotnet/csharp/programming-guide/concepts/linq/",
                    ResourceType = "article",
                    Title = "LINQ — руководство",
                    Position = 0
                },
                new
                {
                    Id = "c3333333-3333-3333-3333-ccccccccccc7",
                    LessonId = "b2222222-2222-2222-2222-bbbbbbbbb17", // http-httpclient
                    Url = "https://learn.microsoft.com/dotnet/api/system.net.http.httpclient",
                    ResourceType = "article",
                    Title = "HttpClient — справочник",
                    Position = 0
                },
                new
                {
                    Id = "c3333333-3333-3333-3333-ccccccccccc8",
                    LessonId = "b2222222-2222-2222-2222-bbbbbbbbb20", // efcore
                    Url = "https://learn.microsoft.com/ef/core/",
                    ResourceType = "article",
                    Title = "EF Core — официальная документация",
                    Position = 0
                }
            );


        }
    }
}
