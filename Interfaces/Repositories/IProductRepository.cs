using CodeSparkNET.Dtos.Course;
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
        Task<Course> GetCourseByIdWithModulesAsync(string courseId);
        Task<Lesson> GetLessonBySlugAsync(string courseSlug, string lessonSlug);
        Task<CourseModule> GetModuleBySlugAsync(string moduleSlug);
        Task<Lesson> GetLessonByIdAsync(string lessonId);
        Task<Course> AddCourseAsync(CreateCourseDto dto);
        Task<bool> UpdateCourseBasicsAsync(UpdateCourseDto model);

        Task<CourseModule> AddModuleAsync(AddModuleDto model);
        Task<bool> UpdateModuleAsync(UpdateModuleDto model);
        Task<bool> DeleteModuleAsync(string moduleSlug);

        Task<Lesson> AddLessonAsync(AddLessonDto model);
        Task<bool> UpdateLessonAsync(string moduleId, string lessonId, string title, string slug, string body, int position, bool isPublished, bool isFreePreview);
        Task<bool> DeleteLessonAsync(string lessonId);
    }
}
