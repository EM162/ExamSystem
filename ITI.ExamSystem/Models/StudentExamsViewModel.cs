namespace ITI.ExamSystem.Models
{
    public class StudentExamsViewModel
    {
        public List<PublishedExam> UpcomingExams { get; set; }
        public List<UserExam> PreviousExams { get; set; }

        public string SearchQuery { get; set; }
        public DateTime? FilterDate { get; set; }
    }
}
