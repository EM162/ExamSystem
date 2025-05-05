using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using ITI.ExamSystem.Models;

namespace ITI.ExamSystem.DTOs.InstructorsDTO
{
    public class InstructorProfileDto
    {
        public int UserID { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string ProfileImagePath { get; set; }
        public List<Branch> Branch { get; set; }
        public List<Track> Track { get; set; }
        public List<Intake> Intake { get; set; }
        public int CourseCount { get; set; }
        public int StudentCount { get; set; }
        public int ExamCount { get; set; }
        public int PublishedExamCount { get; set; }
        public List<Course> Courses { get; set; }
    }
}
