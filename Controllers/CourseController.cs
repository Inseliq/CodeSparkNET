using CodeSparkNET.Data;
using CodeSparkNET.Dtos.Course;
using CodeSparkNET.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CodeSparkNET.Controllers
{
    [AllowAnonymous]
    public class CoursesController : Controller
    {
        private readonly AppDbContext _db;
        private readonly ICourseService _courseService;

        public CoursesController(AppDbContext db, ICourseService courseService)
        {
            _db = db;
            _courseService = courseService;
        }

        [HttpGet("Course/{slug}")]
        public IActionResult Course(string slug)
        {
            return View(model: slug);
        }

        [HttpGet("Courses/GetCourseBySlug/{slug}")]
        [AllowAnonymous] 
        public async Task<ActionResult<CourseDto>> GetCourseBySlug(string slug)
        {
            var courseDto = await _courseService.GetCourseBySlugAsync(slug);

            if (courseDto == null) return NotFound();
            return Ok(courseDto);
        }

        [HttpGet("Courses/GetLessonContent/{courseSlug}/lessons/{lessonSlug}")]
        [AllowAnonymous]
        public async Task<ActionResult<LessonContentDto>> GetLessonBySlug(string courseSlug, string lessonSlug)
        {
            var lesson = await _courseService.GetLessonBySlugAsync(courseSlug, lessonSlug);

            if (lesson == null) return NotFound();
            return Ok(lesson);
        }
    }
}
