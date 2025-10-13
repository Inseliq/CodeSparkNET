using CodeSparkNET.Dtos.Course;
using CodeSparkNET.ViewModels.AdminCourse;

namespace CodeSparkNET.Mapper.AdminCourse
{
    public static class UpdateLessonMapper
    {
        public static UpdateLessonDto ToDto(this UpdateLessonViewModel viewModel)
        {
            if (viewModel == null) return null;
            return new UpdateLessonDto
            {
                Id = viewModel.Id,
                ModuleId = viewModel.ModuleId,
                Title = viewModel.Title.Trim(),
                Slug = viewModel.Slug.Trim().ToLower(),
                Body = viewModel.Body.Trim(),
                Position = viewModel.Position,
                IsPublished = viewModel.IsPublished,
                IsFreePreview = viewModel.IsFreePreview
            };
        }
    }
}
