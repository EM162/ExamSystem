namespace ITI.ExamSystem.Models
{
    public class ExamDetailsViewModel
    {
        public string ExamTitle { get; set; }
        public DateTime ExamDate { get; set; }
        public int Grade { get; set; }
        public List<QuestionDetail> Questions { get; set; }
    }

    public class QuestionDetail
    {
        public string QuestionText { get; set; }
        public string SelectedAnswer { get; set; }
        public string CorrectAnswer { get; set; }
        public decimal Score { get; set; }
    }

}
