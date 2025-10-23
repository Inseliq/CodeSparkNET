using CodeSparkNET.Dtos.Course;
using CodeSparkNET.ViewModels.AdminCourse;

namespace CodeSparkNET.Mapper.AdminCourse
{
    public static class AddLessonMapper
    {
        public static AddLessonDto ToDto(this AddLessonViewModel viewModel)
        {
            return new AddLessonDto
            {
                Id = viewModel.Id,
                ModuleSlug = viewModel.ModuleSlug.Trim().ToLower(),
                Title = viewModel.Title.Trim(),
                Slug = viewModel.Slug.Trim().ToLower(),
                Body = viewModel.Body?.Trim(),
                IsPublished = viewModel.IsPublished,
                IsFreePreview = viewModel.IsFreePreview
            };
        }
    }
}
