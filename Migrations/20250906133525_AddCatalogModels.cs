using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CodeSparkNET.Migrations
{
    /// <inheritdoc />
    public partial class AddCatalogModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseAssignment_Module_ModuleId",
                table: "CourseAssignment");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseAssignment_Product_ProductId",
                table: "CourseAssignment");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseDetail_Product_ProductId",
                table: "CourseDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseEnrollment_AspNetUsers_AppUserId",
                table: "CourseEnrollment");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseEnrollment_CourseDetail_CourseId",
                table: "CourseEnrollment");

            migrationBuilder.DropForeignKey(
                name: "FK_Lesson_Module_ModuleId",
                table: "Lesson");

            migrationBuilder.DropForeignKey(
                name: "FK_Module_CourseDetail_CourseId",
                table: "Module");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAssignment_AspNetUsers_AppUserId",
                table: "UserAssignment");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAssignment_CourseAssignment_AssignmentId",
                table: "UserAssignment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserAssignment",
                table: "UserAssignment");

            migrationBuilder.DropIndex(
                name: "IX_UserAssignment_AppUserId",
                table: "UserAssignment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Product",
                table: "Product");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Module",
                table: "Module");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Lesson",
                table: "Lesson");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CourseEnrollment",
                table: "CourseEnrollment");

            migrationBuilder.DropIndex(
                name: "IX_CourseEnrollment_AppUserId",
                table: "CourseEnrollment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CourseDetail",
                table: "CourseDetail");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CourseAssignment",
                table: "CourseAssignment");

            migrationBuilder.DropColumn(
                name: "AppUserId",
                table: "UserAssignment");

            migrationBuilder.DropColumn(
                name: "Grade",
                table: "UserAssignment");

            migrationBuilder.DropColumn(
                name: "CratedAt",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "AppUserId",
                table: "CourseEnrollment");

            migrationBuilder.RenameTable(
                name: "UserAssignment",
                newName: "UserAssignments");

            migrationBuilder.RenameTable(
                name: "Product",
                newName: "Products");

            migrationBuilder.RenameTable(
                name: "Module",
                newName: "Modules");

            migrationBuilder.RenameTable(
                name: "Lesson",
                newName: "Lessons");

            migrationBuilder.RenameTable(
                name: "CourseEnrollment",
                newName: "CourseEnrollments");

            migrationBuilder.RenameTable(
                name: "CourseDetail",
                newName: "CourseDetails");

            migrationBuilder.RenameTable(
                name: "CourseAssignment",
                newName: "CourseAssignments");

            migrationBuilder.RenameIndex(
                name: "IX_UserAssignment_AssignmentId",
                table: "UserAssignments",
                newName: "IX_UserAssignments_AssignmentId");

            migrationBuilder.RenameIndex(
                name: "IX_Product_Slug",
                table: "Products",
                newName: "IX_Products_Slug");

            migrationBuilder.RenameColumn(
                name: "Order",
                table: "Modules",
                newName: "SortOrder");

            migrationBuilder.RenameIndex(
                name: "IX_Module_CourseId",
                table: "Modules",
                newName: "IX_Modules_CourseId");

            migrationBuilder.RenameIndex(
                name: "IX_Lesson_ModuleId",
                table: "Lessons",
                newName: "IX_Lessons_ModuleId");

            migrationBuilder.RenameIndex(
                name: "IX_CourseEnrollment_CourseId",
                table: "CourseEnrollments",
                newName: "IX_CourseEnrollments_CourseId");

            migrationBuilder.RenameIndex(
                name: "IX_CourseAssignment_ProductId",
                table: "CourseAssignments",
                newName: "IX_CourseAssignments_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_CourseAssignment_ModuleId",
                table: "CourseAssignments",
                newName: "IX_CourseAssignments_ModuleId");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "UserAssignments",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "FileUrl",
                table: "UserAssignments",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Modules",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Lessons",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "CourseAssignments",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<Guid>(
                name: "ProductId",
                table: "CourseAssignments",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserAssignments",
                table: "UserAssignments",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Products",
                table: "Products",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Modules",
                table: "Modules",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Lessons",
                table: "Lessons",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CourseEnrollments",
                table: "CourseEnrollments",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CourseDetails",
                table: "CourseDetails",
                column: "ProductId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CourseAssignments",
                table: "CourseAssignments",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_UserAssignments_UserId",
                table: "UserAssignments",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseAssignments_Modules_ModuleId",
                table: "CourseAssignments",
                column: "ModuleId",
                principalTable: "Modules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseAssignments_Products_ProductId",
                table: "CourseAssignments",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseDetails_Products_ProductId",
                table: "CourseDetails",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseEnrollments_AspNetUsers_UserId",
                table: "CourseEnrollments",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseEnrollments_CourseDetails_CourseId",
                table: "CourseEnrollments",
                column: "CourseId",
                principalTable: "CourseDetails",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Lessons_Modules_ModuleId",
                table: "Lessons",
                column: "ModuleId",
                principalTable: "Modules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Modules_CourseDetails_CourseId",
                table: "Modules",
                column: "CourseId",
                principalTable: "CourseDetails",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserAssignments_AspNetUsers_UserId",
                table: "UserAssignments",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserAssignments_CourseAssignments_AssignmentId",
                table: "UserAssignments",
                column: "AssignmentId",
                principalTable: "CourseAssignments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseAssignments_Modules_ModuleId",
                table: "CourseAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseAssignments_Products_ProductId",
                table: "CourseAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseDetails_Products_ProductId",
                table: "CourseDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseEnrollments_AspNetUsers_UserId",
                table: "CourseEnrollments");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseEnrollments_CourseDetails_CourseId",
                table: "CourseEnrollments");

            migrationBuilder.DropForeignKey(
                name: "FK_Lessons_Modules_ModuleId",
                table: "Lessons");

            migrationBuilder.DropForeignKey(
                name: "FK_Modules_CourseDetails_CourseId",
                table: "Modules");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAssignments_AspNetUsers_UserId",
                table: "UserAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAssignments_CourseAssignments_AssignmentId",
                table: "UserAssignments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserAssignments",
                table: "UserAssignments");

            migrationBuilder.DropIndex(
                name: "IX_UserAssignments_UserId",
                table: "UserAssignments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Products",
                table: "Products");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Modules",
                table: "Modules");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Lessons",
                table: "Lessons");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CourseEnrollments",
                table: "CourseEnrollments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CourseDetails",
                table: "CourseDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CourseAssignments",
                table: "CourseAssignments");

            migrationBuilder.RenameTable(
                name: "UserAssignments",
                newName: "UserAssignment");

            migrationBuilder.RenameTable(
                name: "Products",
                newName: "Product");

            migrationBuilder.RenameTable(
                name: "Modules",
                newName: "Module");

            migrationBuilder.RenameTable(
                name: "Lessons",
                newName: "Lesson");

            migrationBuilder.RenameTable(
                name: "CourseEnrollments",
                newName: "CourseEnrollment");

            migrationBuilder.RenameTable(
                name: "CourseDetails",
                newName: "CourseDetail");

            migrationBuilder.RenameTable(
                name: "CourseAssignments",
                newName: "CourseAssignment");

            migrationBuilder.RenameIndex(
                name: "IX_UserAssignments_AssignmentId",
                table: "UserAssignment",
                newName: "IX_UserAssignment_AssignmentId");

            migrationBuilder.RenameIndex(
                name: "IX_Products_Slug",
                table: "Product",
                newName: "IX_Product_Slug");

            migrationBuilder.RenameColumn(
                name: "SortOrder",
                table: "Module",
                newName: "Order");

            migrationBuilder.RenameIndex(
                name: "IX_Modules_CourseId",
                table: "Module",
                newName: "IX_Module_CourseId");

            migrationBuilder.RenameIndex(
                name: "IX_Lessons_ModuleId",
                table: "Lesson",
                newName: "IX_Lesson_ModuleId");

            migrationBuilder.RenameIndex(
                name: "IX_CourseEnrollments_CourseId",
                table: "CourseEnrollment",
                newName: "IX_CourseEnrollment_CourseId");

            migrationBuilder.RenameIndex(
                name: "IX_CourseAssignments_ProductId",
                table: "CourseAssignment",
                newName: "IX_CourseAssignment_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_CourseAssignments_ModuleId",
                table: "CourseAssignment",
                newName: "IX_CourseAssignment_ModuleId");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "UserAssignment",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "FileUrl",
                table: "UserAssignment",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AppUserId",
                table: "UserAssignment",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Grade",
                table: "UserAssignment",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CratedAt",
                table: "Product",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Module",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Lesson",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AddColumn<string>(
                name: "AppUserId",
                table: "CourseEnrollment",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "CourseAssignment",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<Guid>(
                name: "ProductId",
                table: "CourseAssignment",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserAssignment",
                table: "UserAssignment",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Product",
                table: "Product",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Module",
                table: "Module",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Lesson",
                table: "Lesson",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CourseEnrollment",
                table: "CourseEnrollment",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CourseDetail",
                table: "CourseDetail",
                column: "ProductId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CourseAssignment",
                table: "CourseAssignment",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_UserAssignment_AppUserId",
                table: "UserAssignment",
                column: "AppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseEnrollment_AppUserId",
                table: "CourseEnrollment",
                column: "AppUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseAssignment_Module_ModuleId",
                table: "CourseAssignment",
                column: "ModuleId",
                principalTable: "Module",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseAssignment_Product_ProductId",
                table: "CourseAssignment",
                column: "ProductId",
                principalTable: "Product",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseDetail_Product_ProductId",
                table: "CourseDetail",
                column: "ProductId",
                principalTable: "Product",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseEnrollment_AspNetUsers_AppUserId",
                table: "CourseEnrollment",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseEnrollment_CourseDetail_CourseId",
                table: "CourseEnrollment",
                column: "CourseId",
                principalTable: "CourseDetail",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Lesson_Module_ModuleId",
                table: "Lesson",
                column: "ModuleId",
                principalTable: "Module",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Module_CourseDetail_CourseId",
                table: "Module",
                column: "CourseId",
                principalTable: "CourseDetail",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserAssignment_AspNetUsers_AppUserId",
                table: "UserAssignment",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserAssignment_CourseAssignment_AssignmentId",
                table: "UserAssignment",
                column: "AssignmentId",
                principalTable: "CourseAssignment",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
