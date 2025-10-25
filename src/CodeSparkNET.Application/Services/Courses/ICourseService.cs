using CodeSparkNET.Application.Dtos.Course;

namespace CodeSparkNET.Application.Services.Courses
{
    public interface ICourseService
    {
        Task<CourseDto> GetCourseBySlugAsync(string slug);
        Task<ModuleDto> GetModuleBySlugAsync(string slug);
        Task<LessonContentDto> GetLessonBySlugAsync(string courseSlug, string lessonSlug);
        Task<CourseDto> CreateCourseAsync(CreateCourseDto dto);
        Task<bool> UpdateCourseAsync(UpdateCourseDto model);

        Task<ModuleDto> AddModuleAsync(AddModuleDto model);
        Task<bool> UpdateModuleAsync(UpdateModuleDto model);
        Task<bool> DeleteModuleAsync(string moduleSlug);

        Task<LessonContentDto> GetLessonByIdAsync(string lessonId);

        Task<LessonContentDto> AddLessonAsync(AddLessonDto model);
        Task<bool> UpdateLessonAsync(UpdateLessonDto model);
        Task<bool> DeleteLessonAsync(string lessonId);
    }
}
