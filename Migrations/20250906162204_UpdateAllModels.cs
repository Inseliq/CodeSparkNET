using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CodeSparkNET.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAllModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    RegisteredAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    EmailMarketingConsent = table.Column<bool>(type: "bit", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ShortDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ThumbnailUrl = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    ProductType = table.Column<int>(type: "int", nullable: false),
                    IsPublished = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CourseDetails",
                columns: table => new
                {
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FullDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EstimatedHours = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseDetails", x => x.ProductId);
                    table.ForeignKey(
                        name: "FK_CourseDetails_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DiplomaDetail",
                columns: table => new
                {
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FullDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DiplomaType = table.Column<int>(type: "int", nullable: false),
                    Specialization = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Keywords = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DocumentUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PresentationUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SourceCodeUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Bibliography = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Requirements = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedYear = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HasPlagiarismCheck = table.Column<bool>(type: "bit", nullable: false),
                    PlagiarismPercentage = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiplomaDetail", x => x.ProductId);
                    table.ForeignKey(
                        name: "FK_DiplomaDetail_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WebTemplateDetail",
                columns: table => new
                {
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FullDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Framework = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Technologies = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsResponsive = table.Column<bool>(type: "bit", nullable: false),
                    BrowserSupport = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SourceCodeUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DemoUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DocumentationUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FigmaUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PagesCount = table.Column<int>(type: "int", nullable: false),
                    ColorScheme = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FontsUsed = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HasDarkMode = table.Column<bool>(type: "bit", nullable: false),
                    HasAnimations = table.Column<bool>(type: "bit", nullable: false),
                    Features = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Dependencies = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InstallationInstructions = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    License = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebTemplateDetail", x => x.ProductId);
                    table.ForeignKey(
                        name: "FK_WebTemplateDetail_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CourseEnrollments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EnrolledAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseEnrollments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseEnrollments_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseEnrollments_CourseDetails_CourseId",
                        column: x => x.CourseId,
                        principalTable: "CourseDetails",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Modules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Modules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Modules_CourseDetails_CourseId",
                        column: x => x.CourseId,
                        principalTable: "CourseDetails",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DiplomaOrder",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AppUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DiplomaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CustomerNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AdminNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaidAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiplomaOrder", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiplomaOrder_AspNetUsers_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DiplomaOrder_DiplomaDetail_DiplomaId",
                        column: x => x.DiplomaId,
                        principalTable: "DiplomaDetail",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WebTemplateOrder",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AppUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CustomerNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AdminNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaidAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebTemplateOrder", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WebTemplateOrder_AspNetUsers_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WebTemplateOrder_WebTemplateDetail_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "WebTemplateDetail",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WebTemplateReview",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AppUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    ReviewText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsPublished = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebTemplateReview", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WebTemplateReview_AspNetUsers_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WebTemplateReview_WebTemplateDetail_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "WebTemplateDetail",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CourseAssignments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModuleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DeadLine = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CourseDetailProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseAssignments_CourseDetails_CourseDetailProductId",
                        column: x => x.CourseDetailProductId,
                        principalTable: "CourseDetails",
                        principalColumn: "ProductId");
                    table.ForeignKey(
                        name: "FK_CourseAssignments_Modules_ModuleId",
                        column: x => x.ModuleId,
                        principalTable: "Modules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Lessons",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModuleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lessons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Lessons_Modules_ModuleId",
                        column: x => x.ModuleId,
                        principalTable: "Modules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserAssignments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssignmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AnswerText = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    FileUrl = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserAssignments_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserAssignments_CourseAssignments_AssignmentId",
                        column: x => x.AssignmentId,
                        principalTable: "CourseAssignments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "a3f9c6d2-1f4b-4b8e-9f2a-111111111111", "c1f9c6d2-1f4b-4b8e-9f2a-111111111111", "Admin", "ADMIN" },
                    { "b4f9c6d2-2f4b-4b8e-9f2a-222222222222", "d2f9c6d2-2f4b-4b8e-9f2a-222222222222", "User", "USER" },
                    { "c5f9c6d2-3f4b-4b8e-9f2a-333333333333", "e3f9c6d2-3f4b-4b8e-9f2a-333333333333", "Prime", "PRIME" }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CreatedAt", "IsPublished", "Price", "ProductType", "ShortDescription", "Slug", "ThumbnailUrl", "Title" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), new DateTime(2024, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc), true, 4999.00m, 1, "Изучите JavaScript от основ до продвинутых концепций. Практические проекты, ES6+, асинхронное программирование.", "javascript-full-course", "/images/courses/javascript-course.jpg", "Полный курс JavaScript с нуля до профи" },
                    { new Guid("22222222-2222-2222-2222-222222222222"), new DateTime(2024, 2, 10, 0, 0, 0, 0, DateTimeKind.Utc), true, 25000.00m, 3, "Дипломная работа по разработке полнофункционального веб-приложения с использованием ASP.NET Core и React.", "project-management-web-app-thesis", "/images/diplomas/project-management-thesis.jpg", "Разработка веб-приложения для управления проектами" },
                    { new Guid("33333333-3333-3333-3333-333333333333"), new DateTime(2024, 3, 5, 0, 0, 0, 0, DateTimeKind.Utc), true, 2999.00m, 2, "Стильный адаптивный лендинг с анимациями, темной темой и современным дизайном. HTML5, CSS3, JavaScript.", "modern-it-landing-template", "/images/templates/modern-landing.jpg", "ModernLanding - Современный лендинг для IT-компании" }
                });

            migrationBuilder.InsertData(
                table: "CourseDetails",
                columns: new[] { "ProductId", "EstimatedHours", "FullDescription" },
                values: new object[] { new Guid("11111111-1111-1111-1111-111111111111"), 120, "Этот курс предназначен для всех, кто хочет освоить JavaScript с нуля и стать профессиональным разработчиком.\r\n\r\n**Что вы изучите:**\r\n- Основы JavaScript: переменные, функции, объекты\r\n- ES6+ возможности: стрелочные функции, деструктуризация, модули\r\n- DOM манипуляции и обработка событий\r\n- Асинхронное программирование: Promises, async/await\r\n- Работа с API и AJAX запросами\r\n- Современные фреймворки и библиотеки\r\n- Тестирование JavaScript кода\r\n- Оптимизация и лучшие практики\r\n\r\n**Проекты в курсе:**\r\n1. Калькулятор с расширенными функциями\r\n2. Todo-приложение с localStorage\r\n3. Погодное приложение с API\r\n4. Интерактивная игра\r\n5. Мини-социальная сеть\r\n\r\nКурс включает более 50 практических заданий и 5 крупных проектов для портфолио." });

            migrationBuilder.InsertData(
                table: "DiplomaDetail",
                columns: new[] { "ProductId", "Bibliography", "CreatedYear", "DiplomaType", "DocumentUrl", "FullDescription", "HasPlagiarismCheck", "Keywords", "PlagiarismPercentage", "PresentationUrl", "Requirements", "SourceCodeUrl", "Specialization", "Subject" },
                values: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), "1. Троelsen Э. C# 10 и .NET 6. Полное руководство. - СПб.: Питер, 2022.\r\n2. Фримен А. ASP.NET Core MVC. Полное руководство. - СПб.: Питер, 2021.\r\n3. Гринс Р. React быстро. Веб-приложения на React, JSX, Redux и GraphQL. - СПб.: Питер, 2020.\r\n4. Microsoft Docs. ASP.NET Core Documentation. - URL: https://docs.microsoft.com/aspnet/core/\r\n5. React Documentation. - URL: https://reactjs.org/docs/", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, "/files/diplomas/project-management-app-thesis.pdf", "Дипломная работа представляет собой полнофункциональное веб-приложение для управления проектами и задачами в команде.\r\n\r\n**Основные возможности системы:**\r\n- Регистрация и авторизация пользователей\r\n- Создание и управление проектами\r\n- Система ролей (администратор, менеджер, разработчик)\r\n- Трекинг времени выполнения задач\r\n- Канбан-доски для визуализации процесса\r\n- Система уведомлений и комментариев\r\n- Отчеты и аналитика\r\n- Интеграция с внешними API\r\n\r\n**Технический стек:**\r\n- Backend: ASP.NET Core 6.0, Entity Framework Core\r\n- Frontend: React 18, TypeScript, Material-UI\r\n- База данных: SQL Server\r\n- Аутентификация: JWT токены\r\n- Тестирование: xUnit, Jest\r\n- Развертывание: Docker, Azure\r\n\r\nРабота включает полную техническую документацию, диаграммы архитектуры, тестирование и презентацию.", true, "веб-приложение, управление проектами, ASP.NET Core, React, Entity Framework, система управления задачами", 92.5m, "/files/diplomas/project-management-app-presentation.pptx", "Система должна поддерживать не менее 100 одновременных пользователей, обеспечивать безопасность данных и иметь адаптивный интерфейс.", "/files/diplomas/project-management-app-source.zip", "09.03.02 Информационные системы и технологии", "Информационные системы и технологии" });

            migrationBuilder.InsertData(
                table: "WebTemplateDetail",
                columns: new[] { "ProductId", "BrowserSupport", "ColorScheme", "DemoUrl", "Dependencies", "DocumentationUrl", "Features", "FigmaUrl", "FontsUsed", "Framework", "FullDescription", "HasAnimations", "HasDarkMode", "InstallationInstructions", "IsResponsive", "License", "PagesCount", "SourceCodeUrl", "Technologies" },
                values: new object[] { new Guid("33333333-3333-3333-3333-333333333333"), "Chrome 90+, Firefox 88+, Safari 14+, Edge 90+", "Темно-синий градиент (#1a1a2e, #16213e, #0f3460) с акцентным оранжевым (#ff6b35)", "https://demo.codesparknet.com/modern-landing", "Bootstrap 5.3.0, AOS.js 2.3.4, Swiper.js 8.4.7, Font Awesome 6.4.0", "/files/templates/modern-landing-docs.pdf", "Параллакс эффекты, плавная прокрутка, анимации при скролле, интерактивные элементы, адаптивное меню, контактная форма, слайдеры, модальные окна", "https://figma.com/file/modern-landing-design", "Inter (основной), Space Grotesk (заголовки)", "Vanilla JavaScript", "ModernLanding - это современный и стильный шаблон лендинга, специально разработанный для IT-компаний, стартапов и технологических проектов.\r\n\r\n**Особенности дизайна:**\r\n- Минималистичный и современный стиль\r\n- Градиентные элементы и плавные анимации\r\n- Темная и светлая темы\r\n- Адаптивный дизайн для всех устройств\r\n- Высокая скорость загрузки\r\n\r\n**Секции шаблона:**\r\n1. Hero секция с анимированным фоном\r\n2. О компании с интерактивными карточками\r\n3. Услуги с hover-эффектами\r\n4. Портфолио с фильтрацией\r\n5. Команда с социальными ссылками\r\n6. Отзывы клиентов (слайдер)\r\n7. Контакты с интерактивной картой\r\n8. Подвал с полезными ссылками\r\n\r\n**Технические возможности:**\r\n- Плавная прокрутка между секциями\r\n- Параллакс эффекты\r\n- Анимации при скролле (AOS)\r\n- Валидация контактной формы\r\n- Интеграция с Google Analytics\r\n- SEO оптимизация\r\n\r\nШаблон готов к использованию и легко кастомизируется под ваши потребности.", true, true, "1. Распакуйте архив в папку вашего проекта\r\n2. Откройте index.html в браузере для просмотра\r\n3. Настройте контактную форму в файле js/contact.js\r\n4. Замените демо-контент на ваш в HTML файлах\r\n5. Настройте цвета в файле scss/_variables.scss\r\n6. Скомпилируйте SCSS в CSS (опционально)\r\n\r\nПодробная инструкция находится в файле README.md", true, "Standard License (может использоваться для одного проекта)", 1, "/files/templates/modern-landing-source.zip", "HTML5, CSS3, JavaScript ES6+, SCSS, Bootstrap 5, AOS.js, Swiper.js" });

            migrationBuilder.InsertData(
                table: "Modules",
                columns: new[] { "Id", "CourseId", "Order", "Title" },
                values: new object[,]
                {
                    { new Guid("44444444-4444-4444-4444-444444444444"), new Guid("11111111-1111-1111-1111-111111111111"), 1, "Основы JavaScript" },
                    { new Guid("55555555-5555-5555-5555-555555555555"), new Guid("11111111-1111-1111-1111-111111111111"), 2, "DOM и события" },
                    { new Guid("66666666-6666-6666-6666-666666666666"), new Guid("11111111-1111-1111-1111-111111111111"), 3, "Асинхронное программирование" }
                });

            migrationBuilder.InsertData(
                table: "CourseAssignments",
                columns: new[] { "Id", "CourseDetailProductId", "DeadLine", "Description", "ModuleId", "Title" },
                values: new object[,]
                {
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), null, new DateTime(2024, 12, 31, 23, 59, 59, 0, DateTimeKind.Utc), "Создайте простой калькулятор с базовыми арифметическими операциями. Используйте функции для каждой операции.", new Guid("44444444-4444-4444-4444-444444444444"), "Создание калькулятора" },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), null, new DateTime(2025, 1, 15, 23, 59, 59, 0, DateTimeKind.Utc), "Создайте форму с валидацией полей в реальном времени. Добавьте обработчики событий для всех полей.", new Guid("55555555-5555-5555-5555-555555555555"), "Интерактивная форма" }
                });

            migrationBuilder.InsertData(
                table: "Lessons",
                columns: new[] { "Id", "Content", "ModuleId", "Order", "Title" },
                values: new object[,]
                {
                    { new Guid("77777777-7777-7777-7777-777777777771"), "В этом уроке мы изучим историю JavaScript, его роль в веб-разработке и настроим среду разработки.", new Guid("44444444-4444-4444-4444-444444444444"), 1, "Введение в JavaScript" },
                    { new Guid("77777777-7777-7777-7777-777777777772"), "Изучаем объявление переменных с помощью var, let и const. Разбираем примитивные и ссылочные типы данных.", new Guid("44444444-4444-4444-4444-444444444444"), 2, "Переменные и типы данных" },
                    { new Guid("77777777-7777-7777-7777-777777777773"), "Изучаем объявление функций, параметры, возвращаемые значения и область видимости.", new Guid("44444444-4444-4444-4444-444444444444"), 3, "Функции" },
                    { new Guid("88888888-8888-8888-8888-888888888881"), "Изучаем Document Object Model, поиск элементов и изменение содержимого страницы.", new Guid("55555555-5555-5555-5555-555555555555"), 1, "Работа с DOM" },
                    { new Guid("88888888-8888-8888-8888-888888888882"), "Изучаем addEventListener, типы событий и делегирование событий.", new Guid("55555555-5555-5555-5555-555555555555"), 2, "Обработка событий" },
                    { new Guid("99999999-9999-9999-9999-999999999991"), "Изучаем асинхронное программирование, обещания и современный синтаксис async/await.", new Guid("66666666-6666-6666-6666-666666666666"), 1, "Promises и async/await" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CourseAssignments_CourseDetailProductId",
                table: "CourseAssignments",
                column: "CourseDetailProductId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseAssignments_ModuleId",
                table: "CourseAssignments",
                column: "ModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseEnrollment_EnrolledAt",
                table: "CourseEnrollments",
                column: "EnrolledAt");

            migrationBuilder.CreateIndex(
                name: "IX_CourseEnrollment_User_Course",
                table: "CourseEnrollments",
                columns: new[] { "UserId", "CourseId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CourseEnrollments_CourseId",
                table: "CourseEnrollments",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_DiplomaDetail_DiplomaType",
                table: "DiplomaDetail",
                column: "DiplomaType");

            migrationBuilder.CreateIndex(
                name: "IX_DiplomaOrder_AppUserId",
                table: "DiplomaOrder",
                column: "AppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_DiplomaOrder_DiplomaId",
                table: "DiplomaOrder",
                column: "DiplomaId");

            migrationBuilder.CreateIndex(
                name: "IX_DiplomaOrder_OrderDate",
                table: "DiplomaOrder",
                column: "OrderDate");

            migrationBuilder.CreateIndex(
                name: "IX_DiplomaOrder_Status",
                table: "DiplomaOrder",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Lesson_Module_Order",
                table: "Lessons",
                columns: new[] { "ModuleId", "Order" });

            migrationBuilder.CreateIndex(
                name: "IX_Module_Course_Order",
                table: "Modules",
                columns: new[] { "CourseId", "Order" });

            migrationBuilder.CreateIndex(
                name: "IX_Products_Slug",
                table: "Products",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserAssignment_SubmittedAt",
                table: "UserAssignments",
                column: "SubmittedAt");

            migrationBuilder.CreateIndex(
                name: "IX_UserAssignment_User_Assignment",
                table: "UserAssignments",
                columns: new[] { "UserId", "AssignmentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserAssignments_AssignmentId",
                table: "UserAssignments",
                column: "AssignmentId");

            migrationBuilder.CreateIndex(
                name: "IX_WebTemplateDetail_Framework",
                table: "WebTemplateDetail",
                column: "Framework");

            migrationBuilder.CreateIndex(
                name: "IX_TemplateOrder_OrderDate",
                table: "WebTemplateOrder",
                column: "OrderDate");

            migrationBuilder.CreateIndex(
                name: "IX_TemplateOrder_Status",
                table: "WebTemplateOrder",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_WebTemplateOrder_AppUserId",
                table: "WebTemplateOrder",
                column: "AppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_WebTemplateOrder_TemplateId",
                table: "WebTemplateOrder",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_TemplateReview_CreatedAt",
                table: "WebTemplateReview",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_TemplateReview_Template_Rating",
                table: "WebTemplateReview",
                columns: new[] { "TemplateId", "Rating" });

            migrationBuilder.CreateIndex(
                name: "IX_WebTemplateReview_AppUserId",
                table: "WebTemplateReview",
                column: "AppUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "CourseEnrollments");

            migrationBuilder.DropTable(
                name: "DiplomaOrder");

            migrationBuilder.DropTable(
                name: "Lessons");

            migrationBuilder.DropTable(
                name: "UserAssignments");

            migrationBuilder.DropTable(
                name: "WebTemplateOrder");

            migrationBuilder.DropTable(
                name: "WebTemplateReview");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "DiplomaDetail");

            migrationBuilder.DropTable(
                name: "CourseAssignments");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "WebTemplateDetail");

            migrationBuilder.DropTable(
                name: "Modules");

            migrationBuilder.DropTable(
                name: "CourseDetails");

            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
