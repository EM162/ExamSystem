namespace ITI.ExamSystem.Models
{
    public class CourseIndexViewModel
    {
        public List<CourseViewModel> Courses { get; set; }
        public List<Course> DeletedCourses { get; set; } 
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }

    }
}
