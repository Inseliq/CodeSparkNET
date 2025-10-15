using System.ComponentModel.DataAnnotations;

namespace CodeSparkNET.Dtos.Profile
{
    public class PersonalProfileDto
    {
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? Roles { get; set; }
        public DateTime EmailAddAt { get; set; }
        public DateTime EmailConfirmedAt { get; set; }
        public DateTime EmailChangedAt { get; set; }
        public List<AllUserCoursesDto> AllUserCourses { get; set; }
    }
}