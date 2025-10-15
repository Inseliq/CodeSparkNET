using CodeSparkNET.Dtos.Profile;
using System.ComponentModel.DataAnnotations;

namespace CodeSparkNET.ViewModels.Profile
{
    public class PersonalProfileViewModel
    {
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? Roles { get; set; }
        public DateTime EmailAddAt { get; set; }
        public DateTime EmailConfirmedAt { get; set; }
        public DateTime EmailChangedAt { get; set; }
        public List<AllUserCoursesViewModel> AllUserCourses { get; set; }
    }
}
