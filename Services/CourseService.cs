using CodeSparkNET.Dtos.Course;
using CodeSparkNET.Interfaces.Repositories;
using CodeSparkNET.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace CodeSparkNET.Services
{
    public class CourseService : ICourseService
    {
        private readonly IProductRepository _productRepository;

        public CourseService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }
        public async Task<CourseDto> GetCourseBySlugAsync(string slug)
        {
            var course = await _productRepository.GetCourseBySlugAsync(slug);
            if (course == null) 
                return null;

            return new CourseDto
            {
                Id = course.Id,
                Name = course.Name,
                Slug = course.Slug,
                ShortDescription = course.ShortDescription,
                Images = course.ProductImages
                    .OrderBy(pi => pi.Position)
                    .Select(pi => new ProductImageDto
                    {
                        Url = pi.Url ?? pi.Name ?? "",
                        AltText = pi.Name,
                        IsMain = pi.IsMain
                    })
                    .ToList(),
                Modules = course.Modules
                    .OrderBy(m => m.Position)
                    .Select(m => new ModuleDto
                    {
                        Id = m.Id,
                        Title = m.Title,
                        Position = m.Position,
                        Lessons = m.Lessons
                            .OrderBy(l => l.Position)
                            .Select(l => new LessonListItemDto
                            {
                                Id = l.Id,
                                Title = l.Title,
                                Position = l.Position,
                                Slug = l.Slug,
                                IsFreePreview = l.IsFreePreview,
                            })
                            .ToList()
                    }).ToList()
            };
        }



        public async Task<LessonContentDto> GetLessonBySlugAsync(string courseSlug, string lessonSlug)
        {
            var lesson = await _productRepository.GetLessonBySlugAsync(courseSlug, lessonSlug);
            if (lesson == null)
                return null;

            return new LessonContentDto
            {
                Id = lesson.Id,
                Title = lesson.Title,
                Body = lesson.Body,
                Resources = lesson.Resources.OrderBy(r => r.Position)
                              .Select(r => new LessonResourceDto
                              {
                                  Id = r.Id,
                                  Url = r.Url,
                                  ResourceType = r.ResourceType,
                                  Title = r.Title
                              }).ToList()
            };
        }
    }
}
