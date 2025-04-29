using X.PagedList;

namespace ITI.ExamSystem.Models
{
    public class StudentIndexViewModel
    {
        public List<IntakeBranchTrackUser> ActiveStudents { get; set; }
        public List<User> DeletedStudents { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public List<StudnetViewModel> Students { get; set; }
        public IPagedList<User> Users { get; set; }
        


    }
}
