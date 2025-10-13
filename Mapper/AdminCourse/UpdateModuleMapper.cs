using CodeSparkNET.Dtos.Course;
using CodeSparkNET.ViewModels.AdminCourse;

namespace CodeSparkNET.Mapper.AdminCourse
{
    public static class UpdateModuleMapper
    {
        public static UpdateModuleDto ToDto(this UpdateModuleViewModel viewModel)
        {
            if (viewModel == null) return null;
            return new UpdateModuleDto
            {
                Slug = viewModel.Slug.Trim().ToLower(),
                Title = viewModel.Title.Trim(),
                Position = viewModel.Position
            };
        }
    }
}
