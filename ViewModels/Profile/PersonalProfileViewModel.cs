using CodeSparkNET.Dtos.Profile;
using System.ComponentModel.DataAnnotations;

namespace CodeSparkNET.ViewModels.Profile
{
    public class PersonalProfileViewModel
    {
        [Required]
        public string? UserName { get; set; }
        [Required]
        public string? Email { get; set; }
        [Required]
        public string? Roles { get; set; }
        [Required]
        public DateTime EmailAddAt { get; set; }
        public DateTime EmailConfirmedAt { get; set; }
        public DateTime EmailChangedAt { get; set; }
        public List<AllUserCoursesViewModel> AllUserCourses { get; set; }
    }
}
