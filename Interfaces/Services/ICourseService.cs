using CodeSparkNET.Dtos.Course;
using Microsoft.AspNetCore.Mvc;

namespace CodeSparkNET.Interfaces.Services
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

        Task<LessonContentDto> AddLessonAsync(AddLessonDto model);
    }
}
