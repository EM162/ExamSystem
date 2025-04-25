namespace ITI.ExamSystem.Models
{
    public class StudentIndexViewModel
    {
        public List<IntakeBranchTrackUser> ActiveStudents { get; set; }
        public List<User> DeletedStudents { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }

    }
}
