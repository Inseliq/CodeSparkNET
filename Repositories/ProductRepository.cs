using CodeSparkNET.Data;
using CodeSparkNET.Dtos.Course;
using CodeSparkNET.Interfaces.Repositories;
using CodeSparkNET.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CodeSparkNET.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Course>> GetAllUserCoursesAsync(string userId)
        {
            return await _context.UserCourses
                .Where(uc => uc.UserId == userId)
                .Select(uc => uc.Course)
                .ToListAsync();
        }

        public async Task<Course> GetUserCourseBySlugAsync(string userId, string courseSlug)
        {
            return await _context.UserCourses
                .Where(uc => uc.UserId == userId && uc.CourseSlug == courseSlug)
                .Select(uc => uc.Course)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> AddCourseToUserAsync(string userId, string courseSlug)
        {
            // Find the course by slug
            var course = await _context.Products
                .OfType<Course>()
                .FirstOrDefaultAsync(c => c.Slug == courseSlug);
            if (course == null)
                return false;

            // Check if the user is already enrolled in the course
            var alreadyEnrolled = await _context.UserCourses
                .AnyAsync(uc => uc.UserId == userId && uc.CourseSlug == courseSlug);
            if (alreadyEnrolled)
                return false;

            // Create new enrollment
            var userCourse = new UserCourse
            {
                UserId = userId,
                CourseSlug = courseSlug,
                Course = course,
                IsCompleted = false,
                EnrolledAt = DateTime.UtcNow
            };
            _context.UserCourses.Add(userCourse);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IsCourseAlreadyEnrolledAsync(string userId, string courseSlug)
        {
            // Find the course by slug
            var course = await _context.Products
                .OfType<Course>()
                .FirstOrDefaultAsync(c => c.Slug == courseSlug);
            if (course == null)
                return false;

            // Check if the user is already enrolled in the course
            return await _context.UserCourses
                .AnyAsync(uc => uc.UserId == userId && uc.CourseSlug == courseSlug);
        }

        public async Task<Course> GetCourseBySlugAsync(string slug)
        {
            return await _context.Courses
                .AsNoTracking()
                .Where(c => c.Slug == slug && c.IsPublished)
                .Include(c => c.Modules)
                    .ThenInclude(c => c.Lessons)
                .FirstOrDefaultAsync();
        }

        public async Task<Lesson> GetLessonBySlugAsync(string courseSlug, string lessonSlug)
        {
            return await _context.Lessons
                .AsNoTracking()
                .Where(l => l.Slug == lessonSlug && l.Module.Course.Slug == courseSlug && l.Module.Course.IsPublished)
                .FirstOrDefaultAsync();
        }
    }
}
