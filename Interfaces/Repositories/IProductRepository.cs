using CodeSparkNET.Models;

namespace CodeSparkNET.Interfaces.Repositories
{
    public interface IProductRepository
    {
        Task<IEnumerable<Course>> GetAllUserCoursesAsync(string userId);
        Task<Course> GetUserCourseBySlugAsync(string userId, string courseSlug);
        Task<bool> AddCourseToUserAsync(AppUser user, string courseSlug);

    }
}
