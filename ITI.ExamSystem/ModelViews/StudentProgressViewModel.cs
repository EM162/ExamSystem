namespace ITI.ExamSystem.ModelViews
{
    public class StudentProgressViewModel
    {
        public int UserID { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public double AverageGrade { get; set; }
        public int ExamsTaken { get; set; }
        public Dictionary<string, double> SubjectAverages { get; set; }
        public List<ExamResult> ExamHistory { get; set; }

        public class ExamResult
        {
            public string ExamTitle { get; set; }
            public DateTime ExamDate { get; set; }
            public int Grade { get; set; }
            public string Subject { get; set; }
            public int QuestionCount { get; set; }
            public int Duration { get; set; }  // In minutes
            public string ExamType { get; set; }
        }
    }
}
