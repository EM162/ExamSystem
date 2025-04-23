using ITI.ExamSystem.Models;

namespace ITI.ExamSystem.DTOs.StudentDTO
{
    public class StudentCoursesDTO
    {
        public int CourseID { get; set; }
        public string?CourseName { get; set; }
        public int Duration { get; set; }

       public int Grade { get; set; }
        public string? CourseImagePath { get; set; }

        public List<StudentTopicsDTO>?Topics { get; set; }

       // public UserExam gradeUser { get; set; }


    }
}
