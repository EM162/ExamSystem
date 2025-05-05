namespace ITI.ExamSystem.Models
{
    public class StudentExamsViewModel
    {
        public List<PublishedExam> UpcomingExams { get; set; }
        public List<UserExam> PreviousExams { get; set; }
        public HashSet<int> FinishedExamIds { get; set; } = new();
        //public string SearchQuery { get; set; }
        //public DateTime? FilterDate { get; set; }
    }
}
