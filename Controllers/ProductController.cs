using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CodeSparkNET.Data;
using CodeSparkNET.Models.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;

namespace CodeSparkNET.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private readonly ILogger<ProductController> _logger;
        private readonly AppDbContext _context;

        public ProductController(ILogger<ProductController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        // public async Task<IActionResult> Details(string slug)
        // {
        //     var product = await _context.Products
        //         .FirstOrDefaultAsync(p => p.Slug == slug && p.IsPublished);

        //     if (product == null) return NotFound();

        //     // Определяем тип товара и возвращаем соответствующую ViewModel
        //     return product.ProductType switch
        //     {
        //         ProductType.Course => await GetCourseDetails(product.Id),
        //         ProductType.Diploma => await GetDiplomaDetails(product.Id),
        //         ProductType.Template => await GetTemplateDetails(product.Id),
        //         _ => NotFound()
        //     };
        // }

        public async Task<IActionResult> CourseDetails(string slug)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Slug == slug && p.IsPublished && p.ProductType == ProductType.Course);

            if (product == null) return NotFound();

            var courseDetail = await _context.CourseDetails
                .FirstOrDefaultAsync(cd => cd.ProductId == product.Id);

            if (courseDetail == null) return NotFound();

            // Получаем текущего пользователя
            var userId = User.Identity?.Name; // Или другой способ получения ID пользователя

            // Проверяем, может ли пользователь заказать курс и заказывал ли он его ранее
            bool canUserOrder = true; // Логика определения, может ли пользователь заказать курс
            // bool hasUserOrdered = await _context.CourseOrders
            //     .AnyAsync(co => co.UserId == userId && co.CourseId == courseDetail.Id);

            var dto = new Dtos.Products.CourseDto
            {
                Product = product,
                CourseDetail = courseDetail,
                Modules = courseDetail.Modules?.ToList() ?? new List<Models.Module>(),
                TotalLessons = courseDetail.Modules?.Sum(m => m.Lessons.Count) ?? 0,
                IsUserEnrolled = false,
                EnrolledStudents = 0
            };

            return View("CourseDetails", dto);

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}