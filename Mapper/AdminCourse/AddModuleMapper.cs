using CodeSparkNET.Dtos.Course;
using CodeSparkNET.ViewModels.AdminCourse;

namespace CodeSparkNET.Mapper.AdminCourse
{
    public static class AddModuleMapper
    {
        public static AddModuleDto ToDto(this AddModuleViewModel viewModel)
        {
            if (viewModel == null) return null;
            return new AddModuleDto
            {
                Id = viewModel.Id,
                Slug = viewModel.Slug.Trim().ToLower(),
                CourseSlug = viewModel.CourseSlug.Trim().ToLower(),
                Title = viewModel.Title.Trim(),
                Position = viewModel.Position
            };
        }
    }
}
