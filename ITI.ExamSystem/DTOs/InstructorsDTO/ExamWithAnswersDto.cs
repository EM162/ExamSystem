namespace ITI.ExamSystem.DTOs.InstructorsDTO
{
    public class ExamWithAnswersDto
    {
        public int ExamId { get; set; }

        public string ExamName { get; set; }
        public string ExamType { get; set; }
        public DateTime? ExamDate { get; set; }
        public int Grade { get; set; }
        public List<AnswerDto> Answers { get; set; } = new();
    }
}
