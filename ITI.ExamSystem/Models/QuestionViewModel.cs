namespace ITI.ExamSystem.Models
{
    public class QuestionViewModel
    {
        public int ExamID { get; set; }
        public string ExamTitle { get; set; }

        public int QuestionID { get; set; }
        public string QuestionText { get; set; }

        public int QuestionIndex { get; set; }
        public int QuestionNumber => QuestionIndex + 1;
        public bool IsLastQuestion { get; set; }

        public List<ChoiceViewModel> Choices { get; set; }

        public int? SelectedChoiceID { get; set; }

        public int Progress { get; set; }
        public int TimeRemaining { get; set; }

        public int TimeRemainingSeconds { get; set; }

    }

    public class ChoiceViewModel
    {
        public int ChoiceID { get; set; }
        public string ChoiceText { get; set; }
    }

}
