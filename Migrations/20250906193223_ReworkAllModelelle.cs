using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CodeSparkNET.Migrations
{
    /// <inheritdoc />
    public partial class ReworkAllModelelle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "a3f9c6d2-1f4b-4b8e-9f2a-111111111111", "c1f9c6d2-1f4b-4b8e-9f2a-111111111111", "Admin", "ADMIN" },
                    { "b4f9c6d2-2f4b-4b8e-9f2a-222222222222", "d2f9c6d2-2f4b-4b8e-9f2a-222222222222", "User", "USER" },
                    { "c5f9c6d2-3f4b-4b8e-9f2a-333333333333", "e3f9c6d2-3f4b-4b8e-9f2a-333333333333", "Prime", "PRIME" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a3f9c6d2-1f4b-4b8e-9f2a-111111111111");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b4f9c6d2-2f4b-4b8e-9f2a-222222222222");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c5f9c6d2-3f4b-4b8e-9f2a-333333333333");
        }
    }
}
