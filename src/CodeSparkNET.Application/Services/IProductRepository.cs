using CodeSparkNET.Application.Dtos.Course;
using CodeSparkNET.Domain.Models;

namespace CodeSparkNET.Application.Services
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
        Task<bool> UpdateLessonAsync(UpdateLessonDto model);
        Task<bool> DeleteLessonAsync(string lessonId);

        Task<Template> CreateTemplateAsync(Template model);
        Task<Template> GetTemplateByIdAsync(string id);
        Task<Template> GetTemplateBySlugAsync(string slug);
        Task<Template> GetTemplateByIdOrSlugAsync(string query);
        Task<IEnumerable<Template>> GetAllTemplatesAsync();
        Task<bool> UpdateTemplateAsync(Template model);
        Task<bool> DeleteTemplateByIdAsync(string id);
        Task<bool> DeleteTemplateBySlugAsync(string slug);
    }
}
