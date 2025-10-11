// File: Controllers/AdminCourseController.cs
using CodeSparkNET.Dtos.Course;
using CodeSparkNET.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodeSparkNET.Controllers
{
    //[Authorize(Roles = "Admin")]
    public class AdminCourseController : Controller
    {
        private readonly ICourseService _courseService;

        public AdminCourseController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        [HttpGet]
        public IActionResult CreateCourse()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCourse(CreateCourseDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var created = await _courseService.CreateCourseAsync(model);
                // redirect to edit page so admin can add modules/lessons later
                return RedirectToAction("EditCourse", new { slug = created.Slug });
            }
            catch (Exception ex)
            {
                // better log the error
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditCourse(string slug)
        {
            if (string.IsNullOrWhiteSpace(slug)) return BadRequest();

            var course = await _courseService.GetCourseBySlugAsync(slug);
            if (course == null) return NotFound();

            var model = new UpdateCourseDto
            {
                Id = course.Id,
                Name = course.Name,
                Slug = course.Slug,
                ShortDescription = course.ShortDescription,
                FullDescription = course.FullDescription,
                MainImageUrl = course.Images?.FirstOrDefault(i => i.IsMain)?.Url
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateCourse(UpdateCourseDto model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Ошибка обновления курса" });
            }

            var ok = await _courseService.UpdateCourseAsync(model);
            if (!ok)
                return Json(new { success = false, message = "Ошибка обновления курса" });

            return Json(new { success = true, message = "Успешное обновление курса" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteModule(string moduleSlug)
        {
            if (!ModelState.IsValid && !string.IsNullOrEmpty(moduleSlug))
            {
                return Json(new { success = false, message = "Ошибка удаления модуля" });
            }

            var ok = await _courseService.DeleteModuleAsync(moduleSlug);

            if (!ok)
                return Json(new { success = false, message = "Ошибка удаления модуля" });
            return Json(new { success = true, message = "Успешное удаление модуля" });
        }
    }
}
