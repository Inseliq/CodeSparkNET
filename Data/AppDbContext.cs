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
                    Name = "Библиотека знаний",
                    Slug = "library",
                    IsVisible = true,
                    IsLinkOnly = false
                },
                new Catalog
                {
                    Id = "d1f9c6d2-4b4b-4b8e-9f2a-aaaa00000002",
                    Name = "Курсы",
                    Slug = "courses",
                    IsVisible = true,
                    IsLinkOnly = false
                },
                new Catalog
                {
                    Id = "d1f9c6d2-4b4b-4b8e-9f2a-aaaa00000003",
                    Name = "Искуственный интелект",
                    Slug = "ai",
                    IsVisible = true,
                    IsLinkOnly = true,
                    PageName = "AI",
                    PageController = "Home"
                }
            );

            // Seed: Course 
            builder.Entity<Course>().HasData(
                new
                {
                    Id = "e1f9c6d2-5b4b-4b8e-9f2a-bbbb00000004",
                    CatalogId = "d1f9c6d2-4b4b-4b8e-9f2a-aaaa00000002",
                    Name = "Курс по ASP.NET Core",
                    Slug = "aspnet-core-web-development",
                    Price = 2499.99m,
                    Level = "Intermediate",
                    ProductType = "Course",
                    Currency = "RUB",
                    InStock = 10,
                    IsPublished = true
                },
                new
                {
                    Id = "e1f9c6d2-5b4b-4b8e-9f2a-bbbb00000005",
                    CatalogId = "d1f9c6d2-4b4b-4b8e-9f2a-aaaa00000003",
                    Name = "Введение в машинное обучение",
                    Slug = "intro-to-machine-learning",
                    Price = 2999.99m,
                    Level = "Advanced",
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

            // Seed: Tutorial
            builder.Entity<Course>().HasData(
                new
                {
                    Id = "e1f9c6d2-5b4b-4b8e-9f2a-bbbb00000006",
                    CatalogId = "d1f9c6d2-4b4b-4b8e-9f2a-aaaa00000001",
                    Name = "Руководство по Keras",
                    Slug = "keras-tutorial",
                    FullDescription = "Keras — это платформа с открытым исходным кодом глубокого обучения для Python. Она была разработана исследователем в области искусственного интеллекта из Google Франсом Шолле. В настоящее время Keras используется такими ведущими организациями, как Google, Square, Netflix, Huawei и Uber. В этом учебном руководстве рассматриваются установка Keras, основы глубокого обучения, модели Keras, слои Keras, модули Keras, а также приводятся некоторые примеры реальных приложений.\r\n",
                    Price = 0m,
                    ProductType = "Tutorial",
                    Currency = "RUB",
                    InStock = 999,
                    IsPublished = true
                }
            );

            // ProductImage seeds 
            builder.Entity<ProductImage>().HasData(
                //new ProductImage
                //{
                //    Id = "f1f9c6d2-6b4b-4b8e-9f2a-cccc00000001",
                //    ProductId = "e1f9c6d2-5b4b-4b8e-9f2a-bbbb00000001",
                //    Name = "itcubic-main.jpg",
                //    Url = "https://cdn.example.com/courses/csharp/itcubic-main.jpg",
                //    IsMain = true,
                //    Position = 0
                //},

                //new ProductImage
                //{
                //    Id = "f1f9c6d2-6b4b-4b8e-9f2a-cccc00000002",
                //    ProductId = "e1f9c6d2-5b4b-4b8e-9f2a-bbbb00000002",
                //    Name = "codespark-main.jpg",
                //    Url = null,
                //    IsMain = true,
                //    Position = 0
                //},
                //new ProductImage
                //{
                //    Id = "f1f9c6d2-6b4b-4b8e-9f2a-cccc00000003",
                //    ProductId = "e1f9c6d2-5b4b-4b8e-9f2a-bbbb00000003",
                //    Name = "montazhka-main.jpg",
                //    Url = null,
                //    IsMain = true,
                //    Position = 0
                //},
                new ProductImage
                {
                    Id = "f1f9c6d2-6b4b-4b8e-9f2a-cccc00000006",
                    ProductId = "e1f9c6d2-5b4b-4b8e-9f2a-bbbb00000006",
                    Name = "keras-main.jpg",
                    Url = "https://avatars.mds.yandex.net/get-entity_search/2273637/428090467/S600xU",
                    IsMain = true,
                    Position = 0
                }
            );

            // ---------- Seed sample module + lesson + resource for the seeded course ----------
            var moduleId = "a1111111-1111-1111-1111-aaaaaaaaaaa1";
            var lessonId = "b2222222-2222-2222-2222-bbbbbbbbbbb2";
            var resourceId = "c3333333-3333-3333-3333-ccccccccccc3";

            // CourseId
            var seededCourseId = "e1f9c6d2-5b4b-4b8e-9f2a-bbbb00000006";

            // Модули
            builder.Entity<CourseModule>().HasData(
                new
                {
                    Id = "keras-for-beginners-main",
                    CourseId = seededCourseId,
                    Title = "Keras для начинающих",
                    Position = 1,
                }
            );

            // Уроки для модуля Keras для начинающих
            builder.Entity<Lesson>().HasData(
                new
                {
                    Id = "keras-for-beginners-module-1.01",
                    ModuleId = "keras-for-beginners-main",
                    Title = "Keras - Введение",
                    Slug = "keras-introduction",
                    Body = "<div style=\"font-family: 'Segoe UI', Arial, sans-serif; line-height: 1.6; color: #222;\">\r\n  <h2 style=\"color: #2a4d9b; margin-bottom: 0.4em;\">Глубокое обучение</h2>\r\n  <p>Глубокое обучение является одним из основных направлений машинного обучения. Машинное обучение изучает построение алгоритмов, вдохновлённых моделью человеческого мозга. Глубокое обучение становится всё более популярным в областях науки о данных, таких как робототехника, искусственный интеллект (AI), распознавание аудио и видео, а также распознавание изображений. Искусственная нейронная сеть является ядром методов глубокого обучения. Глубокое обучение поддерживается различными библиотеками, такими как Theano, TensorFlow, Caffe, Mxnet и другими. Keras — одна из самых мощных и простых в использовании библиотек Python, построенная поверх популярных библиотек глубокого обучения, таких как TensorFlow и Theano, и предназначенная для создания моделей глубокого обучения.</p>\r\n\r\n  <h2 style=\"color: #2a4d9b; margin-top: 1.2em; margin-bottom: 0.4em;\">Обзор Keras</h2>\r\n  <p>Keras работает поверх открытых библиотек машинного обучения, таких как TensorFlow, Theano или Cognitive Toolkit (CNTK). Theano — это библиотека Python, предназначенная для быстрого выполнения численных вычислений. TensorFlow — наиболее известная библиотека символьной математики, используемая для создания нейронных сетей и моделей глубокого обучения. TensorFlow отличается гибкостью, а его основным преимуществом является поддержка распределённых вычислений. CNTK — это фреймворк глубокого обучения, разработанный компанией Microsoft. Он поддерживает библиотеки Python, C#, C++, а также может использоваться как самостоятельный инструмент машинного обучения. Хотя Theano и TensorFlow являются мощными библиотеками, их сложно использовать для построения нейронных сетей напрямую.</p>\r\n\r\n  <p>Keras основан на минималистичной структуре, которая обеспечивает чистый и удобный способ создания моделей глубокого обучения на основе TensorFlow или Theano. Он предназначен для быстрого определения и построения моделей глубокого обучения. Таким образом, Keras является оптимальным выбором для приложений, использующих глубокое обучение.</p>\r\n\r\n  <h3 style=\"color: #3b5fc0; margin-top: 1em;\">Особенности</h3>\r\n  <p>Keras применяет различные методы оптимизации, чтобы сделать высокоуровневый API для нейронных сетей простым и эффективным. Он поддерживает следующие особенности:</p>\r\n  <ul style=\"margin-left: 1.2em;\">\r\n    <li>Единый, простой и расширяемый API.</li>\r\n    <li>Минималистичная структура — позволяет достичь результата без излишней сложности.</li>\r\n    <li>Поддержка нескольких платформ и бэкендов.</li>\r\n    <li>Дружественный к пользователю фреймворк, работающий как на CPU, так и на GPU.</li>\r\n    <li>Высокая масштабируемость вычислений.</li>\r\n  </ul>\r\n\r\n  <h3 style=\"color: #3b5fc0; margin-top: 1em;\">Преимущества</h3>\r\n  <p>Keras — это мощный и динамичный фреймворк, который обладает следующими преимуществами:</p>\r\n  <ul style=\"margin-left: 1.2em;\">\r\n    <li>Большое сообщество пользователей и поддержка.</li>\r\n    <li>Простота тестирования.</li>\r\n    <li>Нейронные сети Keras написаны на Python, что упрощает разработку.</li>\r\n    <li>Поддержка как сверточных (convolutional), так и рекуррентных (recurrent) сетей.</li>\r\n    <li>Модели глубокого обучения состоят из независимых компонентов, которые можно комбинировать различными способами.</li>\r\n  </ul>\r\n</div>\r\n",
                    Position = 0,
                    DurationMinutes = 12,
                    IsPublished = true,
                    IsFreePreview = true
                },
                new
                {
                    Id = "keras-for-beginners-module-1.02",
                    ModuleId = "keras-for-beginners-main",
                    Title = "Keras - Установка",
                    Slug = "keras-installation",
                    Body = "<div style=\"font-family: 'Segoe UI', Arial, sans-serif; line-height: 1.6; color: #222;\">\r\n  <h2 style=\"color: #2a4d9b;\">Установка Keras</h2>\r\n  <p>В этой главе объясняется, как установить Keras на ваш компьютер. Перед установкой рассмотрим основные требования к Keras.</p>\r\n\r\n  <h3 style=\"color: #3b5fc0;\">Предварительные требования</h3>\r\n  <p>Вы должны удовлетворять следующим условиям:</p>\r\n  <ul style=\"margin-left: 1.2em;\">\r\n    <li>Любая операционная система (Windows, Linux или Mac)</li>\r\n    <li>Python версии 3.5 или выше</li>\r\n  </ul>\r\n\r\n  <h3 style=\"color: #3b5fc0;\">Python</h3>\r\n  <p>Keras — это библиотека нейронных сетей на основе Python, поэтому Python должен быть установлен. Если он уже установлен, откройте терминал и введите:</p>\r\n  <pre style=\"background:#f6f8fa;padding:10px;border-radius:6px;\">python</pre>\r\n  <p>Пример ответа системы:</p>\r\n  <pre style=\"background:#f6f8fa;padding:10px;border-radius:6px;\">Python 3.6.5 (v3.6.5:f59c0932b4, Mar 28 2018, 17:00:18)\r\n[MSC v.1900 64 bit (AMD64)] on win32\r\nType \"help\", \"copyright\", \"credits\" or \"license\" for more information.\r\n&gt;&gt;&gt;</pre>\r\n  <p>На данный момент актуальная версия — 3.7.2. Если Python не установлен, посетите официальный сайт <a href=\"https://www.python.org\" target=\"_blank\">python.org</a> и скачайте последнюю версию для своей операционной системы.</p>\r\n\r\n  <h3 style=\"color: #3b5fc0;\">Шаги установки Keras</h3>\r\n  <p>Процесс установки Keras достаточно прост. Следуйте инструкциям ниже:</p>\r\n\r\n  <h4 style=\"color: #3b5fc0;\">Шаг 1: Создание виртуального окружения</h4>\r\n  <p>Virtualenv используется для управления пакетами Python для разных проектов. Это помогает избежать конфликтов между зависимостями. Рекомендуется использовать виртуальное окружение.</p>\r\n\r\n  <p><strong>Linux / macOS:</strong></p>\r\n  <pre style=\"background:#f6f8fa;padding:10px;border-radius:6px;\">python3 -m venv kerasenv</pre>\r\n\r\n  <p><strong>Windows:</strong></p>\r\n  <pre style=\"background:#f6f8fa;padding:10px;border-radius:6px;\">py -m venv keras</pre>\r\n\r\n  <h4 style=\"color: #3b5fc0;\">Шаг 2: Активация окружения</h4>\r\n  <p><strong>Linux / macOS:</strong></p>\r\n  <pre style=\"background:#f6f8fa;padding:10px;border-radius:6px;\">cd kerasenv\r\nsource bin/activate</pre>\r\n\r\n  <p><strong>Windows:</strong></p>\r\n  <pre style=\"background:#f6f8fa;padding:10px;border-radius:6px;\">.\\env\\Scripts\\activate</pre>\r\n\r\n  <h4 style=\"color: #3b5fc0;\">Шаг 3: Установка библиотек Python</h4>\r\n  <p>Keras зависит от следующих библиотек Python:</p>\r\n  <ul style=\"margin-left: 1.2em;\">\r\n    <li>Numpy</li>\r\n    <li>Pandas</li>\r\n    <li>Scikit-learn</li>\r\n    <li>Matplotlib</li>\r\n    <li>Scipy</li>\r\n    <li>Seaborn</li>\r\n  </ul>\r\n\r\n  <p>Если они не установлены, выполните поочерёдно:</p>\r\n  <pre style=\"background:#f6f8fa;padding:10px;border-radius:6px;\">pip install numpy\r\npip install pandas\r\npip install matplotlib\r\npip install scipy\r\npip install seaborn</pre>\r\n\r\n  <p><strong>Установка scikit-learn:</strong></p>\r\n  <p>Scikit-learn — это библиотека машинного обучения с открытым исходным кодом. Для установки требуется:</p>\r\n  <ul style=\"margin-left: 1.2em;\">\r\n    <li>Python версии 3.5 или выше</li>\r\n    <li>NumPy 1.11.0 или выше</li>\r\n    <li>SciPy 0.17.0 или выше</li>\r\n    <li>joblib 0.11 или выше</li>\r\n  </ul>\r\n  <p>Команда установки:</p>\r\n  <pre style=\"background:#f6f8fa;padding:10px;border-radius:6px;\">pip install -U scikit-learn</pre>\r\n\r\n  <h4 style=\"color: #3b5fc0;\">Установка Keras</h4>\r\n  <p>После установки всех зависимостей выполните:</p>\r\n  <pre style=\"background:#f6f8fa;padding:10px;border-radius:6px;\">pip install keras</pre>\r\n\r\n  <p>Чтобы выйти из виртуального окружения:</p>\r\n  <pre style=\"background:#f6f8fa;padding:10px;border-radius:6px;\">deactivate</pre>\r\n\r\n  <h3 style=\"color: #3b5fc0;\">Установка через Anaconda</h3>\r\n  <p>Если вы используете <strong>Anaconda</strong>, убедитесь, что она установлена. Если нет, скачайте её с сайта <a href=\"https://www.anaconda.com/download\" target=\"_blank\">anaconda.com</a>.</p>\r\n\r\n  <h4 style=\"color: #3b5fc0;\">Создание нового окружения</h4>\r\n  <pre style=\"background:#f6f8fa;padding:10px;border-radius:6px;\">conda create --name PythonCPU</pre>\r\n\r\n  <h4 style=\"color: #3b5fc0;\">Активация окружения</h4>\r\n  <pre style=\"background:#f6f8fa;padding:10px;border-radius:6px;\">activate PythonCPU</pre>\r\n\r\n  <h4 style=\"color: #3b5fc0;\">Установка Spyder</h4>\r\n  <p>Spyder — это IDE для запуска Python-приложений. Установите его в окружение:</p>\r\n  <pre style=\"background:#f6f8fa;padding:10px;border-radius:6px;\">conda install spyder</pre>\r\n\r\n  <h4 style=\"color: #3b5fc0;\">Установка библиотек Python</h4>\r\n  <p>Для установки библиотек, необходимых для Keras, используйте синтаксис:</p>\r\n  <pre style=\"background:#f6f8fa;padding:10px;border-radius:6px;\">conda install -c anaconda &lt;module-name&gt;</pre>\r\n  <p>Например, для установки pandas:</p>\r\n  <pre style=\"background:#f6f8fa;padding:10px;border-radius:6px;\">conda install -c anaconda pandas</pre>\r\n\r\n  <h4 style=\"color: #3b5fc0;\">Установка Keras</h4>\r\n  <pre style=\"background:#f6f8fa;padding:10px;border-radius:6px;\">conda install -c anaconda keras</pre>\r\n\r\n  <h4 style=\"color: #3b5fc0;\">Запуск Spyder</h4>\r\n  <p>После установки запустите Spyder командой:</p>\r\n  <pre style=\"background:#f6f8fa;padding:10px;border-radius:6px;\">spyder</pre>\r\n\r\n  <p>Чтобы проверить корректность установки, попробуйте импортировать библиотеки в Spyder. Если какая-то библиотека отсутствует, вы получите сообщение об ошибке <code>ModuleNotFoundError</code>.</p>\r\n</div>\r\n",
                    Position = 1,
                    DurationMinutes = 18,
                    IsPublished = true,
                    IsFreePreview = true
                },
                new
                {
                    Id = "keras-for-beginners-module-1.03",
                    ModuleId = "keras-for-beginners-main",
                    Title = "Keras - Настройка серверной части",
                    Slug = "keras-backend-configuration",
                    Body = "<div style=\"font-family: 'Segoe UI', Arial, sans-serif; line-height: 1.6; color: #222;\">\r\n  <h2 style=\"color: #2a4d9b; margin-bottom: 0.4em;\">Keras: backend'ы — TensorFlow и Theano</h2>\r\n\r\n  <p>В этой главе подробно рассматриваются реализации backend в Keras — <strong>TensorFlow</strong> и <strong>Theano</strong>. Разберём каждую по отдельности.</p>\r\n\r\n  <h3 style=\"color: #3b5fc0; margin-top: 1em;\">TensorFlow</h3>\r\n  <p><strong>TensorFlow</strong> — открытая библиотека машинного обучения от Google для численных вычислений. Keras — это высокоуровневый API, построенный поверх TensorFlow или Theano.</p>\r\n\r\n  <p>Если TensorFlow не установлен, установите его командой:</p>\r\n  <pre style=\"background:#f6f8fa;padding:10px;border-radius:6px;overflow:auto;\">pip install tensorflow</pre>\r\n\r\n  <p>После установки и при первом запуске Keras файл конфигурации обычно находится в домашней папке:</p>\r\n  <pre style=\"background:#f6f8fa;padding:10px;border-radius:6px;overflow:auto;\">~/.keras/keras.json</pre>\r\n\r\n  <p>Пример содержимого <code>keras.json</code>:</p>\r\n  <pre style=\"background:#f6f8fa;padding:10px;border-radius:6px;overflow:auto;\">{\r\n  \"image_data_format\": \"channels_last\",\r\n  \"epsilon\": 1e-07,\r\n  \"floatx\": \"float32\",\r\n  \"backend\": \"tensorflow\"\r\n}</pre>\r\n\r\n  <p><em>Кратко о полях:</em></p>\r\n  <ul style=\"margin-left: 1.2em;\">\r\n    <li><code>image_data_format</code> — формат представления изображений (например, каналы последними или первыми).</li>\r\n    <li><code>epsilon</code> — малое числовое значение для предотвращения деления на ноль.</li>\r\n    <li><code>floatx</code> — тип с плавающей запятой по умолчанию (обычно <code>float32</code>).</li>\r\n    <li><code>backend</code> — используемый бэкенд (например, <code>tensorflow</code>).</li>\r\n  </ul>\r\n\r\n  <p>Если файл отсутствует, создайте папку и файл вручную:</p>\r\n  <pre style=\"background:#f6f8fa;padding:10px;border-radius:6px;overflow:auto;\">cd ~\r\nmkdir .keras\r\nvi .keras/keras.json</pre>\r\n\r\n  <h3 style=\"color: #3b5fc0; margin-top: 1em;\">Theano</h3>\r\n  <p><strong>Theano</strong> — библиотека для эффективных вычислений с многомерными массивами. Установить её можно командой:</p>\r\n  <pre style=\"background:#f6f8fa;padding:10px;border-radius:6px;overflow:auto;\">pip install theano</pre>\r\n\r\n  <p>По умолчанию Keras использует TensorFlow. Чтобы переключиться на Theano, измените значение <code>\"backend\"</code> в <code>keras.json</code>:</p>\r\n  <pre style=\"background:#f6f8fa;padding:10px;border-radius:6px;overflow:auto;\">{\r\n  \"image_data_format\": \"channels_last\",\r\n  \"epsilon\": 1e-07,\r\n  \"floatx\": \"float32\",\r\n  \"backend\": \"theano\"\r\n}</pre>\r\n\r\n  <p>Сохраните файл, перезапустите терминал и импортируйте Keras. При успешном переключении вы увидите сообщение об использовании Theano:</p>\r\n  <pre style=\"background:#f6f8fa;padding:10px;border-radius:6px;overflow:auto;\">>>> import keras as k\r\nusing theano backend.</pre>\r\n\r\n  <p style=\"margin-top:1em;\">Этот HTML готов к сохранению в базе данных — он не содержит скриптов и потенциально опасных атрибутов, стиль минимален и встроен напрямую в теги.</p>\r\n</div>\r\n",
                    Position = 2,
                    DurationMinutes = 6,
                    IsPublished = true,
                    IsFreePreview = true
                },
                new
                {
                    Id = "keras-for-beginners-module-1.04",
                    ModuleId = "keras-for-beginners-main",
                    Title = "Keras - Обзор возможностей глубокого обучения",
                    Slug = "keras-overview-of-deep-learning",
                    Body = "<div style=\"font-family: 'Segoe UI', Arial, sans-serif; line-height:1.6; color:#222;\">\r\n  <h2 style=\"color:#2a4d9b; margin-bottom:0.4em;\">Глубокое обучение</h2>\r\n\r\n  <p>Глубокое обучение — развивающаяся область машинного обучения. Оно предполагает послойный анализ входа, где каждый слой последовательно извлекает более высокоуровневые признаки.</p>\r\n\r\n  <p>Рассмотрим простую ситуацию анализа изображения. Предположим, что изображение представлено в виде прямоугольной сетки пикселей. Первый слой абстрагирует пиксели, второй — понимает границы (edges), следующий формирует узлы из границ, затем обнаруживаются ветви из узлов, и наконец выходной слой распознаёт целый объект. Процесс извлечения признаков передаёт выход одного слоя на вход следующему.</p>\r\n\r\n  <p>Такой подход позволяет обрабатывать огромные объёмы признаков, поэтому глубокое обучение — мощный инструмент. Алгоритмы глубокого обучения особенно полезны для анализа неструктурированных данных. В этой главе рассмотрим основы глубокого обучения.</p>\r\n\r\n  <h3 style=\"color:#3b5fc0; margin-top:1em;\">Искусственные нейронные сети (ANN)</h3>\r\n\r\n  <p>Основной подход в глубоком обучении — использование искусственных нейронных сетей (ANN), вдохновлённых моделью человеческого мозга. Человеческий мозг состоит из миллиардов нейронов, связанных аксонами и дендритами: аксоны передают информацию, дендриты её принимают. Каждый нейрон обрабатывает небольшую часть информации и передаёт результат дальше — так происходит обработка речи, изображений и других данных.</p>\r\n\r\n  <p>Первая искусственная нейронная сеть (перцептрон) была предложена психологом Фрэнком Розенблаттом в 1958 году. ANN состоят из множества узлов (аналог нейронов), связанных между собой и организованных в слои: входной слой принимает данные, они последовательно проходят через один или несколько скрытых слоев и попадают в выходной слой, который выдаёт предсказание (например, на входе — изображение, на выходе — «кот»).</p>\r\n\r\n  <p>Один нейрон (перцептрон) условно можно описать так: входы с весами — это дендриты; сумма входов и функция активации — сам нейрон; выход — это аксон, передающий значение дальше. Функция активации преобразует сумму в значение (например, 0 или 1, либо значение в диапазоне 0–1).</p>\r\n\r\n  <h4 style=\"color:#3b5fc0; margin-top:1em;\">Типы нейронных сетей</h4>\r\n\r\n  <h5 style=\"color:#3b5fc0; margin-top:0.6em;\">Многослойный перцептрон (MLP)</h5>\r\n  <p>MLP — простейшая форма ANN. Состоит из входного слоя, одного или нескольких скрытых слоёв и выходного слоя. Каждый слой представляет собой набор перцептронов: скрытые слои последовательно обрабатывают признаки, а выходной слой выдаёт итоговое предсказание.</p>\r\n\r\n  <h5 style=\"color:#3b5fc0; margin-top:0.6em;\">Свёрточная нейронная сеть (CNN)</h5>\r\n  <p>CNN широко используется в задачах распознавания изображений и видео. Основная идея — операция свёртки. В отличие от MLP, в CNN до полностью связанных слоёв идут последовательности свёрточных и пула (pooling) слоёв.</p>\r\n  <ul style=\"margin-left:1.2em;\">\r\n    <li><strong>Свёрточный слой</strong> — основной блок, выполняет вычисления свёртки.</li>\r\n    <li><strong>Пуллинговый слой</strong> — уменьшает размерность представления, удаляя избыточную информацию, что ускоряет вычисления.</li>\r\n    <li><strong>Полносвязный слой</strong> — располагается после свёрток и пулинга и выполняет классификацию.</li>\r\n  </ul>\r\n  <p>Типичная простая архитектура CNN: несколько блоков «свёртка → пул» и в конце один или несколько полносвязных слоёв для вывода результата.</p>\r\n\r\n  <h5 style=\"color:#3b5fc0; margin-top:0.6em;\">Рекуррентная нейронная сеть (RNN)</h5>\r\n  <p>RNN решают проблему отсутствия памяти о предыдущих шагах в стандартных ANN. RNN хранят прошлую информацию, и решения зависят от контекста, извлечённого из прошлых состояний. Это полезно для последовательных данных (текст, временные ряды).</p>\r\n  <p>Бидирекционные RNN (bidirectional RNN) позволяют учитывать как прошлое, так и будущее для улучшения распознавания контекста (например, при обработке рукописного текста, где смысл текущего символа может зависеть от соседних символов).</p>\r\n\r\n  <h3 style=\"color:#3b5fc0; margin-top:1em;\">Рабочий процесс разработки с ANN</h3>\r\n  <p>Парадигма обучения с использованием ANN обычно включает следующие этапы:</p>\r\n  <ol style=\"margin-left:1.2em;\">\r\n    <li><strong>Сбор данных</strong> — накопление большого объёма входных данных.</li>\r\n    <li><strong>Анализ данных</strong> — исследование и понимание структуры данных.</li>\r\n    <li><strong>Выбор алгоритма (модели)</strong> — подбор подходящей архитектуры (например, CNN для изображений, RNN для последовательностей).</li>\r\n    <li><strong>Подготовка данных</strong> — очистка, нормализация, аугментация и т. п.</li>\r\n    <li><strong>Разбиение данных</strong> — разделение на обучающую и тестовую выборки.</li>\r\n    <li><strong>Компиляция модели</strong> — выбор функции потерь и оптимизатора для обучения.</li>\r\n    <li><strong>Обучение (fit)</strong> — процесс фактического обучения модели на тренировочных данных.</li>\r\n    <li><strong>Предсказание</strong> — получение результатов для новых (неизвестных) данных.</li>\r\n    <li><strong>Оценка модели</strong> — проверка качества на тестовой выборке и сравнение с эталоном.</li>\r\n    <li><strong>Зафиксировать / изменить / выбрать другую модель</strong> — если результат удовлетворителен, сохранить модель; иначе изменить архитектуру или гиперпараметры и повторить процесс.</li>\r\n  </ol>\r\n\r\n  <p>Этот цикл повторяется до тех пор, пока не будет найдена оптимальная модель для конкретной задачи.</p>\r\n\r\n  <p style=\"margin-top:1em;\">HTML готов для сохранения в базу данных: он не содержит скриптов и потенциально опасных атрибутов; стили минимальны и встроены в теги.</p>\r\n</div>\r\n",
                    Position = 3,
                    DurationMinutes = 16,
                    IsPublished = true,
                    IsFreePreview = true
                },
                new
                {
                    Id = "keras-for-beginners-module-1.05",
                    ModuleId = "keras-for-beginners-main",
                    Title = "Keras - Глубокое обучение",
                    Slug = "keras-deep-learning",
                    Body = "<div style=\"font-family:'Segoe UI', Arial, sans-serif; line-height:1.6; color:#222;\">\r\n  <h2 style=\"color:#2a4d9b;\">Архитектура Keras</h2>\r\n\r\n  <p>Keras предоставляет полноценный фреймворк для создания любых типов нейронных сетей. Этот инструмент инновационный и при этом простой в изучении. Он поддерживает как простые, так и очень сложные модели нейронных сетей. В этой главе мы рассмотрим архитектуру Keras и то, как он помогает в задачах глубокого обучения.</p>\r\n\r\n  <h3 style=\"color:#3b5fc0; margin-top:1em;\">Общая структура Keras</h3>\r\n  <p>API Keras можно разделить на три основные категории:</p>\r\n  <ul style=\"margin-left:1.2em;\">\r\n    <li><strong>Model</strong> (модель)</li>\r\n    <li><strong>Layer</strong> (слой)</li>\r\n    <li><strong>Core Modules</strong> (основные модули)</li>\r\n  </ul>\r\n\r\n  <p>Каждая нейронная сеть (ANN) в Keras представлена объектом <strong>Model</strong>. Каждая модель состоит из набора <strong>Layers</strong>, соответствующих слоям сети: входным, скрытым, выходным, а также свёрточным, пуллинговым и другим. Модели и слои Keras используют <strong>модули</strong> (activation, loss, regularization и т. д.), что позволяет описывать любые алгоритмы ANN (CNN, RNN и др.) просто и эффективно.</p>\r\n\r\n  <h3 style=\"color:#3b5fc0; margin-top:1em;\">Модели в Keras</h3>\r\n  <p>Существует два основных типа моделей в Keras:</p>\r\n\r\n  <h4 style=\"color:#3b5fc0; margin-top:0.5em;\">1. Sequential Model</h4>\r\n  <p>Последовательная модель (<strong>Sequential</strong>) — это линейная композиция слоёв. Она проста, минималистична и подходит для большинства типов нейронных сетей.</p>\r\n\r\n  <pre style=\"background:#f6f6f6; padding:0.8em; border-radius:6px; overflow:auto;\">\r\nfrom keras.models import Sequential\r\nfrom keras.layers import Dense, Activation\r\n\r\nmodel = Sequential()\r\nmodel.add(Dense(512, activation='relu', input_shape=(784,)))\r\n  </pre>\r\n\r\n  <p><strong>Пояснение:</strong></p>\r\n  <ul style=\"margin-left:1.2em;\">\r\n    <li>Импортируется класс <code>Sequential</code> из <code>keras.models</code>.</li>\r\n    <li>Импортируются слои <code>Dense</code> и <code>Activation</code>.</li>\r\n    <li>Создаётся новая модель с помощью API <code>Sequential</code>.</li>\r\n    <li>Добавляется плотный слой (Dense) с функцией активации <code>relu</code>.</li>\r\n  </ul>\r\n\r\n  <p>Sequential-модель также может использовать наследование (sub-classing) для создания собственных сложных моделей.</p>\r\n\r\n  <h4 style=\"color:#3b5fc0; margin-top:0.5em;\">2. Functional API</h4>\r\n  <p><strong>Functional API</strong> используется для построения сложных и разветвлённых моделей, которые не могут быть описаны простой линейной последовательностью слоёв.</p>\r\n\r\n  <h3 style=\"color:#3b5fc0; margin-top:1em;\">Слои (Layers)</h3>\r\n  <p>Каждый слой в Keras соответствует реальному слою нейронной сети (входному, скрытому, выходному и т. д.). Библиотека предоставляет множество готовых слоёв, что делает создание сложных моделей простым и наглядным.</p>\r\n\r\n  <ul style=\"margin-left:1.2em;\">\r\n    <li><strong>Core Layers</strong> — базовые слои.</li>\r\n    <li><strong>Convolution Layers</strong> — свёрточные слои.</li>\r\n    <li><strong>Pooling Layers</strong> — слои подвыборки.</li>\r\n    <li><strong>Recurrent Layers</strong> — рекуррентные слои.</li>\r\n  </ul>\r\n\r\n  <p>Пример кода нейронной сети на Keras с использованием Sequential-модели:</p>\r\n\r\n  <pre style=\"background:#f6f6f6; padding:0.8em; border-radius:6px; overflow:auto;\">\r\nfrom keras.models import Sequential\r\nfrom keras.layers import Dense, Activation, Dropout\r\n\r\nmodel = Sequential()\r\nmodel.add(Dense(512, activation='relu', input_shape=(784,)))\r\nmodel.add(Dropout(0.2))\r\nmodel.add(Dense(512, activation='relu'))\r\nmodel.add(Dropout(0.2))\r\nmodel.add(Dense(num_classes, activation='softmax'))\r\n  </pre>\r\n\r\n  <p><strong>Пояснение:</strong></p>\r\n  <ul style=\"margin-left:1.2em;\">\r\n    <li>Импортируются необходимые слои и модель.</li>\r\n    <li>Создаётся модель <code>Sequential</code>.</li>\r\n    <li>Добавляются плотные слои (<code>Dense</code>) с функцией активации <code>relu</code>.</li>\r\n    <li>Добавляются слои <code>Dropout</code> для предотвращения переобучения.</li>\r\n    <li>Последний слой использует <code>softmax</code> для классификации.</li>\r\n  </ul>\r\n\r\n  <p>Keras также позволяет создавать собственные кастомные слои путём наследования от класса <code>keras.Layer</code>, что аналогично созданию собственных моделей через наследование.</p>\r\n\r\n  <h3 style=\"color:#3b5fc0; margin-top:1em;\">Основные модули (Core Modules)</h3>\r\n  <p>Для корректного создания и обучения моделей Keras предоставляет множество встроенных функций, разделённых по категориям:</p>\r\n\r\n  <ul style=\"margin-left:1.2em;\">\r\n    <li><strong>Activation Module</strong> — функции активации: <code>relu</code>, <code>softmax</code>, <code>sigmoid</code> и др.</li>\r\n    <li><strong>Loss Module</strong> — функции потерь: <code>mean_squared_error</code>, <code>mean_absolute_error</code>, <code>poisson</code> и др.</li>\r\n    <li><strong>Optimizer Module</strong> — оптимизаторы: <code>adam</code>, <code>sgd</code> и другие.</li>\r\n    <li><strong>Regularizers</strong> — регуляризаторы: <code>L1</code>, <code>L2</code> и их комбинации.</li>\r\n  </ul>\r\n\r\n  <p style=\"margin-top:1em;\">Таким образом, Keras объединяет модели, слои и модули в единый удобный фреймворк, который позволяет создавать, обучать и тестировать нейронные сети любой сложности простыми и понятными средствами.</p>\r\n</div>",
                    Position = 4,
                    DurationMinutes = 8,
                    IsPublished = true,
                    IsFreePreview = true
                },
                new
                {
                    Id = "keras-for-beginners-module-1.06",
                    ModuleId = "keras-for-beginners-main",
                    Title = "Keras - Модули",
                    Slug = "keras-modules",
                    Body = "<div class=\"keras-content\">\r\n  <style>\r\n    .keras-content {\r\n      font-family: \"Segoe UI\", sans-serif;\r\n      line-height: 1.6;\r\n      color: #333;\r\n    }\r\n    .keras-content h2 {\r\n      color: #2c3e50;\r\n      border-bottom: 2px solid #ccc;\r\n      padding-bottom: 4px;\r\n      margin-top: 25px;\r\n    }\r\n    .keras-content h3 {\r\n      color: #34495e;\r\n      margin-top: 20px;\r\n    }\r\n    .keras-content code {\r\n      background: #f5f5f5;\r\n      padding: 2px 6px;\r\n      border-radius: 4px;\r\n      font-size: 0.95em;\r\n    }\r\n    .keras-content pre {\r\n      background: #f8f9fa;\r\n      border-left: 3px solid #3498db;\r\n      padding: 10px;\r\n      overflow-x: auto;\r\n      font-size: 0.95em;\r\n    }\r\n    .keras-content ul {\r\n      margin-left: 20px;\r\n    }\r\n  </style>\r\n\r\n  <h2>Модули Keras</h2>\r\n  <p>Keras предоставляет готовые классы, функции и переменные, которые полезны при создании алгоритмов глубокого обучения. Ниже рассмотрим доступные модули и их назначение.</p>\r\n\r\n  <h3>Доступные модули</h3>\r\n  <ul>\r\n    <li><b>Initializers</b> — функции инициализации параметров модели.</li>\r\n    <li><b>Regularizers</b> — функции регуляризации.</li>\r\n    <li><b>Constraints</b> — функции ограничений параметров.</li>\r\n    <li><b>Activations</b> — функции активации (ReLU, softmax и др.).</li>\r\n    <li><b>Losses</b> — функции потерь (используются при обучении).</li>\r\n    <li><b>Metrics</b> — функции для расчёта метрик модели.</li>\r\n    <li><b>Optimizers</b> — функции оптимизации (Adam, SGD и др.).</li>\r\n    <li><b>Callback</b> — функции обратного вызова, например <code>EarlyStopping</code>.</li>\r\n    <li><b>Text processing</b> — преобразование текста в формат NumPy.</li>\r\n    <li><b>Image processing</b> — преобразование изображений в формат NumPy.</li>\r\n    <li><b>Sequence processing</b> — работа с временными последовательностями.</li>\r\n    <li><b>Backend</b> — операции на уровне TensorFlow, Theano или CNTK.</li>\r\n    <li><b>Utilities</b> — вспомогательные функции глубокого обучения.</li>\r\n  </ul>\r\n\r\n  <h3>Модуль backend</h3>\r\n  <p>Модуль <b>backend</b> используется для работы с низкоуровневыми операциями. По умолчанию Keras использует <code>TensorFlow</code>, но можно переключить на другие движки, изменив файл <code>~/.keras/keras.json</code>.</p>\r\n\r\n  <pre><code>from keras import backend as k\r\nk.backend()        # 'tensorflow'\r\nk.epsilon()        # 1e-07\r\nk.image_data_format()  # 'channels_last'\r\nk.floatx()         # 'float32'</code></pre>\r\n\r\n  <p>Некоторые полезные функции:</p>\r\n  <ul>\r\n    <li><b>get_uid()</b> — возвращает идентификатор текущего графа.</li>\r\n    <li><b>reset_uids()</b> — сбрасывает счётчик идентификаторов.</li>\r\n    <li><b>placeholder()</b> — создаёт пустой тензор указанной формы.</li>\r\n    <li><b>dot()</b> — умножение двух тензоров.</li>\r\n    <li><b>ones()</b> — инициализация матрицы единицами.</li>\r\n    <li><b>batch_dot()</b> — пакетное перемножение данных.</li>\r\n    <li><b>variable()</b> — создание переменной (например, для транспонирования).</li>\r\n    <li><b>is_sparse()</b> — проверяет, является ли тензор разрежённым.</li>\r\n    <li><b>to_dense()</b> — преобразует разрежённый тензор в плотный.</li>\r\n    <li><b>random_uniform_variable()</b> — создаёт переменные, распределённые равномерно.</li>\r\n  </ul>\r\n\r\n  <pre><code>a = k.random_uniform_variable(shape=(2, 3), low=0, high=1)\r\nb = k.random_uniform_variable(shape=(3, 2), low=0, high=1)\r\nc = k.dot(a, b)\r\nk.int_shape(c)  # (2, 2)</code></pre>\r\n\r\n  <h3>Модуль utils</h3>\r\n  <p>Модуль <b>utils</b> предоставляет вспомогательные функции для работы с моделями и данными:</p>\r\n\r\n  <ul>\r\n    <li><b>HDF5Matrix</b> — работа с данными в формате HDF5.</li>\r\n    <li><b>to_categorical()</b> — преобразует метки классов в бинарную матрицу.</li>\r\n  </ul>\r\n\r\n  <pre><code>from keras.utils import to_categorical\r\nlabels = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9]\r\nto_categorical(labels)</code></pre>\r\n\r\n  <ul>\r\n    <li><b>normalize()</b> — нормализует данные.</li>\r\n    <li><b>print_summary()</b> — выводит сводку модели.</li>\r\n    <li><b>plot_model()</b> — сохраняет визуализацию структуры модели.</li>\r\n  </ul>\r\n\r\n  <pre><code>from keras.utils import plot_model\r\nplot_model(model, to_file='model.png')</code></pre>\r\n</div>\r\n",
                    Position = 5,
                    DurationMinutes = 14,
                    IsPublished = true,
                    IsFreePreview = true
                },
                new
                {
                    Id = "keras-for-beginners-module-1.07",
                    ModuleId = "keras-for-beginners-main",
                    Title = "Keras - Слои",
                    Slug = "keras-layers",
                    Body = "<div style=\"font-family:'Segoe UI', Arial, sans-serif; line-height:1.6; color:#222;\">\r\n  <h2 style=\"color:#2a4d9b; margin-bottom:0.3em;\">Слои Keras</h2>\r\n\r\n  <p>Как уже изучалось ранее, слои — основной строительный блок моделей Keras. Каждый слой принимает вход, выполняет вычисления и возвращает преобразованный выход, который служит входом для следующего слоя.</p>\r\n\r\n  <h3 style=\"color:#3b5fc0; margin-top:0.8em;\">Введение</h3>\r\n  <p>Для создания слоя Keras обычно требуется указать:</p>\r\n  <ul style=\"margin-left:1.2em;\">\r\n    <li>форму входных данных (<code>input_shape</code>);</li>\r\n    <li>число нейронов / единиц в слое;</li>\r\n    <li>инициализатор весов (initializers);</li>\r\n    <li>регуляризатор (regularizers);</li>\r\n    <li>ограничения (constraints);</li>\r\n    <li>функцию активации (activation).</li>\r\n  </ul>\r\n\r\n  <p>Пример простого определения модели через Sequential API:</p>\r\n  <pre style=\"background:#f6f8fa;padding:10px;border-radius:6px;overflow:auto;\">\r\nfrom keras.models import Sequential\r\nfrom keras.layers import Activation, Dense\r\nfrom keras import initializers, regularizers, constraints\r\n\r\nmodel = Sequential()\r\nmodel.add(Dense(32, input_shape=(16,), kernel_initializer='he_uniform',\r\n                kernel_regularizer=None, kernel_constraint='MaxNorm', activation='relu'))\r\nmodel.add(Dense(16, activation='relu'))\r\nmodel.add(Dense(8))\r\n  </pre>\r\n\r\n  <h3 style=\"color:#3b5fc0; margin-top:0.8em;\">Базовые понятия</h3>\r\n\r\n  <h4 style=\"margin-top:0.6em;\">Форма входа (input shape)</h4>\r\n  <p>Данные в ML представляются массивами: одномерными, двумерными или многомерными. Форма задаётся кортежем целых чисел, например <code>(4, 2)</code> — матрица 4×2, <code>(3,4,2)</code> — набор из трёх матриц 4×2.</p>\r\n\r\n  <h4 style=\"margin-top:0.6em;\">Инициализаторы (Initializers)</h4>\r\n  <p>Инициализаторы задают начальные веса слоя. Некоторые распространённые:</p>\r\n  <ul style=\"margin-left:1.2em;\">\r\n    <li><b>Zeros</b>, <b>Ones</b> — заполняют нулями или единицами.</li>\r\n    <li><b>Constant</b> — одна константа для всех весов.</li>\r\n    <li><b>RandomNormal</b>, <b>RandomUniform</b>, <b>TruncatedNormal</b> — рандом по распределениям.</li>\r\n    <li><b>VarianceScaling</b>, <b>lecun_normal</b>, <b>lecun_uniform</b>, <b>glorot_normal</b>, <b>glorot_uniform</b>, <b>he_normal</b>, <b>he_uniform</b> — инициализаторы с учётом размеров слоёв (fan_in/fan_out).</li>\r\n    <li><b>Orthogonal</b>, <b>Identity</b> — ортогональная или единичная матрица.</li>\r\n  </ul>\r\n\r\n  <h4 style=\"margin-top:0.6em;\">Ограничения (Constraints)</h4>\r\n  <p>Constraints применяются к параметрам слоя во время оптимизации:</p>\r\n  <ul style=\"margin-left:1.2em;\">\r\n    <li><b>NonNeg</b> — веса ≥ 0;</li>\r\n    <li><b>UnitNorm</b> — норма единична (по оси);</li>\r\n    <li><b>MaxNorm</b> — норма ≤ заданного значения;</li>\r\n    <li><b>MinMaxNorm</b> — норма в пределах [min, max].</li>\r\n  </ul>\r\n\r\n  <h4 style=\"margin-top:0.6em;\">Регуляризаторы (Regularizers)</h4>\r\n  <p>Регуляризация добавляет штрафы к функции потерь, чтобы уменьшить переобучение. В Keras:</p>\r\n  <ul style=\"margin-left:1.2em;\">\r\n    <li><b>l1()</b> — L1-регуляризация;</li>\r\n    <li><b>l2()</b> — L2-регуляризация;</li>\r\n    <li><b>l1_l2()</b> — комбинированная L1+L2.</li>\r\n  </ul>\r\n\r\n  <h4 style=\"margin-top:0.6em;\">Активации (Activations)</h4>\r\n  <p>Функция активации делает преобразование нелинейным. Часто используемые:</p>\r\n  <ul style=\"margin-left:1.2em;\">\r\n    <li><code>linear</code> — линейная (без изменений);</li>\r\n    <li><code>elu</code>, <code>selu</code>;</li>\r\n    <li><code>relu</code>;</li>\r\n    <li><code>softmax</code>;</li>\r\n    <li><code>softplus</code>, <code>softsign</code>;</li>\r\n    <li><code>tanh</code>;</li>\r\n    <li><code>sigmoid</code>, <code>hard_sigmoid</code>;</li>\r\n    <li><code>exponential</code>.</li>\r\n  </ul>\r\n\r\n  <h3 style=\"color:#3b5fc0; margin-top:0.8em;\">Основные типы слоёв</h3>\r\n  <table style=\"border-collapse:collapse; width:100%; margin-top:8px;\">\r\n    <tbody>\r\n      <tr><td style=\"padding:6px; vertical-align:top;\"><b>Dense</b></td><td style=\"padding:6px;\">Полносвязный слой (обычный глубоко связанный слой).</td></tr>\r\n      <tr><td style=\"padding:6px; vertical-align:top;\"><b>Dropout</b></td><td style=\"padding:6px;\">Отключение входов случайно для борьбы с переобучением.</td></tr>\r\n      <tr><td style=\"padding:6px; vertical-align:top;\"><b>Flatten</b></td><td style=\"padding:6px;\">Выравнивание входа в вектор.</td></tr>\r\n      <tr><td style=\"padding:6px; vertical-align:top;\"><b>Reshape</b></td><td style=\"padding:6px;\">Изменение формы входа.</td></tr>\r\n      <tr><td style=\"padding:6px; vertical-align:top;\"><b>Permute</b></td><td style=\"padding:6px;\">Перестановка осей входа по шаблону.</td></tr>\r\n      <tr><td style=\"padding:6px; vertical-align:top;\"><b>RepeatVector</b></td><td style=\"padding:6px;\">Повторение входа заданное число раз.</td></tr>\r\n      <tr><td style=\"padding:6px; vertical-align:top;\"><b>Lambda</b></td><td style=\"padding:6px;\">Применение произвольного выражения/функции к входу.</td></tr>\r\n      <tr><td style=\"padding:6px; vertical-align:top;\"><b>Convolution</b></td><td style=\"padding:6px;\">Свёрточные слои (Conv1D/2D/3D) для CNN.</td></tr>\r\n      <tr><td style=\"padding:6px; vertical-align:top;\"><b>Pooling</b></td><td style=\"padding:6px;\">Операции подвыборки (max/avg pooling).</td></tr>\r\n      <tr><td style=\"padding:6px; vertical-align:top;\"><b>LocallyConnected</b></td><td style=\"padding:6px;\">Похоже на Conv, но веса не шарятся.</td></tr>\r\n      <tr><td style=\"padding:6px; vertical-align:top;\"><b>Merge</b></td><td style=\"padding:6px;\">Слияние нескольких входов (concat, add и т.д.).</td></tr>\r\n      <tr><td style=\"padding:6px; vertical-align:top;\"><b>Embedding</b></td><td style=\"padding:6px;\">Слой встраивания для категориальных/текстовых признаков.</td></tr>\r\n    </tbody>\r\n  </table>\r\n\r\n </div>\r\n",
                    Position = 6,
                    DurationMinutes = 16,
                    IsPublished = true,
                    IsFreePreview = true
                },
                new
                {
                    Id = "keras-for-beginners-module-1.08",
                    ModuleId = "keras-for-beginners-main",
                    Title = "Keras - Настроиваемые слои",
                    Slug = "keras-custom-layers",
                    Body = "<div style=\"font-family:'Segoe UI', Arial, sans-serif; line-height:1.6; color:#222;\">\r\n  <h2 style=\"color:#2a4d9b; margin-bottom:0.4em;\">Создание кастомного слоя в Keras</h2>\r\n\r\n  <p>Keras позволяет создавать собственные слои — после определения новый слой можно использовать в любой модели без ограничений. В этом разделе показано, как реализовать простой кастомный слой, который инициализирует веса по нормальному распределению и во время обучения возвращает сумму произведений входа на веса.</p>\r\n\r\n  <h3 style=\"color:#3b5fc0; margin-top:0.8em;\">Шаг 1. Импорт модулей</h3>\r\n  <p>Импортируем необходимые модули:</p>\r\n  <pre style=\"background:#f6f8fa;padding:10px;border-radius:6px;overflow:auto;\">\r\nfrom keras import backend as K\r\nfrom keras.layers import Layer\r\n  </pre>\r\n  <p><em>Здесь</em> <code>backend</code> используется для операции <code>dot</code>, а <code>Layer</code> — базовый класс для наследования.</p>\r\n\r\n  <h3 style=\"color:#3b5fc0; margin-top:0.8em;\">Шаг 2. Определение класса слоя</h3>\r\n  <p>Создадим класс <code>MyCustomLayer</code>, унаследовавшись от <code>Layer</code>:</p>\r\n  <pre style=\"background:#f6f8fa;padding:10px;border-radius:6px;overflow:auto;\">\r\nclass MyCustomLayer(Layer):\r\n    ...\r\n  </pre>\r\n\r\n  <h3 style=\"color:#3b5fc0; margin-top:0.8em;\">Шаг 3. Инициализация</h3>\r\n  <p>Реализуем конструктор и сохраним размер выходного пространства:</p>\r\n  <pre style=\"background:#f6f8fa;padding:10px;border-radius:6px;overflow:auto;\">\r\ndef __init__(self, output_dim, **kwargs):\r\n    self.output_dim = output_dim\r\n    super(MyCustomLayer, self).__init__(**kwargs)\r\n  </pre>\r\n\r\n  <h3 style=\"color:#3b5fc0; margin-top:0.8em;\">Шаг 4. Метод <code>build</code></h3>\r\n  <p><code>build</code> отвечает за создание весов слоя. Здесь мы добавляем параметр <code>kernel</code> с инициализацией <code>normal</code> и отмечаем его trainable:</p>\r\n  <pre style=\"background:#f6f8fa;padding:10px;border-radius:6px;overflow:auto;\">\r\ndef build(self, input_shape):\r\n    self.kernel = self.add_weight(name='kernel',\r\n                                  shape=(input_shape[1], self.output_dim),\r\n                                  initializer='normal',\r\n                                  trainable=True)\r\n    super(MyCustomLayer, self).build(input_shape)\r\n  </pre>\r\n\r\n  <h3 style=\"color:#3b5fc0; margin-top:0.8em;\">Шаг 5. Метод <code>call</code></h3>\r\n  <p><code>call</code> определяет поведение слоя при прямом проходе — возвращаем матричное умножение входа на kernel:</p>\r\n  <pre style=\"background:#f6f8fa;padding:10px;border-radius:6px;overflow:auto;\">\r\ndef call(self, input_data):\r\n    return K.dot(input_data, self.kernel)\r\n  </pre>\r\n\r\n  <h3 style=\"color:#3b5fc0; margin-top:0.8em;\">Шаг 6. <code>compute_output_shape</code></h3>\r\n  <p>Определяем форму выхода слоя:</p>\r\n  <pre style=\"background:#f6f8fa;padding:10px;border-radius:6px;overflow:auto;\">\r\ndef compute_output_shape(self, input_shape):\r\n    return (input_shape[0], self.output_dim)\r\n  </pre>\r\n\r\n  <h3 style=\"color:#3b5fc0; margin-top:0.8em;\">Полный пример кода</h3>\r\n  <pre style=\"background:#f6f8fa;padding:10px;border-radius:6px;overflow:auto;\">\r\nfrom keras import backend as K\r\nfrom keras.layers import Layer\r\n\r\nclass MyCustomLayer(Layer):\r\n    def __init__(self, output_dim, **kwargs):\r\n        self.output_dim = output_dim\r\n        super(MyCustomLayer, self).__init__(**kwargs)\r\n\r\n    def build(self, input_shape):\r\n        self.kernel = self.add_weight(name='kernel',\r\n                                      shape=(input_shape[1], self.output_dim),\r\n                                      initializer='normal',\r\n                                      trainable=True)\r\n        super(MyCustomLayer, self).build(input_shape)\r\n\r\n    def call(self, input_data):\r\n        return K.dot(input_data, self.kernel)\r\n\r\n    def compute_output_shape(self, input_shape):\r\n        return (input_shape[0], self.output_dim)\r\n  </pre>\r\n\r\n  <h3 style=\"color:#3b5fc0; margin-top:0.8em;\">Использование кастомного слоя</h3>\r\n  <p>Добавим слой в простую Sequential модель:</p>\r\n  <pre style=\"background:#f6f8fa;padding:10px;border-radius:6px;overflow:auto;\">\r\nfrom keras.models import Sequential\r\nfrom keras.layers import Dense\r\n\r\nmodel = Sequential()\r\nmodel.add(MyCustomLayer(32, input_shape=(16,)))\r\nmodel.add(Dense(8, activation='softmax'))\r\nmodel.summary()\r\n  </pre>\r\n\r\n  <p>Пример вывода <code>model.summary()</code> (параметры будут зависеть от размеров):</p>\r\n  <pre style=\"background:#f6f8fa;padding:10px;border-radius:6px;overflow:auto;\">\r\nModel: \"sequential_1\"\r\n_________________________________________________________________\r\nLayer (type)                 Output Shape       Param #\r\n=================================================================\r\nmy_custom_layer_1 (MyCustom (None, 32)         512\r\n_________________________________________________________________\r\ndense_1 (Dense)             (None, 8)           264\r\n=================================================================\r\nTotal params: 776\r\nTrainable params: 776\r\nNon-trainable params: 0\r\n_________________________________________________________________\r\n  </pre>\r\n\r\n </div>\r\n",
                    Position = 7,
                    DurationMinutes = 13,
                    IsPublished = true,
                    IsFreePreview = true
                },
                new
                {
                    Id = "keras-for-beginners-module-1.09",
                    ModuleId = "keras-for-beginners-main",
                    Title = "Keras - Модели",
                    Slug = "keras-models",
                    Body = "<div style=\"font-family:'Segoe UI', Arial, sans-serif; line-height:1.6; color:#222;\">\r\n  <h2 style=\"color:#2a4d9b; margin-bottom:0.4em;\">Создание модели в Keras: Sequential и Functional API</h2>\r\n\r\n  <p>Keras предоставляет два способа создания моделей: простой <strong>Sequential API</strong> и более гибкий <strong>Functional API</strong>. Рассмотрим оба подхода.</p>\r\n\r\n  <h3 style=\"color:#3b5fc0;\">Sequential API</h3>\r\n  <p>Sequential API основан на последовательном расположении слоёв — данные проходят от одного слоя к следующему, пока не достигнут выходного слоя.</p>\r\n\r\n  <h4>Создание модели</h4>\r\n  <pre style=\"background:#f6f8fa;padding:10px;border-radius:6px;\">\r\nfrom keras.models import Sequential\r\nmodel = Sequential()\r\n  </pre>\r\n\r\n  <h4>Добавление слоёв</h4>\r\n  <pre style=\"background:#f6f8fa;padding:10px;border-radius:6px;\">\r\nfrom keras.models import Sequential\r\nfrom keras.layers import Dense\r\n\r\nmodel = Sequential()\r\ninput_layer = Dense(32, input_shape=(8,))\r\nmodel.add(input_layer)\r\n\r\nhidden_layer = Dense(64, activation='relu')\r\nmodel.add(hidden_layer)\r\n\r\noutput_layer = Dense(8)\r\nmodel.add(output_layer)\r\n  </pre>\r\n  <p>Здесь создан входной, скрытый и выходной слои.</p>\r\n\r\n  <h4>Доступ к модели</h4>\r\n  <p>Keras предоставляет методы для получения информации о модели:</p>\r\n  <ul>\r\n    <li><code>model.layers</code> — список всех слоёв.</li>\r\n    <li><code>model.inputs</code> — список входных тензоров.</li>\r\n    <li><code>model.outputs</code> — список выходных тензоров.</li>\r\n    <li><code>model.get_weights()</code> — возвращает веса в виде NumPy-массивов.</li>\r\n    <li><code>model.set_weights(array)</code> — задаёт веса вручную.</li>\r\n  </ul>\r\n\r\n  <h4>Сериализация модели</h4>\r\n  <p>Модель можно сохранить и восстановить позже:</p>\r\n\r\n  <pre style=\"background:#f6f8fa;padding:10px;border-radius:6px;\">\r\nconfig = model.get_config()\r\nnew_model = Sequential.from_config(config)\r\n\r\njson_string = model.to_json()\r\nfrom keras.models import model_from_json\r\nnew_model = model_from_json(json_string)\r\n\r\nyaml_string = model.to_yaml()\r\nfrom keras.models import model_from_yaml\r\nnew_model = model_from_yaml(yaml_string)\r\n  </pre>\r\n\r\n  <h4>Пример структуры JSON модели</h4>\r\n  <pre style=\"background:#f6f8fa;padding:10px;border-radius:6px;overflow:auto;max-height:250px;\">\r\n{\"class_name\": \"Sequential\", \"config\": {\r\n  \"name\": \"sequential_10\",\r\n  \"layers\": [\r\n    {\"class_name\": \"Dense\", \"config\": {\"units\": 32, \"activation\": \"linear\"}},\r\n    {\"class_name\": \"Dense\", \"config\": {\"units\": 64, \"activation\": \"relu\"}},\r\n    {\"class_name\": \"Dense\", \"config\": {\"units\": 8, \"activation\": \"linear\"}}\r\n  ]\r\n}}\r\n  </pre>\r\n\r\n  <h4>Сводка модели</h4>\r\n  <pre style=\"background:#f6f8fa;padding:10px;border-radius:6px;\">\r\n>>> model.summary()\r\nModel: \"sequential_10\"\r\n_________________________________________________________________\r\nLayer (type)                 Output Shape              Param #\r\n=================================================================\r\ndense_13 (Dense)             (None, 32)                288\r\n_________________________________________________________________\r\ndense_14 (Dense)             (None, 64)                2112\r\n_________________________________________________________________\r\ndense_15 (Dense)             (None, 8)                 520\r\n=================================================================\r\nTotal params: 2,920\r\nTrainable params: 2,920\r\nNon-trainable params: 0\r\n_________________________________________________________________\r\n  </pre>\r\n\r\n  <h4>Обучение и предсказание</h4>\r\n  <ul>\r\n    <li><code>compile()</code> — настройка процесса обучения.</li>\r\n    <li><code>fit()</code> — обучение модели на тренировочных данных.</li>\r\n    <li><code>evaluate()</code> — оценка модели на тестовых данных.</li>\r\n    <li><code>predict()</code> — предсказание для новых входов.</li>\r\n  </ul>\r\n\r\n  <hr style=\"margin:1.5em 0;\">\r\n\r\n  <h3 style=\"color:#3b5fc0;\">Functional API</h3>\r\n  <p>Functional API используется для создания более сложных моделей, включая многовходовые или многовыходные архитектуры, где слои могут быть связаны нелинейно.</p>\r\n\r\n  <h4>Создание модели</h4>\r\n  <pre style=\"background:#f6f8fa;padding:10px;border-radius:6px;\">\r\nfrom keras.layers import Input, Dense\r\nfrom keras.models import Model\r\n\r\ndata = Input(shape=(2, 3))\r\nlayer = Dense(2)(data)\r\nmodel = Model(inputs=data, outputs=layer)\r\nmodel.summary()\r\n  </pre>\r\n\r\n  <h4>Пример вывода</h4>\r\n  <pre style=\"background:#f6f8fa;padding:10px;border-radius:6px;\">\r\n_________________________________________________________________\r\nLayer (type)               Output Shape              Param #\r\n=================================================================\r\ninput_2 (InputLayer)       (None, 2, 3)              0\r\n_________________________________________________________________\r\ndense_2 (Dense)            (None, 2, 2)              8\r\n=================================================================\r\nTotal params: 8\r\nTrainable params: 8\r\nNon-trainable params: 0\r\n_________________________________________________________________\r\n  </pre>\r\n\r\n  <p>Таким образом, <strong>Sequential API</strong> подходит для простых моделей, а <strong>Functional API</strong> — для продвинутых архитектур с несколькими потоками данных и выходами.</p>\r\n</div>\r\n",
                    Position = 8,
                    DurationMinutes = 12,
                    IsPublished = true,
                    IsFreePreview = true
                },
                new
                {
                    Id = "keras-for-beginners-module-1.10",
                    ModuleId = "keras-for-beginners-main",
                    Title = "Keras - Компиляция модели",
                    Slug = "keras-model-compilation",
                    Body = "<div style=\"font-family:'Segoe UI', Arial, sans-serif; line-height:1.6; color:#222;\">\r\n  <h2 style=\"color:#2a4d9b; margin-bottom:0.4em;\">Компиляция и обучение модели в Keras</h2>\r\n\r\n  <h3 style=\"color:#3b5fc0; margin-top:0.6em;\">Введение</h3>\r\n  <p>Компиляция — финальный шаг при создании модели: после неё можно переходить к обучению. Для корректной компиляции нужно выбрать функцию потерь (loss), оптимизатор (optimizer) и метрики (metrics).</p>\r\n\r\n  <h3 style=\"color:#3b5fc0; margin-top:0.6em;\">Loss (функции потерь)</h3>\r\n  <p>Функция потерь измеряет ошибку модели. Keras предоставляет множество готовых функций:</p>\r\n  <ul style=\"margin-left:1.2em;\">\r\n    <li>mean_squared_error</li>\r\n    <li>mean_absolute_error</li>\r\n    <li>mean_absolute_percentage_error</li>\r\n    <li>mean_squared_logarithmic_error</li>\r\n    <li>squared_hinge, hinge, categorical_hinge</li>\r\n    <li>logcosh, huber_loss</li>\r\n    <li>categorical_crossentropy, sparse_categorical_crossentropy</li>\r\n    <li>binary_crossentropy</li>\r\n    <li>kullback_leibler_divergence, poisson, cosine_proximity</li>\r\n  </ul>\r\n  <p>Функции принимают два аргумента: <code>y_true</code> и <code>y_pred</code>. Импорт:</p>\r\n  <pre style=\"background:#f6f8fa;padding:10px;border-radius:6px;overflow:auto;\">\r\nfrom keras import losses\r\n  </pre>\r\n\r\n  <h3 style=\"color:#3b5fc0; margin-top:0.6em;\">Optimizer (оптимизаторы)</h3>\r\n  <p>Оптимизатор минимизирует функцию потерь, обновляя веса сети. Примеры:</p>\r\n  <ul style=\"margin-left:1.2em;\">\r\n    <li>SGD: <code>keras.optimizers.SGD(learning_rate=0.01, momentum=0.0, nesterov=False)</code></li>\r\n    <li>RMSprop: <code>keras.optimizers.RMSprop(learning_rate=0.001, rho=0.9)</code></li>\r\n    <li>Adagrad: <code>keras.optimizers.Adagrad(learning_rate=0.01)</code></li>\r\n    <li>Adadelta: <code>keras.optimizers.Adadelta(learning_rate=1.0, rho=0.95)</code></li>\r\n    <li>Adam: <code>keras.optimizers.Adam(learning_rate=0.001, beta_1=0.9, beta_2=0.999)</code></li>\r\n    <li>Adamax, Nadam и др.</li>\r\n  </ul>\r\n  <pre style=\"background:#f6f8fa;padding:10px;border-radius:6px;overflow:auto;\">\r\nfrom keras import optimizers\r\n  </pre>\r\n\r\n  <h3 style=\"color:#3b5fc0; margin-top:0.6em;\">Metrics (метрики)</h3>\r\n  <p>Метрики используются для оценки качества модели (не влияют на обучение):</p>\r\n  <ul style=\"margin-left:1.2em;\">\r\n    <li>accuracy, binary_accuracy</li>\r\n    <li>categorical_accuracy, sparse_categorical_accuracy</li>\r\n    <li>top_k_categorical_accuracy, sparse_top_k_categorical_accuracy</li>\r\n    <li>cosine_proximity и др.</li>\r\n  </ul>\r\n  <pre style=\"background:#f6f8fa;padding:10px;border-radius:6px;overflow:auto;\">\r\nfrom keras import metrics\r\n  </pre>\r\n\r\n  <h3 style=\"color:#3b5fc0; margin-top:0.6em;\">Компиляция модели</h3>\r\n  <p>Метод <code>compile()</code> принимает ключевые параметры:</p>\r\n  <pre style=\"background:#f6f8fa;padding:10px;border-radius:6px;overflow:auto;\">\r\nmodel.compile(\r\n  optimizer,\r\n  loss=None,\r\n  metrics=None,\r\n  loss_weights=None,\r\n  sample_weight_mode=None,\r\n  weighted_metrics=None,\r\n  target_tensors=None\r\n)\r\n  </pre>\r\n  <p>Пример:</p>\r\n  <pre style=\"background:#f6f8fa;padding:10px;border-radius:6px;overflow:auto;\">\r\nfrom keras import losses, optimizers, metrics\r\nmodel.compile(loss='mean_squared_error', optimizer='sgd', metrics=[metrics.categorical_accuracy])\r\n  </pre>\r\n\r\n  <h3 style=\"color:#3b5fc0; margin-top:0.6em;\">Обучение модели (fit)</h3>\r\n  <p>Для обучения используется <code>fit()</code> с NumPy-массивами:</p>\r\n  <pre style=\"background:#f6f8fa;padding:10px;border-radius:6px;overflow:auto;\">\r\nmodel.fit(X, y, epochs=..., batch_size=...)\r\n  </pre>\r\n  <p>Аргументы:</p>\r\n  <ul style=\"margin-left:1.2em;\">\r\n    <li><code>X, y</code> — данные и метки</li>\r\n    <li><code>epochs</code> — число проходов по данным</li>\r\n    <li><code>batch_size</code> — размер мини-батча</li>\r\n  </ul>\r\n\r\n  <h3 style=\"color:#3b5fc0; margin-top:0.6em;\">Пример с генерацией случайных данных</h3>\r\n  <pre style=\"background:#f6f8fa;padding:10px;border-radius:6px;overflow:auto;\">\r\nimport numpy as np\r\n\r\nx_train = np.random.random((100, 4, 8))\r\ny_train = np.random.random((100, 10))\r\n\r\nx_val = np.random.random((100, 4, 8))\r\ny_val = np.random.random((100, 10))\r\n\r\nfrom keras.models import Sequential\r\nfrom keras.layers import LSTM, Dense\r\n\r\nmodel = Sequential()\r\nmodel.add(LSTM(16, return_sequences=True, input_shape=(4,8)))\r\nmodel.add(Dense(10, activation='softmax'))\r\n\r\nmodel.compile(loss='categorical_crossentropy', optimizer='sgd', metrics=['accuracy'])\r\n\r\nmodel.fit(x_train, y_train, batch_size=32, epochs=5, validation_data=(x_val, y_val))\r\n  </pre>\r\n\r\n  <h3 style=\"color:#3b5fc0; margin-top:0.6em;\">Создание MLP на примере MNIST</h3>\r\n  <p>Используем датасет MNIST (60 000 обучающих, 10 000 тестовых изображений 28×28).</p>\r\n\r\n  <h4 style=\"margin-top:0.5em;\">Шаг 1 — импорт и загрузка данных</h4>\r\n  <pre style=\"background:#f6f8fa;padding:10px;border-radius:6px;overflow:auto;\">\r\nimport keras\r\nfrom keras.datasets import mnist\r\nfrom keras.models import Sequential\r\nfrom keras.layers import Dense, Dropout\r\nfrom keras.optimizers import RMSprop\r\nimport numpy as np\r\n\r\n(x_train, y_train), (x_test, y_test) = mnist.load_data()\r\n  </pre>\r\n\r\n  <h4 style=\"margin-top:0.5em;\">Шаг 2 — подготовка данных</h4>\r\n  <pre style=\"background:#f6f8fa;padding:10px;border-radius:6px;overflow:auto;\">\r\nx_train = x_train.reshape(60000, 784).astype('float32') / 255\r\nx_test  = x_test.reshape(10000, 784).astype('float32') / 255\r\n\r\ny_train = keras.utils.to_categorical(y_train, 10)\r\ny_test  = keras.utils.to_categorical(y_test, 10)\r\n  </pre>\r\n\r\n  <h4 style=\"margin-top:0.5em;\">Шаг 3 — создание модели</h4>\r\n  <pre style=\"background:#f6f8fa;padding:10px;border-radius:6px;overflow:auto;\">\r\nmodel = Sequential()\r\nmodel.add(Dense(512, activation='relu', input_shape=(784,)))\r\nmodel.add(Dropout(0.2))\r\nmodel.add(Dense(512, activation='relu'))\r\nmodel.add(Dropout(0.2))\r\nmodel.add(Dense(10, activation='softmax'))\r\n  </pre>\r\n\r\n  <h4 style=\"margin-top:0.5em;\">Шаг 4 — компиляция</h4>\r\n  <pre style=\"background:#f6f8fa;padding:10px;border-radius:6px;overflow:auto;\">\r\nmodel.compile(loss='categorical_crossentropy',\r\n              optimizer=RMSprop(),\r\n              metrics=['accuracy'])\r\n  </pre>\r\n\r\n  <h4 style=\"margin-top:0.5em;\">Шаг 5 — обучение</h4>\r\n  <pre style=\"background:#f6f8fa;padding:10px;border-radius:6px;overflow:auto;\">\r\nhistory = model.fit(x_train, y_train,\r\n                    batch_size=128,\r\n                    epochs=20,\r\n                    verbose=1,\r\n                    validation_data=(x_test, y_test))\r\n  </pre>\r\n\r\n  <h3 style=\"color:#3b5fc0; margin-top:0.6em;\">Выводы</h3>\r\n  <p>После обучения модель можно оценить (<code>evaluate()</code>) и использовать для предсказаний (<code>predict()</code>). В процессе настройки можно изменять функцию потерь, оптимизатор и метрики, а также параметры обучения (batch_size, epochs) для улучшения качества.</p>\r\n</div>\r\n",
                    Position = 9,
                    DurationMinutes = 18,
                    IsPublished = true,
                    IsFreePreview = true
                },
                new
                {
                    Id = "keras-for-beginners-module-1.11",
                    ModuleId = "keras-for-beginners-main",
                    Title = "Keras - Оценка и прогнозирование с помощью модели",
                    Slug = "keras-model-evaluation-and-model-prediction",
                    Body = "<div style=\"font-family:'Segoe UI', Arial, sans-serif; line-height:1.6; color:#222;\">\r\n  <h2 style=\"color:#2a4d9b; margin-bottom:0.4em;\">Оценка и предсказание модели в Keras</h2>\r\n\r\n  <h3 style=\"color:#3b5fc0; margin-top:0.6em;\">Введение</h3>\r\n  <p>После того как модель обучена, необходимо проверить, насколько хорошо она справляется с задачей (оценка), и затем использовать её для получения предсказаний (prediction). В этом разделе мы рассмотрим оба этапа.</p>\r\n\r\n  <h3 style=\"color:#3b5fc0; margin-top:0.6em;\">Оценка модели (Model Evaluation)</h3>\r\n  <p>Оценка — это процесс проверки того, насколько хорошо обученная модель подходит для данных тестовой выборки.  \r\n  В Keras для этого используется метод <code>evaluate()</code>.</p>\r\n\r\n  <h4>Сигнатура метода:</h4>\r\n  <pre style=\"background:#f6f8fa;padding:10px;border-radius:6px;overflow:auto;\">\r\nscore = model.evaluate(x_test, y_test, verbose=0)\r\n  </pre>\r\n\r\n  <p><b>Основные аргументы:</b></p>\r\n  <ul style=\"margin-left:1.2em;\">\r\n    <li><code>x_test</code> — тестовые данные</li>\r\n    <li><code>y_test</code> — метки тестовых данных</li>\r\n    <li><code>verbose</code> — уровень вывода (0 или 1)</li>\r\n  </ul>\r\n\r\n  <p>Пример оценки модели, созданной в предыдущей главе:</p>\r\n  <pre style=\"background:#f6f8fa;padding:10px;border-radius:6px;overflow:auto;\">\r\nscore = model.evaluate(x_test, y_test, verbose=0)\r\n\r\nprint('Test loss:', score[0])\r\nprint('Test accuracy:', score[1])\r\n  </pre>\r\n\r\n  <p><b>Пример вывода:</b></p>\r\n  <pre style=\"background:#f6f8fa;padding:10px;border-radius:6px;overflow:auto;\">\r\nTest loss: 0.0981\r\nTest accuracy: 0.9828\r\n  </pre>\r\n\r\n  <p>Таким образом, точность тестирования составила около <b>98.28%</b>, что показывает отличное качество модели для распознавания рукописных цифр. Однако всегда есть возможность дальнейшего улучшения модели (например, подбором гиперпараметров).</p>\r\n\r\n  <h3 style=\"color:#3b5fc0; margin-top:0.6em;\">Предсказание модели (Model Prediction)</h3>\r\n  <p>Предсказание — это финальный этап, на котором мы используем обученную модель для прогнозирования результатов на новых данных.  \r\n  В Keras это выполняется методом <code>predict()</code>.</p>\r\n\r\n  <h4>Сигнатура метода:</h4>\r\n  <pre style=\"background:#f6f8fa;padding:10px;border-radius:6px;overflow:auto;\">\r\npredict(\r\n  x,\r\n  batch_size=None,\r\n  verbose=0,\r\n  steps=None,\r\n  callbacks=None,\r\n  max_queue_size=10,\r\n  workers=1,\r\n  use_multiprocessing=False\r\n)\r\n  </pre>\r\n\r\n  <p><b>Аргументы:</b></p>\r\n  <ul style=\"margin-left:1.2em;\">\r\n    <li><code>x</code> — входные данные для предсказания (обязательный аргумент)</li>\r\n    <li>Остальные параметры — необязательные и управляют процессом выполнения</li>\r\n  </ul>\r\n\r\n  <h4>Пример предсказания для MLP-модели:</h4>\r\n  <pre style=\"background:#f6f8fa;padding:10px;border-radius:6px;overflow:auto;\">\r\npred = model.predict(x_test)\r\npred = np.argmax(pred, axis=1)[:5]\r\nlabel = np.argmax(y_test, axis=1)[:5]\r\n\r\nprint(pred)\r\nprint(label)\r\n  </pre>\r\n\r\n  <p><b>Пояснения:</b></p>\r\n  <ul style=\"margin-left:1.2em;\">\r\n    <li>Строка 1 — получение предсказаний модели для тестовых данных.</li>\r\n    <li>Строка 2 — извлечение первых пяти предсказанных меток.</li>\r\n    <li>Строка 3 — извлечение первых пяти реальных меток тестовых данных.</li>\r\n    <li>Строки 5–6 — вывод результатов для сравнения.</li>\r\n  </ul>\r\n\r\n  <h4>Пример вывода:</h4>\r\n  <pre style=\"background:#f6f8fa;padding:10px;border-radius:6px;overflow:auto;\">\r\n[7 2 1 0 4]\r\n[7 2 1 0 4]\r\n  </pre>\r\n\r\n  <p>Оба массива совпадают, что означает, что модель правильно предсказала метки для первых пяти изображений.</p>\r\n\r\n  <h3 style=\"color:#3b5fc0; margin-top:0.6em;\">Вывод</h3>\r\n  <p>Мы научились:</p>\r\n  <ul style=\"margin-left:1.2em;\">\r\n    <li>оценивать модель с помощью <code>evaluate()</code>;</li>\r\n    <li>делать предсказания с помощью <code>predict()</code>;</li>\r\n    <li>сравнивать предсказанные и реальные метки для проверки точности.</li>\r\n  </ul>\r\n</div>\r\n",
                    Position = 10,
                    DurationMinutes = 6,
                    IsPublished = true,
                    IsFreePreview = true
                },
                new
                {
                    Id = "keras-for-beginners-module-1.12",
                    ModuleId = "keras-for-beginners-main",
                    Title = "Keras - Сверточная нейронная сеть",
                    Slug = "keras-convolution-neural-network",
                    Body = "<div style=\"font-family:'Segoe UI', Arial, sans-serif; line-height:1.6; color:#222;\">\r\n  <h2 style=\"color:#2a4d9b; margin-bottom:0.4em;\">Переход от MLP к сверточной нейронной сети (CNN) для задачи распознавания цифр</h2>\r\n\r\n  <p>Заменим ранее построенную MLP-модель на CNN для задачи распознавания рукописных цифр (MNIST). Ниже — пошаговое руководство с минимальным оформлением.</p>\r\n\r\n  <h3 style=\"color:#3b5fc0; margin-top:0.8em;\">Архитектура CNN</h3>\r\n  <ul style=\"margin-left:1.2em;\">\r\n    <li>Вход: shape = (28, 28, 1) (или (1, 28, 28) при <code>channels_first</code>).</li>\r\n    <li>Conv2D → 32 фильтра, <code>kernel_size=(3,3)</code>, <code>activation='relu'</code>.</li>\r\n    <li>Conv2D → 64 фильтра, <code>kernel_size=(3,3)</code>, <code>activation='relu'</code>.</li>\r\n    <li>MaxPooling2D → <code>pool_size=(2,2)</code>.</li>\r\n    <li>Dropout → 0.25.</li>\r\n    <li>Flatten.</li>\r\n    <li>Dense → 128 нейронов, <code>activation='relu'</code>.</li>\r\n    <li>Dropout → 0.5.</li>\r\n    <li>Dense → 10 нейронов, <code>activation='softmax'</code> (выход).</li>\r\n  </ul>\r\n\r\n  <h3 style=\"color:#3b5fc0; margin-top:0.8em;\">Параметры обучения</h3>\r\n  <ul style=\"margin-left:1.2em;\">\r\n    <li>Loss: <code>categorical_crossentropy</code>.</li>\r\n    <li>Optimizer: <code>Adadelta()</code>.</li>\r\n    <li>Metrics: <code>accuracy</code>.</li>\r\n    <li>Batch size: 128.</li>\r\n    <li>Epochs: 12 (в примере использовано 12; можно изменить).</li>\r\n  </ul>\r\n\r\n  <h3 style=\"color:#3b5fc0; margin-top:0.8em;\">Шаг 1 — импорт модулей</h3>\r\n  <pre style=\"background:#f6f8fa;padding:10px;border-radius:6px;overflow:auto;\">\r\nimport keras\r\nfrom keras.datasets import mnist\r\nfrom keras.models import Sequential\r\nfrom keras.layers import Dense, Dropout, Flatten\r\nfrom keras.layers import Conv2D, MaxPooling2D\r\nfrom keras import backend as K\r\nimport numpy as np\r\n  </pre>\r\n\r\n  <h3 style=\"color:#3b5fc0; margin-top:0.8em;\">Шаг 2 — загрузка данных</h3>\r\n  <pre style=\"background:#f6f8fa;padding:10px;border-radius:6px;overflow:auto;\">\r\n(x_train, y_train), (x_test, y_test) = mnist.load_data()\r\n  </pre>\r\n\r\n  <h3 style=\"color:#3b5fc0; margin-top:0.8em;\">Шаг 3 — предобработка данных</h3>\r\n  <pre style=\"background:#f6f8fa;padding:10px;border-radius:6px;overflow:auto;\">\r\nimg_rows, img_cols = 28, 28\r\n\r\nif K.image_data_format() == 'channels_first':\r\n    x_train = x_train.reshape(x_train.shape[0], 1, img_rows, img_cols)\r\n    x_test  = x_test.reshape(x_test.shape[0], 1, img_rows, img_cols)\r\n    input_shape = (1, img_rows, img_cols)\r\nelse:\r\n    x_train = x_train.reshape(x_train.shape[0], img_rows, img_cols, 1)\r\n    x_test  = x_test.reshape(x_test.shape[0], img_rows, img_cols, 1)\r\n    input_shape = (img_rows, img_cols, 1)\r\n\r\nx_train = x_train.astype('float32') / 255\r\nx_test  = x_test.astype('float32') / 255\r\n\r\ny_train = keras.utils.to_categorical(y_train, 10)\r\ny_test  = keras.utils.to_categorical(y_test, 10)\r\n  </pre>\r\n\r\n  <h3 style=\"color:#3b5fc0; margin-top:0.8em;\">Шаг 4 — создание модели</h3>\r\n  <pre style=\"background:#f6f8fa;padding:10px;border-radius:6px;overflow:auto;\">\r\nmodel = Sequential()\r\nmodel.add(Conv2D(32, kernel_size=(3, 3), activation='relu', input_shape=input_shape))\r\nmodel.add(Conv2D(64, kernel_size=(3, 3), activation='relu'))\r\nmodel.add(MaxPooling2D(pool_size=(2, 2)))\r\nmodel.add(Dropout(0.25))\r\nmodel.add(Flatten())\r\nmodel.add(Dense(128, activation='relu'))\r\nmodel.add(Dropout(0.5))\r\nmodel.add(Dense(10, activation='softmax'))\r\n  </pre>\r\n\r\n  <h3 style=\"color:#3b5fc0; margin-top:0.8em;\">Шаг 5 — компиляция</h3>\r\n  <pre style=\"background:#f6f8fa;padding:10px;border-radius:6px;overflow:auto;\">\r\nmodel.compile(loss=keras.losses.categorical_crossentropy,\r\n              optimizer=keras.optimizers.Adadelta(),\r\n              metrics=['accuracy'])\r\n  </pre>\r\n\r\n  <h3 style=\"color:#3b5fc0; margin-top:0.8em;\">Шаг 6 — обучение</h3>\r\n  <pre style=\"background:#f6f8fa;padding:10px;border-radius:6px;overflow:auto;\">\r\nmodel.fit(x_train, y_train,\r\n          batch_size=128,\r\n          epochs=12,\r\n          verbose=1,\r\n          validation_data=(x_test, y_test))\r\n  </pre>\r\n\r\n  <h3 style=\"color:#3b5fc0; margin-top:0.8em;\">Ожидаемый вывод (пример)</h3>\r\n  <pre style=\"background:#f6f8fa;padding:10px;border-radius:6px;overflow:auto;\">\r\nTrain on 60000 samples, validate on 10000 samples\r\nEpoch 1/12 ... - loss: 0.2687 - acc: 0.9173 - val_loss: 0.0549 - val_acc: 0.9827\r\n...\r\nEpoch 12/12 ... - loss: 0.0265 - acc: 0.9920 - val_loss: 0.0249 - val_acc: 0.9922\r\n  </pre>\r\n\r\n  <h3 style=\"color:#3b5fc0; margin-top:0.8em;\">Шаг 7 — оценка</h3>\r\n  <pre style=\"background:#f6f8fa;padding:10px;border-radius:6px;overflow:auto;\">\r\nscore = model.evaluate(x_test, y_test, verbose=0)\r\nprint('Test loss:', score[0])\r\nprint('Test accuracy:', score[1])\r\n  </pre>\r\n  <p>Пример результата: <code>Test loss: 0.0249</code>, <code>Test accuracy: 0.9922</code> (≈ 99.22%).</p>\r\n\r\n  <h3 style=\"color:#3b5fc0; margin-top:0.8em;\">Шаг 8 — предсказание</h3>\r\n  <pre style=\"background:#f6f8fa;padding:10px;border-radius:6px;overflow:auto;\">\r\npred = model.predict(x_test)\r\npred = np.argmax(pred, axis=1)[:5]\r\nlabel = np.argmax(y_test, axis=1)[:5]\r\n\r\nprint(pred)\r\nprint(label)\r\n  </pre>\r\n  <p>Ожидаемый вывод (первые 5):<br><code>[7 2 1 0 4]</code><br><code>[7 2 1 0 4]</code></p>\r\n\r\n</div>\r\n",
                    Position = 11,
                    DurationMinutes = 16,
                    IsPublished = true,
                    IsFreePreview = true
                },
                new
                {
                    Id = "keras-for-beginners-module-1.13",
                    ModuleId = "keras-for-beginners-main",
                    Title = "Keras - Регрессивное прогнозиорвание с MPL",
                    Slug = "keras-regression-prediction-using-mpl",
                    Body = "<div style=\"font-family:'Segoe UI', Arial, sans-serif; line-height:1.6; color:#222;\">\r\n  <h2 style=\"color:#2a4d9b; margin-bottom:0.4em;\">Регрессия с помощью MLP (Boston Housing)</h2>\r\n\r\n  <p>В этой главе создаём простую MLP-модель для задачи регрессии (предсказание цены жилья). Входной вектор содержит 13 признаков. Модель и параметры обучения описаны ниже.</p>\r\n\r\n  <h3 style=\"color:#3b5fc0; margin-top:0.8em;\">Архитектура модели</h3>\r\n  <ul style=\"margin-left:1.2em;\">\r\n    <li>Вход: <code>(13,)</code>.</li>\r\n    <li>Dense — 64 units, <code>kernel_initializer='normal'</code>, <code>activation='relu'</code>.</li>\r\n    <li>Dense — 64 units, <code>activation='relu'</code>.</li>\r\n    <li>Выходной Dense — 1 unit (регрессия).</li>\r\n  </ul>\r\n\r\n  <h3 style=\"color:#3b5fc0; margin-top:0.8em;\">Параметры обучения</h3>\r\n  <ul style=\"margin-left:1.2em;\">\r\n    <li>Loss: <code>mse</code>.</li>\r\n    <li>Optimizer: <code>RMSprop()</code>.</li>\r\n    <li>Metrics: <code>mean_absolute_error</code>.</li>\r\n    <li>Batch size: 128.</li>\r\n    <li>Epochs: до 500 (с <code>EarlyStopping</code>).</li>\r\n  </ul>\r\n\r\n  <h3 style=\"color:#3b5fc0; margin-top:0.8em;\">Шаг 1 — импорт модулей</h3>\r\n  <pre style=\"background:#f6f8fa;padding:10px;border-radius:6px;overflow:auto;\">\r\nimport keras\r\nfrom keras.datasets import boston_housing\r\nfrom keras.models import Sequential\r\nfrom keras.layers import Dense\r\nfrom keras.optimizers import RMSprop\r\nfrom keras.callbacks import EarlyStopping\r\nfrom sklearn import preprocessing\r\nfrom sklearn.preprocessing import scale\r\n  </pre>\r\n\r\n  <h3 style=\"color:#3b5fc0; margin-top:0.8em;\">Шаг 2 — загрузка данных</h3>\r\n  <pre style=\"background:#f6f8fa;padding:10px;border-radius:6px;overflow:auto;\">\r\n(x_train, y_train), (x_test, y_test) = boston_housing.load_data()\r\n  </pre>\r\n\r\n  <h3 style=\"color:#3b5fc0; margin-top:0.8em;\">Шаг 3 — предобработка</h3>\r\n  <p>Нормализация признаков: масштабируем тренировочную выборку и применяем те же параметры к тестовой.</p>\r\n  <pre style=\"background:#f6f8fa;padding:10px;border-radius:6px;overflow:auto;\">\r\nx_train_scaled = preprocessing.scale(x_train)\r\nscaler = preprocessing.StandardScaler().fit(x_train)\r\nx_test_scaled = scaler.transform(x_test)\r\n  </pre>\r\n\r\n  <h3 style=\"color:#3b5fc0; margin-top:0.8em;\">Шаг 4 — создание модели</h3>\r\n  <pre style=\"background:#f6f8fa;padding:10px;border-radius:6px;overflow:auto;\">\r\nmodel = Sequential()\r\nmodel.add(Dense(64, kernel_initializer='normal', activation='relu', input_shape=(13,)))\r\nmodel.add(Dense(64, activation='relu'))\r\nmodel.add(Dense(1))\r\n  </pre>\r\n\r\n  <h3 style=\"color:#3b5fc0; margin-top:0.8em;\">Шаг 5 — компиляция</h3>\r\n  <pre style=\"background:#f6f8fa;padding:10px;border-radius:6px;overflow:auto;\">\r\nmodel.compile(\r\n  loss='mse',\r\n  optimizer=RMSprop(),\r\n  metrics=['mean_absolute_error']\r\n)\r\n  </pre>\r\n\r\n  <h3 style=\"color:#3b5fc0; margin-top:0.8em;\">Шаг 6 — обучение</h3>\r\n  <p>Используем <code>EarlyStopping</code>, чтобы прекратить обучение при отсутствии улучшения по валидационной потере.</p>\r\n  <pre style=\"background:#f6f8fa;padding:10px;border-radius:6px;overflow:auto;\">\r\nhistory = model.fit(\r\n  x_train_scaled, y_train,\r\n  batch_size=128,\r\n  epochs=500,\r\n  verbose=1,\r\n  validation_split=0.2,\r\n  callbacks=[EarlyStopping(monitor='val_loss', patience=20)]\r\n)\r\n  </pre>\r\n\r\n  <h3 style=\"color:#3b5fc0; margin-top:0.8em;\">Пример вывода во время обучения</h3>\r\n  <pre style=\"background:#f6f8fa;padding:10px;border-radius:6px;overflow:auto;\">\r\nTrain on 323 samples, validate on 81 samples\r\n...\r\nEpoch 143/500 - loss: 8.1004 - mean_absolute_error: 2.0002 - val_loss: 14.6286 - val_mean_absolute_error: 2.5904\r\nEpoch 144/500 - loss: 8.0300 - mean_absolute_error: 1.9683 - val_loss: 14.5949 - val_mean_absolute_error: 2.5843\r\nEpoch 145/500 - loss: 7.8704 - mean_absolute_error: 1.9313 - val_loss: 14.3770 - val_mean_absolute_error: 2.4996\r\n  </pre>\r\n\r\n  <h3 style=\"color:#3b5fc0; margin-top:0.8em;\">Шаг 7 — оценка</h3>\r\n  <pre style=\"background:#f6f8fa;padding:10px;border-radius:6px;overflow:auto;\">\r\nscore = model.evaluate(x_test_scaled, y_test, verbose=0)\r\nprint('Test loss:', score[0])\r\nprint('Test MAE:', score[1])\r\n  </pre>\r\n  <p>Пример: <code>Test loss: 21.92847</code>, <code>Test MAE: 2.95992</code>.</p>\r\n\r\n  <h3 style=\"color:#3b5fc0; margin-top:0.8em;\">Шаг 8 — предсказание</h3>\r\n  <pre style=\"background:#f6f8fa;padding:10px;border-radius:6px;overflow:auto;\">\r\nprediction = model.predict(x_test_scaled)\r\nprint(prediction.flatten())\r\nprint(y_test)\r\n  </pre>\r\n\r\n  <p>Вывод показывает предсказанные и реальные значения цен. Разница обычно находится в разумном диапазоне (в примере около 10–30%).</p>\r\n\r\n </div>\r\n",
                    Position = 12,
                    DurationMinutes = 16,
                    IsPublished = true,
                    IsFreePreview = true
                },
                new
                {
                    Id = "keras-for-beginners-module-1.14",
                    ModuleId = "keras-for-beginners-main",
                    Title = "Keras - Прогнозирование временных рядов с LSTM RNN",
                    Slug = "keras-time-series-prediction-using-lstm-rnn",
                    Body = "",
                    Position = 13,
                    DurationMinutes = 16,
                    IsPublished = true,
                    IsFreePreview = true
                },
                new
                {
                    Id = "keras-for-beginners-module-1.15",
                    ModuleId = "keras-for-beginners-main",
                    Title = "Keras - Слои",
                    Slug = "keras-layers",
                    Body = "",
                    Position = 14,
                    DurationMinutes = 16,
                    IsPublished = true,
                    IsFreePreview = true
                },
                new
                {
                    Id = "keras-for-beginners-module-1.16",
                    ModuleId = "keras-for-beginners-main",
                    Title = "Keras - Слои",
                    Slug = "keras-layers",
                    Body = "",
                    Position = 15,
                    DurationMinutes = 16,
                    IsPublished = true,
                    IsFreePreview = true
                },
                new
                {
                    Id = "keras-for-beginners-module-1.17",
                    ModuleId = "keras-for-beginners-main",
                    Title = "Keras - Слои",
                    Slug = "keras-layers",
                    Body = "",
                    Position = 16,
                    DurationMinutes = 16,
                    IsPublished = true,
                    IsFreePreview = true
                }
            );

            //// Ресурсы (несколько примеров)
            //builder.Entity<LessonResource>().HasData(
            //    new
            //    {
            //        Id = "c3333333-3333-3333-3333-ccccccccccc4",
            //        LessonId = "b2222222-2222-2222-2222-bbbbbbbbbbb4", // control-flow
            //        Url = "https://docs.microsoft.com/dotnet/csharp/programming-guide/inside-a-program",
            //        ResourceType = "article",
            //        Title = "Документация Microsoft: Inside a Program",
            //        Position = 0
            //    },
            //    new
            //    {
            //        Id = "c3333333-3333-3333-3333-ccccccccccc5",
            //        LessonId = "b2222222-2222-2222-2222-bbbbbbbbbbb8", // inheritance
            //        Url = "https://docs.microsoft.com/dotnet/csharp/fundamentals/object-oriented/",
            //        ResourceType = "article",
            //        Title = "OOP в C# — официальная документация",
            //        Position = 0
            //    },
            //    new
            //    {
            //        Id = "c3333333-3333-3333-3333-ccccccccccc6",
            //        LessonId = "b2222222-2222-2222-2222-bbbbbbbbb12", // linq-intro
            //        Url = "https://docs.microsoft.com/dotnet/csharp/programming-guide/concepts/linq/",
            //        ResourceType = "article",
            //        Title = "LINQ — руководство",
            //        Position = 0
            //    },
            //    new
            //    {
            //        Id = "c3333333-3333-3333-3333-ccccccccccc7",
            //        LessonId = "b2222222-2222-2222-2222-bbbbbbbbb17", // http-httpclient
            //        Url = "https://learn.microsoft.com/dotnet/api/system.net.http.httpclient",
            //        ResourceType = "article",
            //        Title = "HttpClient — справочник",
            //        Position = 0
            //    },
            //    new
            //    {
            //        Id = "c3333333-3333-3333-3333-ccccccccccc8",
            //        LessonId = "b2222222-2222-2222-2222-bbbbbbbbb20", // efcore
            //        Url = "https://learn.microsoft.com/ef/core/",
            //        ResourceType = "article",
            //        Title = "EF Core — официальная документация",
            //        Position = 0
            //    }
            //);


        }
    }
}
