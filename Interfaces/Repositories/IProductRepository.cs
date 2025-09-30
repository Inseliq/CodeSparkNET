using CodeSparkNET.Models;

namespace CodeSparkNET.Interfaces.Repositories
{
    public interface IProductRepository
    {
        Task<IEnumerable<Course>> GetAllUserCoursesAsync(string userId);
        Task<Course> GetUserCourseBySlugAsync(string userId, string courseSlug);
        Task<bool> AddCourseToUserAsync(string userId, string courseSlug);
        Task<bool> IsCourseAlreadyEnrolledAsync(string userId, string courseSlug);
        Task<Course> GetCourseBySlugAsync(string slug);
        Task<Lesson> GetLessonBySlugAsync(string courseSlug, string lessonSlug);
    }
}
