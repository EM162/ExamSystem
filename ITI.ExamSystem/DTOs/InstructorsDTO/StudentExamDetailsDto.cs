namespace ITI.ExamSystem.DTOs.InstructorsDTO
{
    public class StudentExamDetailsDto
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public string StudentEmail { get; set; }
        public List<ExamWithAnswersDto> Exams { get; set; } = new();
    }
}
