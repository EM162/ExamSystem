namespace ITI.ExamSystem.DTOs.InstructorsDTO
{
    public class AnswerDto
    {
        public int QuestionId { get; set; }
        public string StudentAnswer { get; set; }
        public string CorrectAnswer { get; set; }
        public decimal? StudentScore { get; set; }
    }
}
