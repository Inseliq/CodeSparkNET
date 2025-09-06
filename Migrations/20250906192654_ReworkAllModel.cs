using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CodeSparkNET.Migrations
{
    /// <inheritdoc />
    public partial class ReworkAllModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("136eaf07-0d67-4999-9209-d5f0f458d02c"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("13856811-69dc-46c2-ab08-991286d89a01"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("8c7b7360-47de-49cd-9d4c-1a1e7e5dbc3b"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("91757b46-3d2a-401a-8d1e-730a6ebe0cd3"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("cb0d9f77-feab-4eb4-b2a0-0dc43fbf6d4c"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("2f40ca2d-4a0f-492c-9cdf-08076e4af3bb"));

            migrationBuilder.UpdateData(
                table: "Catalogs",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 10, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 10, 12, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "CreatedAt", "Description", "IconUrl", "ImageUrl", "IsActive", "IsVisible", "MetaDescription", "MetaKeywords", "MetaTitle", "Name", "ParentCategoryId", "Slug", "SortOrder", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), new DateTime(2024, 10, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "Курсы по программированию и разработке ПО", null, null, true, true, "Изучите программирование с нашими курсами", null, "Курсы программирования", "Программирование", null, "programming", 1, new DateTime(2024, 10, 12, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("44444444-4444-4444-4444-444444444444"), new DateTime(2024, 10, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "Курсы по анализу данных и машинному обучению", null, null, true, true, "Изучите анализ данных и машинное обучение", null, "Курсы Data Science", "Data Science", null, "data-science", 2, new DateTime(2024, 10, 12, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("55555555-5555-5555-5555-555555555555"), new DateTime(2024, 10, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "Готовые шаблоны для веб-сайтов", null, null, true, true, "Скачайте готовые шаблоны для ваших проектов", null, "Веб-шаблоны", "Веб-шаблоны", null, "web-templates", 3, new DateTime(2024, 10, 12, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("66666666-6666-6666-6666-666666666666"), new DateTime(2024, 10, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "Готовые дипломные и курсовые работы", null, null, true, true, "Качественные дипломные работы по программированию", null, "Дипломные работы", "Дипломные работы", null, "diploma-works", 4, new DateTime(2024, 10, 12, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("22222222-2222-2222-2222-222222222222"), new DateTime(2024, 10, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "Курсы по созданию веб-сайтов и веб-приложений", null, null, true, true, "Освойте создание современных веб-приложений", null, "Курсы веб-разработки", "Веб-разработка", new Guid("11111111-1111-1111-1111-111111111111"), "web-development", 1, new DateTime(2024, 10, 12, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("33333333-3333-3333-3333-333333333333"), new DateTime(2024, 10, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "Курсы по созданию мобильных приложений", null, null, true, true, "Создавайте мобильные приложения для iOS и Android", null, "Курсы мобильной разработки", "Мобильная разработка", new Guid("11111111-1111-1111-1111-111111111111"), "mobile-development", 2, new DateTime(2024, 10, 12, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));

            migrationBuilder.UpdateData(
                table: "Catalogs",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 6, 19, 23, 42, 920, DateTimeKind.Utc).AddTicks(5423), new DateTime(2025, 9, 6, 19, 23, 42, 920, DateTimeKind.Utc).AddTicks(5655) });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "CreatedAt", "Description", "IconUrl", "ImageUrl", "IsActive", "IsVisible", "MetaDescription", "MetaKeywords", "MetaTitle", "Name", "ParentCategoryId", "Slug", "SortOrder", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("136eaf07-0d67-4999-9209-d5f0f458d02c"), new DateTime(2025, 9, 6, 19, 23, 42, 921, DateTimeKind.Utc).AddTicks(8191), "Готовые шаблоны для веб-сайтов", null, null, true, true, "Скачайте готовые шаблоны для ваших проектов", null, "Веб-шаблоны", "Веб-шаблоны", null, "web-templates", 3, new DateTime(2025, 9, 6, 19, 23, 42, 921, DateTimeKind.Utc).AddTicks(8192) },
                    { new Guid("13856811-69dc-46c2-ab08-991286d89a01"), new DateTime(2025, 9, 6, 19, 23, 42, 921, DateTimeKind.Utc).AddTicks(8452), "Готовые дипломные и курсовые работы", null, null, true, true, "Качественные дипломные работы по программированию", null, "Дипломные работы", "Дипломные работы", null, "diploma-works", 4, new DateTime(2025, 9, 6, 19, 23, 42, 921, DateTimeKind.Utc).AddTicks(8453) },
                    { new Guid("2f40ca2d-4a0f-492c-9cdf-08076e4af3bb"), new DateTime(2025, 9, 6, 19, 23, 42, 921, DateTimeKind.Utc).AddTicks(6037), "Курсы по программированию и разработке ПО", null, null, true, true, "Изучите программирование с нашими курсами", null, "Курсы программирования", "Программирование", null, "programming", 1, new DateTime(2025, 9, 6, 19, 23, 42, 921, DateTimeKind.Utc).AddTicks(6258) },
                    { new Guid("cb0d9f77-feab-4eb4-b2a0-0dc43fbf6d4c"), new DateTime(2025, 9, 6, 19, 23, 42, 921, DateTimeKind.Utc).AddTicks(7923), "Курсы по анализу данных и машинному обучению", null, null, true, true, "Изучите анализ данных и машинное обучение", null, "Курсы Data Science", "Data Science", null, "data-science", 2, new DateTime(2025, 9, 6, 19, 23, 42, 921, DateTimeKind.Utc).AddTicks(7923) },
                    { new Guid("8c7b7360-47de-49cd-9d4c-1a1e7e5dbc3b"), new DateTime(2025, 9, 6, 19, 23, 42, 921, DateTimeKind.Utc).AddTicks(7362), "Курсы по созданию веб-сайтов и веб-приложений", null, null, true, true, "Освойте создание современных веб-приложений", null, "Курсы веб-разработки", "Веб-разработка", new Guid("2f40ca2d-4a0f-492c-9cdf-08076e4af3bb"), "web-development", 1, new DateTime(2025, 9, 6, 19, 23, 42, 921, DateTimeKind.Utc).AddTicks(7363) },
                    { new Guid("91757b46-3d2a-401a-8d1e-730a6ebe0cd3"), new DateTime(2025, 9, 6, 19, 23, 42, 921, DateTimeKind.Utc).AddTicks(7654), "Курсы по созданию мобильных приложений", null, null, true, true, "Создавайте мобильные приложения для iOS и Android", null, "Курсы мобильной разработки", "Мобильная разработка", new Guid("2f40ca2d-4a0f-492c-9cdf-08076e4af3bb"), "mobile-development", 2, new DateTime(2025, 9, 6, 19, 23, 42, 921, DateTimeKind.Utc).AddTicks(7654) }
                });
        }
    }
}
