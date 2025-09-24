using CodeSparkNET.Dtos.Course;
using Microsoft.AspNetCore.Mvc;

namespace CodeSparkNET.Interfaces.Services
{
    public interface ICourseService
    {
        Task<CourseDto> GetCourseBySlugAsync(string slug);
        Task<LessonContentDto> GetLessonBySlugAsync(string courseSlug, string lessonSlug);
    }
}
