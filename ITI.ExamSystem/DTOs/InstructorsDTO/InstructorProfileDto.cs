using ITI.ExamSystem.Models;

namespace ITI.ExamSystem.DTOs.InstructorsDTO
{
    public class InstructorProfileDto
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string ProfileImagePath { get; set; }

        public string Branch { get; set; }
        public string Track { get; set; }
        public string Intake { get; set; }
        public int CourseCount { get; set; }
        public int StudentCount { get; set; }
        public List<Course> Courses { get; set; }
    }
}
