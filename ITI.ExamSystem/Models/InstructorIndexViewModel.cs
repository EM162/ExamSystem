namespace ITI.ExamSystem.Models
{
    public class InstructorIndexViewModel
    {
        public List<IntakeBranchTrackUser> ActiveInstructors { get; set; }
        public List<User> DeletedInstructors { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public List<InstructorViewModel> Instructors { get; set; }
    }
}
