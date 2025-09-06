using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeSparkNET.Models;
using CodeSparkNET.Models.Enum;
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

            // AppUser конфигурация (Identity уже настроен базово)
            builder.Entity<AppUser>(entity =>
            {
                entity.Property(e => e.FullName)
                    .HasMaxLength(100);

                entity.Property(e => e.RegisteredAt)
                    .HasDefaultValueSql("GETUTCDATE()");
            });

            // Product конфигурация
            builder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Slug)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.HasIndex(e => e.Slug)
                    .IsUnique();

                entity.Property(e => e.Price)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                entity.Property(e => e.ShortDescription)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.ThumbnailUrl)
                    .IsRequired()
                    .HasMaxLength(300);

                entity.Property(e => e.ProductType)
                    .IsRequired();

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("GETUTCDATE()");
            });

            // CourseDetail конфигурация
            builder.Entity<CourseDetail>(entity =>
            {
                entity.HasKey(e => e.ProductId);

                entity.Property(e => e.FullDescription)
                    .IsRequired();

                entity.Property(e => e.EstimatedHours)
                    .IsRequired();

                // Связь 1:1 с Product
                entity.HasOne(cd => cd.Product)
                    .WithOne()
                    .HasForeignKey<CourseDetail>(cd => cd.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Module конфигурация
            builder.Entity<Module>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Order)
                    .IsRequired();

                // Связь с CourseDetail
                entity.HasOne(m => m.Course)
                    .WithMany(cd => cd.Modules)
                    .HasForeignKey(m => m.CourseId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Lesson конфигурация
            builder.Entity<Lesson>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Content)
                    .IsRequired();

                entity.Property(e => e.Order)
                    .IsRequired();

                // Связь с Module
                entity.HasOne(l => l.Module)
                    .WithMany(m => m.Lessons)
                    .HasForeignKey(l => l.ModuleId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // CourseAssignment конфигурация
            builder.Entity<CourseAssignment>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Description)
                    .IsRequired();

                entity.Property(e => e.DeadLine)
                    .IsRequired();

                // Связь с Module
                entity.HasOne(ca => ca.Module)
                    .WithMany(m => m.Assignments)
                    .HasForeignKey(ca => ca.ModuleId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // CourseEnrollment конфигурация
            builder.Entity<CourseEnrollment>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.EnrolledAt)
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(e => e.IsCompleted)
                    .HasDefaultValue(false);

                // Связь с AppUser
                entity.HasOne(ce => ce.AppUser)
                    .WithMany(u => u.Enrollments)
                    .HasForeignKey(ce => ce.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Связь с CourseDetail
                entity.HasOne(ce => ce.Course)
                    .WithMany()
                    .HasForeignKey(ce => ce.CourseId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Составной индекс для предотвращения дублирования записей
                entity.HasIndex(e => new { e.UserId, e.CourseId })
                    .IsUnique()
                    .HasDatabaseName("IX_CourseEnrollment_User_Course");
            });

            // UserAssignment конфигурация
            builder.Entity<UserAssignment>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.AnswerText)
                    .HasMaxLength(2000);

                entity.Property(e => e.FileUrl)
                    .HasMaxLength(300);

                entity.Property(e => e.SubmittedAt)
                    .HasDefaultValueSql("GETUTCDATE()");

                // Связь с CourseAssignment
                entity.HasOne(ua => ua.CourseAssignment)
                    .WithMany(ca => ca.UserAssignments)
                    .HasForeignKey(ua => ua.AssignmentId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Связь с AppUser
                entity.HasOne(ua => ua.AppUser)
                    .WithMany(u => u.Assignments)
                    .HasForeignKey(ua => ua.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Составной индекс для предотвращения дублирования
                entity.HasIndex(e => new { e.UserId, e.AssignmentId })
                    .IsUnique()
                    .HasDatabaseName("IX_UserAssignment_User_Assignment");
            });

            // Дополнительные индексы для производительности
            builder.Entity<Module>()
                .HasIndex(m => new { m.CourseId, m.Order })
                .HasDatabaseName("IX_Module_Course_Order");

            builder.Entity<Lesson>()
                .HasIndex(l => new { l.ModuleId, l.Order })
                .HasDatabaseName("IX_Lesson_Module_Order");

            builder.Entity<CourseEnrollment>()
                .HasIndex(ce => ce.EnrolledAt)
                .HasDatabaseName("IX_CourseEnrollment_EnrolledAt");

            builder.Entity<UserAssignment>()
                .HasIndex(ua => ua.SubmittedAt)
                .HasDatabaseName("IX_UserAssignment_SubmittedAt");

            // Дополнительные индексы для новых моделей
            builder.Entity<DiplomaOrder>()
                .HasIndex(do_ => do_.OrderDate)
                .HasDatabaseName("IX_DiplomaOrder_OrderDate");

            builder.Entity<DiplomaOrder>()
                .HasIndex(do_ => do_.Status)
                .HasDatabaseName("IX_DiplomaOrder_Status");

            builder.Entity<WebTemplateOrder>()
                .HasIndex(to_ => to_.OrderDate)
                .HasDatabaseName("IX_TemplateOrder_OrderDate");

            builder.Entity<WebTemplateOrder>()
                .HasIndex(to_ => to_.Status)
                .HasDatabaseName("IX_TemplateOrder_Status");

            builder.Entity<WebTemplateReview>()
                .HasIndex(tr => tr.CreatedAt)
                .HasDatabaseName("IX_TemplateReview_CreatedAt");

            builder.Entity<WebTemplateReview>()
                .HasIndex(tr => new { tr.TemplateId, tr.Rating })
                .HasDatabaseName("IX_TemplateReview_Template_Rating");

            builder.Entity<DiplomaDetail>()
                .HasIndex(dd => dd.DiplomaType)
                .HasDatabaseName("IX_DiplomaDetail_DiplomaType");

            builder.Entity<WebTemplateDetail>()
                .HasIndex(wtd => wtd.Framework)
                .HasDatabaseName("IX_WebTemplateDetail_Framework");

            // Seed данные для тестирования - используем статические значения
            #region Seed Data

            // Создаем Products с фиксированными датами
            var courseProductId = Guid.Parse("11111111-1111-1111-1111-111111111111");
            var diplomaProductId = Guid.Parse("22222222-2222-2222-2222-222222222222");
            var templateProductId = Guid.Parse("33333333-3333-3333-3333-333333333333");

            builder.Entity<Product>().HasData(
                // Курс
                new Product
                {
                    Id = courseProductId,
                    Title = "Полный курс JavaScript с нуля до профи",
                    Slug = "javascript-full-course",
                    Price = 4999.00m,
                    ShortDescription = "Изучите JavaScript от основ до продвинутых концепций. Практические проекты, ES6+, асинхронное программирование.",
                    ThumbnailUrl = "/images/courses/javascript-course.jpg",
                    ProductType = ProductType.Course,
                    IsPublished = true,
                    CreatedAt = new DateTime(2024, 1, 15, 0, 0, 0, DateTimeKind.Utc)
                },
                // Диплом
                new Product
                {
                    Id = diplomaProductId,
                    Title = "Разработка веб-приложения для управления проектами",
                    Slug = "project-management-web-app-thesis",
                    Price = 25000.00m,
                    ShortDescription = "Дипломная работа по разработке полнофункционального веб-приложения с использованием ASP.NET Core и React.",
                    ThumbnailUrl = "/images/diplomas/project-management-thesis.jpg",
                    ProductType = ProductType.Diploma,
                    IsPublished = true,
                    CreatedAt = new DateTime(2024, 2, 10, 0, 0, 0, DateTimeKind.Utc)
                },
                // Веб-шаблон
                new Product
                {
                    Id = templateProductId,
                    Title = "ModernLanding - Современный лендинг для IT-компании",
                    Slug = "modern-it-landing-template",
                    Price = 2999.00m,
                    ShortDescription = "Стильный адаптивный лендинг с анимациями, темной темой и современным дизайном. HTML5, CSS3, JavaScript.",
                    ThumbnailUrl = "/images/templates/modern-landing.jpg",
                    ProductType = ProductType.Template,
                    IsPublished = true,
                    CreatedAt = new DateTime(2024, 3, 5, 0, 0, 0, DateTimeKind.Utc)
                }
            );

            // CourseDetail для курса JavaScript
            builder.Entity<CourseDetail>().HasData(
                new CourseDetail
                {
                    ProductId = courseProductId,
                    FullDescription = @"Этот курс предназначен для всех, кто хочет освоить JavaScript с нуля и стать профессиональным разработчиком.

**Что вы изучите:**
- Основы JavaScript: переменные, функции, объекты
- ES6+ возможности: стрелочные функции, деструктуризация, модули
- DOM манипуляции и обработка событий
- Асинхронное программирование: Promises, async/await
- Работа с API и AJAX запросами
- Современные фреймворки и библиотеки
- Тестирование JavaScript кода
- Оптимизация и лучшие практики

**Проекты в курсе:**
1. Калькулятор с расширенными функциями
2. Todo-приложение с localStorage
3. Погодное приложение с API
4. Интерактивная игра
5. Мини-социальная сеть

Курс включает более 50 практических заданий и 5 крупных проектов для портфолио.",
                    EstimatedHours = 120
                }
            );

            // DiplomaDetail для дипломной работы
            builder.Entity<DiplomaDetail>().HasData(
                new DiplomaDetail
                {
                    ProductId = diplomaProductId,
                    FullDescription = @"Дипломная работа представляет собой полнофункциональное веб-приложение для управления проектами и задачами в команде.

**Основные возможности системы:**
- Регистрация и авторизация пользователей
- Создание и управление проектами
- Система ролей (администратор, менеджер, разработчик)
- Трекинг времени выполнения задач
- Канбан-доски для визуализации процесса
- Система уведомлений и комментариев
- Отчеты и аналитика
- Интеграция с внешними API

**Технический стек:**
- Backend: ASP.NET Core 6.0, Entity Framework Core
- Frontend: React 18, TypeScript, Material-UI
- База данных: SQL Server
- Аутентификация: JWT токены
- Тестирование: xUnit, Jest
- Развертывание: Docker, Azure

Работа включает полную техническую документацию, диаграммы архитектуры, тестирование и презентацию.",
                    Subject = "Информационные системы и технологии",
                    DiplomaType = DiplomaType.Thesis,
                    Specialization = "09.03.02 Информационные системы и технологии",
                    Keywords = "веб-приложение, управление проектами, ASP.NET Core, React, Entity Framework, система управления задачами",
                    DocumentUrl = "/files/diplomas/project-management-app-thesis.pdf",
                    PresentationUrl = "/files/diplomas/project-management-app-presentation.pptx",
                    SourceCodeUrl = "/files/diplomas/project-management-app-source.zip",
                    Bibliography = @"1. Троelsen Э. C# 10 и .NET 6. Полное руководство. - СПб.: Питер, 2022.
2. Фримен А. ASP.NET Core MVC. Полное руководство. - СПб.: Питер, 2021.
3. Гринс Р. React быстро. Веб-приложения на React, JSX, Redux и GraphQL. - СПб.: Питер, 2020.
4. Microsoft Docs. ASP.NET Core Documentation. - URL: https://docs.microsoft.com/aspnet/core/
5. React Documentation. - URL: https://reactjs.org/docs/",
                    Requirements = "Система должна поддерживать не менее 100 одновременных пользователей, обеспечивать безопасность данных и иметь адаптивный интерфейс.",
                    CreatedYear = new DateTime(2024, 1, 1),
                    HasPlagiarismCheck = true,
                    PlagiarismPercentage = 92.5m
                }
            );

            // WebTemplateDetail для веб-шаблона
            builder.Entity<WebTemplateDetail>().HasData(
                new WebTemplateDetail
                {
                    ProductId = templateProductId,
                    FullDescription = @"ModernLanding - это современный и стильный шаблон лендинга, специально разработанный для IT-компаний, стартапов и технологических проектов.

**Особенности дизайна:**
- Минималистичный и современный стиль
- Градиентные элементы и плавные анимации
- Темная и светлая темы
- Адаптивный дизайн для всех устройств
- Высокая скорость загрузки

**Секции шаблона:**
1. Hero секция с анимированным фоном
2. О компании с интерактивными карточками
3. Услуги с hover-эффектами
4. Портфолио с фильтрацией
5. Команда с социальными ссылками
6. Отзывы клиентов (слайдер)
7. Контакты с интерактивной картой
8. Подвал с полезными ссылками

**Технические возможности:**
- Плавная прокрутка между секциями
- Параллакс эффекты
- Анимации при скролле (AOS)
- Валидация контактной формы
- Интеграция с Google Analytics
- SEO оптимизация

Шаблон готов к использованию и легко кастомизируется под ваши потребности.",
                    Framework = "Vanilla JavaScript",
                    Technologies = "HTML5, CSS3, JavaScript ES6+, SCSS, Bootstrap 5, AOS.js, Swiper.js",
                    IsResponsive = true,
                    BrowserSupport = "Chrome 90+, Firefox 88+, Safari 14+, Edge 90+",
                    SourceCodeUrl = "/files/templates/modern-landing-source.zip",
                    DemoUrl = "https://demo.codesparknet.com/modern-landing",
                    DocumentationUrl = "/files/templates/modern-landing-docs.pdf",
                    FigmaUrl = "https://figma.com/file/modern-landing-design",
                    PagesCount = 1,
                    ColorScheme = "Темно-синий градиент (#1a1a2e, #16213e, #0f3460) с акцентным оранжевым (#ff6b35)",
                    FontsUsed = "Inter (основной), Space Grotesk (заголовки)",
                    HasDarkMode = true,
                    HasAnimations = true,
                    Features = "Параллакс эффекты, плавная прокрутка, анимации при скролле, интерактивные элементы, адаптивное меню, контактная форма, слайдеры, модальные окна",
                    Dependencies = "Bootstrap 5.3.0, AOS.js 2.3.4, Swiper.js 8.4.7, Font Awesome 6.4.0",
                    InstallationInstructions = @"1. Распакуйте архив в папку вашего проекта
2. Откройте index.html в браузере для просмотра
3. Настройте контактную форму в файле js/contact.js
4. Замените демо-контент на ваш в HTML файлах
5. Настройте цвета в файле scss/_variables.scss
6. Скомпилируйте SCSS в CSS (опционально)

Подробная инструкция находится в файле README.md",
                    License = "Standard License (может использоваться для одного проекта)"
                }
            );

            // Создаем модули для курса JavaScript
            var module1Id = Guid.Parse("44444444-4444-4444-4444-444444444444");
            var module2Id = Guid.Parse("55555555-5555-5555-5555-555555555555");
            var module3Id = Guid.Parse("66666666-6666-6666-6666-666666666666");

            builder.Entity<Module>().HasData(
                new Module
                {
                    Id = module1Id,
                    CourseId = courseProductId,
                    Title = "Основы JavaScript",
                    Order = 1
                },
                new Module
                {
                    Id = module2Id,
                    CourseId = courseProductId,
                    Title = "DOM и события",
                    Order = 2
                },
                new Module
                {
                    Id = module3Id,
                    CourseId = courseProductId,
                    Title = "Асинхронное программирование",
                    Order = 3
                }
            );

            // Создаем уроки для модулей
            builder.Entity<Lesson>().HasData(
                // Уроки модуля 1
                new Lesson
                {
                    Id = Guid.Parse("77777777-7777-7777-7777-777777777771"),
                    ModuleId = module1Id,
                    Title = "Введение в JavaScript",
                    Content = "В этом уроке мы изучим историю JavaScript, его роль в веб-разработке и настроим среду разработки.",
                    Order = 1
                },
                new Lesson
                {
                    Id = Guid.Parse("77777777-7777-7777-7777-777777777772"),
                    ModuleId = module1Id,
                    Title = "Переменные и типы данных",
                    Content = "Изучаем объявление переменных с помощью var, let и const. Разбираем примитивные и ссылочные типы данных.",
                    Order = 2
                },
                new Lesson
                {
                    Id = Guid.Parse("77777777-7777-7777-7777-777777777773"),
                    ModuleId = module1Id,
                    Title = "Функции",
                    Content = "Изучаем объявление функций, параметры, возвращаемые значения и область видимости.",
                    Order = 3
                },
                // Уроки модуля 2
                new Lesson
                {
                    Id = Guid.Parse("88888888-8888-8888-8888-888888888881"),
                    ModuleId = module2Id,
                    Title = "Работа с DOM",
                    Content = "Изучаем Document Object Model, поиск элементов и изменение содержимого страницы.",
                    Order = 1
                },
                new Lesson
                {
                    Id = Guid.Parse("88888888-8888-8888-8888-888888888882"),
                    ModuleId = module2Id,
                    Title = "Обработка событий",
                    Content = "Изучаем addEventListener, типы событий и делегирование событий.",
                    Order = 2
                },
                // Уроки модуля 3
                new Lesson
                {
                    Id = Guid.Parse("99999999-9999-9999-9999-999999999991"),
                    ModuleId = module3Id,
                    Title = "Promises и async/await",
                    Content = "Изучаем асинхронное программирование, обещания и современный синтаксис async/await.",
                    Order = 1
                }
            );

            // Создаем задания с фиксированными датами
            var assignment1Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
            var assignment2Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");

            builder.Entity<CourseAssignment>().HasData(
                new CourseAssignment
                {
                    Id = assignment1Id,
                    ModuleId = module1Id,
                    Title = "Создание калькулятора",
                    Description = "Создайте простой калькулятор с базовыми арифметическими операциями. Используйте функции для каждой операции.",
                    DeadLine = new DateTime(2024, 12, 31, 23, 59, 59, DateTimeKind.Utc)
                },
                new CourseAssignment
                {
                    Id = assignment2Id,
                    ModuleId = module2Id,
                    Title = "Интерактивная форма",
                    Description = "Создайте форму с валидацией полей в реальном времени. Добавьте обработчики событий для всех полей.",
                    DeadLine = new DateTime(2025, 1, 15, 23, 59, 59, DateTimeKind.Utc)
                }
            );

            #endregion
        }
    }
}